
# Unity URP 2022_3_62f3 Term 3 Git Diff

## 01/07/26

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git status
Refresh index: 100% (7772/7772), done.
On branch Unity_URP_2022_3_Term3

## Changes not staged for commit:

### **modified:   Assets/1_SelfDrivingCar/Scripts/CommandServer.cs**:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
diff --git a/Assets/1_SelfDrivingCar/Scripts/CommandServer.cs b/Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
index 5d2c944..e87d8a8 100644
--- a/Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
+++ b/Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
@@ -1,125 +1,204 @@
-﻿using System;
 using UnityEngine;
 using System.Collections.Generic;
 using System.Collections;
 using SocketIO;
 using UnityStandardAssets.Vehicles.Car;
+using System;
 using System.Security.AccessControl;
-using System.Globalization;

 public class CommandServer : MonoBehaviour
 {
-       public CarRemoteControl CarRemoteControl;
+       public GameObject Car;
        public Camera FrontFacingCamera;
        private SocketIOComponent _socket;
        private CarController _carController;
+       private perfect_controller point_path;
+       private CarTraffic car_traffic;
+
+
+       // Convert angle (degrees) from Unity orientation to
+       //            90
+       //
+       //  180                   0/360
+       //
+       //            270
+       //
+       // This is the standard format used in mathematical functions.
+       float convertAngle(float psi) {
+               if (psi >= 0 && psi <= 90) {
+                       return 90 - psi;
+               }
+               else if (psi > 90 && psi <= 180) {
+                       return 90 + 270 - (psi - 90);
+               }
+               else if (psi > 180 && psi <= 270) {
+                       return 180 + 90 - (psi - 180);
+               }
+               return 270 - 90 - (psi - 270);
+       }

        // Use this for initialization
        void Start()
        {
                _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
                _socket.On("open", OnOpen);
-               _socket.On("steer", OnSteer);
                _socket.On("manual", onManual);
-               _carController = CarRemoteControl.GetComponent<CarController>();
-       }
+               _socket.On("control", Control);
+               _carController = Car.GetComponent<CarController>();
+               point_path = Car.GetComponent<perfect_controller>();
+               car_traffic = Car.GetComponent<CarTraffic>();

-       // Update is called once per frame
-       void Update()
-       {
        }

        void OnOpen(SocketIOEvent obj)
        {
                Debug.Log("Connection Open");
+               point_path.OpenScript ();
                EmitTelemetry(obj);
        }
+       void OnClose(SocketIOEvent obj)
+       {
+               Debug.Log("Connection Closed");
+               point_path.CloseScript ();
+
+       }

        //
        void onManual(SocketIOEvent obj)
        {
-               Debug.Log("Triggered Callback Driving Manually");
                EmitTelemetry (obj);
        }

-       void OnSteer(SocketIOEvent obj)
+       void Control(SocketIOEvent obj)
        {
-               Debug.Log("Triggered Callback Driving Autonomously via PID Steering");
-
-               Debug.Log("PID Control Active");
-
-               try {
-                       // Add null check
-                       if(obj.data == null) {
-                               Debug.LogError("Empty steering command received");
-                               return;
-                       }
+               JSONObject jsonObject = obj.data;

-                       // Add parse error handling
-                       JSONObject jsonObject = obj.data;
+               //Debug.Log ("sending control");

-                       Debug.Log($"Full JSON Structure: {jsonObject.Print(true)}");

-                       if(!jsonObject.HasField("steering_angle") || !jsonObject.HasField("throttle")) {
-                               Debug.LogError($"Missing fields. Actual data: {string.Join(",", jsonObject.keys)}");
-                               return;
-                       }
+               var next_x = jsonObject.GetField ("next_x");
+               var next_y = jsonObject.GetField ("next_y");
+               List<float> my_next_x = new List<float> ();
+               List<float> my_next_y = new List<float> ();

-                       // Add format verification
-                       float steeringAngle = jsonObject.GetField("steering_angle").f;
-                       float throttle = jsonObject.GetField("throttle").f;
+               for (int i = 0; i < next_x.Count; i++)
+               {
+                       my_next_x.Add (float.Parse((next_x [i]).ToString()));
+                       my_next_y.Add (float.Parse((next_y [i]).ToString()));
+               }

-                       Debug.Log($"Raw steeringAngle = {steeringAngle} (Type: {jsonObject.GetField("steering_angle").type})");
-                       Debug.Log($"Raw throttle = {throttle} (Type: {jsonObject.GetField("throttle").type})");
+               point_path.setControlPath (my_next_x, my_next_y);
+               //point_path.ProgressPath ();

-                       CarRemoteControl.SteeringAngle = Mathf.Clamp(steeringAngle, -1, 1);
-                       CarRemoteControl.Acceleration = Mathf.Clamp(throttle, 0, 1);
+               point_path.setSimulatorProcess();

-                       EmitTelemetry(obj);
-               } catch(Exception ex) {
-                       Debug.LogError($"Steering error: {ex}\nFull JSON: {obj.data}");
-               }
+               EmitTelemetry (obj);
        }
+

        void EmitTelemetry(SocketIOEvent obj)
        {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
-                       try {

-                               print("Attempting to Send...");
-                               // send only if it's not being manually driven
-                               if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
-                                       _socket.Emit("telemetry", new JSONObject());
-                               }
-                               else {
-                                       // Add CTE calculation (replace with your actual CTE logic)
-                                       float currentCTE = CalculateCTE();
-
-                                       // Collect Simulated Data from the Car
-                                       JSONObject telemetryData = new JSONObject(JSONObject.Type.OBJECT);
-
-                                       // Add fields individually
-                                       telemetryData.AddField("cte", currentCTE);
-                                       telemetryData.AddField("steering_angle", _carController.CurrentSteerAngle);
-                                       telemetryData.AddField("throttle", _carController.AccelInput);
-                                       telemetryData.AddField("speed", _carController.CurrentSpeed);
-                                       telemetryData.AddField("image", Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera)));
-                                       _socket.Emit("telemetry", telemetryData);
-                               }
+                       //print("Attempting to Send...");
+                       // send only if it's not being manually driven
+                       if ( !point_path.isServerProcess() ) {
+                               _socket.Emit("telemetry", new JSONObject());
+

-                       } catch(Exception ex) {
-                               Debug.LogError($"Telemetry error: {ex}");
                        }
+                       else {
+
+                               point_path.ServerPause();
+
+                               // Collect Data from the Car
+                               Dictionary<string, JSONObject> data = new Dictionary<string, JSONObject>();
+
+                               // localization of car
+                               data["x"] = new JSONObject(Car.transform.position.x);
+                               data["y"] = new JSONObject(Car.transform.position.z);
+                               data["yaw"] = new JSONObject (convertAngle(Car.transform.rotation.eulerAngles.y));
+                               data["speed"] = new JSONObject(_carController.CurrentSpeed);
+
+                               CarAIControl carAI = (CarAIControl) Car.GetComponent(typeof(CarAIControl));
+
+                               List<float> frenet_values = carAI.getThisFrenetFrame();
+
+                               data["s"] = new JSONObject(frenet_values[0]);
+                               data["d"] = new JSONObject(frenet_values[1]);

+                               // Previous Path data
+                               JSONObject arr_x = new JSONObject(JSONObject.Type.ARRAY);
+                               JSONObject arr_y = new JSONObject(JSONObject.Type.ARRAY);
+                               var previous_path_x = point_path.previous_path_x();
+                               var previous_path_y = point_path.previous_path_y();

+                               for( int i = 0; i < previous_path_x.Count; i++)
+                               {
+                                               arr_x.Add(previous_path_x[i]);
+                                               arr_y.Add(previous_path_y[i]);
+                               }
+
+                               var previous_y = JsonUtility.ToJson(point_path.previous_path_y());
+                               data["previous_path_x"] = arr_x;
+                               data["previous_path_y"] = arr_y;
+
+                               var end_path_s = 0.0f;
+                               var end_path_d = 0.0f;
+
+                               if(previous_path_x.Count > 0)
+                               {
+                                       List<float> frenet_values_others = carAI.getFrenetFrame(previous_path_x[previous_path_x.Count-1],previous_path_y[previous_path_y.Count-1]);
+                                       end_path_s = frenet_values_others[0];
+                                       end_path_d = frenet_values_others[1];
+                               }
+
+                               //End path S and D values
+                               data["end_path_s"] = new JSONObject(end_path_s);
+                               data["end_path_d"] = new JSONObject(end_path_d);
+
+
+                               //data["v_x"] = new JSONObject((Car.GetComponent<Rigidbody>().velocity.x));
+                               //data["v_y"] = new JSONObject((Car.GetComponent<Rigidbody>().velocity.z));
+                               //Vector3 vdir = Car.GetComponent<Rigidbody>().velocity;
+                               //data["v_yaw"] = new JSONObject((float)convertAngle(Mathf.Atan2(vdir.x,vdir.z)*Mathf.Rad2Deg));
+                               //data["a_x"] = new JSONObject(_carController.SenseAcc().x);
+                               //data["a_y"] = new JSONObject(_carController.SenseAcc().z);
+                               //Vector3 adir = _carController.SenseAcc();
+                               //data["a_yaw"] = new JSONObject(((float)convertAngle(Mathf.Atan2(adir.x,adir.z)*Mathf.Rad2Deg)));
+
+                               CarTraffic cars = (CarTraffic) Car.GetComponent(typeof(CarTraffic));
+                               data["sensor_fusion"] = new JSONObject(cars.example_sensor_fusion());
+
+
+                               //data["steering_angle"] = new JSONObject(_carController.CurrentSteerAngle);
+                               //data["throttle"] = new JSONObject(_carController.AccelInput);
+                               //data["speed"] = new JSONObject(_carController.CurrentSpeed);
+                               _socket.Emit("telemetry", new JSONObject(data));
+                       }
                });
-       }

-       // Add this temporary CTE calculation method
-       float CalculateCTE()
-       {
-               // Replace with your actual CTE calculation logic
-               return UnityEngine.Random.Range(0.0f, 1.0f);
+               //    UnityMainThreadDispatcher.Instance().Enqueue(() =>
+               //    {
+               //
+               //
+               //
+               //              // send only if it's not being manually driven
+               //              if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
+               //                      _socket.Emit("telemetry", new JSONObject());
+               //              }
+               //              else {
+               //                      // Collect Data from the Car
+               //                      Dictionary<string, string> data = new Dictionary<string, string>();
+               //                      data["steering_angle"] = _carController.CurrentSteerAngle.ToString("N4");
+               //                      data["throttle"] = _carController.AccelInput.ToString("N4");
+               //                      data["speed"] = _carController.CurrentSpeed.ToString("N4");
+               //                      data["image"] = Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera));
+               //                      _socket.Emit("telemetry", new JSONObject(data));
+               //              }
+               //
+               ////
+               //    });
        }
-}
\ No newline at end of file
+}

### modified:   Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
diff --git a/Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs b/Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
index f28442f..f8d253e 100644
--- a/Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
+++ b/Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
@@ -5,233 +5,32 @@ using UnityEngine.SceneManagement;

 public class MenuOptions : MonoBehaviour
 {
-    // Term selection state
-    // public static bool IsTerm1 { get; private set; }
-    public bool IsTerm1 = false;
-    public bool IsTerm2 = false;
-
-    // Term 1 Components
-    private int track = 0;
     private Outline[] outlines;

-    // Term 2 Components
-    private int project = 0;
-    public Text projectName;
-    public Image projectImage;
-       public Sprite project_1;
-       public Sprite project_2;
-       public Sprite project_3;
-       public Sprite project_4;
-       public Sprite project_5;
-
-    // Shared UI elements
-    // public GameObject term1UI;
-    // public GameObject term2UI;
-
-    public void Start()
+    public void Start ()
     {
-        InitializeTermUI();
-    }
-
-    void InitializeTermUI()
-    {
-        Debug.Log("Initialize Term UI");
-        Debug.Log($"IsTerm1 = {IsTerm1}");
-        Debug.Log($"IsTerm2 = {IsTerm2}");
-        // term1UI.SetActive(IsTerm1);
-        // term2UI.SetActive(!IsTerm1);
-
-        if (IsTerm1) {
-            InitializeTerm1();
-        }
-        else if (IsTerm2) {
-            InitializeTerm2();
-        }
+        outlines = GetComponentsInChildren<Outline>();
+               Debug.Log ("in menu script "+outlines.Length);
+               if (outlines.Length > 0)
+               {
+                       outlines [0].effectColor = new Color (0, 0, 0);
+               }
     }

-    #region Shared Methods
        public void ControlMenu()
        {
-        Debug.Log("Going to control menu scene");
-        Debug.Log($"IsTerm1 = {IsTerm1}");
-        Debug.Log($"IsTerm2 = {IsTerm2}");
-        if (IsTerm1) {
-            SceneManager.LoadScene("ControlMenu");
-        }
-        else if (IsTerm2) {
-            SceneManager.LoadScene("ControlMenuTerm2");
-        }
+               SceneManager.LoadScene ("ControlMenu");
        }

        public void MainMenu()
        {
-        Debug.Log("Going to main menu scene");
-        Debug.Log($"IsTerm1 = {IsTerm1}");
-        Debug.Log($"IsTerm2 = {IsTerm2}");
-        if (IsTerm1) {
-            SceneManager.LoadScene("MenuScene");
-        }
-        else if (IsTerm2) {
-            SceneManager.LoadScene("MenuSceneTerm2");
-        }
+               Debug.Log ("go to main menu");
+               SceneManager.LoadScene ("MenuScene");
        }
-    #endregion // end of Shared Methods
-
-
-    #region Term 1 Methods
-    void InitializeTerm1()
+
+    public void StartPathPlanning()
     {
-        outlines = GetComponentsInChildren<Outline>();
-        if(outlines.Length > 0) {
-            outlines[0].effectColor = Color.black;
-            outlines[1].effectColor = Color.white;
-        }
+               SceneManager.LoadScene("PathPlanning");
     }

-    public void StartDrivingMode()
-    {
-        SceneManager.LoadScene(track == 0 ?
-            "LakeTrackTraining" : "JungleTrackTraining");
-    }
-
-    public void StartAutonomousMode()
-    {
-        SceneManager.LoadScene(track == 0 ?
-            "LakeTrackAutonomous" : "JungleTrackAutonomous");
-    }
-
-    public void SetLakeTrack()
-    {
-        track = 0;
-        outlines[0].effectColor = Color.black;
-        outlines[1].effectColor = Color.white;
-    }
-
-    public void SetMountainTrack()
-    {
-        track = 1;
-        outlines[1].effectColor = Color.black;
-        outlines[0].effectColor = Color.white;
-    }
-    #endregion // end of Term 1 Methods
-
-    #region Term 2 Methods
-    void InitializeTerm2()
-    {
-        if(projectName == null || projectImage == null)
-        {
-            Debug.Log("Missing Term 2 UI references! Maybe in Control Menu");
-            return;
-        }
-
-        project = 0;
-        UpdateProjectDisplay();
-    }
-
-    public void SelectMode()
-    {
-        switch(project)
-        {
-            case 0:
-                SceneManager.LoadScene("EKF_project");
-                break;
-            case 1:
-                SceneManager.LoadScene("UKF_project");
-                break;
-            case 2:
-                SceneManager.LoadScene("particle_filter_v2");
-                break;
-            case 3:
-                SceneManager.LoadScene("LakeTrackAutonomous_pid");
-                break;
-            case 4:
-                SceneManager.LoadScene("LakeTrackAutonomous_mpc");
-                break;
-        }
-    }
-
-       public void Next()
-       {
-               project = (project + 1) % 5;
-        UpdateProjectDisplay();
-    }
-
-    public void Previous()
-    {
-        project = (project == 0) ? 4 : project - 1;
-        UpdateProjectDisplay();
-    }
-
-
-    void UpdateProjectDisplay()
-    {
-
-        if(projectName == null || projectImage == null)
-        {
-            Debug.Log("Project UI elements not assigned! Maybe in Control Menu");
-            return;
-        }
-
-
-        projectName.text = project switch
-        {
-            0 => "Project 1: Extended Kalman Filter",
-            1 => "Project 2: Unscented Kalman filters",
-            2 => "Project 3: Kidnapped Vehicle",
-            3 => "Project 4: PID Controller",
-            4 => "Project 5: MPC Controller",
-            _ => "Invalid Project"
-        };
-
-        projectImage.sprite = project switch
-        {
-            0 => project_1,
-            1 => project_2,
-            2 => project_3,
-            3 => project_4,
-            4 => project_5,
-            _ => null
-        };
-    }
-    #endregion // end of Term 2 Methods
-
-    // Add this to preserve term state across scenes
-    // void OnEnable()
-    // {
-    //     SceneManager.sceneLoaded += OnSceneLoaded;
-    // }
-
-    // void OnDisable()
-    // {
-    //     SceneManager.sceneLoaded -= OnSceneLoaded;
-    // }
-
-    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
-    // {
-    //     if(scene.name == "MenuScene") {
-    //         SwitchToTerm1();
-    //     }
-    //     else if(scene.name == "MenuSceneTerm2") {
-    //         SwitchToTerm2();
-    //     }
-    // }
-
-    // public static bool GetTerm()
-    // {
-    //     return IsTerm1;
-    // }
-
-    // #region Term Switching
-    // public void SwitchToTerm1()
-    // {
-    //     // IsTerm1 = true;
-    //     InitializeTermUI();
-    // }
-
-    // public void SwitchToTerm2()
-    // {
-    //     // IsTerm1 = false;
-    //     InitializeTermUI();
-    // }
-    // #endregion
 }

### modified:   Assets/1_SelfDrivingCar/Scripts/UISystem.cs

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/UISystem.cs
diff --git a/Assets/1_SelfDrivingCar/Scripts/UISystem.cs b/Assets/1_SelfDrivingCar/Scripts/UISystem.cs
index bc06889..4aa3d61 100644
--- a/Assets/1_SelfDrivingCar/Scripts/UISystem.cs
+++ b/Assets/1_SelfDrivingCar/Scripts/UISystem.cs
@@ -1,4 +1,4 @@
-﻿using UnityEngine;
+using UnityEngine;
 using UnityEngine.UI;
 using System.Collections;
 using UnityStandardAssets.Vehicles.Car;
