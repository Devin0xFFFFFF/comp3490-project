using MeshVoxelizerProject;
using UnityEngine;
using VolumetricLines;

namespace Comp3490Project
{
    public class MiningLaser : MonoBehaviour
    {
        public VolumetricLineBehavior Laser;
        public AudioSource HumSound;
        public AudioSource PulseSound;
        public AudioSource OverheatSound;
        public AudioSource NotReadySound;

        public float FireRange = 100.0f;
        public float MaxFiringDuration = 5;
        public float CooldownDuration = 5;

        private bool firing;
        private bool coolingDown;
        private float accumulatedTime;
        private Light laserLight;

        [Range(0,16)]
        public int xoffset = 0;
        [Range(0, 16)]
        public int yoffset = 0;
        [Range(0, 16)]
        public int zoffset = 0;


        public float Heat { get { return accumulatedTime; } }

        private void Awake()
        {
            firing = false;
            coolingDown = false;
            accumulatedTime = 0;

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
                accumulatedTime += Time.deltaTime;
                if(accumulatedTime > MaxFiringDuration)
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
                accumulatedTime -= Time.deltaTime;
                if(accumulatedTime <= 0)
                {
                    coolingDown = false;
                }
            }

            accumulatedTime = Mathf.Clamp(accumulatedTime, 0, MaxFiringDuration);
            
            if(accumulatedTime > 0)
            {
                if(!Laser.gameObject.activeInHierarchy)
                {
                    Laser.gameObject.SetActive(true);
                }
                if(!firing)
                {
                    Laser.LineWidth = Mathf.Lerp(Laser.LineWidth, 0, Time.deltaTime*2);
                    laserLight.range = Mathf.Lerp(Laser.LineWidth, 0, Time.deltaTime * 2);
                }
                else
                {
                    Laser.LineWidth = Mathf.Lerp(0, 100, accumulatedTime) + Mathf.PingPong(Time.time, 0.75f) * 200;
                    laserLight.range = Mathf.Lerp(0, 100, accumulatedTime) + Mathf.PingPong(Time.time, 0.75f) * 200;  
                }
            }
            else if(Laser.gameObject.activeInHierarchy)
            {
                Laser.gameObject.SetActive(false);
            }
        }
    }
}
