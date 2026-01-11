using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarControllerTerm2))]
    public class CarUserControlTerm2 : CarUserControlBase
    {
        private CarControllerTerm2 m_Car;

        private void Awake()
        {
            m_Car = GetComponent<CarControllerTerm2>();
            base.Awake();
        }

        protected override void ApplyMove(float h, float v)
        {
            m_Car.Move(h, v, v, 0f);
        }
    }
}