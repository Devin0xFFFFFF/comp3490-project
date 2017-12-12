using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshVoxelizerProject;


namespace Comp3490Project
{


    public class cameraTesting : MonoBehaviour
    {
        Camera cam;
        [Range(0, 16)]
        public int xoffset = 0;
        [Range(0, 16)]
        public int yoffset = 0;
        [Range(0, 16)]
        public int zoffset = 0;
        // Use this for initialization
        void Start()
        {
            cam = GetComponent<Camera>();

        }

        // Update is called once per frame
        void Update()
        {
            
            
            Vector3 mousePosition = Input.mousePosition;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                IsTargetVisible(mousePosition);
            }




        }
        private void IsTargetVisible(Vector3 mousePosition)
        {
            //SphereVoxelizerTest.pos - Laser.StartPos
            Ray ray = cam.ScreenPointToRay(new Vector3(200, 200, 0));
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            Debug.LogFormat("are we here yet?");

            RaycastHit h;
            if (Physics.Raycast(ray, out h))
            {
                Vector3 fwd = transform.TransformDirection(Vector3.forward);
                int[,,] voxels = SphereVoxelizerTest.voxels;
                Vector3 hit = SphereVoxelizerTest.sphere.transform.InverseTransformPoint(h.point);// world to local co-ordinites

                //float size = 0.770306f;
                //Box3 bounds = new Box3(SphereVoxelizerTest.meshFilter.mesh.bounds.min, SphereVoxelizerTest.meshFilter.mesh.bounds.max);
                //Vector3 scale = new Vector3(h.point.x * size, h.point.y * size, h.point.z * size);



                System.Random random = new System.Random();

                //int x = (int)(hit.x);
                //int y = (int)(hit.y);
                //int z = (int)(hit.z);


                int x = (int)(xoffset);
                int y = (int)(yoffset);
                int z = (int)(zoffset);

                Debug.LogFormat("Object:{0},x:{1},y:{2},z:{3}", hit, x, y, z);

                // fall back astroid destruction code
                //int x = random.Next(0, 16);
                //int y = random.Next(0, 16);
                //int z = random.Next(0, 16);

                SphereVoxelizerTest.voxels[x, y, z] = 0;// out of bounds 

                SphereVoxelizerTest.changes();
            }
        }
    }
}