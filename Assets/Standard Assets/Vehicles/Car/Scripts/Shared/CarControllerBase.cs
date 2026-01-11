using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public abstract class CarControllerBase : MonoBehaviour
    {
        // Shared runtime
        protected Rigidbody m_Rigidbody;

        // Shared steering/accel state
        protected float m_SteerAngle;
        public float CurrentSteerAngle
        {
            get { return m_SteerAngle; }
            set { m_SteerAngle = value; }
        }
        public float AccelInput { get; set; }

        // Speed
        public float CurrentSpeed { get { return (m_Rigidbody != null ? m_Rigidbody.velocity.magnitude : 0f) * 2.23693629f; } }
        // public float MaxSpeed { get { return GetTopSpeed(); } }
        // protected abstract float GetTopSpeed();
        public float Revs { get; protected set; }

        // Sensors shared across terms
        [SerializeField] protected List<GameObject> sensors = new List<GameObject>();
        protected List<float> sensor_values = new List<float>();
        public bool sensor_visable;
        public bool main_car;

        // Motion sensing shared across terms
        protected float AccelerationT;
        protected float AccelerationN;
        protected float Jerk;
        protected float lastSpeed;
        protected float lastAcc;

        protected List<Vector3> previous_pos = new List<Vector3>();
        protected List<float> averageSpeed = new List<float>();
        protected List<float> averageAcc = new List<float>();

        protected void InitControllerBase()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            lastSpeed = 0f;
            lastAcc = 0f;
            Jerk = 0f;
            AccelerationT = 0f;
            AccelerationN = 0f;
        }

        protected void UpdateMainCarMotionSensing()
        {
            // average over last frames
            int time_steps = 10;
            float speed = (m_Rigidbody != null) ? m_Rigidbody.velocity.magnitude : 0f;

            if (averageSpeed.Count >= time_steps)
            {
                float averaged_speed = AverageLastSpeed();
                AccelerationT = (averaged_speed - lastSpeed) / (time_steps * Time.deltaTime);
                AccelerationN = (averaged_speed) * (averaged_speed) * SenseCurve();
                lastSpeed = averaged_speed;

                float currentAcc = SenseAcc();
                averageAcc.Add(currentAcc);

                averageSpeed.Clear();
                previous_pos.Clear();
            }

            averageSpeed.Add(speed);
            previous_pos.Add(transform.position);

            if (averageAcc.Count >= 5)
            {
                float averaged_acc = AverageLastAcc();
                Jerk = (averaged_acc - lastAcc) / ((5 * time_steps) * Time.deltaTime);
                lastAcc = averaged_acc;

                averageAcc.Clear();
            }
        }

        // Utilities
        public Vector3 Position() { return transform.position; }
        public List<int> getGPS()
        {
            return new List<int> { (int)transform.position.x, (int)transform.position.z };
        }
        public Quaternion Orientation() { return transform.rotation; }

        public void ToggleSensorView()
        {
            sensor_visable = !sensor_visable;
            foreach (GameObject sensor in sensors)
            {
                var lr = sensor.GetComponent<LineRenderer>();
                if (lr) lr.enabled = sensor_visable;
            }
        }

        public void SenseDistance()
        {
            if (sensors.Count == 0) return;
            var distances = new List<float>();
            foreach (GameObject sensor in sensors)
            {
                RaycastHit hit;
                Physics.Raycast(sensor.transform.position, sensor.transform.forward, out hit);
                var lineRenderer = sensor.GetComponentInParent<LineRenderer>();
                if (sensor_visable && lineRenderer != null)
                {
                    if (hit.collider) lineRenderer.SetPosition(1, new Vector3(0, 0, 10 * hit.distance));
                    else lineRenderer.SetPosition(1, new Vector3(0, 0, 1000));
                }
                distances.Add(hit.distance);
            }
            sensor_values = distances;
        }

        public List<float> getSensors() { return sensor_values; }

        public float SenseAccT() { return AccelerationT; }
        public float SenseAccN() { return AccelerationN; }
        public float SenseAcc() { return Mathf.Sqrt(AccelerationT * AccelerationT + AccelerationN * AccelerationN); }
        public float SenseJerk() { return Jerk; }

        protected float AverageLastSpeed()
        {
            float sum = 0f;
            for (int i = 0; i < averageSpeed.Count; i++) sum += averageSpeed[i];
            return sum / Mathf.Max(1, averageSpeed.Count);
        }

        protected float AverageLastAcc()
        {
            float sum = 0f;
            for (int i = 0; i < averageAcc.Count; i++) sum += averageAcc[i];
            return sum / Mathf.Max(1, averageAcc.Count);
        }

        protected float SenseCurve()
        {
            float averaged_curve = 0.0f;
            for (int i = 0; i < previous_pos.Count - 2; i++)
            {
                float x1 = previous_pos[i].x, x2 = previous_pos[i + 1].x, x3 = previous_pos[i + 2].x;
                float y1 = previous_pos[i].z, y2 = previous_pos[i + 1].z, y3 = previous_pos[i + 2].z;

                Vector2 ray1 = new Vector2(x2 - x1, y2 - y1);
                Vector2 ray2 = new Vector2(x3 - x2, y3 - y2);
                if (ray1.magnitude == 0 || ray2.magnitude == 0) continue;

                Vector2 ray3 = new Vector2(x3 - x1, y3 - y1);
                float corner_angle = Mathf.Abs(Vector2.Angle(ray1, ray2));
                if (ray3.magnitude != 0 && corner_angle != 180)
                    averaged_curve += 2 * Mathf.Sin(corner_angle * Mathf.Deg2Rad) / ray3.magnitude;
                else
                    averaged_curve += 1000000; // illegal move; infinite curvature
            }
            return (previous_pos.Count >= 3) ? averaged_curve / (previous_pos.Count - 2) : 0f;
        }
    }
}
