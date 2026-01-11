using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public abstract class BrakeLightBase : MonoBehaviour
    {
        protected Renderer m_Renderer;

        protected abstract float GetBrakeInput();

        protected void Start()
        {
            m_Renderer = GetComponent<Renderer>();
        }

        protected void Update()
        {
            if (m_Renderer == null) return;
            m_Renderer.enabled = GetBrakeInput() > 0f;
        }
    }
}
