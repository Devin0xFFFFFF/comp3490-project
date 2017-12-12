using CielaSpike;
using SuperSystems.UnityTools;
using System.Collections;
using UnityEngine;

namespace Comp3490Project
{
    public class BodyGenerator: MonoBehaviour
    {
        public int MaxBodies = 5000;
        public float Dispersion = 500;
        public GameObject Prefab;
   
        public float MinRotationForce = 0.0f;
        public float MaxRotationForce = 10.0f;

        public float MinArbitraryScaler = 1.0f;
        public float MaxArbitraryScaler = 5.0f;

        public float MinScaleVariance = 1.0f;
        public float MaxScaleVariance = 1.8f;

        private void Awake()
        {
            Body[] bodies = GameManager.GetBodies();
            if (bodies != null)
            {
                GenerateBodies(bodies);
            }
        }

        private void GenerateBodies(Body[] bodies, int segments = 4)
        {
            if(bodies.Length == 0)
            {
                return;
            }

            int length = Mathf.Max(bodies.Length, MaxBodies);
            int segmentLength = (length / segments);
            int remainder = length % segments;

            for (int i = 0; i <= segments; i++)
            {
                if(i == segments)
                {
                    this.StartCoroutineAsync(GenerateBodies(bodies, i, (i * segmentLength) + remainder));
                }
                else
                {
                    this.StartCoroutineAsync(GenerateBodies(bodies, i, i * segmentLength));
                }
            }
        }

        private IEnumerator GenerateBodies(Body[] bodies, int startIndex, int endIndex)
        {
            yield return Ninja.JumpToUnity;

            for (int i = startIndex; i < endIndex; i++)
            {
                Vector3 bodyPos = bodies[i].GetPosition() * Dispersion;
                GameObject obj = Instantiate(Prefab, bodyPos, Random.rotation);
                AsteroidDeformation deform = obj.GetComponent<AsteroidDeformation>();
                
                Task task;
                yield return this.StartCoroutineAsync(deform.Deform(), out task);
                yield return StartCoroutine(task.Wait());

                AutoRotate autoRotate = obj.GetComponent<AutoRotate>();
                autoRotate.speed = new Vector3(
                    Random.Range(MinRotationForce, MaxRotationForce),
                    Random.Range(MinRotationForce, MaxRotationForce),
                    Random.Range(MinRotationForce, MaxRotationForce)
                    );

                Vector3 scaleVector = new Vector3(
                    Random.Range(MinScaleVariance, MaxScaleVariance),
                    Random.Range(MinScaleVariance, MaxScaleVariance),
                    Random.Range(MinScaleVariance, MaxScaleVariance)
                    ) * bodies[i].Size * Random.Range(MinArbitraryScaler, MaxArbitraryScaler);
                obj.transform.localScale = scaleVector;

                AsteroidDetailer detailer = obj.GetComponent<AsteroidDetailer>();
                detailer.Detail();

                obj.transform.SetParent(transform);
            }
        }
    }
}