@@ -7,44 +7,51 @@ using UnityEngine.SceneManagement;
 public class UISystem : MonoSingleton<UISystem> {

     public CarController carController;
+       public Camera mainCamera;
     public string GoodCarStatusMessage;
     public string BadSCartatusMessage;
     public Text MPH_Text;
     public Image MPH_Animation;
-    public Text Angle_Text;
-    public Text RecordStatus_Text;
-       public Text DriveStatus_Text;
-       public Text SaveStatus_Text;
-    public GameObject RecordingPause;
-       public GameObject RecordDisabled;
-       public bool isTraining = false;
+       public Text AccT_Text;
+       public Text AccN_Text;
+       public Text Acc_Text;
+       public Text Jerk_Text;
+       public Text Collision_Text;
+       public Text Speeding_Text;
+       public Text Lane_Text;
+
+       //Distance Evaluation
+       public Text Best_Distance_Text;
+       public Text Curr_Distance_Text;
+       public Text Curr_Time_Text;
+       private float best_dist_eval = 0;
+       private bool check_incidents;
+
+       public Text AccStatus_Text;
+       public Text JerkStatus_Text;

     private bool recording;
     private float topSpeed;
        private bool saveRecording;

+       private bool auto_drive;
+
+       private CarAIControl carAI;

     // Use this for initialization
     void Start() {
-               Debug.Log (isTraining);
+
         topSpeed = carController.MaxSpeed;
-        recording = false;
-        RecordingPause.SetActive(false);
-               RecordStatus_Text.text = "RECORD";
-               DriveStatus_Text.text = "";
-               SaveStatus_Text.text = "";
-               SetAngleValue(0);
+
+               AccStatus_Text.text = "";
+               JerkStatus_Text.text = "";
+
         SetMPHValue(0);
-               if (!isTraining) {
-                       DriveStatus_Text.text = "Mode: Autonomous";
-                       RecordDisabled.SetActive (true);
-                       RecordStatus_Text.text = "";
-               }
-    }

-    public void SetAngleValue(float value)
-    {
-        Angle_Text.text = value.ToString("N2") + "°";
+               carAI = (CarAIControl) carController.GetComponent(typeof(CarAIControl));
+
+               auto_drive = true;
+
     }

     public void SetMPHValue(float value)
@@ -53,126 +60,166 @@ public class UISystem : MonoSingleton<UISystem> {
         //Do something with value for fill amounts
         MPH_Animation.fillAmount = value/topSpeed;
     }
-
-    public void ToggleRecording()
-    {
-               // Don't record in autonomous mode
-               if (!isTraining) {
-                       return;
+       public void SetAccTValue(float value)
+       {
+               AccT_Text.text = "AccT: "+value.ToString ("N0")+" m/s^2";
+       }
+       public void SetAccNValue(float value)
+       {
+               AccN_Text.text = "AccN: "+value.ToString ("N0")+" m/s^2";
+       }
+       public void SetAccValue(float value)
+       {
+               Acc_Text.text = "AccTotal: "+value.ToString ("N0")+" m/s^2";
+               if (value >= 10)
+               {
+                       Acc_Text.color = Color.red;
+                       AccStatus_Text.text = "Max Acceleration Exceeded!";
+                       check_incidents = true;
+               }
+               else
+               {
+                       Acc_Text.color = Color.white;
+                       AccStatus_Text.text = "";
                }

-        if (!recording)
-        {
-                       if (carController.checkSaveLocation())
-                       {
-                               recording = true;
-                               RecordingPause.SetActive (true);
-                               RecordStatus_Text.text = "RECORDING";
-                               carController.IsRecording = true;
-                       }
-        }
-        else
-        {
-                       saveRecording = true;
-                       carController.IsRecording = false;
-        }
-    }
-
-    void UpdateCarValues()
-    {
-        SetMPHValue(carController.CurrentSpeed);
-        SetAngleValue(carController.CurrentSteerAngle);
-    }
-
-       // Update is called once per frame
-       void Update () {
+       }
+       public void SetJerkValue(float value)
+       {
+               Jerk_Text.text = "Jerk: "+value.ToString ("N0")+" m/s^3";
+               if (Mathf.Abs(value) >= 10)
+               {
+                       Jerk_Text.color = Color.red;
+                       JerkStatus_Text.text = "Max Jerk Exceeded!";
+                       check_incidents = true;
+               }
+               else
+               {
+                       Jerk_Text.color = Color.white;
+                       JerkStatus_Text.text = "";
+               }
+       }
+       public void SetCollisionValue(bool collision)
+       {
+               if (collision)
+               {
+                       Collision_Text.color = Color.red;
+                       Collision_Text.text = "Collision!";
+                       check_incidents = true;
+               }
+               else
+               {
+                       Collision_Text.color = Color.white;
+                       Collision_Text.text = "";
+               }
+       }

-        // Easier than pressing the actual button :-)
-        // Should make recording training data more pleasant.
+       public void SetSpeedingValue(bool speeding)
+       {
+               if (speeding)
+               {
+                       Speeding_Text.color = Color.red;
+                       Speeding_Text.text = "Violated Speed Limit!";
+                       check_incidents = true;
+               }
+               else
+               {
+                       Speeding_Text.color = Color.white;
+                       Speeding_Text.text = "";
+               }
+       }

-               if (carController.getSaveStatus ()) {
-                       SaveStatus_Text.text = "Capturing Data: " + (int)(100 * carController.getSavePercent ()) + "%";
-                       //Debug.Log ("save percent is: " + carController.getSavePercent ());
+       public void SetLaneValue(bool outside_lane)
+       {
+               if (outside_lane)
+               {
+                       Lane_Text.color = Color.red;
+                       Lane_Text.text = "Outside of Lane!";
+                       check_incidents = true;
                }
-               else if(saveRecording)
+               else
                {
-                       SaveStatus_Text.text = "";
-                       recording = false;
-                       RecordingPause.SetActive(false);
-                       RecordStatus_Text.text = "RECORD";
-                       saveRecording = false;
+                       Lane_Text.color = Color.white;
+                       Lane_Text.text = "";
                }
+       }

-        if (Input.GetKeyDown(KeyCode.R))
-        {
-            ToggleRecording();
-        }
+       public void SetDistanceValue(float dist_eval)
+       {
+               if (auto_drive && (dist_eval > best_dist_eval) )
+               {
+                       best_dist_eval = dist_eval;
+                       Best_Distance_Text.text = "Best: "+dist_eval.ToString("N2")+" Miles";
+               }
+
+               Curr_Distance_Text.text = "Curr: "+dist_eval.ToString("N2")+" Miles";
+
+       }
+       public void SetTimerValue(int seconds)
+       {
+               int hours = (seconds / 3600);
+               seconds -= hours * 3600;
+               int minutes = seconds / 60;
+               seconds -= minutes * 60;
+
+               Curr_Time_Text.text = "Timer: " + (hours).ToString ("0") + ":" + (minutes).ToString ("00") + ":" + (seconds).ToString ("00");

-               if (!isTraining)
+       }
+
+    void UpdateCarValues()
+    {
+               check_incidents = false; // are there any incidents?
+        SetMPHValue(carController.CurrentSpeed);
+               SetAccTValue(carController.SenseAccT());
+               SetAccNValue(carController.SenseAccN());
+               SetAccValue(carController.SenseAcc());
+               SetJerkValue(carController.SenseJerk());
+               SetCollisionValue(carAI.CheckCollision());
+               SetSpeedingValue(carAI.CheckSpeeding());
+               SetLaneValue(carAI.CheckLanePos());
+               if (check_incidents)
                {
-                       if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S)))
-                       {
-                               DriveStatus_Text.color = Color.red;
-                               DriveStatus_Text.text = "Mode: Manual";
-                       }
-                       else
-                       {
-                               DriveStatus_Text.color = Color.white;
-                               DriveStatus_Text.text = "Mode: Autonomous";
-                       }
+                       carAI.ResetDistance ();
                }
+               SetDistanceValue(carAI.DistanceEval());
+               SetTimerValue(carAI.TimerEval());
+
+    }
+
+       // Update is called once per frame
+       void Update () {

            if(Input.GetKeyDown(KeyCode.Escape))
         {
             //Do Menu Here
-            // SceneManager.LoadScene("MenuScene");
-            ReturnToAppropriateMenu();
-        }
-
-        if (Input.GetKeyDown(KeyCode.Return))
-        {
-            //Do Console Here
+            SceneManager.LoadScene("MenuScene");
         }

         UpdateCarValues();
     }

-    // Automatic Term Detection without MenuOptions
-    void ReturnToAppropriateMenu()
-    {
-        bool isTerm1Scene = false;
-        string currentScene = SceneManager.GetActiveScene().name;
-
-        if(currentScene == "LakeTrackTraining" || currentScene == "LakeTrackAutonomous") {
-            isTerm1Scene = true;
-        }
-        else if(currentScene == "JungleTrackTraining" || currentScene == "JungleTrackAutonomous") {
-            isTerm1Scene = true;
-        }
+       public void ToggleDriveMode()
+       {
+               auto_drive = !auto_drive;

-        Debug.Log($"isTerm1Scene = {isTerm1Scene}");
+               if (!auto_drive)
+               {
+                       carController.GetComponent<Rigidbody> ().isKinematic = false;
+                       carController.GetComponent<CarUserControl> ().enabled = true;
+                       mainCamera.GetComponent<MouseOrbitImproved> ().enabled = false;

-        // bool isTerm1Scene = currentScene.Contains("LakeTrack") || currentScene.Contains("JungleTrack");

-        SceneManager.LoadScene(isTerm1Scene ? "MenuScene" : "MenuSceneTerm2");
-    }
+                       mainCamera.transform.position = carController.transform.TransformPoint( new Vector3 (0f, 3.2f, -8.2f));
+                       mainCamera.transform.localEulerAngles = new Vector3 (15f, 0f, 0f);
+               }
+               else
+               {
+                       carController.GetComponent<Rigidbody> ().isKinematic = true;
+                       carController.GetComponent<CarUserControl> ().enabled = false;
+                       mainCamera.GetComponent<MouseOrbitImproved> ().enabled = true;
+               }

-    // Automatic Term Detection with MenuOptions (Under Development)
-    // void ReturnToAppropriateMenu()
-    // {
-    //     // Get term state from persistent MenuOptions
-    //     bool isTerm1 = MenuOptions.GetTerm();
-
-    //     // Verify scene existence first
-    //     string targetScene = isTerm1 ? "MenuScene" : "MenuSceneTerm2";
-    //     if(Application.CanStreamedLevelBeLoaded(targetScene))
-    //     {
-    //         SceneManager.LoadScene(targetScene);
-    //     }
-    //     else
-    //     {
-    //         Debug.LogError($"Missing scene: {targetScene}");
-    //         SceneManager.LoadScene("MenuScene"); // Fallback to Term1
-    //     }
-    // }
+               carAI.ResetDistance ();
+       }
+
 }

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs
index ac0ac7b..e5e91fe 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs
@@ -1,6 +1,9 @@
 using System;
 using UnityEngine;
+using System.IO;
 using Random = UnityEngine.Random;
