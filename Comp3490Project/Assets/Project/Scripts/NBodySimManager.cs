using CielaSpike;
using NBodySimProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Comp3490Project
{
    public class NBodySimManager : MonoBehaviour
    {
        public float BoundDistance = 30;
        public float MergeFactor = 100;
        public GameObject BoundingSphere;
        public Text HUDText;

        private NBodySim nBodySim;
        private AudioSource audioSource;

        private bool simRunning = true;
        private bool computingBodies = false;

        private void Awake()
        {
            nBodySim = GetComponent<NBodySim>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            BoundingSphere.transform.localScale = Vector3.one * BoundDistance;
            BoundingSphere.SetActive(false);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                simRunning = !simRunning;
                nBodySim.ToggleIsUpdating();
                BoundingSphere.SetActive(false);
                HUDText.text = "";
            }
            else if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                if(computingBodies)
                {
                    return;
                }

                if(!BoundingSphere.activeInHierarchy)
                {
                    BoundingSphere.SetActive(true);
                }
                
                BoundingSphere.transform.position = transform.position;
                audioSource.Play();

                if (simRunning)
                {
                    simRunning = false;
                    nBodySim.ToggleIsUpdating();
                }
                computingBodies = true;
                this.StartCoroutineAsync(ComputeBodies());
            }
            else if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                BoundingSphere.SetActive(false);
                HUDText.text = "";
            }
            else if(Input.GetKeyDown(KeyCode.Return))
            {
                if(BoundingSphere.activeInHierarchy && !computingBodies)
                {
                    SceneManager.LoadScene(1);
                }
            }
        }

        private IEnumerator ComputeBodies()
        {
            yield return Ninja.JumpToUnity;

            Vector3 origin = BoundingSphere.transform.position;
            float distThreshold = BoundDistance;
            float[] points = new float[nBodySim.Positions.count * 4];
            nBodySim.Positions.GetData(points);

            yield return Ninja.JumpBack;

            Body[] bodies = ComputeBoundedPoints(origin, distThreshold, points);

            yield return Ninja.JumpToUnity;

           GameManager.SetBodies(bodies);
            computingBodies = false;

            HUDText.text = string.Format("Identified {0} unique bodies within the bounding sphere.", bodies.Length);
        }

        private Body[] ComputeBoundedPoints(Vector3 origin, float distThreshold, float[] points)
        {
            Dictionary<int, Body> boundedPoints = new Dictionary<int, Body>();
            int step = 4;

            for (int i = 0; i < points.Length; i += step)
            {
                Vector3 point = new Vector3(points[i], points[i + 1], points[i + 2]);
                float distance = Vector3.Distance(origin, point);
                if(distance <= distThreshold)
                {
                    int body = (int)(distance * MergeFactor);
                    if(!boundedPoints.ContainsKey(body))
                    {
                        boundedPoints.Add(body, new Body());
                    }
                    Vector3 shiftedPoint = point - origin;
                    Vector3 s = Vector3.MoveTowards(origin, point, distance);
                    boundedPoints[body].Points.Add(shiftedPoint);
                }
            }

            Body[] bodies = new Body[boundedPoints.Count];
            boundedPoints.Values.CopyTo(bodies, 0);

            return bodies;
        }
    }
}
