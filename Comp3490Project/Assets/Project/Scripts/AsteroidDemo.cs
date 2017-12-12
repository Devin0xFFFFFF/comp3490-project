using UnityEngine;

namespace Comp3490Project
{
    public class AsteroidDemo : MonoBehaviour
    {
        private Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Hit(Input.mousePosition);
            }
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