+using System.Collections.Generic;
+using System.Collections;

 namespace UnityStandardAssets.Vehicles.Car
 {
@@ -50,13 +53,25 @@ namespace UnityStandardAssets.Vehicles.Car
         // what should the AI consider when accelerating/braking?
         [SerializeField] private bool m_Driving;
         // whether the AI is currently actively driving or stopped.
-        [SerializeField] private Transform m_Target;
+               [SerializeField] private List<Transform> waypoints;
+
+               public GameObject front_sensor;
+               public List<GameObject> right_sensors;
+               public List<GameObject> left_sensors;
+
+               // controls the timing of lane changing
+               private int lane_change_time;
+
         // 'target' the target object to aim for.
         [SerializeField] private bool m_StopWhenTargetReached;
         // should we stop driving when we reach the target?
         [SerializeField] private float m_ReachTargetThreshold = 2;
         // proximity to target to consider we 'reached' it, and stop driving.

+               //public Transform m_Start;
+
+               public GameObject mycar;
+
         private float m_RandomPerlin;
         // A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
         private CarController m_CarController;
@@ -69,165 +84,847 @@ namespace UnityStandardAssets.Vehicles.Car
         // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding
         private Rigidbody m_Rigidbody;

+               public CarController follow_car;
+               // the car that we are looping with
+               public float MaxDistance;
+
+               //keep track of the waypoint from the list
+               private int current_waypoint;
+
+               private int lane; // 0,1,2 from left to right
+
+               private bool staged;
+
+               //the driving direction of the car
+               public bool forward;
+
+               //use if your the main car
+               private bool autodrive;
+               public bool maincar;
+
+               //frenet coordinates
+               public float frenet_s = -1;
+               public float frenet_d = -1;
+
+               //check for collision on main car
+               private bool main_collison = false;
+               private int collision_display = 0; //the amount of time to display that a collsion happened
+
+               //check for going over speed limit on main car
+               private bool main_spdlmt = false;
+               private int spdlmt_display = 0; //the amount of time to display incident
+
+               //check if on right hand side of the road
+               private bool main_lanekeep = false;
+               private int lanekeep_display = 0; //the amount of time to display incident
+               private int timer_lanekeep = 0; // time before action is an offense
+
+               //track distance in miles without incident
+               private float dist_eval = 0;
+               private float time_eval = 0;
+
+               private bool simulator_process;
+
+               private int lane0_clear = 0;
+               private int lane1_clear = 0;
+               private int lane2_clear = 0;

         private void Awake ()
         {
-            // get the car controller reference
-            m_CarController = GetComponent<CarController> ();
+                       //PrintWaypoints ();

-            // give the random perlin a random value
-            m_RandomPerlin = Random.value * 100;
+                       //get max S
+                       /*
+                       var s = 0.0;
+                       for (int i = 0; i < waypoints.Count-1; i++)
+                       {
+                               s += (waypoints[i+1].transform.position - waypoints[i].transform.position).magnitude;
+                       }
+                       s += (waypoints[waypoints.Count-1].transform.position - waypoints[0].transform.position).magnitude;
+                       Debug.Log ("Max S is " + s);
+                       */

-            m_Rigidbody = GetComponent<Rigidbody> ();
-        }
+                       staged = false;

+                       // get the car controller reference
+                       m_CarController = GetComponent<CarController> ();
+
+                       m_Rigidbody = GetComponent<Rigidbody> ();
+
+                       // give the random perlin a random value
+                       m_RandomPerlin = Random.value * 100;

-        private void FixedUpdate ()
-        {
-            if (m_Target == null || !m_Driving) {
-                // Car should not be moving,
-                // use handbrake to stop
-                    m_CarController.Move (0, 0, -1f, 1f);
-                }
-            else
-            {
-                Vector3 fwd = transform.forward;
-                if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed * 0.1f) {
-                    fwd = m_Rigidbody.velocity;
-                }
-
-                float desiredSpeed = m_CarController.MaxSpeed;
-
-                // now it's time to decide if we should be slowing down...
-                switch (m_BrakeCondition) {
-                case BrakeCondition.TargetDirectionDifference:
-                    {
-                        // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.
-
-                        // check out the angle of our target compared to the current direction of the car
-                        float approachingCornerAngle = Vector3.Angle (m_Target.forward, fwd);
-
-                        // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
-                        float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;
-
-                        // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
-                        float cautiousnessRequired = Mathf.InverseLerp (0, m_CautiousMaxAngle,
-                                                             Mathf.Max (spinningAngle,
-                                                                 approachingCornerAngle));
-                        desiredSpeed = Mathf.Lerp (m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
-                            cautiousnessRequired);
-                        break;
-                    }
-
-                case BrakeCondition.TargetDistance:
-                    {
-                        // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
-                        // head for a stationary target and come to rest when it arrives there.
-
-                        // check out the distance to target
-                        Vector3 delta = m_Target.position - transform.position;
-                        float distanceCautiousFactor = Mathf.InverseLerp (m_CautiousMaxDistance, 0, delta.magnitude);
-
-                        // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
-                        float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;
-
-                        // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
-                        float cautiousnessRequired = Mathf.Max (
-                                                             Mathf.InverseLerp (0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);
-                        desiredSpeed = Mathf.Lerp (m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
-                            cautiousnessRequired);
-                        break;
-                    }
-
-                case BrakeCondition.NeverBrake:
-                    break;
-                }
-
-                // Evasive action due to collision with other cars:
-
-                // our target position starts off as the 'real' target position
-                Vector3 offsetTargetPos = m_Target.position;
-
-                // if are we currently taking evasive action to prevent being stuck against another car:
-                if (Time.time < m_AvoidOtherCarTime) {
-                    // slow down if necessary (if we were behind the other car when collision occured)
-                    desiredSpeed *= m_AvoidOtherCarSlowdown;
-
-                    // and veer towards the side of our path-to-target that is away from the other car
-                    offsetTargetPos += m_Target.right * m_AvoidPathOffset;
-                } else {
-                    // no need for evasive action, we can just wander across the path-to-target in a random way,
-                    // which can help prevent AI from seeming too uniform and robotic in their driving
-                    offsetTargetPos += m_Target.right *
-                    (Mathf.PerlinNoise (Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) *
-                    m_LateralWanderDistance;
-                }
-
-                // use different sensitivity depending on whether accelerating or braking:
-                float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed)
-                                                  ? m_BrakeSensitivity
-                                                  : m_AccelSensitivity;

-                // decide the actual amount of accel/brake input to achieve desired speed.
-                float accel = Mathf.Clamp ((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);
+                       MaxDistance = 200;

-                // add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
-                // i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
-                accel *= (1 - m_AccelWanderAmount) +
-                (Mathf.PerlinNoise (Time.time * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);
+                       autodrive = false;

-                // calculate the local-relative position of the target, to steer towards
-                Vector3 localTarget = transform.InverseTransformPoint (offsetTargetPos);
+                       //flag new data is ready to process
+                       simulator_process = false;

-                // work out the local angle towards the target
-                float targetAngle = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;
+        }

-                // get the amount of steering needed to aim the car towards the target
-                float steer = Mathf.Clamp (targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign (m_CarController.CurrentSpeed);
+               public void Spawn(List<GameObject> cars)
+               {
+                       int direction = 1;
+                       if(!forward)
+                       {
+                               direction = -1;
+                       }
+
+                       int escape_cnt = 0;
+                       Vector3 spawn_pos = waypoints[0].position;
+                       int compare_start = 0;
+                       bool spawn_check = false;
+                       while(!spawn_check && escape_cnt < 500)
+                       {
+                               spawn_check = true;
+                               lane = Random.Range (0, 3);
+
+
+                               lane_change_time = 0;
+
+                               int waypoint_offset = 0;
+                               if (forward)
+                               {
+                                       waypoint_offset = Random.Range (-3, -1);
+                                       m_CarController.setMaxSpeed (50 + 10 * Random.Range (0.0f, 1.0f));
+                                       int infront_or_behind = Random.Range (0, 2);
+                                       //int infront_or_behind = 0;
+                                       if (infront_or_behind == 1)
+                                       {
+                                               waypoint_offset = Random.Range (4, 6);
+                                               m_CarController.setMaxSpeed (50 + 10 * Random.Range (-1.0f, 0.0f));
+                                       }
+                               }
+                               else
+                               {
+                                       waypoint_offset = Random.Range (3, 7);
+                                       m_CarController.setMaxSpeed (50 + 10 * Random.Range (-1.0f, 1.0f));
+                               }
+                               Vector3 follow_car_pos = follow_car.transform.position;
+                               compare_start = ListIndex(ClosestWaypoint (follow_car_pos)+waypoint_offset);
+                               Vector3 start_pos = waypoints[compare_start].position;
+
+                               Vector3 start_offset = waypoints [compare_start].right * 2*direction;
+                               if (lane == 1)
+                               {
+                                       start_offset = waypoints [compare_start].right * 6*direction;
+                               }
+                               else if(lane==2)
+                               {
+                                       start_offset = waypoints [compare_start].right * 10*direction;
+                               }
+
+                               spawn_pos = new Vector3 (start_pos.x + start_offset.x, start_pos.y + start_offset.y, start_pos.z + start_offset.z);
+
+                               //make sure there we wont spawn ontop of a existing car
+                               foreach (GameObject car in cars)
+                               {
+                                       if (Vector3.Distance (car.transform.position,spawn_pos) < 6)
+                                       {
+                                               spawn_check = false;
+                                       }
+                               }
+
+                               escape_cnt++;
+                       }
+
+                       if (escape_cnt < 500)
+                       {
+                               staged = false;
+
+                               m_CarController.transform.position = spawn_pos;
+
+                               m_CarController.transform.rotation = waypoints [compare_start].transform.rotation;
+                               if (!forward) {
+                                       Vector3 rot = m_CarController.transform.rotation.eulerAngles;
+                                       rot = new Vector3 (rot.x, rot.y + 180, rot.z);
+                                       m_CarController.transform.rotation = Quaternion.Euler (rot);
+                               }
+                               m_Rigidbody.velocity = waypoints [compare_start].transform.forward * direction * m_CarController.MaxSpeed;
+                               current_waypoint = ListIndex (compare_start);
+                       }
+
+                       else
+                       {
+                               Debug.Log ("BAD SPAWN");
+                       }
+
+               }
+
+               public int getLane()
+               {
+                       return lane;
+               }
+
+               public void PrintWaypoints()
+               {
+                       int wp = 0;
+                       foreach (Transform t in waypoints) {
+
+                               float x_pos = t.position.x;
+                               float y_pos = t.position.z;
+
+                               var s = 0.0;
+                               for (int i = 0; i < wp; i++)
+                               {
+                                       s += (waypoints[i+1].transform.position - waypoints[i].transform.position).magnitude;
+                               }
+
+                               var d_x = t.right.x;
+                               var d_y = t.right.z;
+
+                               string row = string.Format ("{0} {1} {2} {3} {4}\n", x_pos, y_pos, s, d_x, d_y);
+                string m_saveLocation = "C:/data/udacity/sdcnd/term3/CarND_Path_Planning";
+                               // string m_saveLocation = "C:/Users/aaron/Documents/udacity/sdcnd/term2/CarND-Path-Planning/data";
+                               File.AppendAllText (Path.Combine (m_saveLocation, "highway_map.csv"), row);
+                               wp++;
+                       }
+
+
+               }
+
+               private int ClosestWaypoint(Vector3 p) {
+                       //Vector3 p = car.Position ();
+                       //Quaternion o = m_Car.Orientation ();
+                       float closestLen = 100000; // large number
+                       int closestWaypoint = 0;
+
+                       int i = 0;
+                       foreach (Transform t in waypoints) {
+                               float dist = Vector3.Distance (t.position, p);
+                               if (dist < closestLen) {
+                                       closestLen = dist;
+                                       closestWaypoint = i;
+                               }
+                               i += 1;
+                       }
+
+                       return closestWaypoint;
+               }
+
+
+               private int MyClosestWaypoint() {
+                       Vector3 p = transform.position;
+                       //Quaternion o = m_Car.Orientation ();
+                       float closestLen = 100000; // large number
+                       int closestWaypoint = 0;
+
+                       int i = 0;
+                       foreach (Transform t in waypoints) {
+                               float dist = Vector3.Distance (t.position, p);
+                               if (dist < closestLen) {
+                                       closestLen = dist;
+                                       closestWaypoint = i;
+                               }
+                               i += 1;
+                       }
+
+                       return closestWaypoint;
+               }
+
+               // Compute the next waypoint we should go to
+               private int NextWaypoint(float pos_x, float pos_y) {
+
+                       Vector3 p = new Vector3(pos_x,0,pos_y);
+
+                       int closestWaypoint = MyClosestWaypoint();
+
+                       Vector3 heading = waypoints[closestWaypoint].transform.position - p;
+                       float hx = heading.x;
+                       float hy = heading.z;
+
+                       //Normal vector:
+                       float nx =  waypoints[closestWaypoint].transform.right.x;
+                       float ny =  waypoints[closestWaypoint].transform.right.z;
+
+                       //Vector into the direction of the road (perpendicular to the normal vector)
+                       float vx = -ny;
+                       float vy = nx;
+
+                       //If the inner product of v and h is positive then we are behind the waypoint so we do not need to
+                       //increment closestWaypoint, otherwise we are beyond the waypoint and we need to increment closestWaypoint.
+
+                       float inner = hx * vx + hy * vy;
+                       if (inner < 0)
+                       {
+                               return (closestWaypoint + 1) % waypoints.Count;
+                       }
+                       else
+                       {
+                               return closestWaypoint;
+                       }
+
+               }
+
+               public float NextWaypointDistance()
+               {
+                       int nextwp = NextWaypoint(transform.position.x,transform.position.z);
+                       return Vector3.Distance (transform.position, waypoints [nextwp].transform.position);
+               }
+
+               public float getS()
+               {
+                       return frenet_s;
+               }
+
+               public float getD()
+               {
+                       return frenet_d;
+               }
+
+               public List<float> getThisFrenetFrame()
+               {
+
+                       List<float> frenet_values2 = getFrenetFrame (transform.position.x, transform.position.z);
+
+                       frenet_s = frenet_values2[0];
+                       frenet_d = frenet_values2[1];
+
+                       return frenet_values2;
+
+               }
+
+               public List<float> getFrenetFrame(float pos_x, float pos_y)
+               {
+
+                       // between 0-4 is lane 0, between 4-8 is lane 1, between 8-12 is lane 2
+
+                       int next_wp = NextWaypoint (pos_x,pos_y);
+
+                       var pos = new Vector2 (pos_x, pos_y);
+
+                       // Previous waypoint
+                       int prev_wp;
+                       prev_wp = next_wp - 1;
+                       if (next_wp == 0) {
+                               prev_wp = waypoints.Count - 1;
+                       }
+
+                       // This projects the vehicle position onto the line
+                       // from the previous waypoint to the next waypoint.
+                       var n_x = waypoints [next_wp].transform.position.x - waypoints [prev_wp].transform.position.x;
+                       var n_y = waypoints [next_wp].transform.position.z - waypoints [prev_wp].transform.position.z;
+                       var x_x = pos.x - waypoints [prev_wp].transform.position.x;
+                       var x_y = pos.y - waypoints [prev_wp].transform.position.z;
+                       var v = new Vector2 (n_x, n_y);
+
+                       // current vehicle position
+                       var x0 = new Vector2 (x_x, x_y);
+
+                       // find the projection of x onto v
+                       var proj = (Vector2.Dot (x0, v) / Mathf.Abs (v.x * v.x + v.y * v.y)) * v;
+
+                       var cte = (x0 - proj).magnitude;
+
+                       // This compares the projected position and the current vehicle position
+                       // to a point in the center of the map.
+                       // If projected position is closer is means the vehicle is to the right of the line
+                       // hence the CTE will be positive.
+                       // If the vehicle position is closer is means the vehicle is to the left of the line
+                       // hence the CTE will be negative.
+                       var centerPoint = new Vector3 (1000f, 50f, 2000f) - waypoints [prev_wp].transform.position;
+                       var centerPoint2D = new Vector2 (centerPoint.x, centerPoint.z);
+                       var centerToPos = Vector2.Distance (centerPoint2D, x0);
+                       var centerToRef = Vector2.Distance (centerPoint2D, proj);
+                       if (centerToPos <= centerToRef) {
+                               cte *= -1f;
+                       }
+
+                       //caculate s value
+                       var s = 0.0;
+                       for (int i = 0; i < prev_wp; i++) {
+                               s += (waypoints [i + 1].transform.position - waypoints [i].transform.position).magnitude;
+                       }
+                       s += proj.magnitude;
+
+                       List<float> frenet_values1 = new List<float> ();
+                       frenet_values1.Add ((float)s);
+                       frenet_values1.Add ((float)cte);
+
+                       return frenet_values1;
+
+               }
+
+               public bool RegenerateCheck()
+               {
+                       //measure distance from follow car
+                       float dist = Vector3.Distance (follow_car.Position (), m_CarController.Position ());
+
+                       // if not staged to regenerate and distance is far, then flag to regenerate
+                       if ((dist > MaxDistance) && !staged) {
+                               return true;
+
+                       }
+
+                       return false;
+               }
+
+               public void setStage()
+               {
+                       staged = true;
+               }
+
+               //check if lane is clear and safe for lane change
+               private bool lane_clear(int this_lane)
+               {
+
+                       CarTraffic car_traffic = follow_car.GetComponent<CarTraffic> ();
+
+                       return car_traffic.lane_clear(mycar,forward,this_lane);
+                       //return false;
+               }
+
+               //wrap index around list
+               public int ListIndex(int index)
+               {
+                       int size = waypoints.Count;
+                       if (index >= size)
+                       {
+                               return index%size;
+                       }
+                       else if(index < 0)
+                       {
+                               return size+index;
+                       }
+                       return index;
+               }
+
+               public bool BlinkerLight()
+               {
+                       return (lane_change_time < 100);
+               }
+
+
+        public void FixedUpdate ()
+        {

-                // feed input to the car controller.
-                m_CarController.Move (steer, accel, accel, 0f);
+                       if (!maincar) {
+
+                               int direction = 1;
+                               if (!forward) {
+                                       direction = -1;
+                               }
+
+                               //Debug.Log (current_waypoint);
+
+                               float waypoint_dist = Vector3.Distance (waypoints [current_waypoint].position, m_CarController.Position ());
+                               if (waypoint_dist < (5 + (4 * lane))) {
+                                       current_waypoint = ListIndex (current_waypoint + direction);
+                               }
+
+                               Transform m_Target = waypoints [current_waypoint];
+                               Vector3 reference_pos = m_Target.position;
+                               Vector3 offset_pos = m_Target.right * 2 * direction;
+                               if (lane == 1) {
+                                       offset_pos = m_Target.right * 6 * direction;
+                               } else if (lane == 2) {
+                                       offset_pos = m_Target.right * 10 * direction;
+                               }
+
+                               m_Target.position = new Vector3 (reference_pos.x + offset_pos.x, reference_pos.y + offset_pos.y, reference_pos.z + offset_pos.z);
+
+
+                               if (m_Target == null || !m_Driving) {
+                                       // Car should not be moving,
+                                       // use handbrake to stop
+                                       m_CarController.Move (0, 0, -1f, 1f);
+                               } else {
+                                       Vector3 fwd = transform.forward;
+                                       if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed * 0.1f) {
+                                               fwd = m_Rigidbody.velocity;
+                                       }
+
+                                       float desiredSpeed = m_CarController.MaxSpeed;
+
+                                       RaycastHit hit;
+                                       Physics.Raycast (front_sensor.transform.position, front_sensor.transform.forward, out hit);
+
+
+                                       float hit_avoidance = 10;
+
+                                       if (hit.rigidbody != null)
+                                       {
+                                               hit_avoidance = 10*(m_CarController.MaxSpeed-hit.rigidbody.velocity.magnitude)/20;
+                                       }
+
+
+                                       //change from 10
+
+                                       if (hit.distance < hit_avoidance) {
+
+
+
+                                               if (hit.rigidbody != null)
+                                               {
+                                                       desiredSpeed = hit.rigidbody.velocity.magnitude;
+                                               }
+
+                                               if (m_CarController.CurrentSpeed > desiredSpeed-5) {
+
+                                                       m_CarController.Move (0, 0, -1f, 1f);
+
+                                               }
+
+
+                                               if (m_CarController.CurrentSpeed > 15 && (lane_change_time >= 100) && NextWaypointDistance() < 15 )
+                                               {
+
+
+
+                                                       //try to merge right
+                                                       if (lane == 0)
+                                                       {
+                                                               if (lane_clear (lane + 1))
+                                                               {
+                                                                       lane1_clear++;
+                                                               }
+
+                                                               else
+                                                               {
+                                                                       lane0_clear = 0;
+                                                                       lane1_clear = 0;
+                                                                       lane2_clear = 0;
+                                                               }
+
+                                                               if (lane1_clear>50)
+                                                               {
+                                                                       lane++;
+                                                                       lane1_clear = 0;
+                                                                       lane_change_time = 0;
+                                                               }
+                                                               //try to merge left
+                                                       }
+                                                       else if (lane == 1)
+                                                       {
+                                                               if (lane_clear (lane - 1))
+                                                               {
+                                                                       lane0_clear++;
+                                                               }
+
+                                                               else
+                                                               {
+                                                                       lane0_clear = 0;
+                                                                       lane1_clear = 0;
+                                                                       lane2_clear = 0;
+                                                               }
+
+                                                               if (lane0_clear>50)
+                                                               {
+                                                                       lane--;
+                                                                       lane0_clear = 0;
+                                                                       lane_change_time = 0;
+                                                               }
+                                                               else
+                                                               {
+                                                                       if (lane_clear (lane + 1))
+                                                                       {
+                                                                               lane2_clear++;
+                                                                       }
+
+                                                                       else
+                                                                       {
+                                                                               lane0_clear = 0;
+                                                                               lane1_clear = 0;
+                                                                               lane2_clear = 0;
+                                                                       }
+
+                                                                       if (lane2_clear>50) {
+                                                                               lane++;
+                                                                               lane2_clear = 0;
+                                                                               lane_change_time = 0;
+                                                                       }
+                                                               }
+                                                       }
+                                                       else
+                                                       {
+                                                               if (lane_clear (lane - 1))
+                                                               {
+                                                                       lane1_clear++;
+                                                               }
+
+                                                               else
+                                                               {
+                                                                       lane0_clear = 0;
+                                                                       lane1_clear = 0;
+                                                                       lane2_clear = 0;
+                                                               }
+
+                                                               if (lane1_clear>50) {
+                                                                       lane--;
+                                                                       lane1_clear = 0;
+                                                                       lane_change_time = 0;
+                                                               }
+                                                       }
+                                               }
+                                       }
+
+                                       if (lane_change_time < 100)
+                                       {
+                                               lane_change_time++;
+                                       }
+
+
+
+                                       // now it's time to decide if we should be slowing down...
+                                       switch (m_BrakeCondition) {
+                                       case BrakeCondition.TargetDirectionDifference:
+                                               {
+                                                       // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.
+
+                                                       // check out the angle of our target compared to the current direction of the car
+                                                       float approachingCornerAngle = Vector3.Angle (m_Target.forward, fwd);
+
+                                                       // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
+                                                       float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;
+
+                                                       // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
+                                                       float cautiousnessRequired = Mathf.InverseLerp (0, m_CautiousMaxAngle,
+                                                                                            Mathf.Max (spinningAngle,
+                                                                                                    approachingCornerAngle));
+                                                       desiredSpeed = Mathf.Lerp (m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
+                                                               cautiousnessRequired);
+                                                       break;
+                                               }
+
+                                       case BrakeCondition.TargetDistance:
+                                               {
+                                                       // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
+                                                       // head for a stationary target and come to rest when it arrives there.
+
+                                                       // check out the distance to target
+                                                       Vector3 delta = m_Target.position - transform.position;
+                                                       float distanceCautiousFactor = Mathf.InverseLerp (m_CautiousMaxDistance, 0, delta.magnitude);
+
+                                                       // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
+                                                       float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;
+
+                                                       // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
+                                                       float cautiousnessRequired = Mathf.Max (
+                                                                                            Mathf.InverseLerp (0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);
+                                                       desiredSpeed = Mathf.Lerp (m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor,
+                                                               cautiousnessRequired);
+                                                       break;
+                                               }
+
+                                       case BrakeCondition.NeverBrake:
+                                               break;
+                                       }
+
+                                       // Evasive action due to collision with other cars:
+
+                                       // our target position starts off as the 'real' target position
+                                       Vector3 offsetTargetPos = m_Target.position;
+
+                                       // if are we currently taking evasive action to prevent being stuck against another car:
+                                       if (Time.time < m_AvoidOtherCarTime) {
+                                               // slow down if necessary (if we were behind the other car when collision occured)
+                                               desiredSpeed *= m_AvoidOtherCarSlowdown;
+
+                                               // and veer towards the side of our path-to-target that is away from the other car
+                                               offsetTargetPos += m_Target.right * m_AvoidPathOffset;
+                                       } else {
+                                               // no need for evasive action, we can just wander across the path-to-target in a random way,
+                                               // which can help prevent AI from seeming too uniform and robotic in their driving
+                                               offsetTargetPos += m_Target.right *
+                                               (Mathf.PerlinNoise (Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) *
+                                               m_LateralWanderDistance;
+                                       }
+
+                                       // use different sensitivity depending on whether accelerating or braking:
+                                       float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed)
+                                                  ? m_BrakeSensitivity
+                                                  : m_AccelSensitivity;
-                // if appropriate, stop driving when we're close enough to the target.
-                if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold) {
-                    m_Driving = false;
-                }
-            }
+                                       // decide the actual amount of accel/brake input to achieve desired speed.
+                                       float accel = Mathf.Clamp ((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);
+
+                                       // add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
+                                       // i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
+                                       accel *= (1 - m_AccelWanderAmount) +
+                                       (Mathf.PerlinNoise (Time.time * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);
+
+                                       // calculate the local-relative position of the target, to steer towards
+                                       Vector3 localTarget = transform.InverseTransformPoint (offsetTargetPos);
+
+                                       // work out the local angle towards the target
+                                       float targetAngle = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;
+
+                                       // get the amount of steering needed to aim the car towards the target
+                                       float steer = Mathf.Clamp (targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign (m_CarController.CurrentSpeed);
+
+                                       // feed input to the car controller.
+                                       m_CarController.Move (steer, accel, accel, 0f);
+
+                                       // feed input to the car controller.
+                                       m_CarController.Move (steer, accel, accel, 0f);
+
+                                       // if appropriate, stop driving when we're close enough to the target.
+                                       if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold) {
+                                               m_Driving = false;
+                                       }
+                               }
+                               m_Target.position = reference_pos;
+                       }
+                       else if(maincar)
+                       {
+
+                               //Add on to the distance travled
+                               float get_speed = m_CarController.CurrentSpeed;
+                               dist_eval += (Time.deltaTime * get_speed/2.23693629f)/1609.34f;
+                               time_eval += Time.deltaTime;
+
+
+                               if(get_speed > 50.0)
+                               {
+                                       main_spdlmt = true;
+                               }
+
+                               List<float> frenet_values = getThisFrenetFrame();
+                               if (frenet_d < .8 || frenet_d > 11.2)
+                               {
+                                       main_lanekeep = true;
+                               }
+                               else if ((frenet_d > 3.2 && frenet_d < 4.8) || (frenet_d > 7.2 && frenet_d < 8.8))
+                               {
+                                       timer_lanekeep++;
+                               }
+                               else
+                               {
+                                       timer_lanekeep = 0;
+                               }
+
+                               if (timer_lanekeep > 150)
+                               {
+                                       main_lanekeep = true;
+                               }
+
+                       }
         }

+               public void SetState(float x, float y, float theta)
+               {
+
+                       // work out the local angle towards the target
+                       float targetAngle = Mathf.Atan2 (x, y) * Mathf.Rad2Deg;
+
+                       // get the amount of steering needed to aim the car towards the target
+                       float steer = Mathf.Clamp (targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign (m_CarController.CurrentSpeed);
+
+
+                       float desiredSpeed = 50;
+
+                       // use different sensitivity depending on whether accelerating or braking:
+                       float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed)
+                               ? m_BrakeSensitivity
+                               : m_AccelSensitivity;
+
+                       // decide the actual amount of accel/brake input to achieve desired speed.
+                       float accel = Mathf.Clamp ((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);
+
+                       // feed input to the car controller.
+                       m_CarController.Move (steer, accel, accel, 0f);
+
+               }
+
+               public void ResetDistance()
+               {
+                       dist_eval = 0;
+                       time_eval = 0;
+               }
+               public float DistanceEval()
+               {
+                       return dist_eval;
+               }
+               public int TimerEval()
+               {
+                       return (int)time_eval;
+               }
+
+               public bool CheckCollision()
+               {
+                       if(main_collison)
+                       {
+                               if (collision_display > 50)
+                               {
+                                       collision_display = 0;
+                                       main_collison = false;
+                               }
+                               collision_display += 1;
+                               return true;
+                       }
+                       return false;
+               }
+
+               public bool CheckSpeeding()
+               {
+                       if(main_spdlmt)
+                       {
+                               if (spdlmt_display > 50)
+                               {
+                                       spdlmt_display = 0;
+                                       main_spdlmt = false;
+                               }
+                               spdlmt_display += 1;
+                               return true;
+                       }
+                       return false;
+               }
+
+               public bool CheckLanePos()
+               {
+                       if(main_lanekeep)
+                       {
+                               if (lanekeep_display > 50)
+                               {
+                                       lanekeep_display = 0;
+                                       main_lanekeep = false;
+                               }
+                               lanekeep_display += 1;
+                               return true;
+                       }
+                       return false;
+               }
+
+
+

         private void OnCollisionStay (Collision col)
-        {
-            // detect collision against other cars, so that we can take evasive action
-            if (col.rigidbody != null) {
-                var otherAI = col.rigidbody.GetComponent<CarAIControl> ();
-                if (otherAI != null) {
-                    // we'll take evasive action for 1 second
-                    m_AvoidOtherCarTime = Time.time + 1;
-
-                    // but who's in front?...
-                    if (Vector3.Angle (transform.forward, otherAI.transform.position - transform.position) < 90) {
-                        // the other ai is in front, so it is only good manners that we ought to brake...
-                        m_AvoidOtherCarSlowdown = 0.5f;
-                    } else {
-                        // we're in front! ain't slowing down for anybody...
-                        m_AvoidOtherCarSlowdown = 1;
-                    }
-
-                    // both cars should take evasive action by driving along an offset from the path centre,
-                    // away from the other car
-                    var otherCarLocalDelta = transform.InverseTransformPoint (otherAI.transform.position);
-                    float otherCarAngle = Mathf.Atan2 (otherCarLocalDelta.x, otherCarLocalDelta.z);
-                    m_AvoidPathOffset = m_LateralWanderDistance * -Mathf.Sign (otherCarAngle);
-                }
-            }
+               {
+                       if (maincar) {
+                               main_collison = true;
+                       }
+
+                       // detect collision against other cars, so that we can take evasive action
+                       if (col.rigidbody != null) {
+                               var otherAI = col.rigidbody.GetComponent<CarAIControl> ();
+                               if (otherAI != null) {
+                                       // we'll take evasive action for 1 second
+                                       m_AvoidOtherCarTime = Time.time + 1;
+
+                                       // but who's in front?...
+                                       if (Vector3.Angle (transform.forward, otherAI.transform.position - transform.position) < 90) {
+                                               // the other ai is in front, so it is only good manners that we ought to brake...
+                                               m_AvoidOtherCarSlowdown = 0.5f;
+                                       } else {
+                                               // we're in front! ain't slowing down for anybody...
+                                               m_AvoidOtherCarSlowdown = 1;
+                                       }
+
+                                       // both cars should take evasive action by driving along an offset from the path centre,
+                                       // away from the other car
+                                       var otherCarLocalDelta = transform.InverseTransformPoint (otherAI.transform.position);
+                                       float otherCarAngle = Mathf.Atan2 (otherCarLocalDelta.x, otherCarLocalDelta.z);
+                                       m_AvoidPathOffset = m_LateralWanderDistance * -Mathf.Sign (otherCarAngle);
+                               }
+                       }
+
         }


-        public void SetTarget (Transform target)
-        {
-            m_Target = target;
-            m_Driving = true;
-        }
+        //public void SetTarget (Transform target)
+        //{
+        //    m_Target = target;
+        //    m_Driving = true;
+        //}
     }
+
 }
+


### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs

    ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs"
    diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs
    index 3032476..8ca27ca 100644
    --- a/Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs
    +++ b/Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs
    @@ -27,7 +27,7 @@ namespace UnityStandardAssets.Vehicles.Car
            [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
            [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
            [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
    -        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
    +        //[SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
            [SerializeField] private Vector3 m_CentreOfMassOffset;
            [SerializeField] private float m_MaximumSteerAngle;
            [Range (0, 1)] [SerializeField] private float m_SteerHelper;
    @@ -45,12 +45,6 @@ namespace UnityStandardAssets.Vehicles.Car
            [SerializeField] private float m_SlipLimit;
            [SerializeField] private float m_BrakeTorque;

    -        public const string CSVFileName = "driving_log.csv";
    -        public const string DirFrames = "IMG";
    -
    -        [SerializeField] private Camera CenterCamera;
    -        [SerializeField] private Camera LeftCamera;
    -        [SerializeField] private Camera RightCamera;

            private Quaternion[] m_WheelMeshLocalRotations;
            private Vector3 m_Prevpos, m_Pos;
    @@ -61,19 +55,12 @@ namespace UnityStandardAssets.Vehicles.Car
            private float m_CurrentTorque;
            private Rigidbody m_Rigidbody;
            private const float k_ReversingThreshold = 0.01f;
    -        private string m_saveLocation = "";
    -        private Queue<CarSample> carSamples;
    -               private int TotalSamples;
    -               private bool isSaving;
    -               private Vector3 saved_position;
    -               private Quaternion saved_rotation;
    +

            public bool Skidding { get; private set; }

            public float BrakeInput { get; private set; }

    -        private bool m_isRecording = false;
    -
                    [SerializeField] private List<GameObject> sensors;
                    private List<float> sensor_values = new List<float> ();

    @@ -167,53 +154,7 @@ namespace UnityStandardAssets.Vehicles.Car
                    {
                            return sensor_values;
                    }
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
    -                                       carSamples = new Queue<CarSample>();
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
    +

            public float CurrentSteerAngle {
                get { return m_SteerAngle; }
    @@ -343,15 +284,6 @@ namespace UnityStandardAssets.Vehicles.Car
                            }
            }

    -
    -        public void Update()
    -        {
    -            if (IsRecording)
    -            {
    -                //Dump();
    -            }
    -        }
    -
                    public float AverageLastSpeed()
                    {

    @@ -521,7 +453,7 @@ namespace UnityStandardAssets.Vehicles.Car
                }

                for (int i = 0; i < 4; i++) {
    -                if (CurrentSpeed > 5 && Vector3.Angle (transform.forward, m_Rigidbody.velocity) < 50f) {
    +                if (CurrentSpeed > 0 && Vector3.Angle (transform.forward, m_Rigidbody.velocity) < 50f) {
                        m_WheelColliders [i].brakeTorque = m_BrakeTorque * footbrake;
                    } else if (footbrake > 0) {
                        m_WheelColliders [i].brakeTorque = 0f;
    @@ -571,17 +503,17 @@ namespace UnityStandardAssets.Vehicles.Car
                    m_WheelColliders [i].GetGroundHit (out wheelHit);

                    // is the tire slipping above the given threshhold
    -                if (Mathf.Abs (wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs (wheelHit.sidewaysSlip) >= m_SlipLimit) {
    -                    m_WheelEffects [i].EmitTyreSmoke ();
    -                    continue;
    -                }
    +                //if (Mathf.Abs (wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs (wheelHit.sidewaysSlip) >= m_SlipLimit) {
    +                //    m_WheelEffects [i].EmitTyreSmoke ();
    +                //    continue;
    +                //}

                    // if it wasnt slipping stop all the audio
    -                if (m_WheelEffects [i].PlayingAudio) {
    -                    m_WheelEffects [i].StopAudio ();
    -                }
    +                //if (m_WheelEffects [i].PlayingAudio) {
    +                //    m_WheelEffects [i].StopAudio ();
    +                //}
                    // end the trail generation
    -                m_WheelEffects [i].EndSkidTrail ();
    +                //m_WheelEffects [i].EndSkidTrail ();
                }
            }

    @@ -629,125 +561,6 @@ namespace UnityStandardAssets.Vehicles.Car
                    }
                }
            }
    -
    -
    -               //Changed the WriteSamplesToDisk to a IEnumerator method that plays back recording along with percent status from UISystem script
    -               //instead of showing frozen screen until all data is recorded
    -               public IEnumerator WriteSamplesToDisk()
    -               {
    -                       yield return new WaitForSeconds(0.000f); //retrieve as fast as we can but still allow communication of main thread to screen and UISystem
    -                       if (carSamples.Count > 0) {
    -                               //pull off a sample from the que
    -                               CarSample sample = carSamples.Dequeue();
    -
    -                               //pysically moving the car to get the right camera position
    -                               transform.position = sample.position;
    -                               transform.rotation = sample.rotation;
    -
    -                               // Capture and Persist Image
    -                               string centerPath = WriteImage (CenterCamera, "center", sample.timeStamp);
    -                               string leftPath = WriteImage (LeftCamera, "left", sample.timeStamp);
    -                               string rightPath = WriteImage (RightCamera, "right", sample.timeStamp);
    -
    -                               string row = string.Format ("{0},{1},{2},{3},{4},{5},{6}\n", centerPath, leftPath, rightPath, sample.steeringAngle, sample.throttle, sample.brake, sample.speed);
    -                               File.AppendAllText (Path.Combine (m_saveLocation, CSVFileName), row);
    -                       }
    -                       if (carSamples.Count > 0) {
    -                               //request if there are more samples to pull
    -                               StartCoroutine(WriteSamplesToDisk());
    -                       }
    -                       else
    -                       {
    -                               //all samples have been pulled
    -                               StopCoroutine(WriteSamplesToDisk());
    -                               isSaving = false;
    -
    -                               //need to reset the car back to its position before ending recording, otherwise sometimes the car ended up in strange areas
    -                               transform.position = saved_position;
    -                               transform.rotation = saved_rotation;
    -                               m_Rigidbody.velocity = new Vector3(0f,-10f,0f);
    -                               Move(0f, 0f, 0f, 0f);
    -
    -                       }
    -               }
    -
    -               public float getSavePercent()
    -               {
    -                       return (float)(TotalSamples-carSamples.Count)/TotalSamples;
    -               }
    -
    -               public bool getSaveStatus()
    -               {
    -                       return isSaving;
    -               }
    -
    -
    -        public IEnumerator Sample()
    -        {
    -            // Start the Coroutine to Capture Data Every Second.
    -            // Persist that Information to a CSV and Perist the Camera Frame
    -            yield return new WaitForSeconds(0.0666666666666667f);
    -
    -            if (m_saveLocation != "")
    -            {
    -                CarSample sample = new CarSample();
    -
    -                sample.timeStamp = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
    -                sample.steeringAngle = m_SteerAngle / m_MaximumSteerAngle;
    -                sample.throttle = AccelInput;
    -                sample.brake = BrakeInput;
    -                sample.speed = CurrentSpeed;
    -                sample.position = transform.position;
    -                sample.rotation = transform.rotation;
    -
    -                carSamples.Enqueue(sample);
    -
    -                sample = null;
    -                //may or may not be needed
    -            }
    -
    -            // Only reschedule if the button hasn't toggled
    -            if (IsRecording)
    -            {
    -                StartCoroutine(Sample());
    -            }
    -
    -        }
    -
    -        private void OpenFolder(string location)
    -        {
    -            m_saveLocation = location;
    -            Directory.CreateDirectory (Path.Combine(m_saveLocation, DirFrames));
    -        }
    -
    -        private string WriteImage (Camera camera, string prepend, string timestamp)
    -        {
    -            //needed to force camera update
    -            camera.Render();
    -            RenderTexture targetTexture = camera.targetTexture;
    -            RenderTexture.active = targetTexture;
    -            Texture2D texture2D = new Texture2D (targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
    -            texture2D.ReadPixels (new Rect (0, 0, targetTexture.width, targetTexture.height), 0, 0);
    -            texture2D.Apply ();
    -            byte[] image = texture2D.EncodeToJPG ();
    -            UnityEngine.Object.DestroyImmediate (texture2D);
    -            string directory = Path.Combine(m_saveLocation, DirFrames);
    -            string path = Path.Combine(directory, prepend + "_" + timestamp + ".jpg");
    -            File.WriteAllBytes (path, image);
    -            image = null;
    -            return path;
    -        }
    -    }
    -
    -    internal class CarSample
    -    {
    -        public Quaternion rotation;
    -        public Vector3 position;
    -        public float steeringAngle;
    -        public float throttle;
    -        public float brake;
    -        public float speed;
    -        public string timeStamp;
    +
        }
    -
    }

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs
index 7ab889e..d5baea2 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs
@@ -1,4 +1,4 @@
-﻿using System;
+using System;
 using UnityEngine;
 using UnityStandardAssets.CrossPlatformInput;

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarUserControl.cs

using UnityEngine;
using System.Collections;


namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car;
        private Steering s;

        private void Awake()
        {
            m_Car = GetComponent<CarController>();
            s = new Steering();
            s.Start();
        }

        private void FixedUpdate()
        {
            s.UpdateValues();
            m_Car.Move(s.H, s.V, s.V, 0f);

        }
    }
}


### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/WheelEffects.cs

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/WheelEffects.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/WheelEffects.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/WheelEffects.cs
index 292e4a6..0071004 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/WheelEffects.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/WheelEffects.cs
@@ -22,15 +22,6 @@ namespace UnityStandardAssets.Vehicles.Car
         {
             skidParticles = transform.root.GetComponentInChildren<ParticleSystem>();

-            if (skidParticles == null)
-            {
-                Debug.LogWarning(" no particle system found on car to generate smoke particles");
-            }
-            else
-            {
-                skidParticles.Stop();
-            }
-
             m_WheelCollider = GetComponent<WheelCollider>();
             m_AudioSource = GetComponent<AudioSource>();
             PlayingAudio = false;
@@ -39,11 +30,25 @@ namespace UnityStandardAssets.Vehicles.Car
             {
                 skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
             }
+
+            // Support Term 3: skip warning when particles are absent; stop only if present
+            if (skidParticles != null)
+            {
+                skidParticles.Stop();
+            }
+            // else
+            // {
+            //     Debug.LogWarning(" no particle system found on car to generate smoke particles");
+            // }
+
         }


         public void EmitTyreSmoke()
         {
+            // Term 3-safe: guard when particle system or collider are absent
+            if (skidParticles == null || m_WheelCollider == null) return;
+
             skidParticles.transform.position = transform.position - transform.up*m_WheelCollider.radius;
             skidParticles.Emit(1);
             if (!skidding)
@@ -70,6 +75,9 @@ namespace UnityStandardAssets.Vehicles.Car
         public IEnumerator StartSkidTrail()
         {
             skidding = true;
+            // Term 3-safe: no skid trail prefab present
+            if (SkidTrailPrefab == null) yield break;
+
             m_SkidTrail = Instantiate(SkidTrailPrefab);
             while (m_SkidTrail == null)
             {
@@ -87,6 +95,9 @@ namespace UnityStandardAssets.Vehicles.Car
                 return;
             }
             skidding = false;
+            // Term 3-safe: guard when skid trail wasn't created
+            if (m_SkidTrail == null) return;
+
             m_SkidTrail.parent = skidTrailsDetachedParent;
             Destroy(m_SkidTrail.gameObject, 10);
         }

## Untracked files:

### Assets/1_SelfDrivingCar/Scripts/term2/

**Kept a Backup of Term 2 Related C# Scripts Before Migrating to Term 3:**

#### `C:\src\self-driving-car-sim\Assets\1_SelfDrivingCar\Scripts\term2\CommandServerTerm2.cs`:

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System.Security.AccessControl;
using System.Globalization;

public class CommandServerTerm2 : MonoBehaviour
{
	public CarRemoteControl CarRemoteControl;
	public Camera FrontFacingCamera;
	private SocketIOComponent _socket;
	private CarController _carController;

	// Use this for initialization
	void Start()
	{
		_socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
		_socket.On("open", OnOpen);
		_socket.On("steer", OnSteer);
		_socket.On("manual", onManual);
		_carController = CarRemoteControl.GetComponent<CarController>();
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
		EmitTelemetry(obj);
	}

	// 
	void onManual(SocketIOEvent obj)
	{
		Debug.Log("Triggered Callback Driving Manually");
		EmitTelemetry (obj);
	}

	void OnSteer(SocketIOEvent obj)
	{
		Debug.Log("Triggered Callback Driving Autonomously via PID Steering");

		Debug.Log("PID Control Active");

		try {
			// Add null check
			if(obj.data == null) {
				Debug.LogError("Empty steering command received");
				return;
			}

			// Add parse error handling
			JSONObject jsonObject = obj.data;

			Debug.Log($"Full JSON Structure: {jsonObject.Print(true)}");

			if(!jsonObject.HasField("steering_angle") || !jsonObject.HasField("throttle")) {
				Debug.LogError($"Missing fields. Actual data: {string.Join(",", jsonObject.keys)}");
				return;
			}

			// Add format verification
			float steeringAngle = jsonObject.GetField("steering_angle").f;
			float throttle = jsonObject.GetField("throttle").f;

			Debug.Log($"Raw steeringAngle = {steeringAngle} (Type: {jsonObject.GetField("steering_angle").type})");
			Debug.Log($"Raw throttle = {throttle} (Type: {jsonObject.GetField("throttle").type})");

			CarRemoteControl.SteeringAngle = Mathf.Clamp(steeringAngle, -1, 1);
			CarRemoteControl.Acceleration = Mathf.Clamp(throttle, 0, 1);

			EmitTelemetry(obj);
		} catch(Exception ex) {
			Debug.LogError($"Steering error: {ex}\nFull JSON: {obj.data}");
		}
	}

	void EmitTelemetry(SocketIOEvent obj)
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			try {

				print("Attempting to Send...");
				// send only if it's not being manually driven
				if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
					_socket.Emit("telemetry", new JSONObject());
				}
				else {
					// Add CTE calculation (replace with your actual CTE logic)
					float currentCTE = CalculateCTE();

					// Collect Simulated Data from the Car
					JSONObject telemetryData = new JSONObject(JSONObject.Type.OBJECT);

					// Add fields individually
					telemetryData.AddField("cte", currentCTE);
					telemetryData.AddField("steering_angle", _carController.CurrentSteerAngle);
					telemetryData.AddField("throttle", _carController.AccelInput);
					telemetryData.AddField("speed", _carController.CurrentSpeed);
					telemetryData.AddField("image", Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera)));
					_socket.Emit("telemetry", telemetryData);
				}

			} catch(Exception ex) {
				Debug.LogError($"Telemetry error: {ex}");
			}


		});
	}

	// Add this temporary CTE calculation method
	float CalculateCTE()
	{
		// Replace with your actual CTE calculation logic
		return UnityEngine.Random.Range(0.0f, 1.0f);
	}
}


