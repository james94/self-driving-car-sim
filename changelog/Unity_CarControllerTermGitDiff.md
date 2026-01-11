ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs
index 0040157..376bd5d 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs
@@ -22,7 +22,7 @@ namespace UnityStandardAssets.Vehicles.Car
         KPH
     }

-    public class CarControllerTerm2 : MonoBehaviour
+    public class CarControllerTerm2 : CarControllerBase^M
     {
         [SerializeField] private CarDriveTypeTerm2 m_CarDriveType = CarDriveTypeTerm2.FourWheelDrive;
         [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
@@ -54,12 +54,10 @@ namespace UnityStandardAssets.Vehicles.Car

         private Quaternion[] m_WheelMeshLocalRotations;
         private Vector3 m_Prevpos, m_Pos;
-        private float m_SteerAngle;
         private int m_GearNum;
         private float m_GearFactor;
         private float m_OldRotation;
         private float m_CurrentTorque;
-        private Rigidbody m_Rigidbody;
         private const float k_ReversingThreshold = 0.01f;
         private string m_saveLocation = "";
         private Queue<CarSampleTerm2> carSamples;
@@ -72,166 +70,12 @@ namespace UnityStandardAssets.Vehicles.Car

         public float BrakeInput { get; private set; }

-        private bool m_isRecording = false;
-
-               [SerializeField] private List<GameObject> sensors;
-               private List<float> sensor_values = new List<float> ();
-
-               public bool sensor_visable;
-
-               public bool main_car;
-
-               // sense acceleration
-               private float AccelerationT;
-               private float AccelerationN;
-               private float Jerk;
-               private float lastSpeed;
-               private float lastAcc;
-
-               //used to calculate path curvatures
-               private List<Vector3> previous_pos = new List<Vector3>();
-
-               private List<float> averageSpeed = new List<float>(); //average speed from previous frames
-               private List<float> averageAcc = new List<float>(); //average acceleration from previous frames
-
-               public Vector3 Position () {
-                       return transform.position;
-               }
-               public List<int> getGPS(){
-                       List<int> gps = new List<int> ();
-                       gps.Add ((int)transform.position.x);
-                       gps.Add ((int)transform.position.z);
-                       return gps;
-               }
-
-               public Quaternion Orientation () {
-                       return transform.rotation;
-               }
-
-               //toggle sensors visable on/off
-               public void ToggleSensorView()
-               {
-                       sensor_visable = !sensor_visable;
-                       if (sensor_visable) {
-                               foreach (GameObject sensor in sensors) {
-                                       sensor.GetComponent<LineRenderer>().enabled = true;
-                               }
-                       }
-                       else {
-                               foreach (GameObject sensor in sensors) {
-                                       sensor.GetComponent<LineRenderer>().enabled = false;
-                               }
-                       }
-               }
-
-               public void SenseDistance()
-               {
-                       if (sensors.Count != 0) {
-                               List<float> distances = new List<float> ();
-                               int i = 0;
-                               foreach (GameObject sensor in sensors) {
-                                       RaycastHit hit;
-                                       Physics.Raycast (sensor.transform.position, sensor.transform.forward, out hit);
-                                       LineRenderer lineRenderer = sensor.GetComponentInParent<LineRenderer> ();
-                                       if (sensor_visable) {
-                                               if (hit.collider) {
-                                                       lineRenderer.SetPosition (1, new Vector3 (0, 0, 10 * hit.distance));
-                                               } else {
-                                                       lineRenderer.SetPosition (1, new Vector3 (0, 0, 1000));
-                                               }
-                                       }
-                                       distances.Add (hit.distance);
-                                       i += 1;
-                               }
-                               sensor_values = distances;
-                       }
-               }
-               public float SenseAccT()
-               {
-                       return AccelerationT;
-               }
-               public float SenseAccN()
-               {
-                       return AccelerationN;
-               }
-               public float SenseAcc()
-               {
-                       return Mathf.Sqrt (AccelerationT * AccelerationT + AccelerationN * AccelerationN);
-               }
-               public float SenseJerk()
-               {
-                       return Jerk;
-               }
-
-               public List<float> getSensors()
-               {
-                       return sensor_values;
-               }
-
-        public bool IsRecording {
-            get
-            {
-                return m_isRecording;
-            }
-
-            set
-            {
-                m_isRecording = value;
-                if(value == true)
-                {
-                                       Debug.Log("Starting to record");
-                                       carSamples = new Queue<CarSampleTerm2>();
-                                       StartCoroutine(Sample());
-                }
-                               else
-                {
-                    Debug.Log("Stopping record");
-                    StopCoroutine(Sample());
-                    Debug.Log("Writing to disk");
-                                       //save the cars coordinate parameters so we can reset it to this properly after capturing data
-                                       saved_position = transform.position;
-                                       saved_rotation = transform.rotation;
-                                       //see how many samples we captured use this to show save percentage in UISystem script
-                                       TotalSamples = carSamples.Count;
-                                       isSaving = true;
-                                       StartCoroutine(WriteSamplesToDisk());
-
-                };
-            }
-
-        }
-
-
-               public bool checkSaveLocation()
-               {
-                       if (m_saveLocation != "")
-                       {
-                               return true;
-                       }
-                       else
-                       {
-                               SimpleFileBrowser.ShowSaveDialog (OpenFolder, null, true, null, "Select Output Folder", "Select");
-                       }
-                       return false;
-               }
-
-        public float CurrentSteerAngle {
-            get { return m_SteerAngle; }
-            set { m_SteerAngle = value; }
+        public float MaxSpeed^M
+        {^M
+            get { return m_Topspeed; }^M
         }

-        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }
-
-        public float MaxSpeed{ get { return m_Topspeed; } }
-
-               public void setMaxSpeed(float Topspeed)
-               {
-                       m_Topspeed = Topspeed;
-               }
-
-        public float Revs { get; private set; }
-
-        public float AccelInput { get; set; }
+        private bool m_isRecording = false;^M

         // Use this for initialization
         private void Start ()
@@ -244,7 +88,9 @@ namespace UnityStandardAssets.Vehicles.Car

             m_MaxHandbrakeTorque = float.MaxValue;

-            m_Rigidbody = GetComponent<Rigidbody> ();
+            // Initialize shared base state^M
+            InitControllerBase();^M
+^M
             m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);

                        lastSpeed = 0;
@@ -309,37 +155,8 @@ namespace UnityStandardAssets.Vehicles.Car
         {
                        if (main_car)
                        {
-                               //average over last frames
-                               int time_steps = 10;
-
-                               float speed = m_Rigidbody.velocity.magnitude;
-
-                               if (averageSpeed.Count >= time_steps) {
-
-                                       float averaged_speed = AverageLastSpeed ();
-                                       AccelerationT = (averaged_speed - lastSpeed) / (time_steps*Time.deltaTime);
-                                       AccelerationN = (averaged_speed) * (averaged_speed) * SenseCurve ();
-                                       lastSpeed = averaged_speed;
-
-                                       float currentAcc = SenseAcc ();
-                                       averageAcc.Add (currentAcc);
-
-                                       averageSpeed.Clear ();
-                                       previous_pos.Clear ();
-                               }
-                               averageSpeed.Add (speed);
-                               previous_pos.Add (transform.position);
-
-                               if (averageAcc.Count >= 5)
-                               {
-                                       float averaged_acc = AverageLastAcc();
-                                       Jerk = (averaged_acc - lastAcc) / ((5*time_steps) * Time.deltaTime);
-                                       lastAcc = averaged_acc;
-
-                                       averageAcc.Clear ();
-                               }
-
-
+                               // Replace duplicated motion-sensing with base helper^M
+                               UpdateMainCarMotionSensing();^M
                        }
         }

@@ -352,7 +169,7 @@ namespace UnityStandardAssets.Vehicles.Car
             }
         }

-               public float AverageLastSpeed()
+        public float AverageLastSpeed()^M
                {

                        float averaged_speed = 0.0f;
                        