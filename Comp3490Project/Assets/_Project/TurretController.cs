using System;
using System.Collections.Generic;
using UnityEngine;

namespace Comp3490Project
{
    public class TurretController : MonoBehaviour
    {
        public AudioSource TurretEnableAudio;
        public Turret[] Turrets;

        private List<GameObject> hostileObjectsInRange;
        private bool turretsEnabled;

        public bool Enabled { get { return turretsEnabled; } }

        private void Awake()
        {
            hostileObjectsInRange = new List<GameObject>();
            turretsEnabled = false;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                ToggleEnabled();
            }
        }

        public void ToggleEnabled()
        {
            SetEnabled(!turretsEnabled);
        }

        public void SetEnabled(bool enabled)
        {
            for (int i = 0; i < Turrets.Length; i++)
            {
                Turrets[i].SetEnabled(enabled);
            }

            if (enabled)
            {
                TurretEnableAudio.Play();
                CalculateTargets();
            }

            turretsEnabled = enabled;
        }

        private void CalculateTargets()
        {
            for (int i = 0; i < hostileObjectsInRange.Count; i++)
            {
                for (int j = 0; j < Turrets.Length; j++)
                {
                    Turrets[j].TrySetTarget(hostileObjectsInRange[i]);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            hostileObjectsInRange.Add(other.gameObject);
            CalculateTargets();
        }

        private void OnTriggerExit(Collider other)
        {
            hostileObjectsInRange.Remove(other.gameObject);
            CalculateTargets();
        }
    }
}
