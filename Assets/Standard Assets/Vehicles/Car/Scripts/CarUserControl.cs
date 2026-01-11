using UnityEngine;
using System.Collections;


namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : CarUserControlBase
    {
        private CarController m_Car;

        private void Awake()
        {
            m_Car = GetComponent<CarController>();
            base.Awake();
        }

        protected override void ApplyMove(float h, float v)
        {
            m_Car.Move(h, v, v, 0f);
        }
    }
}
