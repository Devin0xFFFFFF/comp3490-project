using SuperSystems.UnityTools;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace Comp3490Project
{
    public class SimCamera : MonoBehaviour
    {
        private NBodySimManager nBodySimManager;
        private FreeCamera freeCamera;
        private AutoRotate autoRotate;
        private SmoothFollow smoothFollow;

        private void Awake()
        {
            nBodySimManager = GetComponent<NBodySimManager>();
            freeCamera = GetComponent<FreeCamera>();
            autoRotate = GetComponent<AutoRotate>();
            smoothFollow = GetComponent<SmoothFollow>();
        }

        public void SwitchToGameMode()
        {
            nBodySimManager.enabled = true;
            freeCamera.enabled = true;
            autoRotate.enabled = false;
            smoothFollow.enabled = false;
        }

        public void SwitchToMenuMode()
        {
            nBodySimManager.enabled = false;
            freeCamera.enabled = false;
            autoRotate.enabled = true;
            smoothFollow.enabled = true;
        }
    }
}
