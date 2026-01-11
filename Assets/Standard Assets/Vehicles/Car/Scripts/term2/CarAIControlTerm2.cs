using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent (typeof(CarControllerTerm2))]
    public class CarAIControlTerm2 : CarAIControlBase
    {
        // 'target' the target object to aim for.
        [SerializeField] private bool m_StopWhenTargetReached;
        // should we stop driving when we reach the target?
        [SerializeField] private float m_ReachTargetThreshold = 2;

        // whether the AI is currently actively driving or stopped.
        [SerializeField] private Transform m_Target;

        private CarControllerTerm2 m_CarController;

        // protected float m_RandomPerlin;

        // Reference to actual car controller we are controlling
        // private float m_AvoidOtherCarTime;
        // time until which to avoid the car we recently collided with
        private float m_AvoidOtherCarSlowdown;
        // how much to slow down due to colliding with another car, whilst avoiding
        // private float m_AvoidPathOffset;
        // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding
        // private Rigidbody m_Rigidbody;


        private void Awake ()
        {
            // get the car controller reference
            m_CarController = GetComponent<CarControllerTerm2> ();

            // Initialize shared AI state
            InitAICommon ();
        }


        private void FixedUpdate ()
        {
            if (m_Target == null || !m_Driving) {
                // Car should not be moving,
                // use handbrake to stop
                    m_CarController.Move (0, 0, -1f, 1f);
                }
            else
            {
                Vector3 fwd = transform.forward;
                if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed * 0.1f) {
                    fwd = m_Rigidbody.velocity;
                }

                float desiredSpeed = m_CarController.MaxSpeed;

                // now it's time to decide if we should be slowing down...
                switch (m_BrakeCondition) {
                case BrakeCondition.TargetDirectionDifference:
                    desiredSpeed = DesiredSpeedByDirection(fwd, m_Target, m_CarController.MaxSpeed);
                    break;
                case BrakeCondition.TargetDistance:
                    desiredSpeed = DesiredSpeedByDistance(m_Target, transform.position, m_CarController.MaxSpeed);
                    break;
                case BrakeCondition.NeverBrake:
                    break;
                }

                // our target position starts off as the 'real' target position
                Vector3 offsetTargetPos = m_Target.position;

                // if are we currently taking evasive action to prevent being stuck against another car:
                if (Time.time < m_AvoidOtherCarTime) {
                    // slow down if necessary (if we were behind the other car when collision occured)
                    desiredSpeed *= m_AvoidOtherCarSlowdown;

                    // and veer towards the side of our path-to-target that is away from the other car
                    offsetTargetPos += m_Target.right * m_AvoidPathOffset;
                } else {
                    // no need for evasive action, we can just wander across the path-to-target in a random way,
                    // which can help prevent AI from seeming too uniform and robotic in their driving
                    offsetTargetPos += m_Target.right *
                    (Mathf.PerlinNoise (Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) *
                    m_LateralWanderDistance;
                }

                // use different sensitivity depending on whether accelerating or braking:
                float accel = ComputeAccel(desiredSpeed, m_CarController.CurrentSpeed);

                // calculate the local-relative position of the target, to steer towards
                Vector3 localTarget = transform.InverseTransformPoint (offsetTargetPos);

                // get the amount of steering needed to aim the car towards the target
                float steer = ComputeSteer(localTarget, m_CarController.CurrentSpeed);

                // feed input to the car controller.
                m_CarController.Move (steer, accel, accel, 0f);

                // if appropriate, stop driving when we're close enough to the target.
                if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold) {
                    m_Driving = false;
                }
            }
        }


        private void OnCollisionStay (Collision col)
        {
            // detect collision against other cars, so that we can take evasive action
            if (col.rigidbody != null) {
                var otherAI = col.rigidbody.GetComponent<CarAIControlTerm2> ();
                if (otherAI != null) {
                    // we'll take evasive action for 1 second
                    m_AvoidOtherCarTime = Time.time + 1;

                    // but who's in front?...
                    if (Vector3.Angle (transform.forward, otherAI.transform.position - transform.position) < 90) {
                        // the other ai is in front, so it is only good manners that we ought to brake...
                        m_AvoidOtherCarSlowdown = 0.5f;
                    } else {
                        // we're in front! ain't slowing down for anybody...
                        m_AvoidOtherCarSlowdown = 1;
                    }

                    // both cars should take evasive action by driving along an offset from the path centre,
                    // away from the other car
                    var otherCarLocalDelta = transform.InverseTransformPoint (otherAI.transform.position);
                    float otherCarAngle = Mathf.Atan2 (otherCarLocalDelta.x, otherCarLocalDelta.z);
                    m_AvoidPathOffset = m_LateralWanderDistance * -Mathf.Sign (otherCarAngle);
                }
            }
        }


        public void SetTarget (Transform target)
        {
            m_Target = target;
            m_Driving = true;
        }
    }
}
