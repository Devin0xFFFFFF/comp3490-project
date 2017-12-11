using UnityEngine;

namespace Comp3490Project
{
    public class GameManager : MonoBehaviour
    {
        private static Body[] bodies;

        public Texture2D CursorTexture;

        private void Start()
        {
            Cursor.SetCursor(CursorTexture, Vector2.zero, CursorMode.Auto);
        }

        public static void SetBodies(Body[] bodies)
        {
            GameManager.bodies = bodies;
        }

        public static Body[] GetBodies()
        {
            return bodies;
        }

        private void Update()
        {

        }
    }
}
