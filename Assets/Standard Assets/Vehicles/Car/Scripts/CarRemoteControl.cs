using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarRemoteControl : CarRemoteControlBase
    {
        private CarController m_Car; // the car controller we want to use

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            base.Awake();
        }

        protected override void ApplyMove(float h, float v)
        {
            m_Car.Move(h, v, v, 0f);
        }
    }
}