#### `C:\src\self-driving-car-sim\Assets\1_SelfDrivingCar\Scripts\term2\MenuOptionsTerm2.cs`:

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuOptionsTerm2 : MonoBehaviour
{
    // Term selection state
    // public static bool IsTerm1 { get; private set; }
    public bool IsTerm1 = false;
    public bool IsTerm2 = false;

    // Term 1 Components
    private int track = 0;
    private Outline[] outlines;

    // Term 2 Components
    private int project = 0;
    public Text projectName;
    public Image projectImage;
	public Sprite project_1;
	public Sprite project_2;
	public Sprite project_3;
	public Sprite project_4;
	public Sprite project_5;

    // Shared UI elements
    // public GameObject term1UI;
    // public GameObject term2UI;

    public void Start()
    {
        InitializeTermUI();
    }

    void InitializeTermUI()
    {
        Debug.Log("Initialize Term UI");
        Debug.Log($"IsTerm1 = {IsTerm1}");
        Debug.Log($"IsTerm2 = {IsTerm2}");
        // term1UI.SetActive(IsTerm1);
        // term2UI.SetActive(!IsTerm1);

        if (IsTerm1) {
            InitializeTerm1();
        }
        else if (IsTerm2) {
            InitializeTerm2();
        }
    }

    #region Shared Methods
	public void ControlMenu()
	{
        Debug.Log("Going to control menu scene");
        Debug.Log($"IsTerm1 = {IsTerm1}");
        Debug.Log($"IsTerm2 = {IsTerm2}");
        if (IsTerm1) {
            SceneManager.LoadScene("ControlMenu");
        }
        else if (IsTerm2) {
            SceneManager.LoadScene("ControlMenuTerm2");
        }
	}

	public void MainMenu()
	{
        Debug.Log("Going to main menu scene");
        Debug.Log($"IsTerm1 = {IsTerm1}");
        Debug.Log($"IsTerm2 = {IsTerm2}");
        if (IsTerm1) {
            SceneManager.LoadScene("MenuScene");
        }
        else if (IsTerm2) {
            SceneManager.LoadScene("MenuSceneTerm2");
        }
	}
    #endregion // end of Shared Methods


    #region Term 1 Methods
    void InitializeTerm1()
    {
        outlines = GetComponentsInChildren<Outline>();
        if(outlines.Length > 0) {
            outlines[0].effectColor = Color.black;
            outlines[1].effectColor = Color.white;
        }
    }

    public void StartDrivingMode()
    {
        SceneManager.LoadScene(track == 0 ?
            "LakeTrackTraining" : "JungleTrackTraining");
    }

    public void StartAutonomousMode()
    {
        SceneManager.LoadScene(track == 0 ?
            "LakeTrackAutonomous" : "JungleTrackAutonomous");
    }

    public void SetLakeTrack()
    {
        track = 0;
        outlines[0].effectColor = Color.black;
        outlines[1].effectColor = Color.white;
    }

    public void SetMountainTrack()
    {
        track = 1;
        outlines[1].effectColor = Color.black;
        outlines[0].effectColor = Color.white;
    }
    #endregion // end of Term 1 Methods

    #region Term 2 Methods
    void InitializeTerm2()
    {
        if(projectName == null || projectImage == null)
        {
            Debug.Log("Missing Term 2 UI references! Maybe in Control Menu");
            return;
        }

        project = 0;
        UpdateProjectDisplay();
    }

    public void SelectMode()
    {
        switch(project)
        {
            case 0:
                SceneManager.LoadScene("EKF_project");
                break;
            case 1:
                SceneManager.LoadScene("UKF_project");
                break;
            case 2:
                SceneManager.LoadScene("particle_filter_v2");
                break;
            case 3:
                SceneManager.LoadScene("LakeTrackAutonomous_pid");
                break;
            case 4:
                SceneManager.LoadScene("LakeTrackAutonomous_mpc");
                break;
        }
    }

	public void Next()
	{
		project = (project + 1) % 5;
        UpdateProjectDisplay();
    }

    public void Previous()
    {
        project = (project == 0) ? 4 : project - 1;
        UpdateProjectDisplay();
    }


    void UpdateProjectDisplay()
    {

        if(projectName == null || projectImage == null)
        {
            Debug.Log("Project UI elements not assigned! Maybe in Control Menu");
            return;
        }


        projectName.text = project switch
        {
            0 => "Project 1: Extended Kalman Filter",
            1 => "Project 2: Unscented Kalman filters",
            2 => "Project 3: Kidnapped Vehicle",
            3 => "Project 4: PID Controller",
            4 => "Project 5: MPC Controller",
            _ => "Invalid Project"
        };

        projectImage.sprite = project switch
        {
            0 => project_1,
            1 => project_2,
            2 => project_3,
            3 => project_4,
            4 => project_5,
            _ => null
        };
    }
    #endregion // end of Term 2 Methods

    // Add this to preserve term state across scenes
    // void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     if(scene.name == "MenuScene") {
    //         SwitchToTerm1();
    //     }
    //     else if(scene.name == "MenuSceneTerm2") {
    //         SwitchToTerm2();
    //     }
    // }

    // public static bool GetTerm()
    // {
    //     return IsTerm1;
    // }

    // #region Term Switching
    // public void SwitchToTerm1()
    // {
    //     // IsTerm1 = true;
    //     InitializeTermUI();
    // }

    // public void SwitchToTerm2()
    // {
    //     // IsTerm1 = false;
    //     InitializeTermUI();
    // }
    // #endregion
}


#### `C:\src\self-driving-car-sim\Assets\1_SelfDrivingCar\Scripts\term2\UISystemTerm2.cs`:

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;

public class UISystemTerm2 : MonoSingleton<UISystem> {

    public CarControllerTerm2 carController;
    public string GoodCarStatusMessage;
    public string BadSCartatusMessage;
    public Text MPH_Text;
    public Image MPH_Animation;
    public Text Angle_Text;
    public Text RecordStatus_Text;
	public Text DriveStatus_Text;
	public Text SaveStatus_Text;
    public GameObject RecordingPause; 
	public GameObject RecordDisabled;
	public bool isTraining = false;

    private bool recording;
    private float topSpeed;
	private bool saveRecording;


