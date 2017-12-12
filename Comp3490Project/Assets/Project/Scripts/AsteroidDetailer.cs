using ImprovedPerlinNoiseProject;
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

        private GPUPerlinNoise perlin;
        private Material detailMat;

        private void Start()
        {
            if (RunOnStart)
            {
                Detail();
            }
        }

        public void Detail()
        {
            Renderer renderer = GetComponent<Renderer>();

            if(perlin == null)
            {
                int seed = Random.Range(0, int.MaxValue);
                perlin = new GPUPerlinNoise(seed);
                perlin.LoadResourcesFor3DNoise();
            }

            if (detailMat == null)
            {
                detailMat = Instantiate(renderer.material);
                detailMat.CopyPropertiesFromMaterial(renderer.material);
            }

            renderer.material = detailMat;

            renderer.material.SetTexture("_PermTable2D", perlin.PermutationTable2D);
            renderer.material.SetTexture("_Gradient3D", perlin.Gradient3D);

            renderer.material.SetFloat("_Frequency", Random.Range(MinFrequency, MaxFrequency));
            renderer.material.SetFloat("_Lacunarity", Random.Range(MinLacunarity, MaxLacunarity));
            renderer.material.SetFloat("_Gain", Random.Range(MinGain, MaxGain));
        }

        private void OnDestroy()
        {
            if(perlin != null)
            {
                Destroy(perlin.PermutationTable2D);
                Destroy(perlin.Gradient3D);
                perlin = null;
            }

            if (detailMat != null)
            {
                Destroy(detailMat);
                detailMat = null;
            }
        }
    }
}
