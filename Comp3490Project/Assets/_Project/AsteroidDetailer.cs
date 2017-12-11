using ImprovedPerlinNoiseProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Comp3490Project
{
    public class AsteroidDetailer : MonoBehaviour
    {
        public bool RunOnStart = false;

        public float MinFrequency = 0.3f;
        public float MaxFrequency = 0.9f;

        public float MinLacunarity = 2.0f;
        public float MaxLacunarity = 2.0f;

        public float MinGain = 0.4f;
        public float MaxGain = 0.75f;

        private void Start()
        {
            if (RunOnStart)
            {
                Detail();
            }
        }

        public void Detail()
        {
            int seed = Random.Range(0, int.MaxValue);
            GPUPerlinNoise perlin = new GPUPerlinNoise(seed);
            Renderer renderer = GetComponent<Renderer>();

            perlin.LoadResourcesFor3DNoise();

            Material newMat = Instantiate(renderer.material);
            newMat.CopyPropertiesFromMaterial(renderer.material);
            renderer.material = newMat;

            renderer.material.SetTexture("_PermTable2D", perlin.PermutationTable2D);
            renderer.material.SetTexture("_Gradient3D", perlin.Gradient3D);

            renderer.material.SetFloat("_Frequency", Random.Range(MinFrequency, MaxFrequency));
            renderer.material.SetFloat("_Lacunarity", Random.Range(MinLacunarity, MaxLacunarity));
            renderer.material.SetFloat("_Gain", Random.Range(MinGain, MaxGain));
        }
    }
}
