using UnityEngine;
using UnityEngine.UI;

namespace Comp3490Project
{
    public class HUD : MonoBehaviour
    {
        public GameObject Ship;
        public Text VelocityText;
        public Text RotationText;
        public Slider LaserHeat;
        public Text RadarStatus;
        public Text TurretStatus;
        public Text FlightAssistStatus;

        private Rigidbody shipRigidbody;
        private MiningLaser laser;
        private ShipController shipController;
        private TurretController turretController;

        private void Awake()
        {
            shipRigidbody = Ship.GetComponent<Rigidbody>();
            laser = Ship.GetComponentInChildren<MiningLaser>();
            shipController = Ship.GetComponent<ShipController>();
            turretController = Ship.GetComponentInChildren<TurretController>();

            LaserHeat.maxValue = laser.MaxFiringDuration;
        }

        private void Update()
        {
            LaserHeat.value = laser.Heat;
            RadarStatus.text = "Radar: " + (shipController.RadarOn ? "On" : "Off");
            TurretStatus.text = "Turrets: " + (turretController.Enabled ? "On" : "Off");
            FlightAssistStatus.text = "Flight Assist: " + (shipController.flightAssist ? "On" : "Off");
        }

        private void FixedUpdate()
        {
            Vector3 velocity = shipRigidbody.velocity;
            VelocityText.text = "Velocity: " + "X=" + velocity.x.ToString("0.00") + 
                ", Y=" + velocity.y.ToString("0.00") + 
                ", Z=" + velocity.z.ToString("0.00");

            Vector3 eulerAngles = shipRigidbody.rotation.eulerAngles;
            RotationText.text = "Rotation: " + "X=" + eulerAngles.x.ToString("0.00") + 
                ", Y=" + eulerAngles.y.ToString("0.00") + 
                ", Z=" + eulerAngles.z.ToString("0.00");
        }
    }
}
