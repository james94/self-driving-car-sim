using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarControllerTerm2))]
    public class CarAudioTerm2 : CarAudioBase
    {
        private CarControllerTerm2 m_CarController;

        private void Awake()
        {
            m_CarController = GetComponent<CarControllerTerm2>();
        }

        private void Update()
        {
            UpdateAudio();
        }

        protected override float GetRevs()
        {
            return m_CarController != null ? m_CarController.Revs : 0f;
        }

        protected override float GetAccelInput()
        {
            return m_CarController != null ? m_CarController.AccelInput : 0f;
        }
    }
}
