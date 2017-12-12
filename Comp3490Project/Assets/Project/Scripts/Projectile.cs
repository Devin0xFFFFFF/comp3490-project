using UnityEngine;

namespace Comp3490Project
{
    public class Projectile : MonoBehaviour
    {
        public float Damage = 1;
        public float Speed = 100;
        public float Duration = 2;

        private void Start()
        {
            if(Duration > 0)
            {
                Destroy(gameObject, Duration);
            }
        }

        private void Update()
        {
            transform.position += transform.forward * Time.deltaTime * Speed;
        }
    }
}
