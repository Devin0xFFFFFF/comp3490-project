using System.Collections.Generic;
using UnityEngine;

namespace Comp3490Project
{
    public class Body
    {
        public List<Vector3> Points;

        public Body() { Points = new List<Vector3>(); }

        public int Size { get { return Points.Count; } }

        public Vector3 GetPosition()
        {
            float x, y, z;
            x = y = z = 0;

            for (int i = 0; i < Points.Count; i++)
            {
                x += Points[i].x;
                y += Points[i].y;
                z += Points[i].z;
            }

            return new Vector3(x / Points.Count, y / Points.Count, z / Points.Count);
        }
    }
}
