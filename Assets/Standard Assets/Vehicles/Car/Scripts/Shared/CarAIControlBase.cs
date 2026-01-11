using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    // Shared enum available to both children
    public enum BrakeCondition
    {
        NeverBrake,
        TargetDirectionDifference,
        TargetDistance,
    }

    public abstract class CarAIControlBase : MonoBehaviour
    {
        // Shared driving config (serialized so existing prefab values can carry over)
        [SerializeField] [Range(0, 1)] protected float m_CautiousSpeedFactor = 0.05f;
        [SerializeField] [Range(0, 180)] protected float m_CautiousMaxAngle = 50f;
        [SerializeField] protected float m_CautiousMaxDistance = 100f;
        [SerializeField] protected float m_CautiousAngularVelocityFactor = 30f;
        [SerializeField] protected float m_SteerSensitivity = 0.05f;
        [SerializeField] protected float m_AccelSensitivity = 0.04f;
        [SerializeField] protected float m_BrakeSensitivity = 1f;
        [SerializeField] protected float m_LateralWanderDistance = 3f;
        [SerializeField] protected float m_LateralWanderSpeed = 0.1f;
        [SerializeField] [Range(0, 1)] protected float m_AccelWanderAmount = 0.1f;
        [SerializeField] protected float m_AccelWanderSpeed = 0.1f;
        [SerializeField] protected BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance;
        [SerializeField] protected bool m_Driving;
        // [SerializeField] protected bool m_StopWhenTargetReached;
        // [SerializeField] protected float m_ReachTargetThreshold = 2f;

        // Shared runtime state
        protected float m_RandomPerlin;
        protected float m_AvoidOtherCarTime;
        // protected float m_AvoidOtherCarSlowdown;
        protected float m_AvoidPathOffset;
        protected Rigidbody m_Rigidbody;

        // Call in child Awake()
        protected void InitAICommon()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_RandomPerlin = UnityEngine.Random.value * 100f;
        }

        // Desired speed when braking for upcoming direction change
        protected float DesiredSpeedByDirection(Vector3 fwd, Transform target, float maxSpeed)
        {
            float approachingCornerAngle = Vector3.Angle(target.forward, fwd);
            float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;
            float cautiousnessRequired = Mathf.InverseLerp(0, m_CautiousMaxAngle, Mathf.Max(spinningAngle, approachingCornerAngle));
            return Mathf.Lerp(maxSpeed, maxSpeed * m_CautiousSpeedFactor, cautiousnessRequired);
        }

        // Desired speed when braking by distance to target
        protected float DesiredSpeedByDistance(Transform target, Vector3 selfPos, float maxSpeed)
        {
            Vector3 delta = target.position - selfPos;
            float distanceCautiousFactor = Mathf.InverseLerp(m_CautiousMaxDistance, 0, delta.magnitude);
            float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;
            float cautiousnessRequired = Mathf.Max(Mathf.InverseLerp(0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);
            return Mathf.Lerp(maxSpeed, maxSpeed * m_CautiousSpeedFactor, cautiousnessRequired);
        }

        protected float ComputeAccel(float desiredSpeed, float currentSpeed)
        {
            float accelBrakeSensitivity = (desiredSpeed < currentSpeed) ? m_BrakeSensitivity : m_AccelSensitivity;
            float accel = Mathf.Clamp((desiredSpeed - currentSpeed) * accelBrakeSensitivity, -1, 1);
            accel *= (1 - m_AccelWanderAmount) + (Mathf.PerlinNoise(Time.time * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);
            return accel;
        }

        protected float ComputeSteer(Vector3 localTarget, float currentSpeed)
        {
            float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            return Mathf.Clamp(targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign(currentSpeed);
        }

        protected Vector3 ApplyEvasiveOrWander(Transform target, float lateralWanderDistance)
        {
            Vector3 offsetTargetPos = target.position;
            if (Time.time < m_AvoidOtherCarTime)
            {
                offsetTargetPos += target.right * m_AvoidPathOffset;
            }
            else
            {
                offsetTargetPos += target.right * ((Mathf.PerlinNoise(Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) * lateralWanderDistance);
            }
            return offsetTargetPos;
        }
    }
}
