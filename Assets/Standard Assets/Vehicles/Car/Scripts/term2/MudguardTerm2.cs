using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    // [RequireComponent(typeof(CarControllerTerm2))]
    public class MudguardTerm2 : MudguardBase
    {
        public CarControllerTerm2 carController; // car controller to get the steering angle

        // private Quaternion m_OriginalRotation;


        // private void Start()
        // {
        //     m_OriginalRotation = transform.localRotation;
        // }


        // private void Update()
        // {
        //     transform.localRotation = m_OriginalRotation*Quaternion.Euler(0, carController.CurrentSteerAngle, 0);
        // }

        protected override float GetSteerAngle()
        {
            return carController != null ? carController.CurrentSteerAngle : 0f;
        }
    }
}