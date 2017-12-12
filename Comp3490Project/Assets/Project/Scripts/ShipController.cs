using UnityEngine;

/**
 * Modified from: https://github.com/M4deM4n/ShipController
 * A Unity3D player controller inspired by Elite: Dangerous
 * Original Author: Jeff Pizano
 **/
namespace Comp3490Project
{
    [RequireComponent(typeof(Rigidbody))]
    public class ShipController : MonoBehaviour
    {
        public MiningLaser Laser;

        Rigidbody ship;

        float qtrScreenH;
        float qtrScreenW;

        bool adjustPitch = false;
        bool adjustYaw = false;
        bool adjustRoll = false;
        bool adjustThrustX = false;
        bool adjustThrustY = false;
        bool adjustThrustZ = false;

        [ReadOnly]
        public Vector3 mousePosition;
        Vector3 centerScreen;

        float pitch = 0.0f;
        float yaw = 0.0f;
        float roll = 0.0f;

        float pitchDiff = 0.0f;
        float yawDiff = 0.0f;

        public Vector3 thrust = Vector3.zero;

        // THROTTLE
        public float throttle = 100f;
        [Range(0, 50)]
        public float throttleAmount = 0.25f;
        [Range(0, 500f)]
        public float maxThrottle = 4f;
        [Range(-500, 100f)]
        public float minThrottle = -2f;

        // FLIGHT CONTROL PARAMETERS
        [Range(0, 100f)]
        public float pitchStrength = 1.5f;
        [Range(0, 100f)]
        public float yawStrength = 1.5f;
        [Range(0, 10f)]
        public float rollStrength = 1.5f;

        public bool flightAssist = false;

        private bool freeLook = false;
        private bool radarOn = false;

        private ShipCamera frontCamera;
        private ShipCamera observerCamera;
        private ShipCamera currentCamera;
        private bool isCurrentlyFrontCamera;

        public float PitchSensitivity;
        public float YawSensitivity;

        public bool RadarOn { get { return radarOn; } }

        private void Awake()
        {
            ShipCamera[] cameras = GetComponentsInChildren<ShipCamera>();
            frontCamera = cameras[0];
            observerCamera = cameras[1];
            currentCamera = observerCamera;
            frontCamera.gameObject.SetActive(false);
            isCurrentlyFrontCamera = false;
        }

        private void Start()
        {
            centerScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            ship = GetComponent<Rigidbody>();
            qtrScreenH = Screen.height * 0.25f;
            qtrScreenW = Screen.width * 0.25f;
        }

        private void Update()
        {
            InputUpdate();

            if (flightAssist)
            {
                DampenTransform();
            }
        }

        private void ToggleView()
        {
            frontCamera.gameObject.SetActive(!isCurrentlyFrontCamera);
            observerCamera.gameObject.SetActive(isCurrentlyFrontCamera);

            isCurrentlyFrontCamera = !isCurrentlyFrontCamera;

            currentCamera.SetRadarMode(false);
            currentCamera = isCurrentlyFrontCamera ? frontCamera : observerCamera;
            currentCamera.SetRadarMode(radarOn);
        }

        private void FixedUpdate()
        {
            InputFixedUpdate();
        }

