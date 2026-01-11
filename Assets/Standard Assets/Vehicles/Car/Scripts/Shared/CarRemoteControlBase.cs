using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public abstract class CarRemoteControlBase : MonoBehaviour
    {
        public float SteeringAngle { get; set; }
        public float Acceleration { get; set; }

        protected Steering s;

        protected virtual void Awake()
        {
            s = new Steering();
            s.Start();
        }

        protected virtual void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                s.UpdateValues();
                ApplyMove(s.H, s.V);
            }
            else
            {
                ApplyMove(SteeringAngle, Acceleration);
            }
        }

        // Children translate inputs to their concrete controller Move API
        protected abstract void ApplyMove(float h, float v);
    }
}
