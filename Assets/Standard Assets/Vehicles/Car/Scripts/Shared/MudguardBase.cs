using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public abstract class MudguardBase : MonoBehaviour
    {
        protected Quaternion m_OriginalRotation;

        protected abstract float GetSteerAngle();

        protected void Start()
        {
            m_OriginalRotation = transform.localRotation;
        }

        protected void Update()
        {
            transform.localRotation = m_OriginalRotation * Quaternion.Euler(0f, GetSteerAngle(), 0f);
        }
    }
}
