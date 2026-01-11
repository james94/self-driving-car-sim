using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class BrakeLightTerm2 : BrakeLightBase
    {
        public CarControllerTerm2 car; // drag Term2 controller here

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