    // Use this for initialization
    void Start() {
		Debug.Log (isTraining);
        topSpeed = carController.MaxSpeed;
        recording = false;
        RecordingPause.SetActive(false);
		RecordStatus_Text.text = "RECORD";
		DriveStatus_Text.text = "";
		SaveStatus_Text.text = "";
		SetAngleValue(0);
        SetMPHValue(0);
		if (!isTraining) {
			DriveStatus_Text.text = "Mode: Autonomous";
			RecordDisabled.SetActive (true);
			RecordStatus_Text.text = "";
		} 
    }

    public void SetAngleValue(float value)
    {
        Angle_Text.text = value.ToString("N2") + "°";
    }

    public void SetMPHValue(float value)
    {
        MPH_Text.text = value.ToString("N2");
        //Do something with value for fill amounts
        MPH_Animation.fillAmount = value/topSpeed;
    }

    public void ToggleRecording()
    {
		// Don't record in autonomous mode
		if (!isTraining) {
			return;
		}

        if (!recording)
        {
			if (carController.checkSaveLocation()) 
			{
				recording = true;
				RecordingPause.SetActive (true);
				RecordStatus_Text.text = "RECORDING";
				carController.IsRecording = true;
			}
        }
        else
        {
			saveRecording = true;
			carController.IsRecording = false;
        }
    }
	
    void UpdateCarValues()
    {
        SetMPHValue(carController.CurrentSpeed);
        SetAngleValue(carController.CurrentSteerAngle);
    }

	// Update is called once per frame
	void Update () {

        // Easier than pressing the actual button :-)
        // Should make recording training data more pleasant.

		if (carController.getSaveStatus ()) {
			SaveStatus_Text.text = "Capturing Data: " + (int)(100 * carController.getSavePercent ()) + "%";
			//Debug.Log ("save percent is: " + carController.getSavePercent ());
		} 
		else if(saveRecording) 
		{
			SaveStatus_Text.text = "";
			recording = false;
			RecordingPause.SetActive(false);
			RecordStatus_Text.text = "RECORD";
			saveRecording = false;
		}

        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleRecording();
        }

		if (!isTraining) 
		{
			if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) 
			{
				DriveStatus_Text.color = Color.red;
				DriveStatus_Text.text = "Mode: Manual";
			} 
			else 
			{
				DriveStatus_Text.color = Color.white;
				DriveStatus_Text.text = "Mode: Autonomous";
			}
		}

	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Do Menu Here
            // SceneManager.LoadScene("MenuScene");
            ReturnToAppropriateMenu();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Do Console Here
        }

        UpdateCarValues();
    }

    // Automatic Term Detection without MenuOptions
    void ReturnToAppropriateMenu()
    {
        bool isTerm1Scene = false;
        string currentScene = SceneManager.GetActiveScene().name;

        if(currentScene == "LakeTrackTraining" || currentScene == "LakeTrackAutonomous") {
            isTerm1Scene = true;
        }
        else if(currentScene == "JungleTrackTraining" || currentScene == "JungleTrackAutonomous") {
            isTerm1Scene = true;
        }

        Debug.Log($"isTerm1Scene = {isTerm1Scene}");

        // bool isTerm1Scene = currentScene.Contains("LakeTrack") || currentScene.Contains("JungleTrack");

        SceneManager.LoadScene(isTerm1Scene ? "MenuScene" : "MenuSceneTerm2");
    }

    // Automatic Term Detection with MenuOptions (Under Development)
    // void ReturnToAppropriateMenu()
    // {
    //     // Get term state from persistent MenuOptions
    //     bool isTerm1 = MenuOptions.GetTerm();

    //     // Verify scene existence first
    //     string targetScene = isTerm1 ? "MenuScene" : "MenuSceneTerm2";
    //     if(Application.CanStreamedLevelBeLoaded(targetScene))
    //     {
    //         SceneManager.LoadScene(targetScene);
    //     }
    //     else
    //     {
    //         Debug.LogError($"Missing scene: {targetScene}");
    //         SceneManager.LoadScene("MenuScene"); // Fallback to Term1
    //     }
    // }
}


### Assets/1_SelfDrivingCar/Scripts/term3/

**Earlier I tried to fuse Term 1+2 and Term 3 C# Scripts Together, But Term 3 Highway Driving Path Planner Didn't
Work, So switched to standalone Term 3 C# Scripts for the time being and Highway Driving Path Planner Unity3D
Scene Simulation Launches and Runs successfully; I plan to revisit these fused versions of Term 1+2 and Term 3
C# scripts and then verify that I can run Term 1+2 scenes still in the same Unity project as Term 3. The main
issue I believe has to do with certain C# scripts having less member variables in Term 1+2 compared to Term 3
and I need to account for that difference; Maybe leveraging parent child class approach would be sufficient
like having a parent class for all Term 1,2,3 and then having particular child classes for Term 1, 2, 3**

#### `C:\src\self-driving-car-sim\Assets\1_SelfDrivingCar\Scripts\term3\CommandServerFusedTerm2_3.cs`:

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SocketIO;
using UnityStandardAssets.Vehicles.Car;
using System.Security.AccessControl;
using System.Globalization;

public class CommandServerFusedTerm2_3 : MonoBehaviour
{
	public CarRemoteControl CarRemoteControl;
	public Camera FrontFacingCamera;

	// Term 3 additions
	public GameObject Car; // assign in Term 3
	private perfect_controller _perfectController;
	private CarTraffic _carTraffic;
	private bool _term3Mode = false;

	private SocketIOComponent _socket;
	private CarController _carController;

	// Use this for initialization
	void Start()
	{
		_socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
		_socket.On("open", OnOpen);
		_socket.On("steer", OnSteer);
		_socket.On("manual", onManual);

		// Term 3-specific events
		_socket.On("control", OnControl);
		_socket.On("close", OnClose);

		// Resolve CarController from Term 1/2 or Term 3 target
		_carController = (CarRemoteControl != null) ? CarRemoteControl.GetComponent<CarController>() : null;
		if (Car != null)
		{
			var cc = Car.GetComponent<CarController>();
			if (cc != null) _carController = cc;

			_perfectController = Car.GetComponent<perfect_controller>();
			_carTraffic = Car.GetComponent<CarTraffic>();
			_term3Mode = (_perfectController != null);
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnOpen(SocketIOEvent obj)
	{
		Debug.Log("Connection Open");
		if (_term3Mode)
		{
			_perfectController.OpenScript();
		}
		EmitTelemetry(obj);
	}

	void OnClose(SocketIOEvent obj)
	{
		Debug.Log("Connection Closed");
		if (_term3Mode)
		{
			_perfectController.CloseScript();
		}
	}

	// 
	void onManual(SocketIOEvent obj)
	{
		Debug.Log("Triggered Callback Driving Manually");
		EmitTelemetry (obj);
	}

	void OnSteer(SocketIOEvent obj)
	{
		Debug.Log("Triggered Callback Driving Autonomously via PID Steering");

		Debug.Log("PID Control Active");

		try {
			// Add null check
			if(obj.data == null) {
				Debug.LogError("Empty steering command received");
				return;
			}

			// Add parse error handling
			JSONObject jsonObject = obj.data;

			Debug.Log($"Full JSON Structure: {jsonObject.Print(true)}");

			if(!jsonObject.HasField("steering_angle") || !jsonObject.HasField("throttle")) {
				Debug.LogError($"Missing fields. Actual data: {string.Join(",", jsonObject.keys)}");
				return;
			}

			// Add format verification
			float steeringAngle = jsonObject.GetField("steering_angle").f;
			float throttle = jsonObject.GetField("throttle").f;

			Debug.Log($"Raw steeringAngle = {steeringAngle} (Type: {jsonObject.GetField("steering_angle").type})");
			Debug.Log($"Raw throttle = {throttle} (Type: {jsonObject.GetField("throttle").type})");

			CarRemoteControl.SteeringAngle = Mathf.Clamp(steeringAngle, -1, 1);
			CarRemoteControl.Acceleration = Mathf.Clamp(throttle, 0, 1);

			EmitTelemetry(obj);
		} catch(Exception ex) {
			Debug.LogError($"Steering error: {ex}\nFull JSON: {obj.data}");
		}
	}

	// Term 3: accept planned trajectory from the client
	void OnControl(SocketIOEvent obj)
	{
		if (!_term3Mode || _perfectController == null)
		{
			Debug.LogWarning("Received 'control' but Term 3 mode is not active.");
			return;
		}

		try
		{
			JSONObject jsonObject = obj.data;
			var next_x = jsonObject.GetField("next_x");
			var next_y = jsonObject.GetField("next_y");

			if (next_x == null || next_y == null || next_x.Count != next_y.Count)
			{
				Debug.LogError("Invalid 'control' payload.");
				return;
			}

			List<float> xs = new List<float>();
			List<float> ys = new List<float>();
			for (int i = 0; i < next_x.Count; i++)
			{
				xs.Add(next_x[i].f);
				ys.Add(next_y[i].f);
			}

			_perfectController.setControlPath(xs, ys);
			_perfectController.setSimulatorProcess();

			EmitTelemetry(obj);
		}
		catch (Exception ex)
		{
			Debug.LogError($"Control error: {ex}");
		}
	}

	// Term 3: convert Unity yaw to conventional math orientation
	float convertAngle(float psi) {
		if (psi >= 0 && psi <= 90) {
			return 90 - psi;
		}
		else if (psi > 90 && psi <= 180) {
			return 360 - (psi - 90);
		}
		else if (psi > 180 && psi <= 270) {
			return 270 - (psi - 180);
		}
		return 180 - (psi - 270);
	}

	void EmitTelemetry(SocketIOEvent obj)
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			try {
				// Term 3 telemetry
				if (_term3Mode && _perfectController != null && Car != null)
				{
					if (!_perfectController.isServerProcess())
					{
						_socket.Emit("telemetry", new JSONObject());
					}
					else
					{
						_perfectController.ServerPause();

						// Collect Term 3 Data
						Dictionary<string, JSONObject> data = new Dictionary<string, JSONObject>();
						data["x"] = new JSONObject(Car.transform.position.x);
						data["y"] = new JSONObject(Car.transform.position.z);
						data["yaw"] = new JSONObject(convertAngle(Car.transform.rotation.eulerAngles.y));
						data["speed"] = new JSONObject(_carController != null ? _carController.CurrentSpeed : 0f);

						var carAI = (CarAIControl) Car.GetComponent(typeof(CarAIControl));
						List<float> frenet_values = carAI.getThisFrenetFrame();
						data["s"] = new JSONObject(frenet_values[0]);
						data["d"] = new JSONObject(frenet_values[1]);

						// Previous Path data
						var previous_path_x = _perfectController.previous_path_x();
						var previous_path_y = _perfectController.previous_path_y();

						JSONObject arr_x = new JSONObject(JSONObject.Type.ARRAY);
						JSONObject arr_y = new JSONObject(JSONObject.Type.ARRAY);
						for (int i = 0; i < previous_path_x.Count; i++)
						{
							arr_x.Add(previous_path_x[i]);
							arr_y.Add(previous_path_y[i]);
						}
						data["previous_path_x"] = arr_x;
						data["previous_path_y"] = arr_y;

						// End path S and D values
						var end_path_s = 0.0f;
						var end_path_d = 0.0f;
						if (previous_path_x.Count > 0)
						{
							List<float> fr = carAI.getFrenetFrame(
								previous_path_x[previous_path_x.Count - 1],
								previous_path_y[previous_path_y.Count - 1]);
							end_path_s = fr[0];
							end_path_d = fr[1];
						}
						data["end_path_s"] = new JSONObject(end_path_s);
						data["end_path_d"] = new JSONObject(end_path_d);

						if (_carTraffic == null) _carTraffic = Car.GetComponent<CarTraffic>();
						data["sensor_fusion"] = new JSONObject(_carTraffic.example_sensor_fusion());

						_socket.Emit("telemetry", new JSONObject(data));
					}

					return; // don't fall through to Term 1/2
				}

				// Term 1/2 telemetry (existing behavior)
				print("Attempting to Send...");
				// send only if it's not being manually driven
				if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
					_socket.Emit("telemetry", new JSONObject());
				}
				else {
					// Add CTE calculation (replace with your actual CTE logic)
					float currentCTE = CalculateCTE();

					// Collect Simulated Data from the Car
					JSONObject telemetryData = new JSONObject(JSONObject.Type.OBJECT);

					// Add fields individually
					telemetryData.AddField("cte", currentCTE);
					telemetryData.AddField("steering_angle", _carController != null ? _carController.CurrentSteerAngle : 0f);
					telemetryData.AddField("throttle", _carController != null ? _carController.AccelInput : 0f);
					telemetryData.AddField("speed", _carController != null ? _carController.CurrentSpeed : 0f);
					telemetryData.AddField("image", Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera)));
					_socket.Emit("telemetry", telemetryData);
				}
			} catch(Exception ex) {
				Debug.LogError($"Telemetry error: {ex}");
			}
		});
	}

	// Add this temporary CTE calculation method
	float CalculateCTE()
	{
		// Replace with your actual CTE calculation logic
		return UnityEngine.Random.Range(0.0f, 1.0f);
	}
}

#### `C:\src\self-driving-car-sim\Assets\1_SelfDrivingCar\Scripts\term3\MenuOptionsFusedTerm2_3.cs`:

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuOptionsFusedTerm2_3 : MonoBehaviour
{
    // Term selection state
    // public static bool IsTerm1 { get; private set; }
    public bool IsTerm1 = false;
    public bool IsTerm2 = false;
    public bool IsTerm3 = false; // Term 3 flag

    // Term 1 Components
    private int track = 0;
    private Outline[] outlines;

    // Term 2 Components
    private int project = 0;
    public Text projectName;
    public Image projectImage;
	public Sprite project_1;
	public Sprite project_2;
	public Sprite project_3;
	public Sprite project_4;
	public Sprite project_5;

    // Shared UI elements
    // public GameObject term1UI;
    // public GameObject term2UI;

    public void Start()
    {
        InitializeTermUI();
    }

    void InitializeTermUI()
    {
        Debug.Log("Initialize Term UI");
        Debug.Log($"IsTerm1 = {IsTerm1}");
        Debug.Log($"IsTerm2 = {IsTerm2}");
        // term1UI.SetActive(IsTerm1);
        // term2UI.SetActive(!IsTerm1);

        if (IsTerm1) {
            InitializeTerm1();
        }
        else if (IsTerm2) {
            InitializeTerm2();
        }
        else if (IsTerm3) {
            InitializeTerm3();
        }
    }

    void InitializeTerm3()
    {
        // Match Term 3: set first outline to black
        outlines = GetComponentsInChildren<Outline>();
        if (outlines != null && outlines.Length > 0)
        {
            outlines[0].effectColor = Color.black;
        }
    }

    #region Shared Methods
	public void ControlMenu()
	{
        Debug.Log("Going to control menu scene");
        Debug.Log($"IsTerm1 = {IsTerm1}");
        Debug.Log($"IsTerm2 = {IsTerm2}");
        if (IsTerm1) {
            SceneManager.LoadScene("ControlMenu");
        }
        else if (IsTerm2) {
            SceneManager.LoadScene("ControlMenuTerm2");
        }
        else if (IsTerm3) {
            SceneManager.LoadScene("ControlMenu"); // Term 3 uses ControlMenu
        }
	}

	public void MainMenu()
	{
        Debug.Log("Going to main menu scene");
        Debug.Log($"IsTerm1 = {IsTerm1}");
        Debug.Log($"IsTerm2 = {IsTerm2}");
        if (IsTerm1) {
            SceneManager.LoadScene("MenuScene");
        }
        else if (IsTerm2) {
            SceneManager.LoadScene("MenuSceneTerm2");
        }
        else if (IsTerm3) {
            SceneManager.LoadScene("MenuScene"); // Term 3 uses MenuScene
        }
	}
    #endregion // end of Shared Methods


    #region Term 1 Methods
    void InitializeTerm1()
    {
        outlines = GetComponentsInChildren<Outline>();
        if(outlines.Length > 0) {
            outlines[0].effectColor = Color.black;
            outlines[1].effectColor = Color.white;
        }
    }

    public void StartDrivingMode()
    {
        SceneManager.LoadScene(track == 0 ?
            "LakeTrackTraining" : "JungleTrackTraining");
    }

    public void StartAutonomousMode()
    {
        SceneManager.LoadScene(track == 0 ?
            "LakeTrackAutonomous" : "JungleTrackAutonomous");
    }

    public void SetLakeTrack()
    {
        track = 0;
        outlines[0].effectColor = Color.black;
        outlines[1].effectColor = Color.white;
    }

    public void SetMountainTrack()
    {
        track = 1;
        outlines[1].effectColor = Color.black;
        outlines[0].effectColor = Color.white;
    }
    #endregion // end of Term 1 Methods

    #region Term 2 Methods
    void InitializeTerm2()
    {
        if(projectName == null || projectImage == null)
        {
            Debug.Log("Missing Term 2 UI references! Maybe in Control Menu");
            return;
        }

        project = 0;
        UpdateProjectDisplay();
    }

    public void SelectMode()
    {
        switch(project)
        {
            case 0:
                SceneManager.LoadScene("EKF_project");
                break;
            case 1:
                SceneManager.LoadScene("UKF_project");
                break;
            case 2:
                SceneManager.LoadScene("particle_filter_v2");
                break;
            case 3:
                SceneManager.LoadScene("LakeTrackAutonomous_pid");
                break;
            case 4:
                SceneManager.LoadScene("LakeTrackAutonomous_mpc");
                break;
        }
    }

	public void Next()
	{
		project = (project + 1) % 5;
        UpdateProjectDisplay();
    }

    public void Previous()
    {
        project = (project == 0) ? 4 : project - 1;
        UpdateProjectDisplay();
    }


    void UpdateProjectDisplay()
    {

        if(projectName == null || projectImage == null)
        {
            Debug.Log("Project UI elements not assigned! Maybe in Control Menu");
            return;
        }


        projectName.text = project switch
        {
            0 => "Project 1: Extended Kalman Filter",
            1 => "Project 2: Unscented Kalman filters",
            2 => "Project 3: Kidnapped Vehicle",
            3 => "Project 4: PID Controller",
            4 => "Project 5: MPC Controller",
            _ => "Invalid Project"
        };

        projectImage.sprite = project switch
        {
            0 => project_1,
            1 => project_2,
            2 => project_3,
            3 => project_4,
            4 => project_5,
            _ => null
        };
    }
    #endregion // end of Term 2 Methods

    // Term 3 entry point
    public void StartPathPlanning()
    {
        SceneManager.LoadScene("PathPlanning");
    }

    // Add this to preserve term state across scenes
    // void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     if(scene.name == "MenuScene") {
    //         SwitchToTerm1();
    //     }
    //     else if(scene.name == "MenuSceneTerm2") {
    //         SwitchToTerm2();
    //     }
    // }

    // public static bool GetTerm()
    // {
    //     return IsTerm1;
    // }

    // #region Term Switching
    // public void SwitchToTerm1()
    // {
    //     // IsTerm1 = true;
    //     InitializeTermUI();
    // }

    // public void SwitchToTerm2()
    // {
    //     // IsTerm1 = false;
    //     InitializeTermUI();
    // }
    // #endregion
}


#### `C:\src\self-driving-car-sim\Assets\1_SelfDrivingCar\Scripts\term3\UISystemFusedTerm2_3.cs`:

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;
using System.Reflection; // Term 3 safe sensing via reflection
using System; // fix: needed for Convert.ToSingle

public class UISystemFusedTerm2_3 : MonoSingleton<UISystem> {

    public CarControllerFusedTerm2_3 carController;
    public string GoodCarStatusMessage;
    public string BadSCartatusMessage;
    public Text MPH_Text;
    public Image MPH_Animation;
    public Text Angle_Text;
    public Text RecordStatus_Text;
    public Text DriveStatus_Text;
    public Text SaveStatus_Text;
    public GameObject RecordingPause; 
    public GameObject RecordDisabled;
    public bool isTraining = false;

    // Term 3 additions (optional UI, assign in Term 3 scenes)
    public Camera mainCamera;
    public Text AccT_Text;
    public Text AccN_Text;
    public Text Acc_Text;
    public Text Jerk_Text;
    public Text Collision_Text;
    public Text Speeding_Text;
    public Text Lane_Text;
    public Text Best_Distance_Text;
    public Text Curr_Distance_Text;
    public Text Curr_Time_Text;
    public Text AccStatus_Text;
    public Text JerkStatus_Text;

    private bool recording;
    private float topSpeed;
    private bool saveRecording;

    // Term 3 runtime state
    private bool auto_drive;
    private CarAIControl carAI;
    private float best_dist_eval = 0f;
    private bool check_incidents;
    private bool _term3Mode = false;

