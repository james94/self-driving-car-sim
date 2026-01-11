using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class BrakeLight : BrakeLightBase
    {
        public CarController car; // reference to the car controller, must be dragged in inspector

        // private Renderer m_Renderer;


        // private void Start()
        // {
        //     m_Renderer = GetComponent<Renderer>();
        // }


        // private void Update()
        // {
        //     // enable the Renderer when the car is braking, disable it otherwise.
        //     m_Renderer.enabled = GetBrakeInput() > 0f;
        // }

        protected override float GetBrakeInput()
        {
            return car != null ? car.BrakeInput : 0f;
        }
    }
}
