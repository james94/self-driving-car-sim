using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public abstract class CarUserControlBase : MonoBehaviour
    {
        protected Steering s;

        protected virtual void Awake()
        {
            s = new Steering();
            s.Start();
        }

        protected virtual void FixedUpdate()
        {
            s.UpdateValues();
            ApplyMove(s.H, s.V);
        }

        // Children translate inputs to their concrete controller Move API
        protected abstract void ApplyMove(float h, float v);
    }
}