    // Use this for initialization
    void Start() {
		Debug.Log (isTraining);
        topSpeed = carController.MaxSpeed;
        recording = false;
        RecordingPause.SetActive(false);
		RecordStatus_Text.text = "RECORD";
		DriveStatus_Text.text = "";
		SaveStatus_Text.text = "";
		SetAngleValue(0);
        SetMPHValue(0);
		if (!isTraining) {
			DriveStatus_Text.text = "Mode: Autonomous";
			RecordDisabled.SetActive (true);
			RecordStatus_Text.text = "";
		} 

        // Term 3 detection and init
        string sceneName = SceneManager.GetActiveScene().name;
        bool isPathPlanningScene = sceneName == "PathPlanning";
        bool hasTerm3Ui = AccT_Text != null || AccN_Text != null || Acc_Text != null || Jerk_Text != null;
        bool hasCarAI = carController != null && carController.GetComponent(typeof(CarAIControl)) != null;
        _term3Mode = isPathPlanningScene || (hasTerm3Ui && hasCarAI);

        if (_term3Mode) {
            if (AccStatus_Text != null) AccStatus_Text.text = "";
            if (JerkStatus_Text != null) JerkStatus_Text.text = "";
            SetMPHValue(0);
            carAI = (CarAIControl) carController.GetComponent(typeof(CarAIControl));
            auto_drive = true;
        }
    }

    public void SetAngleValue(float value)
    {
        Angle_Text.text = value.ToString("N2") + "°";
    }

    public void SetMPHValue(float value)
    {
        MPH_Text.text = value.ToString("N2");
        MPH_Animation.fillAmount = value/topSpeed;
    }

    // Term 3 safe sensing helpers (avoid compile-time dependency on methods)
    float SenseFloat(string method) {
        if (carController == null) return 0f;
        var mi = carController.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (mi == null) return 0f;
        var v = mi.Invoke(carController, null);
        return v is float f ? f : Convert.ToSingle(v);
    }

    // Term 3 UI setters (no-op if Text is not assigned)
    public void SetAccTValue(float value) { if (AccT_Text != null) AccT_Text.text = "AccT: " + value.ToString("N0") + " m/s^2"; }
    public void SetAccNValue(float value) { if (AccN_Text != null) AccN_Text.text = "AccN: " + value.ToString("N0") + " m/s^2"; }
    public void SetAccValue(float value) {
        if (Acc_Text != null) {
            Acc_Text.text = "AccTotal: " + value.ToString("N0") + " m/s^2";
            if (value >= 10) {
                Acc_Text.color = Color.red;
                if (AccStatus_Text != null) AccStatus_Text.text = "Max Acceleration Exceeded!";
                check_incidents = true;
            } else {
                Acc_Text.color = Color.white;
                if (AccStatus_Text != null) AccStatus_Text.text = "";
            }
        }
    }
    public void SetJerkValue(float value) {
        if (Jerk_Text != null) {
            Jerk_Text.text = "Jerk: " + value.ToString("N0") + " m/s^3";
            if (Mathf.Abs(value) >= 10) {
                Jerk_Text.color = Color.red;
                if (JerkStatus_Text != null) JerkStatus_Text.text = "Max Jerk Exceeded!";
                check_incidents = true;
            } else {
                Jerk_Text.color = Color.white;
                if (JerkStatus_Text != null) JerkStatus_Text.text = "";
            }
        }
    }
    public void SetCollisionValue(bool collision) {
        if (Collision_Text == null) return;
        if (collision) {
            Collision_Text.color = Color.red;
            Collision_Text.text = "Collision!";
            check_incidents = true;
        } else {
            Collision_Text.color = Color.white;
            Collision_Text.text = "";
        }
    }
    public void SetSpeedingValue(bool speeding) {
        if (Speeding_Text == null) return;
        if (speeding) {
            Speeding_Text.color = Color.red;
            Speeding_Text.text = "Violated Speed Limit!";
            check_incidents = true;
        } else {
            Speeding_Text.color = Color.white;
            Speeding_Text.text = "";
        }
    }
    public void SetLaneValue(bool outside_lane) {
        if (Lane_Text == null) return;
        if (outside_lane) {
            Lane_Text.color = Color.red;
            Lane_Text.text = "Outside of Lane!";
            check_incidents = true;
        } else {
            Lane_Text.color = Color.white;
            Lane_Text.text = "";
        }
    }
    public void SetDistanceValue(float dist_eval) {
        if (!_term3Mode) return;
        if (auto_drive && dist_eval > best_dist_eval) {
            best_dist_eval = dist_eval;
            if (Best_Distance_Text != null) Best_Distance_Text.text = "Best: " + dist_eval.ToString("N2") + " Miles";
        }
        if (Curr_Distance_Text != null) Curr_Distance_Text.text = "Curr: " + dist_eval.ToString("N2") + " Miles";
    }
    public void SetTimerValue(int seconds) {
        if (Curr_Time_Text == null) return;
        int hours = (seconds / 3600);
        seconds -= hours * 3600;
        int minutes = seconds / 60;
        seconds -= minutes * 60;
        Curr_Time_Text.text = "Timer: " + hours.ToString("0") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void ToggleRecording()
    {
		// Don't record in autonomous mode
		if (!isTraining) {
			return;
		}

        if (!recording)
        {
			if (carController.checkSaveLocation()) 
			{
				recording = true;
				RecordingPause.SetActive (true);
				RecordStatus_Text.text = "RECORDING";
				carController.IsRecording = true;
			}
        }
        else
        {
			saveRecording = true;
			carController.IsRecording = false;
        }
    }
	
    void UpdateCarValues()
    {
        if (_term3Mode) {
            check_incidents = false;
            SetMPHValue(carController.CurrentSpeed);
            SetAccTValue(SenseFloat("SenseAccT"));
            SetAccNValue(SenseFloat("SenseAccN"));
            SetAccValue(SenseFloat("SenseAcc"));
            SetJerkValue(SenseFloat("SenseJerk"));

            bool collision = carAI != null && carAI.CheckCollision();
            bool speeding = carAI != null && carAI.CheckSpeeding();
            bool outsideLane = carAI != null && carAI.CheckLanePos();

            SetCollisionValue(collision);
            SetSpeedingValue(speeding);
            SetLaneValue(outsideLane);

            if (check_incidents && carAI != null) {
                carAI.ResetDistance();
            }

            SetDistanceValue(carAI != null ? carAI.DistanceEval() : 0f);
            SetTimerValue(carAI != null ? carAI.TimerEval() : 0);
        } else {
            SetMPHValue(carController.CurrentSpeed);
            SetAngleValue(carController.CurrentSteerAngle);
        }
    }

	// Update is called once per frame
	void Update () {

        // Easier than pressing the actual button :-)
        // Should make recording training data more pleasant.

		if (carController.getSaveStatus ()) {
			SaveStatus_Text.text = "Capturing Data: " + (int)(100 * carController.getSavePercent ()) + "%";
			//Debug.Log ("save percent is: " + carController.getSavePercent ());
		} 
		else if(saveRecording) 
		{
			SaveStatus_Text.text = "";
			recording = false;
			RecordingPause.SetActive(false);
			RecordStatus_Text.text = "RECORD";
			saveRecording = false;
		}

        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleRecording();
        }

		if (!isTraining) 
		{
			if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) 
			{
				DriveStatus_Text.color = Color.red;
				DriveStatus_Text.text = "Mode: Manual";
			} 
			else 
			{
				DriveStatus_Text.color = Color.white;
				DriveStatus_Text.text = "Mode: Autonomous";
			}
		}

	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Do Menu Here
            // SceneManager.LoadScene("MenuScene");
            ReturnToAppropriateMenu();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Do Console Here
        }

        UpdateCarValues();
    }

    // Automatic Term Detection without MenuOptions
    void ReturnToAppropriateMenu()
    {
        bool isTerm1Scene = false;
        bool isTerm3Scene = false;
        string currentScene = SceneManager.GetActiveScene().name;

        if(currentScene == "LakeTrackTraining" || currentScene == "LakeTrackAutonomous") {
            isTerm1Scene = true;
        }
        else if(currentScene == "JungleTrackTraining" || currentScene == "JungleTrackAutonomous") {
            isTerm1Scene = true;
        }
        else if(currentScene == "PathPlanning") {
            isTerm3Scene = true;
        }

        Debug.Log($"isTerm1Scene = {isTerm1Scene}, isTerm3Scene = {isTerm3Scene}");

        SceneManager.LoadScene(isTerm3Scene ? "MenuScene" : (isTerm1Scene ? "MenuScene" : "MenuSceneTerm2"));
    }

    // Term 3: manual vs auto toggle
    public void ToggleDriveMode()
    {
        if (!_term3Mode) return;

        auto_drive = !auto_drive;

        var rb = carController != null ? carController.GetComponent<Rigidbody>() : null;
        var userControl = carController != null ? carController.GetComponent<CarUserControl>() : null;
        var orbit = mainCamera != null ? mainCamera.GetComponent<MouseOrbitImproved>() : null;

        if (!auto_drive) 
        {
            if (rb != null) rb.isKinematic = false;
            if (userControl != null) userControl.enabled = true;
            if (orbit != null) orbit.enabled = false;

            if (mainCamera != null && carController != null)
            {
                mainCamera.transform.position = carController.transform.TransformPoint(new Vector3 (0f, 3.2f, -8.2f));
                mainCamera.transform.localEulerAngles = new Vector3 (15f, 0f, 0f);
            }
        } 
        else 
        {
            if (rb != null) rb.isKinematic = true;
            if (userControl != null) userControl.enabled = false;
            if (orbit != null) orbit.enabled = true;
        }

        carAI.ResetDistance ();

    }
}



### Assets/RoadKit/MouseOrbitImproved.cs

using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {

	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = .5f;
	public float distanceMax = 15f;

	private Rigidbody rigidbody;

	float x = 0.0f;
	float y = 0.0f;

	// Use this for initialization
	void Start () 
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		rigidbody = GetComponent<Rigidbody>();

		// Make the rigid body not change rotation
		if (rigidbody != null)
		{
			rigidbody.freezeRotation = true;
		}
	}

	void LateUpdate () 
	{
		if (target) 
		{
			if (Input.GetMouseButton (0))
			{
				x += Input.GetAxis ("Mouse X") * xSpeed * distance * 0.02f;
				y -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;

				y = ClampAngle (y, yMinLimit, yMaxLimit);
			}

			Quaternion rotation = Quaternion.Euler(y, x, 0);

			distance = Mathf.Clamp (distance - Input.GetAxis ("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;

			transform.rotation = rotation;
			transform.position = position;
		}
	}


	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}


### Assets/Standard Assets/Vehicles/Car/Scripts/CarTraffic.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
	[RequireComponent (typeof(CarController))]
	public class CarTraffic : MonoBehaviour {

	//forward cars
	[SerializeField] private List<GameObject> cars;
	private Queue<GameObject> inactive_cars;

	public GameObject maincar;

	//reverse cars
	[SerializeField] private List<GameObject> carsR;
	private Queue<GameObject> inactive_carsR;
	
	//use counter to update every second
	private int counter;
	//number of cars to push at update, max is 3
	private bool init;
	private int count_max;

	// Use this for initialization
	void Start () {

		inactive_cars = new Queue<GameObject>();
		inactive_carsR = new Queue<GameObject>();
		counter = 0;
		// push two cars at a time

		init = true;
		count_max = Random.Range (20, 60);
		
	}
			
	// Update is called once per frame
	void Update () {
		if (counter >= count_max || init) {
			init = false;
			UpdateForward ();
			UpdateReverse ();
			counter = 0;
			count_max = Random.Range (20, 60);
		}
		else 
		{
			counter++;
		}
	}
	void FixedUpdate()
	{
			foreach (GameObject car in cars) {
				CarAIControl carAI = (CarAIControl)car.GetComponent (typeof(CarAIControl));
				List<float> frenet_values = carAI.getThisFrenetFrame ();
			}
	}

	
	public void UpdateForward(){
		//add any inactive car to inactive car list
		foreach (GameObject car in cars) 
		{
			CarAIControl carAI = (CarAIControl) car.GetComponent(typeof(CarAIControl));
			if (carAI.RegenerateCheck ()) {
				//turn off the car and add it to a list
				carAI.setStage();
				inactive_cars.Enqueue (car);
			} 

		}
		// count how many cars we have pushed
		int push = Random.Range(1,4);
		int pushed = 0;
		
		while ((inactive_cars.Count > 0) && (pushed < push)) {
			GameObject inactive_car = inactive_cars.Dequeue();

			CarAIControl carAI = (CarAIControl) inactive_car.GetComponent(typeof(CarAIControl));
		
			carAI.Spawn (cars);

			pushed++;
		}
		
	}
	public void UpdateReverse(){
		//add any inactive car to inactive car list
		foreach (GameObject car in carsR) 
		{
			CarAIControl carAI = (CarAIControl) car.GetComponent(typeof(CarAIControl));
			if (carAI.RegenerateCheck ()) {
				//turn off the car and add it to a list
				carAI.setStage();
				inactive_carsR.Enqueue (car);
			} 

		}
		// count how many cars we have pushed
		int push = Random.Range(1,4);
		int pushed = 0;
		
		while ((inactive_carsR.Count > 0) && (pushed < push)) {
				GameObject inactive_car = inactive_carsR.Dequeue();

				CarAIControl carAI = (CarAIControl) inactive_car.GetComponent(typeof(CarAIControl));

				carAI.Spawn (carsR);

				pushed++;
		}
	}

	public bool lane_clear(GameObject mycar,bool forward,int lane)
	{
			List<float> s_values = new List<float> ();
			List<int> index_values = new List<int> ();
			int s_index = -1;

			int index = 1;

			List<float> d_array = new List<float> ();

			if (forward) 
			{
				
				foreach(GameObject car in cars) 
				{
					CarAIControl carAI = (CarAIControl)car.GetComponent (typeof(CarAIControl));

					//List<float> frenet_values = carAI.getThisFrenetFrame ();

					d_array.Add (carAI.getD ());

					if (mycar.GetInstanceID () == car.GetInstanceID ()) 
					{
						s_index = s_values.Count;
						s_values.Add (carAI.getS());
						index_values.Add (index);
					}

					else 
					{
						float d_value = carAI.getD ();//frenet_values [1];
						if( ((d_value < (2+lane*4+2)) && (d_value > (2+lane*4-2) )) )// || carAI.BlinkerLight())
						{
							s_values.Add (carAI.getS());
							index_values.Add (index);
						}

					}

					index++;
						

				}

				CarAIControl carAImain = (CarAIControl)maincar.GetComponent (typeof(CarAIControl));

				//List<float> frenet_values_main = carAImain.getThisFrenetFrame ();
				float d_value_main = carAImain.getD();//frenet_values_main[1];
				//check if main car is in lane
				if((d_value_main < (2+lane*4+3)) && (d_value_main > (2+lane*4-3)))
				{
					s_values.Add (carAImain.getS());
					index_values.Add (-1);
				}

			} 
			else 
			{
				
				foreach (GameObject car in carsR) 
				{
					CarAIControl carAI = (CarAIControl)car.GetComponent (typeof(CarAIControl));

					List<float> frenet_values = carAI.getThisFrenetFrame ();

					if (mycar.GetInstanceID () == car.GetInstanceID ()) 
					{
						s_index = s_values.Count;
						s_values.Add (frenet_values [0]);
					} 
					else 
					{
						float d_value = frenet_values [1];
						if( ((d_value < (2+lane*4+2)) && (d_value > (2+lane*4-2))) || carAI.BlinkerLight())
						{
							s_values.Add (frenet_values[0]);
						}
					}
						
				}
					
			}

			bool clear = true;
			//if (forward) 
			//{
			//	Debug.Log ("I am car " + index_values [s_index] + " change into lane "+lane);
			//}
				

			for (int i = 0; i < s_values.Count; i++) 
			{
				
				if (i != s_index) 
				{
					
					clear = clear && (((s_values [i] - s_values [s_index]) > 20) || ((s_values [s_index] - s_values [i]) > 20));
					//if (forward) 
					//{
					//	Debug.Log ("car " + index_values [i] + " is this far " + (s_values [i] - s_values [s_index]));
					//}
				}
			}

			/*
			if (forward) 
			{
				if (clear) 
				{
					Debug.Log ("Its safe");
				} 
				else
				{
					Debug.Log ("Not safe");
				}
				for (int i = 0; i < d_array.Count; i++) 
				{
					int d_elem = i + 1;
					Debug.Log ("d" + d_elem + " " + d_array [i]);
				}
			}
			*/
				
			return clear;

	}

	public string example_sensor_fusion()
	{
			string result = "[";
			int car_id = 0;
			foreach (GameObject car in cars) 
			{
				CarAIControl carAI = (CarAIControl) car.GetComponent(typeof(CarAIControl));

				//List<float> test_values = carAI.getFrenetFrame (1682.316f,2968.043f);

				//Debug.Log ("test values "+test_values [0] + "," + test_values [1]);

				if(car_id > 0)
				{
					result += ",";
				}

				List<float> frenet_values = carAI.getThisFrenetFrame ();

				if(System.Single.IsNaN(frenet_values[0]))
				{
					frenet_values[0] = 0;
				}
				if(System.Single.IsNaN(frenet_values[1]))
				{
					frenet_values[1] = 0;
				}


				//Debug.Log (car.transform.position.x+","+car.transform.position.z+","+frenet_values[0]+","+frenet_values[1]);

				result += "[" + car_id + "," + car.transform.position.x + "," + car.transform.position.z +","+ car.GetComponent<Rigidbody> ().velocity.x 
					+","+ car.GetComponent<Rigidbody>().velocity.z+","+frenet_values[0]+","+frenet_values[1]+"]";

				car_id++;
			}
			result += "]";
			return result;
	}

}
}


