using UnityEngine;

namespace Comp3490Project
{
    //https://assetstore.unity.com/packages/tools/free-turret-script-56646
    //https://assetstore.unity.com/packages/tools/ai/simple-turret-system-92160

    public class TurretTracking : MonoBehaviour
    {
        public Transform YawSegment;
        public Transform PitchSegment;
        public LineRenderer TrackingLine;

        public float TrackingRange = 80;

        public float YawSpeed = 30;
        public float PitchSpeed = 30;
        public float YawRotationOrigin = 0;
        public float PitchRotationOrigin = 0;
        public float YawLimit = 360;
        public float PitchLimit = 30;

        public Vector3 Target;

        private Quaternion yawSegmentOriginRotation;
        private Quaternion pitchSegmentOriginRotation;

        private Quaternion yawSegmentStartRotation;
        private Quaternion pitchSegmentStartRotation;

        private bool trackingEnabled;

        private void Start()
        {
            yawSegmentOriginRotation = YawSegment.localRotation;
            pitchSegmentOriginRotation = PitchSegment.localRotation;
            yawSegmentStartRotation = Quaternion.Euler(0, YawRotationOrigin, 0);
            pitchSegmentStartRotation = Quaternion.Euler(PitchRotationOrigin, 0, 0);
            SetEnabled(false);
        }

        public bool LockedOn()
        {
            if(!trackingEnabled)
            {
                return false;
            }

            RaycastHit hit;
            bool hitObject = Physics.Raycast(TrackingLine.transform.position, TrackingLine.transform.forward, out hit, TrackingRange);

            if (hitObject)
            {
                TrackingLine.SetPosition(1, new Vector3(0, 0, hit.distance));//if raycast hit somewhere then stop laser effect at that point
            }
            else
            {
                TrackingLine.SetPosition(1, new Vector3(0, 0, TrackingRange));//if not hit, laser till range 
            }

            return hitObject;
        }

        public void SetEnabled(bool enabled)
        {
            trackingEnabled = enabled;
            TrackingLine.gameObject.SetActive(trackingEnabled);
        }

        private void Update()
        {
            if(trackingEnabled)
            {
                TrackTarget();
            }
            else
            {
                YawSegment.localRotation = Quaternion.Lerp(YawSegment.localRotation, yawSegmentOriginRotation, Time.deltaTime);
                PitchSegment.localRotation = Quaternion.Lerp(PitchSegment.localRotation, pitchSegmentOriginRotation, Time.deltaTime);
            }
        }

        private void TrackTarget()
        {
            float angle;
            Vector3 targetRelative;
            Quaternion targetRotation;

            if (YawSegment && YawLimit != 0)
            {
                targetRelative = YawSegment.InverseTransformPoint(Target);
                angle = Mathf.Atan2(targetRelative.x, targetRelative.z) * Mathf.Rad2Deg;
                if (angle >= 180f)
                {
                    angle = 180f - angle;
                }
                if (angle <= -180f)
                {
                    angle = -180f + angle;
                }
                targetRotation = YawSegment.rotation * Quaternion.Euler(0f, Mathf.Clamp(angle, -YawSpeed * Time.deltaTime, YawSpeed * Time.deltaTime), 0f);
                if (YawLimit < 360f && YawLimit > 0f)
                {
                    YawSegment.rotation = Quaternion.RotateTowards(YawSegment.parent.rotation * yawSegmentStartRotation, targetRotation, YawLimit);
                }
                else
                {
                    YawSegment.rotation = targetRotation;
                }
            }

            if (PitchSegment && PitchLimit != 0f)
            {
                targetRelative = PitchSegment.InverseTransformPoint(Target);
                angle = -Mathf.Atan2(targetRelative.y, targetRelative.z) * Mathf.Rad2Deg;
                if (angle >= 180f) angle = 180f - angle; if (angle <= -180f) angle = -180f + angle;
                targetRotation = PitchSegment.rotation * Quaternion.Euler(Mathf.Clamp(angle, -PitchSpeed * Time.deltaTime, PitchSpeed * Time.deltaTime), 0f, 0f);
                if (PitchLimit < 360f && PitchLimit > 0f) PitchSegment.rotation = Quaternion.RotateTowards(PitchSegment.parent.rotation * pitchSegmentStartRotation, targetRotation, PitchLimit);
                else PitchSegment.rotation = targetRotation;
            }
        }
    }

}