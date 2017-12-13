using SuperSystems.UnityTools;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

namespace Comp3490Project
{
    // Uses FreeCamera from: https://github.com/ttammear/unitymcubes
    // Uses AutoRotate from: https://github.com/Chaser324/unity-wireframe
    // Uses SmoothFollow from: https://www.assetstore.unity3d.com/en/#!/content/32351
    public class SimCamera : MonoBehaviour
    {
        public MainMenu MainMenu;
        public Text HUDText;

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

            HUDText.text = "";
            HUDText.gameObject.SetActive(true);
        }

        public void SwitchToMenuMode()
        {
            nBodySimManager.enabled = false;
            freeCamera.enabled = false;
            autoRotate.enabled = true;
            smoothFollow.enabled = true;

            HUDText.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                nBodySimManager.BoundingSphere.SetActive(false);
                SwitchToMenuMode();
                MainMenu.gameObject.SetActive(true);
            }
        }
    }
}
