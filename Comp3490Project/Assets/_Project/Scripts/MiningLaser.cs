using UnityEngine;
using VolumetricLines;
//using SphereVoxelizerTest;

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

                    IsTargetVisible();
                }
            }
            else if(Laser.gameObject.activeInHierarchy)
            {
                Laser.gameObject.SetActive(false);
            }
        }
        private void IsTargetVisible()
        {
            //SphereVoxelizerTest.pos - Laser.StartPos
            RaycastHit h;
            if(Physics.Raycast(Laser.transform.position, Laser.transform.TransformDirection(Vector3.forward), out h, laserLight.range))
            {
                Vector3 fwd = transform.TransformDirection(Vector3.forward);
                int[,,] voxels = SphereVoxelizerTest.voxels;
                Vector3 hit = SphereVoxelizerTest.sphere.transform.TransformPoint(h.point);// world to local co-ordinites
                Debug.LogFormat("World:{0}, Object:{1}", h.point, hit / 16);
                SphereVoxelizerTest.voxels[(int)hit.x / 16, (int)hit.y / 16, (int)hit.z / 16] = 0;// out of bounds 


            }
        }
    }
}
