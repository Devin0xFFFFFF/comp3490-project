using System.Collections;
using System.Collections.Generic;
using ProceduralNoiseProject;
using UnityEngine;
using System.Diagnostics;
using System;
using MeshVoxelizerProject;
using MarchingCubesProject;

namespace Comp3490Project
{
    public class Deformation : MonoBehaviour
    {
        public int size = 16;
        
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            meshRenderer.enabled = false;

            Warp();

            int[,,] voxels = VoxelizeMesh();
            voxels = Pad3DArray(voxels);
            MarchMesh(voxels);
            meshFilter.mesh.RecalculateNormals();

            meshRenderer.enabled = true;
        }

        public void Warp()
        {
            Vector3[] vertices = meshFilter.mesh.vertices;
            System.Random random = new System.Random();

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vec3 = vertices[i];
                float x = vec3.x;
                float y = vec3.y;
                float z = vec3.z;
                int test = 140;

                vec3.x += ((y * 5) * (random.Next(100, test) / 100));
                vec3.y -= ((x * 7) * (random.Next(100, test) / 100));
                vec3.z -= ((y * 4.5f) * (random.Next(100, test) / 100));

                vertices[i] = vec3.normalized;
            }

            int seed = random.Next(0, int.MaxValue);
            INoise perlin = new PerlinNoise(seed, 2.0f);
            FractalNoise fractal = new FractalNoise(perlin, 3, 1.0f);


            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vec3 = vertices[i];
                float scale = 50f;
                if (random.Next(1, 2) == 1)
                {
                    vec3.x += fractal.Sample1D(vec3.x) * scale;
                    vec3.y += fractal.Sample1D(vec3.y) * scale;
                    vec3.z += fractal.Sample1D(vec3.z) * scale;
                }
                else
                {
                    vec3.x -= fractal.Sample1D(vec3.x) * scale;
                    vec3.y -= fractal.Sample1D(vec3.y) * scale;
                    vec3.z -= fractal.Sample1D(vec3.z) * scale;
                }
                vertices[i] = vec3.normalized;
            }

            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.RecalculateBounds();
        }

        private int[,,] VoxelizeMesh()
        {
            Mesh mesh = meshFilter.mesh;
            MeshVoxelizer m_voxelizer = new MeshVoxelizer(size, size, size);
            Box3 bounds = new Box3(mesh.bounds.min, mesh.bounds.max);
            m_voxelizer.Voxelize(mesh.vertices, mesh.triangles, bounds);

            return m_voxelizer.Voxels;
        }

        public int[,,] Pad3DArray(int[,,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            int length = array.GetLength(2);

            int pad = 1;
            int doublePad = pad * 2;
            int vwidth = width + doublePad;
            int vheight = height + doublePad;
            int vlength = length + doublePad;

            int[,,] paddedArray = new int[vwidth, vheight, vlength];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        paddedArray[x + pad, y + pad, z + pad] = array[x, y, z];
                    }
                }
            }

            return paddedArray;
        }

        public void MarchMesh(int[,,] voxels)
        {
            Marching marching = new MarchingCubes();

            int width = voxels.GetLength(0);
            int height = voxels.GetLength(1);
            int length = voxels.GetLength(2);

            float[] flatVoxels = Convert3DIntArrayTo1DFloatArray(voxels);

            UnityEngine.Debug.LogFormat(" Width: {0}, Height: {1}, Length: {2}, Width*Height*Length: {3}", width, height, length, (width * height * length));

            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();
            marching.Generate(flatVoxels, width, height, length, verts, indices);

            meshFilter.mesh.Clear();
            meshFilter.mesh.vertices = verts.ToArray();
            meshFilter.mesh.triangles = indices.ToArray();

            UnityEngine.Debug.LogFormat(" verts: {0}", verts.Count);
        }

        static float[] Convert3DIntArrayTo1DFloatArray(int[,,] array)
        {
            int width = array.GetLength(0);// was 32 by default
            int height = array.GetLength(1);// was 32 by default
            int length = array.GetLength(2);// was 32 by default

            List<float> result = new List<float>();
            for(int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < length; k++)
                    {
                        result.Add(array[i,j,k]);
                    }
                }
            }

            return result.ToArray();
        }

        private void DoubleMesh()
        {
            Vector3[] vertices = meshFilter.mesh.vertices;
            Vector3[] normals  = meshFilter.mesh.normals;
            int verticesLength = vertices.Length;
            Vector3[] newVerts = new Vector3[verticesLength * 2];
            Vector3[] newNorms = new Vector3[verticesLength * 2];
            int Count1 = 0;
            for (Count1 = 0; Count1 < verticesLength; Count1++)
            {
                // duplicate vertices and uvs:
                newVerts[Count1] = newVerts[Count1 + verticesLength] = vertices[Count1];//verts[Count1];
                                                                                        // copy the original normals...
                newNorms[Count1] = normals[Count1];
                // and revert the new ones
                newNorms[Count1 + verticesLength] = -normals[Count1];
            }
            int[] triangles = meshFilter.mesh.triangles;
            int trianglesLength = triangles.Length;
            int[] newTris = new int[trianglesLength * 2]; // double the triangles
            for (int Count2 = 0; Count2 < trianglesLength; Count2 += 3)
            {
                // copy the original triangle
                newTris[Count2] = triangles[Count2];
                newTris[Count2 + 1] = triangles[Count2 + 1];
                newTris[Count2 + 2] = triangles[Count2 + 2];
                // save the new reversed triangle
                Count1 = Count2 + trianglesLength;
                newTris[Count1] = triangles[Count2] + verticesLength;
                newTris[Count1 + 2] = triangles[Count2 + 1] + verticesLength;
                newTris[Count1 + 1] = triangles[Count2 + 2] + verticesLength;
            }
            meshFilter.mesh.Clear();
            meshFilter.mesh.vertices = newVerts;
            meshFilter.mesh.normals = newNorms;
            meshFilter.mesh.triangles = newTris; // assign triangles last!
            meshFilter.mesh.RecalculateNormals();
        } 

    }
}
