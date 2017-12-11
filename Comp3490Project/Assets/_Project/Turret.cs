﻿using UnityEngine;

namespace Comp3490Project
{
    public class Turret : MonoBehaviour
    {
        private TurretTracking tracking;
        private TurretFiring firing;
        private GameObject target;
        private Vector3 originForwardPosition;

        private bool trackingEnabled;

        private void Awake()
        {
            tracking = GetComponent<TurretTracking>();
            firing = GetComponent<TurretFiring>();
            SetEnabled(false);
        }

        public void SetEnabled(bool enabled)
        {
            tracking.SetEnabled(enabled);
            trackingEnabled = enabled;
        }

        public void TrySetTarget(GameObject newTarget)
        {
            if(target == null || Vector3.Distance(transform.position, newTarget.transform.position) < 
                Vector3.Distance(transform.position, target.transform.position))
            {
                target = newTarget;
            }
        }

        private void FixedUpdate()
        {
            if(trackingEnabled && target != null)
            {
                tracking.Target = target.transform.position;
                if(tracking.LockedOn() && Vector3.Distance(transform.position, target.transform.position) < firing.FireRange)
                {
                    firing.Fire();
                }
            }
        }
    }
}
