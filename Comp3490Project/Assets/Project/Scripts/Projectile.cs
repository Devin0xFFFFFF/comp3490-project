using System.Collections;
using UnityEngine;

namespace Comp3490Project
{
    public class Projectile : MonoBehaviour
    {
        public float Damage = 1;
        public float Speed = 100;
        public float Duration = 2;
        public float EnableColliderDelay = 0.1f;

        private CapsuleCollider capsuleCollider;

        private void Awake()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            if(Duration > 0)
            {
                Destroy(gameObject, Duration);
            }

            StartCoroutine(EnableColliderAfterSeconds());
        }

        private IEnumerator EnableColliderAfterSeconds()
        {
            yield return new WaitForSeconds(EnableColliderDelay);
            capsuleCollider.enabled = true;
        }

        private void FixedUpdate()
        {
            transform.position += transform.forward * Time.deltaTime * Speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            AsteroidDeformation deformer = collision.transform.GetComponent<AsteroidDeformation>();

            if(deformer != null)
            {
                deformer.Hit(collision.contacts[0].point);
            }
        }
    }
}
