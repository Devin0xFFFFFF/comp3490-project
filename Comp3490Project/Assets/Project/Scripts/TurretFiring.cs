using UnityEngine;

namespace Comp3490Project
{
    public class TurretFiring : MonoBehaviour
    {
        public float FireRate = 0.1f;
        public float FireRange = 50.0f;
        public GameObject ShotPrefab;
        public AudioClip ShotSound;
        public ParticleSystem MuzzleParticles;
        public GameObject[] MuzzlePoints;

        private AudioSource audioSource;
        
        private float accumulatedTime;
        private int currentMuzzle;
        private float currentFireRate;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            accumulatedTime = 0;
            currentMuzzle = 0;
            currentFireRate = Random.Range(FireRate - 0.1f, FireRate + 0.1f);
        }

        private void Update()
        {
            if (accumulatedTime <= currentFireRate)
            {
                accumulatedTime += Time.deltaTime;
            }
        }

        public void Fire()
        {
            if(accumulatedTime > currentFireRate)
            {
                currentFireRate = Random.Range(FireRate - 0.1f, FireRate + 0.1f);
                if (MuzzleParticles != null)
                {
                    MuzzleParticles.Stop();
                    MuzzleParticles.Play();
                }
                
                if(ShotSound != null)
                {
                    audioSource.PlayOneShot(ShotSound);
                }

                Transform muzzleTransform = MuzzlePoints[currentMuzzle].transform;
                Instantiate(ShotPrefab, muzzleTransform.position, muzzleTransform.rotation);
                currentMuzzle = (currentMuzzle + 1) % MuzzlePoints.Length;

                accumulatedTime = 0;
            }
        }
    }
}
