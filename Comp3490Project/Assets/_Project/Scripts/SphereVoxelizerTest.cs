using System.Collections.Generic;
using ProceduralNoiseProject;
using UnityEngine;
using MeshVoxelizerProject;
using MarchingCubesProject;
using System.Collections;
using CielaSpike;

namespace Comp3490Project
{

    public class SphereVoxelizerTest : MonoBehaviour
    {
        public static MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        public static int[,,] voxels;
        public static Vector3 pos;
        public static GameObject sphere;
        private int VoxelSize = 16;

        void Start()
        {
            pos = transform.localPosition;
            sphere = transform.gameObject;
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            Render();
        }

        public void Render()
        {
           
            meshRenderer.enabled = false;

            Vector3[] vertices = meshFilter.mesh.vertices;

            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.RecalculateBounds();

            int[] triangles = meshFilter.mesh.triangles;
            Box3 bounds = new Box3(meshFilter.mesh.bounds.min, meshFilter.mesh.bounds.max);

            voxels = VoxelizeMesh(vertices, triangles, bounds, VoxelSize);
            voxels = Pad3DArray(voxels);

            MarchMesh(voxels, out vertices, out triangles);

            meshFilter.mesh.Clear();
            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.triangles = triangles;
            meshFilter.mesh.RecalculateNormals();

            meshRenderer.enabled = true;


            DestroyImmediate(this.GetComponent<MeshCollider>());
            var collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = meshFilter.mesh;



        }
        public static void changes()
        {
            Vector3[] vertices;
            int[] triangles;


            MarchMesh(voxels, out vertices, out triangles);
            
            for(int i = 0; i < meshFilter.mesh.vertexCount; i++)
            {
                if(meshFilter.mesh.vertices[i] != vertices[i])
                {
                    Debug.LogFormat("meshFilter.mesh.vertices:{0}, changed to: {vertices}", meshFilter.mesh.vertices[i], vertices[i]);
                }
            }

            meshFilter.mesh.Clear();
            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.triangles = triangles;
            meshFilter.mesh.RecalculateNormals();

        }

        private int[,,] VoxelizeMesh(Vector3[] vertices, int[] triangles, Box3 bounds, int size)
        {
            MeshVoxelizer m_voxelizer = new MeshVoxelizer(size, size, size);

            m_voxelizer.Voxelize(vertices, triangles, bounds);

            return m_voxelizer.Voxels;
        }


        private int[,,] Pad3DArray(int[,,] array)
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

        private static void MarchMesh(int[,,] voxels, out Vector3[] vertices, out int[] triangles)
        {
            Marching marching = new MarchingCubes();

            int width = voxels.GetLength(0);
            int height = voxels.GetLength(1);
            int length = voxels.GetLength(2);

            float[] flatVoxels = Convert3DIntArrayTo1DFloatArray(voxels);

            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();

            Debug.LogFormat("verts before marching:{0}", (width*height*length));

            marching.Generate(flatVoxels, width, height, length, verts, indices);
            Debug.LogFormat("verts after marching:{0}", verts.Count);
            float scalar = (float)(width * height * length)/verts.Count;
            Debug.LogFormat("scalar:{0}", scalar);

            vertices = verts.ToArray();
            triangles = indices.ToArray();
        }

        private static float[] Convert3DIntArrayTo1DFloatArray(int[,,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            int length = array.GetLength(2);
            float[] result = new float[width * height * length];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < length; k++)
                    {
                        int pos = i + j * width + k * width * height;
                        result[pos] = array[i, j, k];
                    }
                }
            }

            return result;
        }
    }
}
