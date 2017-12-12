using SuperSystems.UnityTools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Comp3490Project
{
    public class AsteroidDemo : MonoBehaviour
    {
        public GameObject AsteroidPrefab;

        private Camera cam;
        private GameObject currentAsteroid;

        private Quaternion rotation = Quaternion.identity;
        private bool rotating = true;

        private void Start()
        {
            cam = GetComponent<Camera>();
            ChangeAsteroid();
        }

        private void Update()
        {
            if(currentAsteroid != null)
            {
                rotation = currentAsteroid.transform.rotation;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Hit(Input.mousePosition);
            }
            else if(Input.GetKeyDown(KeyCode.Return))
            {
                ChangeAsteroid();
            }
            else if(Input.GetKeyDown(KeyCode.Space))
            {
                rotating = !rotating;
                if(currentAsteroid != null)
                {
                    currentAsteroid.GetComponent<AutoRotate>().enabled = rotating;
                }

            }
            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }
        }

        private void ChangeAsteroid()
        {
            if(currentAsteroid)
            {
                Destroy(currentAsteroid);
            }

            currentAsteroid = Instantiate(AsteroidPrefab);
            currentAsteroid.GetComponent<AsteroidDeformation>().enabled = true;
            currentAsteroid.GetComponent<AsteroidDetailer>().enabled = true;
            currentAsteroid.GetComponent<AutoRotate>().enabled = rotating;
            currentAsteroid.transform.rotation = rotation;
            currentAsteroid.SetActive(true);
        }

        private void Hit(Vector3 mousePosition)
        {
            Ray ray = cam.ScreenPointToRay(mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                AsteroidDeformation deformer = hit.transform.GetComponent<AsteroidDeformation>();

                if (deformer != null)
                {
                    deformer.Hit(hit.point);
                }
            }
        }
    }
}
