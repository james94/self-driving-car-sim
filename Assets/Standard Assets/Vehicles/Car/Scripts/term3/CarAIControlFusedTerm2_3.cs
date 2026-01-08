using System;
using UnityEngine;
using System.IO;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Collections;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent (typeof(CarControllerFusedTerm2_3))]
    public class CarAIControlFusedTerm2_3 : MonoBehaviour
    {
        public enum BrakeCondition
        {
            NeverBrake,
            // the car simply accelerates at full throttle all the time.
            TargetDirectionDifference,
            // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.
            TargetDistance,
            // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
            // head for a stationary target and come to rest when it arrives there.
        }

        // This script provides input to the car controller in the same way that the user control script does.
        // As such, it is really 'driving' the car, with no special physics or animation tricks to make the car behave properly.

        // "wandering" is used to give the cars a more human, less robotic feel. They can waver slightly
        // in speed and direction while driving towards their target.

        [SerializeField] [Range (0, 1)] private float m_CautiousSpeedFactor = 0.05f;
        // percentage of max speed to use when being maximally cautious
        [SerializeField] [Range (0, 180)] private float m_CautiousMaxAngle = 50f;
        // angle of approaching corner to treat as warranting maximum caution
        [SerializeField] private float m_CautiousMaxDistance = 100f;
        // distance at which distance-based cautiousness begins
        [SerializeField] private float m_CautiousAngularVelocityFactor = 30f;
        // how cautious the AI should be when considering its own current angular velocity (i.e. easing off acceleration if spinning!)
        [SerializeField] private float m_SteerSensitivity = 0.05f;
        // how sensitively the AI uses steering input to turn to the desired direction
        [SerializeField] private float m_AccelSensitivity = 0.04f;
        // How sensitively the AI uses the accelerator to reach the current desired speed
        [SerializeField] private float m_BrakeSensitivity = 1f;
        // How sensitively the AI uses the brake to reach the current desired speed
        [SerializeField] private float m_LateralWanderDistance = 3f;
        // how far the car will wander laterally towards its target
        [SerializeField] private float m_LateralWanderSpeed = 0.1f;
        // how fast the lateral wandering will fluctuate
        [SerializeField] [Range (0, 1)] private float m_AccelWanderAmount = 0.1f;
        // how much the cars acceleration will wander
        [SerializeField] private float m_AccelWanderSpeed = 0.1f;
        // how fast the cars acceleration wandering will fluctuate
        [SerializeField] private BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance;
        // what should the AI consider when accelerating/braking?
        [SerializeField] private bool m_Driving;
        // whether the AI is currently actively driving or stopped.
        [SerializeField] private Transform m_Target;
        // 'target' the target object to aim for.
        [SerializeField] private bool m_StopWhenTargetReached;
        // should we stop driving when we reach the target?
        [SerializeField] private float m_ReachTargetThreshold = 2;
        // proximity to target to consider we 'reached' it, and stop driving.

        // Term 3 additions (optional; used when waypoints are assigned)
        [SerializeField] private List<Transform> waypoints;
        public GameObject front_sensor;
        public List<GameObject> right_sensors;
        public List<GameObject> left_sensors;

        private int lane_change_time;
        private int current_waypoint;
        private int lane; // 0,1,2 from left to right
        private bool staged;

        public bool forward;          // highway direction
        public bool maincar;          // true for the ego vehicle
        private bool autodrive;       // unused toggle placeholder

        // Frenet coordinates for ego car
        public float frenet_s = -1f;
        public float frenet_d = -1f;

        // Incident tracking for ego car
        private bool main_collison = false;
        private int collision_display = 0;
        private bool main_spdlmt = false;
        private int spdlmt_display = 0;
        private bool main_lanekeep = false;
        private int lanekeep_display = 0;
        private int timer_lanekeep = 0;

        // Distance/time eval (miles, seconds)
        private float dist_eval = 0f;
        private float time_eval = 0f;

        // Traffic lane-clear counters
        private int lane0_clear = 0;
        private int lane1_clear = 0;
        private int lane2_clear = 0;

        // Follow target and max distance for regen (optional)
        public CarControllerFusedTerm2_3 follow_car;
        public float MaxDistance = 200f;

        // Mode detection
        private bool _term3Mode = false;

        private float m_RandomPerlin;
        // A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
        private CarControllerFusedTerm2_3 m_CarController;
        // Reference to actual car controller we are controlling
        private float m_AvoidOtherCarTime;
        // time until which to avoid the car we recently collided with
        private float m_AvoidOtherCarSlowdown;
        // how much to slow down due to colliding with another car, whilst avoiding
        private float m_AvoidPathOffset;
        // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding
        private Rigidbody m_Rigidbody;


        private void Awake ()
        {
            // get the car controller reference
            m_CarController = GetComponent<CarControllerFusedTerm2_3> ();

            // give the random perlin a random value
            m_RandomPerlin = Random.value * 100;

            m_Rigidbody = GetComponent<Rigidbody> ();

            staged = false;
            autodrive = false;

            // Detect Term 3 mode when waypoints are assigned or ego flag set
            _term3Mode = (waypoints != null && waypoints.Count > 0) || maincar;
        }



		public void Spawn(List<GameObject> cars)
		{
			int direction = 1;
			if(!forward)
			{
				direction = -1;
			}
				
			int escape_cnt = 0;
			Vector3 spawn_pos = waypoints[0].position;
			int compare_start = 0;
			bool spawn_check = false;
			while(!spawn_check && escape_cnt < 500)
			{
				spawn_check = true;
				lane = Random.Range (0, 3);
			

				lane_change_time = 0;
			
				int waypoint_offset = 0;
				if (forward) 
				{
					waypoint_offset = Random.Range (-3, -1);
					m_CarController.setMaxSpeed (50 + 10 * Random.Range (0.0f, 1.0f));
					int infront_or_behind = Random.Range (0, 2);
					//int infront_or_behind = 0;
					if (infront_or_behind == 1) 
					{
						waypoint_offset = Random.Range (4, 6);
						m_CarController.setMaxSpeed (50 + 10 * Random.Range (-1.0f, 0.0f));
					}
				} 
				else 
				{
					waypoint_offset = Random.Range (3, 7);
					m_CarController.setMaxSpeed (50 + 10 * Random.Range (-1.0f, 1.0f));
				}
				Vector3 follow_car_pos = follow_car.transform.position;	
				compare_start = ListIndex(ClosestWaypoint (follow_car_pos)+waypoint_offset);
				Vector3 start_pos = waypoints[compare_start].position;

				Vector3 start_offset = waypoints [compare_start].right * 2*direction;
				if (lane == 1)
				{
					start_offset = waypoints [compare_start].right * 6*direction;
				} 
				else if(lane==2) 
				{
					start_offset = waypoints [compare_start].right * 10*direction;
				}

				spawn_pos = new Vector3 (start_pos.x + start_offset.x, start_pos.y + start_offset.y, start_pos.z + start_offset.z);

				//make sure there we wont spawn ontop of a existing car
				foreach (GameObject car in cars) 
				{
					if (Vector3.Distance (car.transform.position,spawn_pos) < 6)
					{
						spawn_check = false;
					}
				}
			
				escape_cnt++;
			}

			if (escape_cnt < 500) 
			{
				staged = false;

				m_CarController.transform.position = spawn_pos;

				m_CarController.transform.rotation = waypoints [compare_start].transform.rotation;
				if (!forward) {
					Vector3 rot = m_CarController.transform.rotation.eulerAngles;
					rot = new Vector3 (rot.x, rot.y + 180, rot.z);
					m_CarController.transform.rotation = Quaternion.Euler (rot);
				}
				m_Rigidbody.velocity = waypoints [compare_start].transform.forward * direction * m_CarController.MaxSpeed;
				current_waypoint = ListIndex (compare_start);
			}

			else
			{
				Debug.Log ("BAD SPAWN");
			}

		}

		public int getLane()
		{
			return lane;
		}

		public void PrintWaypoints()
		{
			int wp = 0;
			foreach (Transform t in waypoints) {
				
				float x_pos = t.position.x;
				float y_pos = t.position.z;

				var s = 0.0;
				for (int i = 0; i < wp; i++) 
				{
					s += (waypoints[i+1].transform.position - waypoints[i].transform.position).magnitude;
				}

				var d_x = t.right.x;
				var d_y = t.right.z;

				string row = string.Format ("{0} {1} {2} {3} {4}\n", x_pos, y_pos, s, d_x, d_y);
                string m_saveLocation = "C:/data/udacity/sdcnd/term3/CarND_Path_Planning";
				// string m_saveLocation = "C:/Users/aaron/Documents/udacity/sdcnd/term2/CarND-Path-Planning/data";
				File.AppendAllText (Path.Combine (m_saveLocation, "highway_map.csv"), row);
				wp++;
			}


		}




        private void FixedUpdate ()
        {
            if (_term3Mode && maincar)
            {
                // Ego (Term 3): accumulate distance/time and check incidents
                float speedMph = m_CarController.CurrentSpeed;
                dist_eval += (Time.deltaTime * speedMph / 2.23693629f) / 1609.34f;
                time_eval += Time.deltaTime;

                // Speed limit violation (simple 50 mph rule)
                if (speedMph > 50.0f) {
                    main_spdlmt = true;
                }

                // Frenet and lane keeping checks (requires waypoints)
                if (waypoints != null && waypoints.Count > 1)
                {
                    List<float> frenet_values = getThisFrenetFrame();
                    // Outside of lanes
                    if (frenet_d < 0.8f || frenet_d > 11.2f) {
                        main_lanekeep = true;
                    }
                    // Over lane lines (center lines) sustained
                    else if ((frenet_d > 3.2f && frenet_d < 4.8f) || (frenet_d > 7.2f && frenet_d < 8.8f)) {
                        timer_lanekeep++;
                    } else {
                        timer_lanekeep = 0;
                    }

                    if (timer_lanekeep > 150) {
                        main_lanekeep = true;
                    }
                }
            }
            else
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
                        {
                            // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.

                            // check out the angle of our target compared to the current direction of the car
                            float approachingCornerAngle = Vector3.Angle (m_Target.forward, fwd);

                            // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                            float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

                            // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                            float cautiousnessRequired = Mathf.InverseLerp (0, m_CautiousMaxAngle,
                                                                 Math.Max (spinningAngle,
                                                                     approachingCornerAngle));
                            desiredSpeed = Mathf.Lerp (m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
                                cautiousnessRequired);
                            break;
                        }

                    case BrakeCondition.TargetDistance:
                        {
                            // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
                            // head for a stationary target and come to rest when it arrives there.

                            // check out the distance to target
                            Vector3 delta = m_Target.position - transform.position;
                            float distanceCautiousFactor = Mathf.InverseLerp (m_CautiousMaxDistance, 0, delta.magnitude);

                            // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                            float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

                            // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                            float cautiousnessRequired = Mathf.Max (
                                                                 Mathf.InverseLerp (0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);
                            desiredSpeed = Mathf.Lerp (m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
                                cautiousnessRequired);
                            break;
                        }

                    case BrakeCondition.NeverBrake:
                        break;
                    }

                    // Evasive action due to collision with other cars:

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
                    float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed)
                                                      ? m_BrakeSensitivity
                                                      : m_AccelSensitivity;

                    // decide the actual amount of accel/brake input to achieve desired speed.
                    float accel = Mathf.Clamp ((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);

                    // add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
                    // i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
                    accel *= (1 - m_AccelWanderAmount) +
                    (Mathf.PerlinNoise (Time.time * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);

                    // calculate the local-relative position of the target, to steer towards
                    Vector3 localTarget = transform.InverseTransformPoint (offsetTargetPos);

                    // work out the local angle towards the target
                    float targetAngle = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;

                    // get the amount of steering needed to aim the car towards the target
                    float steer = Mathf.Clamp (targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign (m_CarController.CurrentSpeed);

                    // feed input to the car controller.
                    m_CarController.Move (steer, accel, accel, 0f);

                    // if appropriate, stop driving when we're close enough to the target.
                    if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold) {
                        m_Driving = false;
                    }
                }
            }
        }


        private void OnCollisionStay (Collision col)
        {
            // Mark collision incident for ego car
            if (maincar) {
                main_collison = true;
            }

            // detect collision against other cars, so that we can take evasive action
            if (col.rigidbody != null) {
                var otherAI = col.rigidbody.GetComponent<CarAIControlFusedTerm2_3> ();
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


        // Term 3 helpers and APIs

        // Wrap index around list
        private int ListIndex(int index)
        {
            int size = waypoints != null ? waypoints.Count : 0;
            if (size == 0) return 0;
            if (index >= size) return index % size;
            if (index < 0) return size + index;
            return index;
        }

        private int ClosestWaypoint(Vector3 p)
        {
            if (waypoints == null || waypoints.Count == 0) return 0;
            float closestLen = 100000f;
            int closestWaypoint = 0;
            for (int i = 0; i < waypoints.Count; i++) {
                float dist = Vector3.Distance(waypoints[i].position, p);
                if (dist < closestLen) { closestLen = dist; closestWaypoint = i; }
            }
            return closestWaypoint;
        }

        private int MyClosestWaypoint()
        {
            return ClosestWaypoint(transform.position);
        }

        // Compute the next waypoint index we should go to
        private int NextWaypoint(float pos_x, float pos_y)
        {
            if (waypoints == null || waypoints.Count == 0) return 0;

            Vector3 p = new Vector3(pos_x, 0, pos_y);
            int closestWaypoint = MyClosestWaypoint();

            Vector3 heading = waypoints[closestWaypoint].transform.position - p;
            float hx = heading.x, hy = heading.z;

            // Normal vector
            float nx = waypoints[closestWaypoint].transform.right.x;
            float ny = waypoints[closestWaypoint].transform.right.z;

            // Road direction vector (perpendicular to normal)
            float vx = -ny;
            float vy = nx;

            // If inner product of v and h is negative, we passed the waypoint -> increment
            float inner = hx * vx + hy * vy;
            if (inner < 0) {
                return (closestWaypoint + 1) % waypoints.Count;
            }
            return closestWaypoint;
        }

        public float NextWaypointDistance()
        {
            if (waypoints == null || waypoints.Count == 0) return 0f;
            int nextwp = NextWaypoint(transform.position.x, transform.position.z);
            return Vector3.Distance(transform.position, waypoints[nextwp].transform.position);
        }

        public float getS() { return frenet_s; }
        public float getD() { return frenet_d; }

        public List<float> getThisFrenetFrame()
        {
            List<float> fr = getFrenetFrame(transform.position.x, transform.position.z);
            frenet_s = fr[0];
            frenet_d = fr[1];
            return fr;
        }

        public List<float> getFrenetFrame(float pos_x, float pos_y)
        {
            List<float> result = new List<float>() { 0f, 0f };
            if (waypoints == null || waypoints.Count < 2) return result;

            int next_wp = NextWaypoint(pos_x, pos_y);
            int prev_wp = next_wp - 1;
            if (next_wp == 0) prev_wp = waypoints.Count - 1;

            Vector2 v = new Vector2(
                waypoints[next_wp].transform.position.x - waypoints[prev_wp].transform.position.x,
                waypoints[next_wp].transform.position.z - waypoints[prev_wp].transform.position.z
            );

            Vector2 x0 = new Vector2(
                pos_x - waypoints[prev_wp].transform.position.x,
                pos_y - waypoints[prev_wp].transform.position.z
            );

            // Projection of x0 onto v
            Vector2 proj = (Vector2.Dot(x0, v) / Mathf.Abs(v.x * v.x + v.y * v.y)) * v;

            float cte = (x0 - proj).magnitude;

            // Determine sign using a fixed center point heuristic
            Vector3 centerPoint3 = new Vector3(1000f, 50f, 2000f) - waypoints[prev_wp].transform.position;
            Vector2 centerPoint2D = new Vector2(centerPoint3.x, centerPoint3.z);
            float centerToPos = Vector2.Distance(centerPoint2D, x0);
            float centerToRef = Vector2.Distance(centerPoint2D, proj);
            if (centerToPos <= centerToRef) { cte *= -1f; }

            // s value accumulation
            double s = 0.0;
            for (int i = 0; i < prev_wp; i++) {
                s += (waypoints[i + 1].transform.position - waypoints[i].transform.position).magnitude;
            }
            s += proj.magnitude;

            result[0] = (float)s;
            result[1] = cte;
            return result;
        }

        public bool RegenerateCheck()
        {
            if (follow_car == null) return false;
            float dist = Vector3.Distance(follow_car.transform.position, m_CarController.transform.position);
            return (dist > MaxDistance) && !staged;
        }

        public void setStage() { staged = true; }

        // Check if lane is clear (requires CarTraffic on follow_car)
        private bool lane_clear(int this_lane)
        {
            var carTraffic = follow_car != null ? follow_car.GetComponent<CarTraffic>() : null;
            return carTraffic != null ? carTraffic.lane_clear(gameObject, forward, this_lane) : false;
        }

        public bool BlinkerLight() { return (lane_change_time < 100); }

        // Term 3 APIs used by UI
        public void ResetDistance() { dist_eval = 0f; time_eval = 0f; }
        public float DistanceEval() { return dist_eval; }
        public int TimerEval() { return (int)time_eval; }

        public bool CheckCollision()
        {
            if (main_collison) {
                if (collision_display > 50) {
                    collision_display = 0; main_collison = false;
                }
                collision_display += 1;
                return true;
            }
            return false;
        }

        public bool CheckSpeeding()
        {
            if (main_spdlmt) {
                if (spdlmt_display > 50) {
                    spdlmt_display = 0; main_spdlmt = false;
                }
                spdlmt_display += 1;
                return true;
            }
            return false;
        }

        public bool CheckLanePos()
        {
            if (main_lanekeep) {
                if (lanekeep_display > 50) {
                    lanekeep_display = 0; main_lanekeep = false;
                }
                lanekeep_display += 1;
                return true;
            }
            return false;
        }

        // Optional: set simple target state drive (not used by Term 3 path planner)
        public void SetState(float x, float y, float theta)
        {
            // ...existing code or minimal steering toward (x,y)...
            float targetAngle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
            float steer = Mathf.Clamp(targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign(m_CarController.CurrentSpeed);
            float desiredSpeed = 50f;
            float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed) ? m_BrakeSensitivity : m_AccelSensitivity;
            float accel = Mathf.Clamp((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);
            m_CarController.Move(steer, accel, accel, 0f);
        }


        public void SetTarget (Transform target)
        {
            m_Target = target;
            m_Driving = true;
        }
    }
}
