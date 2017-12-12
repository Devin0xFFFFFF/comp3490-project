using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEngine : MonoBehaviour
{
    [Serializable]
    public class ShipEngineSystem
    {
        private ParticleSystem particleSystem;
        private ParticleSystem.MainModule psMain;
        private float initialMinStartSize;

        public ShipEngineSystem(ParticleSystem particleSystem)
        {
            this.particleSystem = particleSystem;
            psMain = particleSystem.main;
            initialMinStartSize = psMain.startSize.constantMin;
        }

        public void Update(float speed)
        {
            psMain.startSpeed = Mathf.Clamp(speed/10, 0, 3);
        }
    }

    private ShipEngineSystem[] engineSystems;
    private Rigidbody rigidBody;
    private AudioSource engineAudio;

    private void Awake()
    {
        ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
        engineSystems = new ShipEngineSystem[systems.Length];
        for (int i = 0; i < systems.Length; i++)
        {
            engineSystems[i] = new ShipEngineSystem(systems[i]);
        }

        rigidBody = GetComponentInParent<Rigidbody>();

        engineAudio = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < engineSystems.Length; i++)
        {
            engineSystems[i].Update(rigidBody.velocity.magnitude);
            engineAudio.volume = Mathf.Clamp01(rigidBody.velocity.magnitude / 50);
        }
    }
}
