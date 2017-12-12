using System.Collections.Generic;
using ProceduralNoiseProject;
using UnityEngine;
using MeshVoxelizerProject;
using MarchingCubesProject;
using System.Collections;
using CielaSpike;

namespace Comp3490Project
{
    public class Deformation : MonoBehaviour
    {
        public bool RunOnStart = false;
        public int VoxelSize = 16;
        
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            if(RunOnStart)
            {
                this.StartCoroutineAsync(Deform());
            }
        }

        public IEnumerator Deform()
        {
            yield return Ninja.JumpToUnity;

            meshRenderer.enabled = false;

            Vector3[] vertices = meshFilter.mesh.vertices;

            yield return Ninja.JumpBack;

            vertices = WarpVertices(vertices);

            yield return Ninja.JumpToUnity;

            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.RecalculateBounds();

            int[] triangles = meshFilter.mesh.triangles;
            Box3 bounds = new Box3(meshFilter.mesh.bounds.min, meshFilter.mesh.bounds.max);

            yield return Ninja.JumpBack;

            int[,,] voxels = VoxelizeMesh(vertices, triangles, bounds, VoxelSize);
            voxels = Pad3DArray(voxels);

            MarchMesh(voxels, out vertices, out triangles);

            yield return Ninja.JumpToUnity;

            meshFilter.mesh.Clear();
            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.triangles = triangles;
            meshFilter.mesh.RecalculateNormals();

            //DestroyImmediate(this.GetComponent<MeshCollider>());
            //var collider = this.AddComponent<MeshCollider>();
            //collider.sharedMesh = meshFilter.mesh;


            meshRenderer.enabled = true;
        }

        private Vector3[] WarpVertices(Vector3[] inputVertices)
        {
            Vector3[] vertices = inputVertices;
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

            return vertices;
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

        private void MarchMesh(int[,,] voxels, out Vector3[] vertices, out int[] triangles)
        {
            Marching marching = new MarchingCubes();

            int width = voxels.GetLength(0);
            int height = voxels.GetLength(1);
            int length = voxels.GetLength(2);

            float[] flatVoxels = Convert3DIntArrayTo1DFloatArray(voxels);

            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();
            marching.Generate(flatVoxels, width, height, length, verts, indices);

            vertices = verts.ToArray();
            triangles = indices.ToArray();
        }

        private float[] Convert3DIntArrayTo1DFloatArray(int[,,] array)
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