### Assets/Standard Assets/Vehicles/Car/Scripts/perfect_controller.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perfect_controller : MonoBehaviour {

	private Rigidbody rb;
	private Collider collider;
	private Vector3 Offset = new Vector3 (0.3f, 0.0f, 0.0f);
	//Frame rate is 50FPS so delta t is .02
	//track points for the next second so 50 points in reference to global coordinates

	private List<float> x_points;
	private List<float> y_points;


	public GameObject next_point;
	private List<GameObject> way_points;


	private bool simulator_process;
	private bool background_process;
	private bool server_process;
	private bool script_running = false;

	// Use this for initialization
	void Start () {
		
		rb = GetComponent<Rigidbody> (); 
		collider = GetComponent<Collider> ();
		way_points = new List<GameObject> ();

		x_points = new List<float> ();
		y_points = new List<float> ();

		//by default create 50 points
		createPoints (50);

		//flag new data is ready to process
		simulator_process = false;
		server_process = true;

		Time.fixedDeltaTime = 0.02f;
	}

	public void setControlPath(List<float> x_set, List<float> y_set)
	{
		//start at the point that closes to the car

		//add more points if we need to
		if (x_set.Count > way_points.Count) 
		{
			createPoints (x_set.Count - way_points.Count);
		}

		float dist = 10000;
		int start_index = 0;

		for (int i = 0; i < x_set.Count; i++)
		{
			Vector2 car_pos = new Vector2 (transform.position.x, transform.position.z); 
			Vector2 point_pos = new Vector2 (x_set[i], y_set[i]); 
			if (Vector2.Distance (car_pos, point_pos) < dist) {
				dist = Vector2.Distance (car_pos, point_pos);
				start_index = i;
			}
		}
			
		bool behind_path = false;

		if (start_index != 0) {
			x_set = x_set.GetRange (start_index, x_set.Count - start_index);
			y_set = y_set.GetRange (start_index, y_set.Count - start_index);
		} 
		else 
		{
			//Debug.Log ("yes its zero with dist "+dist);
			if(dist > 0)
			{
				behind_path = true;
			}
		}

		x_points = x_set;
		y_points = y_set;

		if (!behind_path) 
		{
			ProgressPath ();
		}

	}

	//move forward along control path by removing first elements
	public void ProgressPath()
	{
		if (x_points.Count > 1 && y_points.Count > 1) 
		{
			
			x_points = x_points.GetRange(1,x_points.Count-1);
			y_points = y_points.GetRange(1,y_points.Count-1);

		} 
		else 
		{
			x_points.Clear ();
			y_points.Clear ();

		}
	}
		
	public void FixedUpdate()
	{
		ControllerUpdate ();
		if (simulator_process) 
		{
			server_process = true;
		}

	}
		
	public void ControllerUpdate()
	{
		
		if (x_points.Count > 1 && y_points.Count > 1) 
		{
			
			
			setNextPoint (x_points, y_points);

			Vector3 target = new Vector3 (x_points [0], rb.position.y, y_points [0]);
			Vector3 target2 = new Vector3 (x_points [1], rb.position.y, y_points [1]);

			rb.MovePosition (target);
			Vector3 inverseVect = transform.InverseTransformPoint (target2); 

			var deltaRotation = Quaternion.Euler(0, Mathf.Atan2 (inverseVect.x, inverseVect.z) * Mathf.Rad2Deg, 0);
			rb.MoveRotation (rb.rotation * deltaRotation);


		}
		ProgressPath ();
	}

	public List<float> previous_path_x()
	{
		return x_points;
	}

	public List<float> previous_path_y()
	{
		return y_points;
	}
		
	public bool isServerProcess()
	{
		return server_process;
	}
	public void ServerPause()
	{
		server_process = false;
	}
	public void SimulatorPause()
	{
		simulator_process = false;
		//Time.timeScale = 0.0f;
		//Time.fixedDeltaTime = 0.02f;


	}
	public void setSimulatorProcess()
	{
		simulator_process = true;
		//Time.timeScale = 1.0f;

	}

	public void setNextPoint(List<float> point_x, List<float> point_y)
	{
		int i = 0;
		while(i < way_points.Count && i < point_x.Count) 
		{
			GameObject new_point = way_points [i];

			// Change the coordinate system from (x,y) to (-y,x) so that zero degrees is the front axis of the car instead of to the right of car
			new_point.transform.position = new Vector3 (point_x [i],transform.position.y+0.5f,point_y[i]);

			if (i < point_x.Count - 2) {

				LineRenderer lineRenderer = new_point.GetComponentInParent<LineRenderer> ();

				Vector3 target = new Vector3 (point_x [i + 1], transform.position.y + 0.5f, point_y [i + 1]);
				lineRenderer.SetPosition (1, 4 * (target - new_point.transform.position));
				lineRenderer.SetWidth ((float).5, (float).5);

			}
			//set the line for the tip endpoint 
			else
			{
				LineRenderer lineRenderer = new_point.GetComponentInParent<LineRenderer> ();
				Vector3 target = new Vector3 (point_x [i], transform.position.y + 0.5f, point_y [i]);
				lineRenderer.SetPosition (1, 4 * (target - new_point.transform.position));
				lineRenderer.SetWidth ((float).5, (float).5);

			}
			i++;

		}
		while (i < way_points.Count) 
		{
			//Hide unused waypoints
			GameObject new_point = way_points [i];
			new_point.transform.position = new Vector3 (900f,-1f,1130f);
			i++;
		}

	}
	private void createPoints(int point_count)
	{
		int start_point = way_points.Count;
		int end_point = start_point + point_count;
		for (int i = start_point; i <  end_point; i++) 
		{
			GameObject new_point = (GameObject)Instantiate (next_point);

			new_point.name = "waypoint_"+i;

			way_points.Add (new_point);

		}
	}


	private void clearNextPoint()
	{
		//Clear points
		if (way_points != null) 
		{
			foreach (GameObject get_way_point in way_points)
			{
				if (get_way_point != null) {
					Destroy (get_way_point);
				}
			}
			way_points.Clear ();
		}


	}
	public void OpenScript()
	{
		script_running = true;
	}
	public void CloseScript()
	{
		script_running = false;
	}
}


### Assets/Standard Assets/Vehicles/Car/Scripts/term2/



**Kept a Backup of Term 2 Related C# Scripts Before Migrating to Term 3:**