        private void InputUpdate()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))// || Input.GetKeyUp(KeyCode.Alpha1))
            {
                Laser.ToggleFiring();
            }

            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                radarOn = !radarOn;
                currentCamera.SetRadarMode(radarOn);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                ToggleView();
            }

            currentCamera.Zoom();

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                freeLook = true;
            }
            else if(Input.GetKeyUp(KeyCode.LeftAlt))
            {
                currentCamera.ResetRotation();
                freeLook = false;
            }

            if(freeLook)
            {
                currentCamera.Rotation();
                return;
            }

            mousePosition = Input.mousePosition;
            pitch = GetPitchValue();
            yaw = GetYawValue();
            roll = GetRollValue();
            thrust.x = Input.GetAxis("Horizontal");
            thrust.y = GetThrustY();
            thrust.z = Input.GetAxis("Vertical"); // Z is forward/Back

            // Set Flags
            adjustPitch = Mathf.Abs(pitch) > 0.1f;
            adjustYaw = Mathf.Abs(yaw) > 0.1f;
            adjustRoll = roll != 0;
            adjustThrustX = Mathf.Abs(thrust.x) > 0.1f;
            adjustThrustY = thrust.y != 0;
            adjustThrustZ = Mathf.Abs(thrust.z) > 0.1f;


            // Throttle up
            if (Input.GetKey(KeyCode.Equals))
            {
                throttle += throttleAmount;
            }

            // Throttle down
            if (Input.GetKey(KeyCode.Minus))
            {
                throttle -= throttleAmount;
            }

            // Toggle Inertial dampeners
            if (Input.GetKeyUp(KeyCode.Z))
            {
                flightAssist = !flightAssist;
            }

            throttle = Mathf.Clamp(throttle, minThrottle, maxThrottle);
        }

        private void InputFixedUpdate()
        {
            // ADJUST PITCH (FORWARD/BACK/TILT/LOCAL X)
            if (adjustPitch)
                ship.AddTorque(transform.right * (-pitch * pitchStrength), ForceMode.Force);

            // ADJUST YAW (LEFT/RIGHT/TURN/LOCAL Y)
            if (adjustYaw)
            {
                ship.AddTorque(transform.up * (yaw * yawStrength), ForceMode.Force);
            }

            // ADJUST ROLL (CLOCKWISE/COUNTERCLOCKWISE/LOCAL Z)
            if (adjustRoll)
            {
                ship.AddTorque(transform.forward * (roll * rollStrength), ForceMode.Force);
            }

            // ADJUST THRUST Z (FORWARD/BACK/LOCAL Z)
            if (adjustThrustZ)
            {
                ship.AddForce(transform.forward * (thrust.z * throttle), ForceMode.Force);
            }

            // ADJUST THRUST X (LEFT/RIGHT/STRAFE/LOCAL X)
            if (adjustThrustX)
            {
                ship.AddForce(transform.right * (thrust.x * throttle), ForceMode.Force);
            }

            // ADJUST THRUST Y (UP/DOWN/ASCEND/DESCEND/LOCAL Y)
            if (adjustThrustY)
            {
                ship.AddForce(transform.up * (throttle * thrust.y), ForceMode.Force);
            }
        }

        private float GetPitchValue()
        {
            pitchDiff = -(centerScreen.y - mousePosition.y);
            pitchDiff = Mathf.Clamp(pitchDiff, -qtrScreenH, qtrScreenH);

            if(Mathf.Abs(pitchDiff) < PitchSensitivity)
            {
                pitchDiff = 0;
            }

            return (pitchDiff / qtrScreenH);
        }

        private float GetYawValue()
        {
            yawDiff = -(centerScreen.x - mousePosition.x);
            yawDiff = Mathf.Clamp(yawDiff, -qtrScreenW, qtrScreenW);

            if(Mathf.Abs(yawDiff) < YawSensitivity)
            {
                yawDiff = 0;
            }

            return (yawDiff / qtrScreenW);
        }

        private float GetRollValue()
        {
            if (Input.GetKey(KeyCode.Q))
                return 1.0f;

            if (Input.GetKey(KeyCode.E))
                return -1.0f;

            return 0;
        }

        private float GetThrustY()
        {
            if (Input.GetKey(KeyCode.Space))
                return 1;

            if (Input.GetKey(KeyCode.LeftShift))
                return -1;

            return 0.0f;
        }

        private void DampenTransform()
        {
            Vector3 nVeloc = new Vector3(
                Mathf.Lerp(ship.velocity.x, 0, Time.deltaTime * 0.75f),
                Mathf.Lerp(ship.velocity.y, 0, Time.deltaTime * 0.75f),
                Mathf.Lerp(ship.velocity.z, 0, Time.deltaTime * 0.75f)
                );

            Vector3 nAVeloc = new Vector3(
                Mathf.Lerp(ship.angularVelocity.x, 0, Time.deltaTime),
                Mathf.Lerp(ship.angularVelocity.y, 0, Time.deltaTime),
                Mathf.Lerp(ship.angularVelocity.z, 0, Time.deltaTime)
                );

            ship.velocity = nVeloc;
            ship.angularVelocity = nAVeloc;
        }
    }
}
