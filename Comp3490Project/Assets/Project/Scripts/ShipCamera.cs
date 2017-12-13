using UnityEngine;

namespace Comp3490Project
{
    // Camera Controls based off of : https://github.com/densylkin/RTS_Camera
    public class ShipCamera : MonoBehaviour
    {
        public KeyCode mouseRotationKey = KeyCode.Mouse1;
        public string zoomingAxis = "Mouse ScrollWheel";
        public float mouseRotationSpeed = 10f;
        public float scrollWheelZoomingSensitivity = 25f;
        public float maxZoom = 20f; //maximal height
        public float minZoom = 15f; //minimnal height
        public float ZoomDampening = 5f;
        public float InitialZoom = 0.3f;

        public Material ScanEffectMaterial;
        public float MaxScanDistance = 100;
        public AudioSource ScanAudio;

        private float scanDistance = 0;

        private float zoomPos = 0.3f;

        private Vector3 startingPosition;
        private Quaternion startingRotation;
        private bool radarMode;

        private Camera shipCamera;

        private float ScrollWheel
        {
            get { return Input.GetAxis(zoomingAxis); }
        }

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private void Start()
        {
            shipCamera = GetComponent<Camera>();
            shipCamera.depthTextureMode = DepthTextureMode.Depth;

            zoomPos = InitialZoom;
            startingPosition = transform.localPosition;
            startingRotation = transform.localRotation;
        }

        private void Update()
        {
            if(radarMode)
            {
                //Easing modified from https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBase/Tween/Easing.cs
                float t = Time.deltaTime * 25;
                float b = scanDistance;
                float c = MaxScanDistance;
                float d = 2;
                scanDistance = (t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b - c * 0.001f;
                if (scanDistance > MaxScanDistance)
                {
                    ScanAudio.Play();
                    scanDistance = 0;
                }
            }
        }

        public void SetRadarMode(bool on, bool playAudio = true)
        {
            radarMode = on;
            scanDistance = 0;
            if(radarMode && playAudio)
            {
                ScanAudio.Play();
            }
            else
            {
                ScanAudio.Stop();
            }
        }

        public void ResetRotation()
        {
            transform.localRotation = startingRotation;
        }

        public void Zoom()
        {
            zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            zoomPos = Mathf.Clamp01(zoomPos);
            float targetHeight = Mathf.Lerp(minZoom, maxZoom, zoomPos);
            transform.localPosition = Vector3.Lerp(transform.localPosition,
                    new Vector3(transform.localPosition.x, transform.localPosition.y, targetHeight),
                    Time.deltaTime * ZoomDampening);
        }

        public void Rotation()
        {
            transform.Rotate(Vector3.up, -MouseAxis.x * Time.deltaTime * mouseRotationSpeed, Space.Self);
            transform.Rotate(Vector3.right, -MouseAxis.y * Time.deltaTime * mouseRotationSpeed, Space.Self);
        }

        //Everything below modified from https://github.com/Broxxar/NoMansScanner/

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            ScanEffectMaterial.SetVector("_WorldSpaceScannerPos", transform.position);
            ScanEffectMaterial.SetFloat("_ScanDistance", scanDistance);
            RaycastCornerBlit(src, dst, ScanEffectMaterial);
        }

        void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
        {
            // Compute Frustum Corners
            float camFar = shipCamera.farClipPlane;
            float camFov = shipCamera.fieldOfView;
            float camAspect = shipCamera.aspect;

            float fovWHalf = camFov * 0.5f;

            Vector3 toRight = shipCamera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
            Vector3 toTop = shipCamera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            Vector3 topLeft = (shipCamera.transform.forward - toRight + toTop);
            float camScale = topLeft.magnitude * camFar;

            topLeft.Normalize();
            topLeft *= camScale;

            Vector3 topRight = (shipCamera.transform.forward + toRight + toTop);
            topRight.Normalize();
            topRight *= camScale;

            Vector3 bottomRight = (shipCamera.transform.forward + toRight - toTop);
            bottomRight.Normalize();
            bottomRight *= camScale;

            Vector3 bottomLeft = (shipCamera.transform.forward - toRight - toTop);
            bottomLeft.Normalize();
            bottomLeft *= camScale;

            // Custom Blit, encoding Frustum Corners as additional Texture Coordinates
            RenderTexture.active = dest;

            mat.SetTexture("_MainTex", source);

            GL.PushMatrix();
            GL.LoadOrtho();

            mat.SetPass(0);

            GL.Begin(GL.QUADS);

            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.MultiTexCoord(1, bottomLeft);
            GL.Vertex3(0.0f, 0.0f, 0.0f);

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.MultiTexCoord(1, bottomRight);
            GL.Vertex3(1.0f, 0.0f, 0.0f);

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.MultiTexCoord(1, topRight);
            GL.Vertex3(1.0f, 1.0f, 0.0f);

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.MultiTexCoord(1, topLeft);
            GL.Vertex3(0.0f, 1.0f, 0.0f);

            GL.End();
            GL.PopMatrix();
        }
    }

}