#### `C:\src\self-driving-car-sim\Assets\Standard Assets\Vehicles\Car\Scripts\term2\CarAIControlTerm2.cs`:

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent (typeof(CarControllerTerm2))]
    public class CarAIControlTerm2 : MonoBehaviour
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

        private float m_RandomPerlin;
        // A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
        private CarControllerTerm2 m_CarController;
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
            m_CarController = GetComponent<CarControllerTerm2> ();

            // give the random perlin a random value
            m_RandomPerlin = Random.value * 100;

            m_Rigidbody = GetComponent<Rigidbody> ();
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
                    {
                        // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.

                        // check out the angle of our target compared to the current direction of the car
                        float approachingCornerAngle = Vector3.Angle (m_Target.forward, fwd);

                        // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                        float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

                        // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                        float cautiousnessRequired = Mathf.InverseLerp (0, m_CautiousMaxAngle,
                                                             Mathf.Max (spinningAngle,
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

#### `C:\src\self-driving-car-sim\Assets\Standard Assets\Vehicles\Car\Scripts\term2\CarControllerTerm2.cs`:

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;



namespace UnityStandardAssets.Vehicles.Car
{
    internal enum CarDriveTypeTerm2
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedTypeTerm2
    {
        MPH,
        KPH
    }

    public class CarControllerTerm2 : MonoBehaviour
    {
        [SerializeField] private CarDriveTypeTerm2 m_CarDriveType = CarDriveTypeTerm2.FourWheelDrive;
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range (0, 1)] [SerializeField] private float m_SteerHelper;
        // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range (0, 1)] [SerializeField] private float m_TractionControl;
        // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private SpeedTypeTerm2 m_SpeedType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;

        public const string CSVFileName = "driving_log.csv";
        public const string DirFrames = "IMG";

        [SerializeField] private Camera CenterCamera;
        [SerializeField] private Camera LeftCamera;
        [SerializeField] private Camera RightCamera;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;
        private string m_saveLocation = "";
        private Queue<CarSampleTerm2> carSamples;
		private int TotalSamples;
		private bool isSaving;
		private Vector3 saved_position;
		private Quaternion saved_rotation;

        public bool Skidding { get; private set; }

        public float BrakeInput { get; private set; }

        private bool m_isRecording = false;

		[SerializeField] private List<GameObject> sensors;
		private List<float> sensor_values = new List<float> ();

		public bool sensor_visable;

		public bool main_car;

		// sense acceleration
		private float AccelerationT;
		private float AccelerationN;
		private float Jerk;
		private float lastSpeed;
		private float lastAcc;

		//used to calculate path curvatures
		private List<Vector3> previous_pos = new List<Vector3>();

		private List<float> averageSpeed = new List<float>(); //average speed from previous frames
		private List<float> averageAcc = new List<float>(); //average acceleration from previous frames

		public Vector3 Position () {
			return transform.position;
		}
		public List<int> getGPS(){
			List<int> gps = new List<int> ();
			gps.Add ((int)transform.position.x);
			gps.Add ((int)transform.position.z);
			return gps;
		}

		public Quaternion Orientation () {
			return transform.rotation;
		}

		//toggle sensors visable on/off
		public void ToggleSensorView()
		{
			sensor_visable = !sensor_visable;
			if (sensor_visable) {
				foreach (GameObject sensor in sensors) {
					sensor.GetComponent<LineRenderer>().enabled = true;
				}
			} 
			else {
				foreach (GameObject sensor in sensors) {
					sensor.GetComponent<LineRenderer>().enabled = false;
				}
			}
		}

		public void SenseDistance()
		{
			if (sensors.Count != 0) {
				List<float> distances = new List<float> ();
				int i = 0;
				foreach (GameObject sensor in sensors) {
					RaycastHit hit;
					Physics.Raycast (sensor.transform.position, sensor.transform.forward, out hit);
					LineRenderer lineRenderer = sensor.GetComponentInParent<LineRenderer> ();
					if (sensor_visable) {
						if (hit.collider) {
							lineRenderer.SetPosition (1, new Vector3 (0, 0, 10 * hit.distance));
						} else {
							lineRenderer.SetPosition (1, new Vector3 (0, 0, 1000));
						}
					}
					distances.Add (hit.distance);
					i += 1;
				}
				sensor_values = distances;
			}
		}
		public float SenseAccT()
		{
			return AccelerationT;
		}
		public float SenseAccN()
		{
			return AccelerationN;
		}
		public float SenseAcc()
		{
			return Mathf.Sqrt (AccelerationT * AccelerationT + AccelerationN * AccelerationN);
		}
		public float SenseJerk()
		{
			return Jerk;
		}

		public List<float> getSensors()
		{
			return sensor_values;
		}

        public bool IsRecording {
            get
            {
                return m_isRecording;
            }

            set
            {
                m_isRecording = value;
                if(value == true)
                { 
					Debug.Log("Starting to record");
					carSamples = new Queue<CarSampleTerm2>();
					StartCoroutine(Sample());             
                } 
				else
                {
                    Debug.Log("Stopping record");
                    StopCoroutine(Sample());
                    Debug.Log("Writing to disk");
					//save the cars coordinate parameters so we can reset it to this properly after capturing data
					saved_position = transform.position;
					saved_rotation = transform.rotation;
					//see how many samples we captured use this to show save percentage in UISystem script
					TotalSamples = carSamples.Count;
					isSaving = true;
					StartCoroutine(WriteSamplesToDisk());

                };
            }

        }


		public bool checkSaveLocation()
		{
			if (m_saveLocation != "") 
			{
				return true;
			}
			else
			{
				SimpleFileBrowser.ShowSaveDialog (OpenFolder, null, true, null, "Select Output Folder", "Select");
			}
			return false;
		}

        public float CurrentSteerAngle {
            get { return m_SteerAngle; }
            set { m_SteerAngle = value; }
        }

        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }

        public float MaxSpeed{ get { return m_Topspeed; } }

		public void setMaxSpeed(float Topspeed) 
		{
			m_Topspeed = Topspeed;
		}

        public float Revs { get; private set; }

        public float AccelInput { get; set; }

        // Use this for initialization
        private void Start ()
        {
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++) {
                m_WheelMeshLocalRotations [i] = m_WheelMeshes [i].transform.localRotation;
            }
            m_WheelColliders [0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

            m_Rigidbody = GetComponent<Rigidbody> ();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);

			lastSpeed = 0;
			lastAcc = 0;
			Jerk = 0;
			AccelerationT = 0;
			AccelerationN = 0;
        }

        private void GearChanging ()
        {
            float f = Mathf.Abs (CurrentSpeed / MaxSpeed);
            float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
            float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit) {
                m_GearNum--;
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1))) {
                m_GearNum++;
            }
        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor (float factor)
        {
            return 1 - (1 - factor) * (1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp (float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }


        private void CalculateGearFactor ()
        {
            float f = (1 / (float)NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp (f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs (CurrentSpeed / MaxSpeed));
            m_GearFactor = Mathf.Lerp (m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
        }


        private void CalculateRevs ()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor ();
            var gearNumFactor = m_GearNum / (float)NoOfGears;
            var revsRangeMin = ULerp (0f, m_RevRangeBoundary, CurveFactor (gearNumFactor));
            var revsRangeMax = ULerp (m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp (revsRangeMin, revsRangeMax, m_GearFactor);
        }

        public void FixedUpdate()
        {
			if (main_car) 
			{
				//average over last frames
				int time_steps = 10;

				float speed = m_Rigidbody.velocity.magnitude;

				if (averageSpeed.Count >= time_steps) {
				
					float averaged_speed = AverageLastSpeed ();
					AccelerationT = (averaged_speed - lastSpeed) / (time_steps*Time.deltaTime);
					AccelerationN = (averaged_speed) * (averaged_speed) * SenseCurve ();
					lastSpeed = averaged_speed;

					float currentAcc = SenseAcc ();
					averageAcc.Add (currentAcc);

					averageSpeed.Clear ();
					previous_pos.Clear ();
				}
				averageSpeed.Add (speed);
				previous_pos.Add (transform.position);

				if (averageAcc.Count >= 5) 
				{
					float averaged_acc = AverageLastAcc();
					Jerk = (averaged_acc - lastAcc) / ((5*time_steps) * Time.deltaTime);
					lastAcc = averaged_acc;

					averageAcc.Clear ();
				}
					

			}
        }


        public void Update()
        {
            if (IsRecording)
            {
                //Dump();
            }
        }

		public float AverageLastSpeed()
		{
			
			float averaged_speed = 0.0f;

			for (int i = 0; i < averageSpeed.Count; i++) 
			{
				averaged_speed += averageSpeed[i];
			}

			return averaged_speed / (float)(averageSpeed.Count);

		}
		public float AverageLastAcc()
		{

			float averaged_acc = 0.0f;

			for (int i = 0; i < averageAcc.Count; i++) 
			{
				averaged_acc += averageAcc[i];
			}

			return averaged_acc / (float)(averageAcc.Count);

		}
		public float SenseCurve()
		{
			float averaged_curve = 0.0f;

			for (int i = 0; i < previous_pos.Count-2; i++) 
			{
				

				float x1 = previous_pos [i].x;
				float x2 = previous_pos [i + 1].x;
				float x3 = previous_pos [i + 2].x;

				float y1 = previous_pos [i].z;
				float y2 = previous_pos [i + 1].z;
				float y3 = previous_pos [i + 2].z;

				Vector2 ray1 = new Vector2 (x2 - x1, y2 - y1);
				Vector2 ray2 = new Vector2 (x3 - x2, y3 - y2);

				if (ray1.magnitude != 0 && ray2.magnitude != 0) 
				{

					Vector2 ray3 = new Vector2 (x3 - x1, y3 - y1);

					float corner_angle = Mathf.Abs (Vector2.Angle (ray1, ray2));

					if (ray3.magnitude != 0 && corner_angle != 180) 
					{
						averaged_curve += 2 * Mathf.Sin (corner_angle*Mathf.Deg2Rad) / ray3.magnitude;
					}
					else
					{
						
						//the curve is infinite, this move is totally illegal, just going to return 1000000
						averaged_curve += 1000000;
					}

				}
				//else skip and just say that curve is zero since its stopped
			}

			return averaged_curve / ( (float)(previous_pos.Count-2));

		}

        public void Move (float steering, float accel, float footbrake, float handbrake)
        {
            for (int i = 0; i < 4; i++) {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders [i].GetWorldPose (out position, out quat);
                m_WheelMeshes [i].transform.position = position;
                m_WheelMeshes [i].transform.rotation = quat;
            }

            //clamp input values
            steering = Mathf.Clamp (steering, -1, 1);
            AccelInput = accel = Mathf.Clamp (accel, 0, 1);
            BrakeInput = footbrake = -1 * Mathf.Clamp (footbrake, -1, 0);
            handbrake = Mathf.Clamp (handbrake, 0, 1);


            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders [0].steerAngle = m_SteerAngle;
            m_WheelColliders [1].steerAngle = m_SteerAngle;



            SteerHelper ();
            ApplyDrive (accel, footbrake);
            CapSpeed ();

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake > 0f)
            {
                var hbTorque = handbrake * m_MaxHandbrakeTorque;
                m_WheelColliders [2].brakeTorque = hbTorque;
                m_WheelColliders [3].brakeTorque = hbTorque;
            }

            CalculateRevs ();
            GearChanging ();

            AddDownForce ();
            CheckForWheelSpin ();
            TractionControl ();
        }
       

        private void CapSpeed ()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType) {
            case SpeedTypeTerm2.MPH:

                speed *= 2.23693629f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                break;

            case SpeedTypeTerm2.KPH:
                speed *= 3.6f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
                break;
            }
        }


        private void ApplyDrive (float accel, float footbrake)
        {

			for (int i = 0; i < 4; i++) 
			{
				m_WheelColliders [i].motorTorque = 0f;
				m_WheelColliders [i].brakeTorque = 0f;
			}

            float thrustTorque;
            switch (m_CarDriveType) {
            case CarDriveTypeTerm2.FourWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 4f);
                for (int i = 0; i < 4; i++) {
                    m_WheelColliders [i].motorTorque = thrustTorque;
                }
                break;

            case CarDriveTypeTerm2.FrontWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 2f);
                m_WheelColliders [0].motorTorque = m_WheelColliders [1].motorTorque = thrustTorque;
                break;

            case CarDriveTypeTerm2.RearWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 2f);
                m_WheelColliders [2].motorTorque = m_WheelColliders [3].motorTorque = thrustTorque;
                break;

            }

            for (int i = 0; i < 4; i++) {
                if (CurrentSpeed > 5 && Vector3.Angle (transform.forward, m_Rigidbody.velocity) < 50f) {
                    m_WheelColliders [i].brakeTorque = m_BrakeTorque * footbrake;
                } else if (footbrake > 0) {
                    m_WheelColliders [i].brakeTorque = 0f;
                    m_WheelColliders [i].motorTorque = -m_ReverseTorque * footbrake;
                }
            }
        }


        private void SteerHelper ()
        {
            for (int i = 0; i < 4; i++) {
                WheelHit wheelhit;
                m_WheelColliders [i].GetGroundHit (out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs (m_OldRotation - transform.eulerAngles.y) < 10f) {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis (turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce ()
        {
            m_WheelColliders [0].attachedRigidbody.AddForce (-transform.up * m_Downforce *
            m_WheelColliders [0].attachedRigidbody.velocity.magnitude);
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin ()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++) {
                WheelHit wheelHit;
                m_WheelColliders [i].GetGroundHit (out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs (wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs (wheelHit.sidewaysSlip) >= m_SlipLimit) {
                    m_WheelEffects [i].EmitTyreSmoke ();
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (m_WheelEffects [i].PlayingAudio) {
                    m_WheelEffects [i].StopAudio ();
                }
                // end the trail generation
                m_WheelEffects [i].EndSkidTrail ();
            }
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl ()
        {
            WheelHit wheelHit;
            switch (m_CarDriveType) {
            case CarDriveTypeTerm2.FourWheelDrive:
                    // loop through all wheels
                for (int i = 0; i < 4; i++) {
                    m_WheelColliders [i].GetGroundHit (out wheelHit);

                    AdjustTorque (wheelHit.forwardSlip);
                }
                break;

            case CarDriveTypeTerm2.RearWheelDrive:
                m_WheelColliders [2].GetGroundHit (out wheelHit);
                AdjustTorque (wheelHit.forwardSlip);

                m_WheelColliders [3].GetGroundHit (out wheelHit);
                AdjustTorque (wheelHit.forwardSlip);
                break;

            case CarDriveTypeTerm2.FrontWheelDrive:
                m_WheelColliders [0].GetGroundHit (out wheelHit);
                AdjustTorque (wheelHit.forwardSlip);

                m_WheelColliders [1].GetGroundHit (out wheelHit);
                AdjustTorque (wheelHit.forwardSlip);
                break;
            }
        }


        private void AdjustTorque (float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0) {
                m_CurrentTorque -= 10 * m_TractionControl;
            } else {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels) {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }


		//Changed the WriteSamplesToDisk to a IEnumerator method that plays back recording along with percent status from UISystem script 
		//instead of showing frozen screen until all data is recorded
		public IEnumerator WriteSamplesToDisk()
		{
			yield return new WaitForSeconds(0.000f); //retrieve as fast as we can but still allow communication of main thread to screen and UISystem
			if (carSamples.Count > 0) {
				//pull off a sample from the que
				CarSampleTerm2 sample = carSamples.Dequeue();

				//pysically moving the car to get the right camera position
				transform.position = sample.position;
				transform.rotation = sample.rotation;

				// Capture and Persist Image
				string centerPath = WriteImage (CenterCamera, "center", sample.timeStamp);
				string leftPath = WriteImage (LeftCamera, "left", sample.timeStamp);
				string rightPath = WriteImage (RightCamera, "right", sample.timeStamp);

				string row = string.Format ("{0},{1},{2},{3},{4},{5},{6}\n", centerPath, leftPath, rightPath, sample.steeringAngle, sample.throttle, sample.brake, sample.speed);
				File.AppendAllText (Path.Combine (m_saveLocation, CSVFileName), row);
			}
			if (carSamples.Count > 0) {
				//request if there are more samples to pull
				StartCoroutine(WriteSamplesToDisk()); 
			}
			else 
			{
				//all samples have been pulled
				StopCoroutine(WriteSamplesToDisk());
				isSaving = false;

				//need to reset the car back to its position before ending recording, otherwise sometimes the car ended up in strange areas
				transform.position = saved_position;
				transform.rotation = saved_rotation;
				m_Rigidbody.velocity = new Vector3(0f,-10f,0f);
				Move(0f, 0f, 0f, 0f);

			}
		}

		public float getSavePercent()
		{
			return (float)(TotalSamples-carSamples.Count)/TotalSamples;
		}

		public bool getSaveStatus()
		{
			return isSaving;
		}


        public IEnumerator Sample()
        {
            // Start the Coroutine to Capture Data Every Second.
            // Persist that Information to a CSV and Perist the Camera Frame
            yield return new WaitForSeconds(0.0666666666666667f);

            if (m_saveLocation != "")
            {
                CarSampleTerm2 sample = new CarSampleTerm2();

                sample.timeStamp = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
                sample.steeringAngle = m_SteerAngle / m_MaximumSteerAngle;
                sample.throttle = AccelInput;
                sample.brake = BrakeInput;
                sample.speed = CurrentSpeed;
                sample.position = transform.position;
                sample.rotation = transform.rotation;

                carSamples.Enqueue(sample);

                sample = null;
                //may or may not be needed
            }

            // Only reschedule if the button hasn't toggled
            if (IsRecording)
            {
                StartCoroutine(Sample());
            }
				
        }

        private void OpenFolder(string location)
        {
            m_saveLocation = location;
            Directory.CreateDirectory (Path.Combine(m_saveLocation, DirFrames));
        }

        private string WriteImage (Camera camera, string prepend, string timestamp)
        {
            //needed to force camera update 
            camera.Render();
            RenderTexture targetTexture = camera.targetTexture;
            RenderTexture.active = targetTexture;
            Texture2D texture2D = new Texture2D (targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            texture2D.ReadPixels (new Rect (0, 0, targetTexture.width, targetTexture.height), 0, 0);
            texture2D.Apply ();
            byte[] image = texture2D.EncodeToJPG ();
            UnityEngine.Object.DestroyImmediate (texture2D);
            string directory = Path.Combine(m_saveLocation, DirFrames);
            string path = Path.Combine(directory, prepend + "_" + timestamp + ".jpg");
            File.WriteAllBytes (path, image);
            image = null;
            return path;
        }
    }

    internal class CarSampleTerm2
    {
        public Quaternion rotation;
        public Vector3 position;
        public float steeringAngle;
        public float throttle;
        public float brake;
        public float speed;
        public string timeStamp;
    }

}



### Assets/Standard Assets/Vehicles/Car/Scripts/term3/

**Earlier I tried to fuse Term 1+2 and Term 3 C# Scripts Together, But Term 3 Highway Driving Path Planner Didn't
Work, So switched to standalone Term 3 C# Scripts for the time being and Highway Driving Path Planner Unity3D
Scene Simulation Launches and Runs successfully; I plan to revisit these fused versions of Term 1+2 and Term 3
C# scripts and then verify that I can run Term 1+2 scenes still in the same Unity project as Term 3. The main
issue I believe has to do with certain C# scripts having less member variables in Term 1+2 compared to Term 3
and I need to account for that difference; Maybe leveraging parent child class approach would be sufficient
like having a parent class for all Term 1,2,3 and then having particular child classes for Term 1, 2, 3**

#### `C:\src\self-driving-car-sim\Assets\Standard Assets\Vehicles\Car\Scripts\term3\CarAIControlFusedTerm2_3.cs`:

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


#### `C:\src\self-driving-car-sim\Assets\Standard Assets\Vehicles\Car\Scripts\term3\CarControllerFusedTerm2_3.cs`:

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;



namespace UnityStandardAssets.Vehicles.Car
{
    internal enum CarDriveTypeFusedTerm2_3
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedTypeFusedTerm2_3
    {
        MPH,
        KPH
    }

    public class CarControllerFusedTerm2_3 : MonoBehaviour
    {
        [SerializeField] private CarDriveTypeFusedTerm2_3 m_CarDriveType = CarDriveTypeFusedTerm2_3.FourWheelDrive;
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range (0, 1)] [SerializeField] private float m_SteerHelper;
        // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range (0, 1)] [SerializeField] private float m_TractionControl;
        // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private SpeedTypeFusedTerm2_3 m_SpeedType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;

        public const string CSVFileName = "driving_log.csv";
        public const string DirFrames = "IMG";

        [SerializeField] private Camera CenterCamera;
        [SerializeField] private Camera LeftCamera;
        [SerializeField] private Camera RightCamera;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;
        private string m_saveLocation = "";
        private Queue<CarSample> carSamples;
		private int TotalSamples;
		private bool isSaving;
		private Vector3 saved_position;
		private Quaternion saved_rotation;

        public bool Skidding { get; private set; }

        public float BrakeInput { get; private set; }

        private bool m_isRecording = false;

		[SerializeField] private List<GameObject> sensors;
		private List<float> sensor_values = new List<float> ();

		public bool sensor_visable;

		public bool main_car;

		// sense acceleration
		private float AccelerationT;
		private float AccelerationN;
		private float Jerk;
		private float lastSpeed;
		private float lastAcc;

		//used to calculate path curvatures
		private List<Vector3> previous_pos = new List<Vector3>();

		private List<float> averageSpeed = new List<float>(); //average speed from previous frames
		private List<float> averageAcc = new List<float>(); //average acceleration from previous frames

		public Vector3 Position () {
			return transform.position;
		}
		public List<int> getGPS(){
			List<int> gps = new List<int> ();
			gps.Add ((int)transform.position.x);
			gps.Add ((int)transform.position.z);
			return gps;
		}

		public Quaternion Orientation () {
			return transform.rotation;
		}

		//toggle sensors visable on/off
		public void ToggleSensorView()
		{
			sensor_visable = !sensor_visable;
			if (sensor_visable) {
				foreach (GameObject sensor in sensors) {
					sensor.GetComponent<LineRenderer>().enabled = true;
				}
			} 
			else {
				foreach (GameObject sensor in sensors) {
					sensor.GetComponent<LineRenderer>().enabled = false;
				}
			}
		}

		public void SenseDistance()
		{
			if (sensors.Count != 0) {
				List<float> distances = new List<float> ();
				int i = 0;
				foreach (GameObject sensor in sensors) {
					RaycastHit hit;
					Physics.Raycast (sensor.transform.position, sensor.transform.forward, out hit);
					LineRenderer lineRenderer = sensor.GetComponentInParent<LineRenderer> ();
					if (sensor_visable) {
						if (hit.collider) {
							lineRenderer.SetPosition (1, new Vector3 (0, 0, 10 * hit.distance));
						} else {
							lineRenderer.SetPosition (1, new Vector3 (0, 0, 1000));
						}
					}
					distances.Add (hit.distance);
					i += 1;
				}
				sensor_values = distances;
			}
		}
		public float SenseAccT()
		{
			return AccelerationT;
		}
		public float SenseAccN()
		{
			return AccelerationN;
		}
		public float SenseAcc()
		{
			return Mathf.Sqrt (AccelerationT * AccelerationT + AccelerationN * AccelerationN);
		}
		public float SenseJerk()
		{
			return Jerk;
		}

		public List<float> getSensors()
		{
			return sensor_values;
		}

        public bool IsRecording {
            get
            {
                return m_isRecording;
            }

            set
            {
                m_isRecording = value;
                if(value == true)
                { 
					Debug.Log("Starting to record");
					carSamples = new Queue<CarSample>();
					StartCoroutine(Sample());             
                } 
				else
                {
                    Debug.Log("Stopping record");
                    StopCoroutine(Sample());
                    Debug.Log("Writing to disk");
					//save the cars coordinate parameters so we can reset it to this properly after capturing data
					saved_position = transform.position;
					saved_rotation = transform.rotation;
					//see how many samples we captured use this to show save percentage in UISystem script
					TotalSamples = carSamples.Count;
					isSaving = true;
					StartCoroutine(WriteSamplesToDisk());

                };
            }

        }


		public bool checkSaveLocation()
		{
			if (m_saveLocation != "") 
			{
				return true;
			}
			else
			{
				SimpleFileBrowser.ShowSaveDialog (OpenFolder, null, true, null, "Select Output Folder", "Select");
			}
			return false;
		}

        public float CurrentSteerAngle {
            get { return m_SteerAngle; }
            set { m_SteerAngle = value; }
        }

        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }

        public float MaxSpeed{ get { return m_Topspeed; } }

		public void setMaxSpeed(float Topspeed) 
		{
			m_Topspeed = Topspeed;
		}

        public float Revs { get; private set; }

        public float AccelInput { get; set; }

        // Use this for initialization
        private void Start ()
        {
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++) {
                m_WheelMeshLocalRotations [i] = m_WheelMeshes [i].transform.localRotation;
            }
            m_WheelColliders [0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

            m_Rigidbody = GetComponent<Rigidbody> ();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);

			lastSpeed = 0;
			lastAcc = 0;
			Jerk = 0;
			AccelerationT = 0;
			AccelerationN = 0;
        }

        private void GearChanging ()
        {
            float f = Mathf.Abs (CurrentSpeed / MaxSpeed);
            float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
            float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit) {
                m_GearNum--;
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1))) {
                m_GearNum++;
            }
        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor (float factor)
        {
            return 1 - (1 - factor) * (1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp (float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }


        private void CalculateGearFactor ()
        {
            float f = (1 / (float)NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp (f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs (CurrentSpeed / MaxSpeed));
            m_GearFactor = Mathf.Lerp (m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
        }


        private void CalculateRevs ()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor ();
            var gearNumFactor = m_GearNum / (float)NoOfGears;
            var revsRangeMin = ULerp (0f, m_RevRangeBoundary, CurveFactor (gearNumFactor));
            var revsRangeMax = ULerp (m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp (revsRangeMin, revsRangeMax, m_GearFactor);
        }

        public void FixedUpdate()
        {
			if (main_car) 
			{
				//average over last frames
				int time_steps = 10;

				float speed = m_Rigidbody.velocity.magnitude;

				if (averageSpeed.Count >= time_steps) {
				
					float averaged_speed = AverageLastSpeed ();
					AccelerationT = (averaged_speed - lastSpeed) / (time_steps*Time.deltaTime);
					AccelerationN = (averaged_speed) * (averaged_speed) * SenseCurve ();
					lastSpeed = averaged_speed;

					float currentAcc = SenseAcc ();
					averageAcc.Add (currentAcc);

					averageSpeed.Clear ();
					previous_pos.Clear ();
				}
				averageSpeed.Add (speed);
				previous_pos.Add (transform.position);

				if (averageAcc.Count >= 5) 
				{
					float averaged_acc = AverageLastAcc();
					Jerk = (averaged_acc - lastAcc) / ((5*time_steps) * Time.deltaTime);
					lastAcc = averaged_acc;

					averageAcc.Clear ();
				}
					

			}
        }


        public void Update()
        {
            if (IsRecording)
            {
                //Dump();
            }
        }

		public float AverageLastSpeed()
		{
			
			float averaged_speed = 0.0f;

			for (int i = 0; i < averageSpeed.Count; i++) 
			{
				averaged_speed += averageSpeed[i];
			}

			return averaged_speed / (float)(averageSpeed.Count);

		}
		public float AverageLastAcc()
		{

			float averaged_acc = 0.0f;

			for (int i = 0; i < averageAcc.Count; i++) 
			{
				averaged_acc += averageAcc[i];
			}

			return averaged_acc / (float)(averageAcc.Count);

		}
		public float SenseCurve()
		{
			float averaged_curve = 0.0f;

			for (int i = 0; i < previous_pos.Count-2; i++) 
			{
				

				float x1 = previous_pos [i].x;
				float x2 = previous_pos [i + 1].x;
				float x3 = previous_pos [i + 2].x;

				float y1 = previous_pos [i].z;
				float y2 = previous_pos [i + 1].z;
				float y3 = previous_pos [i + 2].z;

				Vector2 ray1 = new Vector2 (x2 - x1, y2 - y1);
				Vector2 ray2 = new Vector2 (x3 - x2, y3 - y2);

				if (ray1.magnitude != 0 && ray2.magnitude != 0) 
				{

					Vector2 ray3 = new Vector2 (x3 - x1, y3 - y1);

					float corner_angle = Mathf.Abs (Vector2.Angle (ray1, ray2));

					if (ray3.magnitude != 0 && corner_angle != 180) 
					{
						averaged_curve += 2 * Mathf.Sin (corner_angle*Mathf.Deg2Rad) / ray3.magnitude;
					}
					else
					{
						
						//the curve is infinite, this move is totally illegal, just going to return 1000000
						averaged_curve += 1000000;
					}

				}
				//else skip and just say that curve is zero since its stopped
			}

			return averaged_curve / ( (float)(previous_pos.Count-2));

		}

        public void Move (float steering, float accel, float footbrake, float handbrake)
        {
            for (int i = 0; i < 4; i++) {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders [i].GetWorldPose (out position, out quat);
                m_WheelMeshes [i].transform.position = position;
                m_WheelMeshes [i].transform.rotation = quat;
            }

            //clamp input values
            steering = Mathf.Clamp (steering, -1, 1);
            AccelInput = accel = Mathf.Clamp (accel, 0, 1);
            BrakeInput = footbrake = -1 * Mathf.Clamp (footbrake, -1, 0);
            handbrake = Mathf.Clamp (handbrake, 0, 1);


            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders [0].steerAngle = m_SteerAngle;
            m_WheelColliders [1].steerAngle = m_SteerAngle;



            SteerHelper ();
            ApplyDrive (accel, footbrake);
            CapSpeed ();

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake > 0f)
            {
                var hbTorque = handbrake * m_MaxHandbrakeTorque;
                m_WheelColliders [2].brakeTorque = hbTorque;
                m_WheelColliders [3].brakeTorque = hbTorque;
            }

            CalculateRevs ();
            GearChanging ();

            AddDownForce ();
            CheckForWheelSpin ();
            TractionControl ();
        }
       

        private void CapSpeed ()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType) {
            case SpeedTypeFusedTerm2_3.MPH:

                speed *= 2.23693629f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                break;

            case SpeedTypeFusedTerm2_3.KPH:
                speed *= 3.6f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
                break;
            }
        }


        private void ApplyDrive (float accel, float footbrake)
        {

			for (int i = 0; i < 4; i++) 
			{
				m_WheelColliders [i].motorTorque = 0f;
				m_WheelColliders [i].brakeTorque = 0f;
			}

            float thrustTorque;
            switch (m_CarDriveType) {
            case CarDriveTypeFusedTerm2_3.FourWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 4f);
                for (int i = 0; i < 4; i++) {
                    m_WheelColliders [i].motorTorque = thrustTorque;
                }
                break;

            case CarDriveTypeFusedTerm2_3.FrontWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 2f);
                m_WheelColliders [0].motorTorque = m_WheelColliders [1].motorTorque = thrustTorque;
                break;

            case CarDriveTypeFusedTerm2_3.RearWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 2f);
                m_WheelColliders [2].motorTorque = m_WheelColliders [3].motorTorque = thrustTorque;
                break;

            }

            for (int i = 0; i < 4; i++) {
                // changed: allow braking at any forward speed (Term 3 behavior)
                if (CurrentSpeed > 0 && Vector3.Angle (transform.forward, m_Rigidbody.velocity) < 50f) {
                    m_WheelColliders [i].brakeTorque = m_BrakeTorque * footbrake;
                } else if (footbrake > 0) {
                    m_WheelColliders [i].brakeTorque = 0f;
                    m_WheelColliders [i].motorTorque = -m_ReverseTorque * footbrake;
                }
            }
        }


        private void SteerHelper ()
        {
            for (int i = 0; i < 4; i++) {
                WheelHit wheelhit;
                m_WheelColliders [i].GetGroundHit (out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs (m_OldRotation - transform.eulerAngles.y) < 10f) {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis (turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce ()
        {
            m_WheelColliders [0].attachedRigidbody.AddForce (-transform.up * m_Downforce *
            m_WheelColliders [0].attachedRigidbody.velocity.magnitude);
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin ()
        {
            // null-safe: some Term 3 prefabs don’t include WheelEffects
            if (m_WheelEffects == null || m_WheelEffects.Length == 0)
            {
                return;
            }

            // loop through all wheels
            for (int i = 0; i < 4; i++) {
                WheelHit wheelHit;
                m_WheelColliders [i].GetGroundHit (out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs (wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs (wheelHit.sidewaysSlip) >= m_SlipLimit) {
                    if (m_WheelEffects[i] != null) {
                        m_WheelEffects [i].EmitTyreSmoke ();
                    }
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (m_WheelEffects[i] != null && m_WheelEffects [i].PlayingAudio) {
                    m_WheelEffects [i].StopAudio ();
                }
                // end the trail generation
                if (m_WheelEffects[i] != null) {
                    m_WheelEffects [i].EndSkidTrail ();
                }
            }
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl ()
        {
            WheelHit wheelHit;
            switch (m_CarDriveType) {
            case CarDriveTypeFusedTerm2_3.FourWheelDrive:
                    // loop through all wheels
                for (int i = 0; i < 4; i++) {
                    m_WheelColliders [i].GetGroundHit (out wheelHit);

                    AdjustTorque (wheelHit.forwardSlip);
                }
                break;

            case CarDriveTypeFusedTerm2_3.RearWheelDrive:
                m_WheelColliders [2].GetGroundHit (out wheelHit);
                AdjustTorque (wheelHit.forwardSlip);

                m_WheelColliders [3].GetGroundHit (out wheelHit);
                AdjustTorque (wheelHit.forwardSlip);
                break;

            case CarDriveTypeFusedTerm2_3.FrontWheelDrive:
                m_WheelColliders [0].GetGroundHit (out wheelHit);
                AdjustTorque (wheelHit.forwardSlip);

                m_WheelColliders [1].GetGroundHit (out wheelHit);
                AdjustTorque (wheelHit.forwardSlip);
                break;
            }
        }


        private void AdjustTorque (float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0) {
                m_CurrentTorque -= 10 * m_TractionControl;
            } else {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels) {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }


		//Changed the WriteSamplesToDisk to a IEnumerator method that plays back recording along with percent status from UISystem script 
		//instead of showing frozen screen until all data is recorded
		public IEnumerator WriteSamplesToDisk()
		{
			yield return new WaitForSeconds(0.000f); //retrieve as fast as we can but still allow communication of main thread to screen and UISystem
			if (carSamples.Count > 0) {
				//pull off a sample from the que
				CarSample sample = carSamples.Dequeue();

				//pysically moving the car to get the right camera position
				transform.position = sample.position;
				transform.rotation = sample.rotation;

				// Capture and Persist Image
				string centerPath = WriteImage (CenterCamera, "center", sample.timeStamp);
				string leftPath = WriteImage (LeftCamera, "left", sample.timeStamp);
				string rightPath = WriteImage (RightCamera, "right", sample.timeStamp);

				string row = string.Format ("{0},{1},{2},{3},{4},{5},{6}\n", centerPath, leftPath, rightPath, sample.steeringAngle, sample.throttle, sample.brake, sample.speed);
				File.AppendAllText (Path.Combine (m_saveLocation, CSVFileName), row);
			}
			if (carSamples.Count > 0) {
				//request if there are more samples to pull
				StartCoroutine(WriteSamplesToDisk()); 
			}
			else 
			{
				//all samples have been pulled
				StopCoroutine(WriteSamplesToDisk());
				isSaving = false;

				//need to reset the car back to its position before ending recording, otherwise sometimes the car ended up in strange areas
				transform.position = saved_position;
				transform.rotation = saved_rotation;
				m_Rigidbody.velocity = new Vector3(0f,-10f,0f);
				Move(0f, 0f, 0f, 0f);

			}
		}

		public float getSavePercent()
		{
			return (float)(TotalSamples-carSamples.Count)/TotalSamples;
		}

		public bool getSaveStatus()
		{
			return isSaving;
		}


        public IEnumerator Sample()
        {
            // Start the Coroutine to Capture Data Every Second.
            // Persist that Information to a CSV and Perist the Camera Frame
            yield return new WaitForSeconds(0.0666666666666667f);

            if (m_saveLocation != "")
            {
                CarSample sample = new CarSample();

                sample.timeStamp = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
                sample.steeringAngle = m_SteerAngle / m_MaximumSteerAngle;
                sample.throttle = AccelInput;
                sample.brake = BrakeInput;
                sample.speed = CurrentSpeed;
                sample.position = transform.position;
                sample.rotation = transform.rotation;

                carSamples.Enqueue(sample);

                sample = null;
                //may or may not be needed
            }

            // Only reschedule if the button hasn't toggled
            if (IsRecording)
            {
                StartCoroutine(Sample());
            }
				
        }

        private void OpenFolder(string location)
        {
            m_saveLocation = location;
            Directory.CreateDirectory (Path.Combine(m_saveLocation, DirFrames));
        }

        private string WriteImage (Camera camera, string prepend, string timestamp)
        {
            //needed to force camera update 
            camera.Render();
            RenderTexture targetTexture = camera.targetTexture;
            RenderTexture.active = targetTexture;
            Texture2D texture2D = new Texture2D (targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            texture2D.ReadPixels (new Rect (0, 0, targetTexture.width, targetTexture.height), 0, 0);
            texture2D.Apply ();
            byte[] image = texture2D.EncodeToJPG ();
            UnityEngine.Object.DestroyImmediate (texture2D);
            string directory = Path.Combine(m_saveLocation, DirFrames);
            string path = Path.Combine(directory, prepend + "_" + timestamp + ".jpg");
            File.WriteAllBytes (path, image);
            image = null;
            return path;
        }
    }

    internal class CarSample
    {
        public Quaternion rotation;
        public Vector3 position;
        public float steeringAngle;
        public float throttle;
        public float brake;
        public float speed;
        public string timeStamp;
    }

}

