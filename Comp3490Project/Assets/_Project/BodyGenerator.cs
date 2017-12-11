using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Comp3490Project
{
    public class BodyGenerator: MonoBehaviour
    {
        public int MaxBodies = 1000;
        public float Dispersion = 100;
        public GameObject Prefab;

        private void Awake()
        {
            Body[] bodies = GameManager.GetBodies();
            if (bodies != null)
            {
                MaxBodies = 5000;
                Dispersion = 400;
                StartCoroutine(GenerateBodies(bodies));
            }
        }

        private IEnumerator GenerateBodies(Body[] bodies)
        {
            yield return null;

            for (int i = 0; i < bodies.Length && i <= MaxBodies; i++)
            {
                Vector3 bodyPos = bodies[i].GetPosition() * Dispersion;
                GameObject obj = Instantiate(Prefab, bodyPos, Random.rotation);
                //obj.transform.localRotation = 
                obj.transform.localScale = Vector3.one * bodies[i].Size * Random.Range(1, 10);
                obj.transform.SetParent(transform);
            }
        }
    }
}
