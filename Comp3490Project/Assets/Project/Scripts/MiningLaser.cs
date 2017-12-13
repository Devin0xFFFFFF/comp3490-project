using UnityEngine;
using VolumetricLines;

namespace Comp3490Project
{
    // Uses Volumetric Lines from: https://assetstore.unity.com/packages/tools/particles-effects/volumetric-lines-29160
    public class MiningLaser : MonoBehaviour
    {
        public VolumetricLineBehavior Laser;
        public AudioSource HumSound;
        public AudioSource PulseSound;
        public AudioSource OverheatSound;
        public AudioSource NotReadySound;
        public AudioSource HitSound;

        public float FireRange = 100.0f;
        public float MaxFiringDuration = 5;
        public float CooldownDuration = 5;

        private bool firing;
        private bool coolingDown;
        private float accumulatedHeat;
        private float shootAccumulatedTime;
        private Light laserLight;

        public float Heat { get { return accumulatedHeat; } }

        private void Awake()
        {
            firing = false;
            coolingDown = false;
            accumulatedHeat = 0;
            shootAccumulatedTime = 0;

            laserLight = Laser.GetComponent<Light>();
        }

        public void ToggleFiring()
        {
            if(coolingDown)
            {
                NotReadySound.Play();
                return;
            }

            firing = !firing;

            if (firing)
            {
                HumSound.Play();
                PulseSound.Play();
            }
            else
            {
                HumSound.Stop();
                PulseSound.Stop();
            }
        }

        private void Update()
        {
            if(firing)
            {
                accumulatedHeat += Time.deltaTime;
                if(accumulatedHeat > MaxFiringDuration)
                {
                    coolingDown = true;
                    firing = false;
                    HumSound.Stop();
                    PulseSound.Stop();
                    OverheatSound.Play();
                }
            }
            else
            {
                accumulatedHeat -= Time.deltaTime * CooldownDuration;
                if(accumulatedHeat <= 0)
                {
                    coolingDown = false;
                }
            }

            accumulatedHeat = Mathf.Clamp(accumulatedHeat, 0, MaxFiringDuration);
            
            if(accumulatedHeat > 0)
            {
                if(!Laser.gameObject.activeInHierarchy)
                {
                    shootAccumulatedTime = int.MaxValue;
                    Laser.gameObject.SetActive(true);
                }
                if(!firing)
                {
                    Laser.LineWidth = Mathf.Lerp(Laser.LineWidth, 0, Time.deltaTime*2);
                    laserLight.range = Mathf.Lerp(Laser.LineWidth, 0, Time.deltaTime * 2);
                }
                else
                {
                    Laser.LineWidth = Mathf.Lerp(0, 100, accumulatedHeat) + Mathf.PingPong(Time.time, 0.75f) * 200;
                    laserLight.range = Mathf.Lerp(0, 100, accumulatedHeat) + Mathf.PingPong(Time.time, 0.75f) * 200;
                    Laser.EndPos = new Vector3(0, 0, Mathf.Lerp(0, 10000, accumulatedHeat * 4));

                    Shoot();
                }
            }
            else if(Laser.gameObject.activeInHierarchy)
            {
                shootAccumulatedTime = 0;
                Laser.gameObject.SetActive(false);
            }
        }

        private void Shoot()
        {
            shootAccumulatedTime += Time.deltaTime;

            RaycastHit hit;
            if (Physics.Raycast(Laser.transform.position, Laser.transform.TransformDirection(Vector3.forward), out hit, Laser.EndPos.z))
            {
                AsteroidDeformation deformer = hit.transform.GetComponent<AsteroidDeformation>();

                if (deformer != null)
                {
                    deformer.Hit(hit.point);

                    if(shootAccumulatedTime > HitSound.clip.length + 0.001f)
                    {
                        HitSound.Stop();
                        HitSound.pitch = Random.Range(-1.0f, 1.5f);
                        HitSound.Play();
                        shootAccumulatedTime = 0;
                    }
                }
            }
        }
    }
}
