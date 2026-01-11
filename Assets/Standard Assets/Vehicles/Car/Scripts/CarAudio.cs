using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityStandardAssets.Vehicles.Car;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarAudio : CarAudioBase
    {
        // This script reads some of the car's current properties and plays sounds accordingly.
        // The engine sound can be a simple single clip which is looped and pitched, or it
        // can be a crossfaded blend of four clips which represent the timbre of the engine
        // at different RPM and Throttle state.

        // the engine clips should all be a steady pitch, not rising or falling.

        // when using four channel engine crossfading, the four clips should be:
        // lowAccelClip : The engine at low revs, with throttle open (i.e. begining acceleration at very low speed)
        // highAccelClip : Thenengine at high revs, with throttle open (i.e. accelerating, but almost at max speed)
        // lowDecelClip : The engine at low revs, with throttle at minimum (i.e. idling or engine-braking at very low speed)
        // highDecelClip : Thenengine at high revs, with throttle at minimum (i.e. engine-braking at very high speed)

        // For proper crossfading, the clips pitches should all match, with an octave offset between low and high.


        private CarController m_CarController;


        private void Awake()
        {
            m_CarController = GetComponent<CarController>();
        }


        // Update is called once per frame
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
