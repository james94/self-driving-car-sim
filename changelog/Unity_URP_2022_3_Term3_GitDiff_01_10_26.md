
# Unity URP 2022_3_62f3 Term 3 Git Diff (01_10_26)

I updated C# scripts that impact Lake Track Training & Autonomous scenes and Jungle Track Training & Autonomous scenes for Behavioral Cloning in term 1 to make sure they still work.
I updated C# scripts that impact Lake Track PID Control & MPC Control in term 2 to make sure they still work.
I also made sure EKF, UKF, and Particle Filter scenes still work.
Initially I got the Highway Driving Path Planner scene working in Unity3D from term 3, but then needed to make 
sure the other scenes from term 2 and 1 still worked. So, also now from the menu, we can choose to any of these
scenes.

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git status

On branch Unity_URP_2022_3_Term3

## Changes not staged for commit:

modified:   Assets/1_SelfDrivingCar/Scenes/JungleTrackAutonomous.unity
modified:   Assets/1_SelfDrivingCar/Scenes/JungleTrackTraining.unity
modified:   Assets/1_SelfDrivingCar/Scenes/LakeTrackAutonomous.unity
modified:   Assets/1_SelfDrivingCar/Scenes/LakeTrackTraining.unity
modified:   Assets/1_SelfDrivingCar/Term2_Scenes/MenuSceneTerm2.unity
modified:   Assets/1_SelfDrivingCar/Term2_Scenes/project_4/LakeTrackAutonomous_pid.unity
modified:   Assets/1_SelfDrivingCar/Term2_Scenes/project_5/LakeTrackAutonomous_mpc.unity
deleted:    Assets/1_SelfDrivingCar/Term3_Scenes/ControlMenu.unity
deleted:    Assets/1_SelfDrivingCar/Term3_Scenes/ControlMenu.unity.meta
modified:   Assets/1_SelfDrivingCar/Term3_Scenes/MenuScene.unity
modified:   Assets/1_SelfDrivingCar/Term3_Scenes/MenuScene.unity.meta
modified:   Assets/1_SelfDrivingCar/Term3_Scenes/PathPlanning.unity
deleted:    Assets/1_SelfDrivingCar/Scripts/term3/UISystemFusedTerm2_3.cs
deleted:    Assets/1_SelfDrivingCar/Scripts/term3/UISystemFusedTerm2_3.cs.meta

### modified:   Assets/1_SelfDrivingCar/Scripts/CommandServer.cs:

    ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
    diff --git a/Assets/1_SelfDrivingCar/Scripts/CommandServer.cs b/Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
    index e87d8a8..6ef5b5f 100644
    --- a/Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
    +++ b/Assets/1_SelfDrivingCar/Scripts/CommandServer.cs
    @@ -6,48 +6,25 @@ using UnityStandardAssets.Vehicles.Car;
    using System;
    using System.Security.AccessControl;

    -public class CommandServer : MonoBehaviour
    +public class CommandServer : CommandServerBase
    {
            public GameObject Car;
    -       public Camera FrontFacingCamera;
    -       private SocketIOComponent _socket;
    -       private CarController _carController;
    +       // public Camera FrontFacingCamera; // now in base
    +       // private SocketIOComponent _socket; // moved to base
    +       private CarController carController; // moved to base
            private perfect_controller point_path;
            private CarTraffic car_traffic;

    -
    -       // Convert angle (degrees) from Unity orientation to
    -       //            90
    -       //
    -       //  180                   0/360
    -       //
    -       //            270
    -       //
    -       // This is the standard format used in mathematical functions.
    -       float convertAngle(float psi) {
    -               if (psi >= 0 && psi <= 90) {
    -                       return 90 - psi;
    -               }
    -               else if (psi > 90 && psi <= 180) {
    -                       return 90 + 270 - (psi - 90);
    -               }
    -               else if (psi > 180 && psi <= 270) {
    -                       return 180 + 90 - (psi - 180);
    -               }
    -               return 270 - 90 - (psi - 270);
    -       }
    -
            // Use this for initialization
            void Start()
            {
    -               _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
    -               _socket.On("open", OnOpen);
    -               _socket.On("manual", onManual);
    -               _socket.On("control", Control);
    -               _carController = Car.GetComponent<CarController>();
    +               InitSocket();
    +               RegisterHandler("open", OnOpen);
    +               RegisterHandler("manual", onManual);
    +               RegisterHandler("control", Control);
    +               carController = Car.GetComponent<CarController>();
                    point_path = Car.GetComponent<perfect_controller>();
                    car_traffic = Car.GetComponent<CarTraffic>();
    -
            }

            void OnOpen(SocketIOEvent obj)
    @@ -60,7 +37,6 @@ public class CommandServer : MonoBehaviour
            {
                    Debug.Log("Connection Closed");
                    point_path.CloseScript ();
    -
            }

            //
    @@ -95,8 +71,7 @@ public class CommandServer : MonoBehaviour
                    EmitTelemetry (obj);
            }

    -
    -       void EmitTelemetry(SocketIOEvent obj)
    +       protected override void EmitTelemetry(SocketIOEvent obj)
            {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
    @@ -104,7 +79,7 @@ public class CommandServer : MonoBehaviour
                            //print("Attempting to Send...");
                            // send only if it's not being manually driven
                            if ( !point_path.isServerProcess() ) {
    -                               _socket.Emit("telemetry", new JSONObject());
    +                               socket.Emit("telemetry", new JSONObject());


                            }
    @@ -118,8 +93,8 @@ public class CommandServer : MonoBehaviour
                                    // localization of car
                                    data["x"] = new JSONObject(Car.transform.position.x);
                                    data["y"] = new JSONObject(Car.transform.position.z);
    -                               data["yaw"] = new JSONObject (convertAngle(Car.transform.rotation.eulerAngles.y));
    -                               data["speed"] = new JSONObject(_carController.CurrentSpeed);
    +                               data["yaw"] = new JSONObject (ConvertUnityYawToMathAngle(Car.transform.rotation.eulerAngles.y));
    +                               data["speed"] = new JSONObject(carController.CurrentSpeed);

                                    CarAIControl carAI = (CarAIControl) Car.GetComponent(typeof(CarAIControl));

    @@ -175,7 +150,7 @@ public class CommandServer : MonoBehaviour
                                    //data["steering_angle"] = new JSONObject(_carController.CurrentSteerAngle);
                                    //data["throttle"] = new JSONObject(_carController.AccelInput);
                                    //data["speed"] = new JSONObject(_carController.CurrentSpeed);
    -                               _socket.Emit("telemetry", new JSONObject(data));
    +                               socket.Emit("telemetry", new JSONObject(data));
                            }
                    });

### modified:   Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
diff --git a/Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs b/Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
index f8d253e..533bd83 100644
--- a/Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
+++ b/Assets/1_SelfDrivingCar/Scripts/MenuOptions.cs
@@ -5,32 +5,221 @@ using UnityEngine.SceneManagement;

 public class MenuOptions : MonoBehaviour
 {
+    // Term selection state
+    // public static bool IsTerm1 { get; private set; }
+    public bool IsTerm1 = false;
+    public bool IsTerm2 = false;
+    public bool IsTerm3 = false; // Term 3 flag
+
+    // Term 1 Components
+    private int track = 0;
     private Outline[] outlines;

-    public void Start ()
+    // Term 2 Components
+    private int project = 0;
+    public Text projectName;
+    public Image projectImage;
+       public Sprite project_1;
+       public Sprite project_2;
+       public Sprite project_3;
+       public Sprite project_4;
+       public Sprite project_5;
+    public Sprite project_6;
+
+    // Shared UI elements
+    // public GameObject term1UI;
+    // public GameObject term2UI;
+
+    public void Start()
+    {
+        InitializeTermUI();
+    }
+
+    void InitializeTermUI()
     {
+        Debug.Log("Initialize Term UI");
+        Debug.Log($"IsTerm1 = {IsTerm1}");
+        Debug.Log($"IsTerm2 = {IsTerm2}");
+        // term1UI.SetActive(IsTerm1);
+        // term2UI.SetActive(!IsTerm1);
+
+        if (IsTerm1) {
+            InitializeTerm1();
+        }
+        else if (IsTerm2) {
+            InitializeTerm2();
+        }
+        else if (IsTerm3) {
+            InitializeTerm3();
+        }
+    }
+
+    void InitializeTerm3()
+    {
+        // Match Term 3: set first outline to black
         outlines = GetComponentsInChildren<Outline>();
-               Debug.Log ("in menu script "+outlines.Length);
-               if (outlines.Length > 0)
-               {
-                       outlines [0].effectColor = new Color (0, 0, 0);
-               }
+        if (outlines != null && outlines.Length > 0)
+        {
+            outlines[0].effectColor = Color.black;
+        }
     }

+    #region Shared Methods
        public void ControlMenu()
        {
-               SceneManager.LoadScene ("ControlMenu");
+        Debug.Log("Going to control menu scene");
+        Debug.Log($"IsTerm1 = {IsTerm1}");
+        Debug.Log($"IsTerm2 = {IsTerm2}");
+        Debug.Log($"IsTerm3 = {IsTerm3}");
+        if (IsTerm1) {
+            SceneManager.LoadScene("ControlMenu");
+        }
+        else if (IsTerm2) {
+            SceneManager.LoadScene("ControlMenuTerm2");
+        }
+        else if (IsTerm3) {
+            SceneManager.LoadScene("ControlMenuTerm3"); // Term 3 uses ControlMenu
+        }
        }

        public void MainMenu()
        {
-               Debug.Log ("go to main menu");
-               SceneManager.LoadScene ("MenuScene");
+        Debug.Log("Going to main menu scene");
+        Debug.Log($"IsTerm1 = {IsTerm1}");
+        Debug.Log($"IsTerm2 = {IsTerm2}");
+        Debug.Log($"IsTerm3 = {IsTerm3}");
+        if (IsTerm1) {
+            SceneManager.LoadScene("MenuScene");
+        }
+        else if (IsTerm2) {
+            SceneManager.LoadScene("MenuSceneTerm2");
+        }
+        else if (IsTerm3) {
+            SceneManager.LoadScene("MenuSceneTerm3"); // Term 3 uses MenuScene
+        }
        }
-
-    public void StartPathPlanning()
+    #endregion // end of Shared Methods
+
+
+    #region Term 1 Methods
+    void InitializeTerm1()
     {
-               SceneManager.LoadScene("PathPlanning");
+        outlines = GetComponentsInChildren<Outline>();
+        if(outlines.Length > 0) {
+            outlines[0].effectColor = Color.black;
+            outlines[1].effectColor = Color.white;
+        }
+    }
+
+    public void StartDrivingMode()
+    {
+        SceneManager.LoadScene(track == 0 ?
+            "LakeTrackTraining" : "JungleTrackTraining");
     }

+    public void StartAutonomousMode()
+    {
+        SceneManager.LoadScene(track == 0 ?
+            "LakeTrackAutonomous" : "JungleTrackAutonomous");
+    }
+
+    public void SetLakeTrack()
+    {
+        track = 0;
+        outlines[0].effectColor = Color.black;
+        outlines[1].effectColor = Color.white;
+    }
+
+    public void SetMountainTrack()
+    {
+        track = 1;
+        outlines[1].effectColor = Color.black;
+        outlines[0].effectColor = Color.white;
+    }
+    #endregion // end of Term 1 Methods
+
+    #region Term 2 Methods
+    void InitializeTerm2()
+    {
+        if(projectName == null || projectImage == null)
+        {
+            Debug.Log("Missing Term 2 UI references! Maybe in Control Menu");
+            return;
+        }
+
+        project = 0;
+        UpdateProjectDisplay();
+    }
+
+    public void SelectMode()
+    {
+        switch(project)
+        {
+            case 0:
+                SceneManager.LoadScene("EKF_project");
+                break;
+            case 1:
+                SceneManager.LoadScene("UKF_project");
+                break;
+            case 2:
+                SceneManager.LoadScene("particle_filter_v2");
+                break;
+            case 3:
+                SceneManager.LoadScene("LakeTrackAutonomous_pid");
+                break;
+            case 4:
+                SceneManager.LoadScene("LakeTrackAutonomous_mpc");
+                break;
+            case 5:
+                SceneManager.LoadScene("PathPlanning");
+                break;
+        }
+    }
+
+       public void Next()
+       {
+               project = (project + 1) % 6;
+        UpdateProjectDisplay();
+    }
+
+    public void Previous()
+    {
+        project = (project == 0) ? 5 : project - 1;
+        UpdateProjectDisplay();
+    }
+
+
+    void UpdateProjectDisplay()
+    {
+
+        if(projectName == null || projectImage == null)
+        {
+            Debug.Log("Project UI elements not assigned! Maybe in Control Menu");
+            return;
+        }
+
+
+        projectName.text = project switch
+        {
+            0 => "Project 1: Extended Kalman Filter",
+            1 => "Project 2: Unscented Kalman filters",
+            2 => "Project 3: Kidnapped Vehicle",
+            3 => "Project 4: PID Controller",
+            4 => "Project 5: MPC Controller",
+            5 => "Project 6: Path Planning",
+            _ => "Invalid Project"
+        };
+
+        projectImage.sprite = project switch
+        {
+            0 => project_1,
+            1 => project_2,
+            2 => project_3,
+            3 => project_4,
+            4 => project_5,
+            5 => project_6,
+            _ => null
+        };
+    }
+    #endregion // end of Term 2 Methods
 }

### modified:   Assets/1_SelfDrivingCar/Scripts/UISystem.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/UISystem.cs
diff --git a/Assets/1_SelfDrivingCar/Scripts/UISystem.cs b/Assets/1_SelfDrivingCar/Scripts/UISystem.cs
index 4aa3d61..727756f 100644
--- a/Assets/1_SelfDrivingCar/Scripts/UISystem.cs
+++ b/Assets/1_SelfDrivingCar/Scripts/UISystem.cs
@@ -4,14 +4,13 @@ using System.Collections;
 using UnityStandardAssets.Vehicles.Car;
 using UnityEngine.SceneManagement;

-public class UISystem : MonoSingleton<UISystem> {
+public class UISystem : UISystemBase {

     public CarController carController;
        public Camera mainCamera;
     public string GoodCarStatusMessage;
     public string BadSCartatusMessage;
-    public Text MPH_Text;
-    public Image MPH_Animation;
+
        public Text AccT_Text;
        public Text AccN_Text;
        public Text Acc_Text;
@@ -31,7 +30,6 @@ public class UISystem : MonoSingleton<UISystem> {
        public Text JerkStatus_Text;

     private bool recording;
-    private float topSpeed;
        private bool saveRecording;

        private bool auto_drive;
@@ -41,7 +39,7 @@ public class UISystem : MonoSingleton<UISystem> {
     // Use this for initialization
     void Start() {

-        topSpeed = carController.MaxSpeed;
+        InitTopSpeed(carController.MaxSpeed);

                AccStatus_Text.text = "";
                JerkStatus_Text.text = "";
@@ -54,12 +52,6 @@ public class UISystem : MonoSingleton<UISystem> {

     }

-    public void SetMPHValue(float value)
-    {
-        MPH_Text.text = value.ToString("N2");
-        //Do something with value for fill amounts
-        MPH_Animation.fillAmount = value/topSpeed;
-    }
        public void SetAccTValue(float value)
        {
                AccT_Text.text = "AccT: "+value.ToString ("N0")+" m/s^2";
@@ -189,11 +181,7 @@ public class UISystem : MonoSingleton<UISystem> {
        // Update is called once per frame
        void Update () {

-           if(Input.GetKeyDown(KeyCode.Escape))
-        {
-            //Do Menu Here
-            SceneManager.LoadScene("MenuScene");
-        }
+           HandleEscapeKey();

         UpdateCarValues();
     }

### modified:   Assets/1_SelfDrivingCar/Scripts/project_4/CommandServer_pid.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/project_4/CommandServer_pid.cs
diff --git a/Assets/1_SelfDrivingCar/Scripts/project_4/CommandServer_pid.cs b/Assets/1_SelfDrivingCar/Scripts/project_4/CommandServer_pid.cs
index e0f3b1e..9e3bb6f 100644
--- a/Assets/1_SelfDrivingCar/Scripts/project_4/CommandServer_pid.cs
+++ b/Assets/1_SelfDrivingCar/Scripts/project_4/CommandServer_pid.cs
@@ -5,23 +5,23 @@ using UnityStandardAssets.Vehicles.Car;
 using System;
 using UnityEngine.SceneManagement;

-public class CommandServer_pid : MonoBehaviour
+public class CommandServer_pid : CommandServerBase
 {
-       public CarRemoteControl CarRemoteControl;
-       public Camera FrontFacingCamera;
+       public CarRemoteControlTerm2 CarRemoteControl;
+       // public Camera FrontFacingCamera;
        private SocketIOComponent _socket;
-       private CarController _carController;
+       private CarControllerTerm2 _carController;
        private WaypointTracker_pid wpt;

        // Use this for initialization
        void Start()
        {
-               _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
-               _socket.On("open", OnOpen);
-               _socket.On ("reset", OnReset);
-               _socket.On("steer", OnSteer);
-               _socket.On("manual", onManual);
-               _carController = CarRemoteControl.GetComponent<CarController>();
+               InitSocket();
+               RegisterHandler("open", OnOpen);
+               RegisterHandler("reset", OnReset);
+               RegisterHandler("steer", OnSteer);
+               RegisterHandler("manual", onManual);
+               _carController = CarRemoteControl.GetComponent<CarControllerTerm2>();
                wpt = new WaypointTracker_pid ();
        }

@@ -53,33 +53,58 @@ public class CommandServer_pid : MonoBehaviour
        {
         Debug.Log("Steering data event ...");
                JSONObject jsonObject = obj.data;
-               CarRemoteControl.SteeringAngle = float.Parse(jsonObject.GetField("steering_angle").ToString());
-               CarRemoteControl.Acceleration = float.Parse(jsonObject.GetField("throttle").ToString());
+
+               float steering, throttle;
+               bool sOk = TryGetFloat(jsonObject, "steering_angle", out steering);
+               bool tOk = TryGetFloat(jsonObject, "throttle", out throttle);
+               if (!sOk || !tOk)
+               {
+                       Debug.LogError("Invalid steer/throttle payload");
+                       return;
+               }
+
+               CarRemoteControl.SteeringAngle = steering;
+               CarRemoteControl.Acceleration = throttle;
+
                var steering_bias = 1.0f * Mathf.Deg2Rad;
                CarRemoteControl.SteeringAngle += steering_bias;
+
                EmitTelemetry(obj);
        }

-       void EmitTelemetry(SocketIOEvent obj)
+       protected override void EmitTelemetry(SocketIOEvent obj)
        {
-               UnityMainThreadDispatcher.Instance().Enqueue(() =>
+               Enqueue(() =>
                {
-                       print("Attempting to Send...");
-                       // send only if it's not being manually driven
-                       if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
-                               _socket.Emit("telemetry", new JSONObject());
-                       } else {
-                               // Collect Data from the Car
-                               Dictionary<string, string> data = new Dictionary<string, string>();
-                               var cte = wpt.CrossTrackError (_carController);
-                               data["steering_angle"] = _carController.CurrentSteerAngle.ToString("N4");
-                               data["throttle"] = _carController.AccelInput.ToString("N4");
-                               data["speed"] = _carController.CurrentSpeed.ToString("N4");
-                               data["cte"] = cte.ToString("N4");
-                               data["image"] = Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera));
-                               _socket.Emit("telemetry", new JSONObject(data));
+                       try
+                       {
+                               // send only if it's not being manually driven
+                               if (IsManualInputActive())
+                               {
+                                       socket.Emit("telemetry", new JSONObject());
+                               }
+                               else
+                               {
+                                       var telemetryData = new JSONObject(JSONObject.Type.OBJECT);
+
+                                       // Cross-track error and car signals
+                                       float cte = wpt.CrossTrackError(_carController);
+                                       telemetryData.AddField("cte", cte);
+                                       telemetryData.AddField("steering_angle", _carController.CurrentSteerAngle);
+                                       telemetryData.AddField("throttle", _carController.AccelInput);
+                                       telemetryData.AddField("speed", _carController.CurrentSpeed);
+
+                                       // Camera image (base64)
+                                       telemetryData.AddField("image", CaptureFrameBase64(FrontFacingCamera));
+                                       // telemetryData.AddField("image", Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera)));
+
+                                       socket.Emit("telemetry", telemetryData);
+                               }
+                       }
+                       catch (Exception ex)
+                       {
+                               Debug.LogError($"Telemetry error: {ex}");
                        }
                });
-
        }
 }

### modified:   Assets/1_SelfDrivingCar/Scripts/project_5/CommandServer_mpc.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/project_5/CommandServer_mpc.cs
diff --git a/Assets/1_SelfDrivingCar/Scripts/project_5/CommandServer_mpc.cs b/Assets/1_SelfDrivingCar/Scripts/project_5/CommandServer_mpc.cs
index 68e6ad4..782101c 100644
--- a/Assets/1_SelfDrivingCar/Scripts/project_5/CommandServer_mpc.cs
+++ b/Assets/1_SelfDrivingCar/Scripts/project_5/CommandServer_mpc.cs
@@ -4,12 +4,11 @@ using SocketIO;
 using UnityStandardAssets.Vehicles.Car;
 using System;

-public class CommandServer_mpc : MonoBehaviour
+public class CommandServer_mpc : CommandServerBase
 {
-       public CarRemoteControl CarRemoteControl;
-       public Camera FrontFacingCamera;
-       private SocketIOComponent _socket;
-       private CarController _carController;
+       public CarRemoteControlTerm2 CarRemoteControl;
+       // public Camera FrontFacingCamera;
+       private CarControllerTerm2 _carController;
        private PointTracker point_path;
        private WaypointTracker_mpc wpt;
        private int polyOrder;
@@ -17,37 +16,16 @@ public class CommandServer_mpc : MonoBehaviour
        // Use this for initialization
        void Start()
        {
-               _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
-               _socket.On("open", OnOpen);
-               _socket.On("steer", OnSteer);
-               _socket.On("manual", onManual);
-               _carController = CarRemoteControl.GetComponent<CarController>();
+               InitSocket();
+               RegisterHandler("open", OnOpen);
+               RegisterHandler("steer", OnSteer);
+               RegisterHandler("manual", onManual);
+               _carController = CarRemoteControl.GetComponent<CarControllerTerm2>();
                point_path = CarRemoteControl.GetComponent<PointTracker>();
                wpt = new WaypointTracker_mpc ();
                polyOrder = 5;
        }

-       // Convert angle (degrees) from Unity orientation to
-       //            90
-       //
-       //  180                   0/360
-       //
-       //            270
-       //
-       // This is the standard format used in mathematical functions.
-       float convertAngle(float psi) {
-               if (psi >= 0 && psi <= 90) {
-                       return 90 - psi;
-               }
-               else if (psi > 90 && psi <= 180) {
-                       return 90 + 270 - (psi - 90);
-               }
-               else if (psi > 180 && psi <= 270) {
-                       return 180 + 90 - (psi - 180);
-               }
-               return 270 - 90 - (psi - 270);
-       }
-
        // Update is called once per frame
        void Update()
        {
@@ -59,7 +37,6 @@ public class CommandServer_mpc : MonoBehaviour
                EmitTelemetry(obj);
        }

-       //
        void onManual(SocketIOEvent obj)
        {
         Debug.Log("Manual driving event ...");
@@ -70,82 +47,100 @@ public class CommandServer_mpc : MonoBehaviour
        {
         Debug.Log("Steering data event ...");
                JSONObject jsonObject = obj.data;
-               CarRemoteControl.SteeringAngle = float.Parse(jsonObject.GetField("steering_angle").ToString());
-               CarRemoteControl.Acceleration = float.Parse(jsonObject.GetField("throttle").ToString());
-
-               //string next_x = jsonObject.GetField ("next_x").ToString ();
-               //string next_y = jsonObject.GetField ("next_y").ToString ();

-               var next_x = jsonObject.GetField ("next_x");
-               var next_y = jsonObject.GetField ("next_y");
-               List<float> my_next_x = new List<float> ();
-               List<float> my_next_y = new List<float> ();
-
-               for (int i = 0; i < next_x.Count; i++)
+               float steering, throttle;
+               if (!TryGetFloat(jsonObject, "steering_angle", out steering) ||
+                   !TryGetFloat(jsonObject, "throttle", out throttle))
                {
-                       my_next_x.Add (float.Parse((next_x [i]).ToString()));
-                       my_next_y.Add (float.Parse((next_y [i]).ToString()));
+                       Debug.LogError("Invalid steer/throttle payload");
+                       return;
                }
-               point_path.setNextPoint( my_next_x, my_next_y );

-               var mpc_x = jsonObject.GetField ("mpc_x");
-               var mpc_y = jsonObject.GetField ("mpc_y");
-               List<float> my_mpc_x = new List<float> ();
-               List<float> my_mpc_y = new List<float> ();
+               CarRemoteControl.SteeringAngle = steering;
+               CarRemoteControl.Acceleration = throttle;

-               for (int i = 0; i < mpc_x.Count; i++)
+               // Next points
+               var next_x = jsonObject.GetField("next_x");
+               var next_y = jsonObject.GetField("next_y");
+               List<float> my_next_x = new List<float>();
+               List<float> my_next_y = new List<float>();
+               for (int i = 0; i < next_x.Count; i++)
                {
-                       my_mpc_x.Add (float.Parse((mpc_x [i]).ToString()));
-                       my_mpc_y.Add (float.Parse((mpc_y [i]).ToString()));
+                       my_next_x.Add(float.Parse(next_x[i].ToString()));
+                       my_next_y.Add(float.Parse(next_y[i].ToString()));
                }
-
-               point_path.setMpcPoint( my_mpc_x, my_mpc_y );
+               point_path.setNextPoint(my_next_x, my_next_y);
+
+               // MPC points
+               var mpc_x = jsonObject.GetField("mpc_x");
+               var mpc_y = jsonObject.GetField("mpc_y");
+               List<float> my_mpc_x = new List<float>();
+               List<float> my_mpc_y = new List<float>();
+               for (int i = 0; i < mpc_x.Count; i++)
+               {
+                       my_mpc_x.Add(float.Parse(mpc_x[i].ToString()));
+                       my_mpc_y.Add(float.Parse(mpc_y[i].ToString()));
+               }
+               point_path.setMpcPoint(my_mpc_x, my_mpc_y);

                EmitTelemetry(obj);
        }

-       void EmitTelemetry(SocketIOEvent obj)
+       protected override void EmitTelemetry(SocketIOEvent obj)
        {
-               UnityMainThreadDispatcher.Instance().Enqueue(() =>
+               Enqueue(() =>
                {
-                       print("Attempting to Send...");
-                       // send only if it's not being manually driven
-                       if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
-                               _socket.Emit("telemetry", new JSONObject());
-                       } else {
-                               // Collect Data from the Car
-                               Dictionary<string, JSONObject> data = new Dictionary<string, JSONObject>();
-                               var cte = wpt.CrossTrackError (_carController);
-                               Debug.Log(string.Format("In between waypoint {0} and {1}", wpt.prev_wp, wpt.next_wp));
-                               var pos = _carController.Position();
-                               var psi = _carController.Orientation().eulerAngles.y;
-
-                               // Waypoints data
-                               var ptsx = new List<JSONObject>();
-                               var ptsy = new List<JSONObject>();
-                               for (int i = wpt.prev_wp; i < wpt.prev_wp+polyOrder+1; i++) {
-                                       ptsx.Add(new JSONObject(wpt.waypoints[i%wpt.waypoints.Count].x));
-                                       ptsy.Add(new JSONObject(wpt.waypoints[i%wpt.waypoints.Count].z));
+                       try
+                       {
+                               if (IsManualInputActive())
+                               {
+                                       socket.Emit("telemetry", new JSONObject());
+                               }
+                               else
+                               {
+                                       var telemetryData = new JSONObject(JSONObject.Type.OBJECT);
+
+                                       // Waypoints arrays
+                                       JSONObject ptsx = new JSONObject(JSONObject.Type.ARRAY);
+                                       JSONObject ptsy = new JSONObject(JSONObject.Type.ARRAY);
+                                       for (int i = wpt.prev_wp; i < wpt.prev_wp + polyOrder + 1; i++)
+                                       {
+                                               int idx = i % wpt.waypoints.Count;
+                                               ptsx.Add(wpt.waypoints[idx].x);
+                                               ptsy.Add(wpt.waypoints[idx].z);
+                                       }
+                                       telemetryData.AddField("ptsx", ptsx);
+                                       telemetryData.AddField("ptsy", ptsy);
+
+                                       // Orientation
+                                       var psiDeg = _carController.Orientation().eulerAngles.y;
+                                       telemetryData.AddField("psi_unity", psiDeg * Mathf.Deg2Rad);
+                                       telemetryData.AddField("psi", ConvertUnityYawToMathAngle(psiDeg) * Mathf.Deg2Rad);
+
+                                       // Position
+                                       var pos = _carController.Position();
+                                       telemetryData.AddField("x", pos.x);
+                                       telemetryData.AddField("y", pos.z);
+
+                                       // Signals
+                                       telemetryData.AddField("steering_angle", _carController.CurrentSteerAngle * Mathf.Deg2Rad);
+                                       telemetryData.AddField("throttle", _carController.AccelInput);
+                                       telemetryData.AddField("speed", _carController.CurrentSpeed);
+
+                                       // CTE
+                                       float cte = wpt.CrossTrackError(_carController);
+                                       telemetryData.AddField("cte", cte);
+
+                                       // Optional: image capture if needed for MPC
+                                       // telemetryData.AddField("image", CaptureFrameBase64(FrontFacingCamera));
+
+                                       socket.Emit("telemetry", telemetryData);
                                }
-                               data["ptsx"] = new JSONObject(ptsx.ToArray());
-                               data["ptsy"] = new JSONObject(ptsy.ToArray());
-
-                // Orientations
-                data["psi_unity"] = new JSONObject(psi * Mathf.Deg2Rad);
-                               data["psi"] = new JSONObject(convertAngle(psi) * Mathf.Deg2Rad);
-
-                // Global position.
-                data["x"] = new JSONObject(pos.x);
-                data["y"] = new JSONObject(pos.z);
-                // Steering angle
-                               data["steering_angle"] = new JSONObject(_carController.CurrentSteerAngle * Mathf.Deg2Rad);
-                // Throttle
-                               data["throttle"] = new JSONObject(_carController.AccelInput);
-                // Velocity
-                               data["speed"] = new JSONObject(_carController.CurrentSpeed);
-                               _socket.Emit("telemetry", new JSONObject(data));
+                       }
+                       catch (Exception ex)
+                       {
+                               Debug.LogError($"Telemetry error: {ex}");
                        }
                });
-
        }
 }

### modified:   Assets/1_SelfDrivingCar/Scripts/term2/CommandServerTerm2.cs:

    ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/term2/CommandServerTerm2.cs
    diff --git a/Assets/1_SelfDrivingCar/Scripts/term2/CommandServerTerm2.cs b/Assets/1_SelfDrivingCar/Scripts/term2/CommandServerTerm2.cs
    index e73f76d..99484b9 100644
    --- a/Assets/1_SelfDrivingCar/Scripts/term2/CommandServerTerm2.cs
    +++ b/Assets/1_SelfDrivingCar/Scripts/term2/CommandServerTerm2.cs
    @@ -1,125 +1,128 @@
    -using System;
    -using UnityEngine;
    -using System.Collections.Generic;
    -using System.Collections;
    -using SocketIO;
    -using UnityStandardAssets.Vehicles.Car;
    -using System.Security.AccessControl;
    -using System.Globalization;
    -
    -public class CommandServerTerm2 : MonoBehaviour
    -{
    -       public CarRemoteControl CarRemoteControl;
    -       public Camera FrontFacingCamera;
    -       private SocketIOComponent _socket;
    -       private CarController _carController;
    -
    -       // Use this for initialization
    -       void Start()
    -       {
    -               _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
    -               _socket.On("open", OnOpen);
    -               _socket.On("steer", OnSteer);
    -               _socket.On("manual", onManual);
    -               _carController = CarRemoteControl.GetComponent<CarController>();
    -       }
    -
    -       // Update is called once per frame
    -       void Update()
    -       {
    -       }
    -
    -       void OnOpen(SocketIOEvent obj)
    -       {
    -               Debug.Log("Connection Open");
    -               EmitTelemetry(obj);
    -       }
    -
    -       //
    -       void onManual(SocketIOEvent obj)
    -       {
    -               Debug.Log("Triggered Callback Driving Manually");
    -               EmitTelemetry (obj);
    -       }
    -
    -       void OnSteer(SocketIOEvent obj)
    -       {
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
    -
    -                       // Add parse error handling
    -                       JSONObject jsonObject = obj.data;
    -
    -                       Debug.Log($"Full JSON Structure: {jsonObject.Print(true)}");
    -
    -                       if(!jsonObject.HasField("steering_angle") || !jsonObject.HasField("throttle")) {
    -                               Debug.LogError($"Missing fields. Actual data: {string.Join(",", jsonObject.keys)}");
    -                               return;
    -                       }
    -
    -                       // Add format verification
    -                       float steeringAngle = jsonObject.GetField("steering_angle").f;
    -                       float throttle = jsonObject.GetField("throttle").f;
    -
    -                       Debug.Log($"Raw steeringAngle = {steeringAngle} (Type: {jsonObject.GetField("steering_angle").type})");
    -                       Debug.Log($"Raw throttle = {throttle} (Type: {jsonObject.GetField("throttle").type})");
    -
    -                       CarRemoteControl.SteeringAngle = Mathf.Clamp(steeringAngle, -1, 1);
    -                       CarRemoteControl.Acceleration = Mathf.Clamp(throttle, 0, 1);
    -
    -                       EmitTelemetry(obj);
    -               } catch(Exception ex) {
    -                       Debug.LogError($"Steering error: {ex}\nFull JSON: {obj.data}");
    -               }
    -       }
    -
    -       void EmitTelemetry(SocketIOEvent obj)
    -       {
    -               UnityMainThreadDispatcher.Instance().Enqueue(() =>
    -               {
    -                       try {
    -
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
    -
    -                       } catch(Exception ex) {
    -                               Debug.LogError($"Telemetry error: {ex}");
    -                       }
    -
    -
    -               });
    -       }
    -
    -       // Add this temporary CTE calculation method
    -       float CalculateCTE()
    -       {
    -               // Replace with your actual CTE calculation logic
    -               return UnityEngine.Random.Range(0.0f, 1.0f);
    -       }
    -}
    +using System;
    +using UnityEngine;
    +using System.Collections.Generic;
    +using System.Collections;
    +using SocketIO;
    +using UnityStandardAssets.Vehicles.Car;
    +using System.Security.AccessControl;
    +using System.Globalization;
    +
    +public class CommandServerTerm2 : CommandServerBase
    +{
    +       public CarRemoteControlTerm2 CarRemoteControl;
    +       // FrontFacingCamera moved to base
    +       // private SocketIOComponent _socket; // moved to base
    +       // private CarController _carController; // moved to base
    +
    +       protected CarControllerTerm2 carController;
    +
    +       // Use this for initialization
    +       void Start()
    +       {
    +               InitSocket();
    +               RegisterHandler("open", OnOpen);
    +               RegisterHandler("steer", OnSteer);
    +               RegisterHandler("manual", onManual);
    +               carController = CarRemoteControl.GetComponent<CarControllerTerm2>();
    +       }
    +
    +       // Update is called once per frame
    +       void Update()
    +       {
    +               // ...existing code...
    +       }
    +
    +       void OnOpen(SocketIOEvent obj)
    +       {
    +               Debug.Log("Connection Open");
    +               EmitTelemetry(obj);
    +       }
    +
    +       //
    +       void onManual(SocketIOEvent obj)
    +       {
    +               Debug.Log("Triggered Callback Driving Manually");
    +               EmitTelemetry (obj);
    +       }
    +
    +       void OnSteer(SocketIOEvent obj)
    +       {
    +               Debug.Log("Triggered Callback Driving Autonomously via PID Steering");
    +
    +               Debug.Log("PID Control Active");
    +
    +               try {
    +                       // Add null check
    +                       if(obj.data == null) {
    +                               Debug.LogError("Empty steering command received");
    +                               return;
    +                       }
    +
    +                       // Add parse error handling
    +                       JSONObject jsonObject = obj.data;
    +
    +                       Debug.Log($"Full JSON Structure: {jsonObject.Print(true)}");
    +
    +                       if(!jsonObject.HasField("steering_angle") || !jsonObject.HasField("throttle")) {
    +                               Debug.LogError($"Missing fields. Actual data: {string.Join(",", jsonObject.keys)}");
    +                               return;
    +                       }
    +
    +                       // Add format verification
    +                       float steeringAngle = jsonObject.GetField("steering_angle").f;
    +                       float throttle = jsonObject.GetField("throttle").f;
    +
    +                       Debug.Log($"Raw steeringAngle = {steeringAngle} (Type: {jsonObject.GetField("steering_angle").type})");
    +                       Debug.Log($"Raw throttle = {throttle} (Type: {jsonObject.GetField("throttle").type})");
    +
    +                       CarRemoteControl.SteeringAngle = Mathf.Clamp(steeringAngle, -1, 1);
    +                       CarRemoteControl.Acceleration = Mathf.Clamp(throttle, 0, 1);
    +
    +                       EmitTelemetry(obj);
    +               } catch(Exception ex) {
    +                       Debug.LogError($"Steering error: {ex}\nFull JSON: {obj.data}");
    +               }
    +       }
    +
    +       protected override void EmitTelemetry(SocketIOEvent obj)
    +       {
    +               Enqueue(() =>
    +               {
    +                       try {
    +
    +                               print("Attempting to Send...");
    +                               // send only if it's not being manually driven
    +                               if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S))) {
    +                                       socket.Emit("telemetry", new JSONObject());
    +                               }
    +                               else {
    +                                       // Add CTE calculation (replace with your actual CTE logic)
    +                                       float currentCTE = CalculateCTE();
    +
    +                                       // Collect Simulated Data from the Car
    +                                       JSONObject telemetryData = new JSONObject(JSONObject.Type.OBJECT);
    +
    +                                       // Add fields individually
    +                                       telemetryData.AddField("cte", currentCTE);
    +                                       telemetryData.AddField("steering_angle", carController.CurrentSteerAngle);
    +                                       telemetryData.AddField("throttle", carController.AccelInput);
    +                                       telemetryData.AddField("speed", carController.CurrentSpeed);
    +                                       telemetryData.AddField("image", CaptureFrameBase64(FrontFacingCamera));
    +                                       // telemetryData.AddField("image", Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera)));
    +
    +                                       socket.Emit("telemetry", telemetryData);
    +                               }
    +
    +                       } catch(Exception ex) {
    +                               Debug.LogError($"Telemetry error: {ex}");
    +                       }
    +               });
    +       }
    +
    +       // Add this temporary CTE calculation method
    +       float CalculateCTE()
    +       {
    +               // Replace with your actual CTE calculation logic
    +               return UnityEngine.Random.Range(0.0f, 1.0f);
    +       }
    +}

### modified:   Assets/1_SelfDrivingCar/Scripts/term2/UISystemTerm2.cs:

    ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff Assets/1_SelfDrivingCar/Scripts/term2/UISystemTerm2.cs
    diff --git a/Assets/1_SelfDrivingCar/Scripts/term2/UISystemTerm2.cs b/Assets/1_SelfDrivingCar/Scripts/term2/UISystemTerm2.cs
    index c47b2ce..8e7e2f9 100644
    --- a/Assets/1_SelfDrivingCar/Scripts/term2/UISystemTerm2.cs
    +++ b/Assets/1_SelfDrivingCar/Scripts/term2/UISystemTerm2.cs
    @@ -1,178 +1,151 @@
    -using UnityEngine;
    -using UnityEngine.UI;
    -using System.Collections;
    -using UnityStandardAssets.Vehicles.Car;
    -using UnityEngine.SceneManagement;
    -
    -public class UISystemTerm2 : MonoSingleton<UISystem> {
    -
    -    public CarControllerTerm2 carController;
    -    public string GoodCarStatusMessage;
    -    public string BadSCartatusMessage;
    -    public Text MPH_Text;
    -    public Image MPH_Animation;
    -    public Text Angle_Text;
    -    public Text RecordStatus_Text;
    -       public Text DriveStatus_Text;
    -       public Text SaveStatus_Text;
    -    public GameObject RecordingPause;
    -       public GameObject RecordDisabled;
    -       public bool isTraining = false;
    -
    -    private bool recording;
    -    private float topSpeed;
    -       private bool saveRecording;
    -
    -
    -    // Use this for initialization
    -    void Start() {
    -               Debug.Log (isTraining);
    -        topSpeed = carController.MaxSpeed;
    -        recording = false;
    -        RecordingPause.SetActive(false);
    -               RecordStatus_Text.text = "RECORD";
    -               DriveStatus_Text.text = "";
    -               SaveStatus_Text.text = "";
    -               SetAngleValue(0);
    -        SetMPHValue(0);
    -               if (!isTraining) {
    -                       DriveStatus_Text.text = "Mode: Autonomous";
    -                       RecordDisabled.SetActive (true);
    -                       RecordStatus_Text.text = "";
    -               }
    -    }
    -
    -    public void SetAngleValue(float value)
    -    {
    -        Angle_Text.text = value.ToString("N2") + "°";
    -    }
    -
    -    public void SetMPHValue(float value)
    -    {
    -        MPH_Text.text = value.ToString("N2");
    -        //Do something with value for fill amounts
    -        MPH_Animation.fillAmount = value/topSpeed;
    -    }
    -
    -    public void ToggleRecording()
    -    {
    -               // Don't record in autonomous mode
    -               if (!isTraining) {
    -                       return;
    -               }
    -
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
    -
    -        // Easier than pressing the actual button :-)
    -        // Should make recording training data more pleasant.
    -
    -               if (carController.getSaveStatus ()) {
    -                       SaveStatus_Text.text = "Capturing Data: " + (int)(100 * carController.getSavePercent ()) + "%";
    -                       //Debug.Log ("save percent is: " + carController.getSavePercent ());
    -               }
    -               else if(saveRecording)
    -               {
    -                       SaveStatus_Text.text = "";
    -                       recording = false;
    -                       RecordingPause.SetActive(false);
    -                       RecordStatus_Text.text = "RECORD";
    -                       saveRecording = false;
    -               }
    -
    -        if (Input.GetKeyDown(KeyCode.R))
    -        {
    -            ToggleRecording();
    -        }
    -
    -               if (!isTraining)
    -               {
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
    -               }
    -
    -           if(Input.GetKeyDown(KeyCode.Escape))
    -        {
    -            //Do Menu Here
    -            // SceneManager.LoadScene("MenuScene");
    -            ReturnToAppropriateMenu();
    -        }
    -
    -        if (Input.GetKeyDown(KeyCode.Return))
    -        {
    -            //Do Console Here
    -        }
    -
    -        UpdateCarValues();
    -    }
    -
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
    -
    -        Debug.Log($"isTerm1Scene = {isTerm1Scene}");
    -
    -        // bool isTerm1Scene = currentScene.Contains("LakeTrack") || currentScene.Contains("JungleTrack");
    -
    -        SceneManager.LoadScene(isTerm1Scene ? "MenuScene" : "MenuSceneTerm2");
    -    }
    -
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
    -}
    +using UnityEngine;
    +using UnityEngine.UI;
    +using System.Collections;
    +using UnityStandardAssets.Vehicles.Car;
    +using UnityEngine.SceneManagement;
    +
    +public class UISystemTerm2 : UISystemBase {
    +
    +    public CarControllerTerm2 carController;
    +    public string GoodCarStatusMessage;
    +    public string BadSCartatusMessage;
    +    // MPH_Text and MPH_Animation are now inherited from UISystemBase
    +    public Text Angle_Text;
    +    public Text RecordStatus_Text;
    +    public Text DriveStatus_Text;
    +    public Text SaveStatus_Text;
    +    public GameObject RecordingPause;
    +    public GameObject RecordDisabled;
    +    public bool isTraining = false;
    +
    +    private bool recording;
    +    // topSpeed is now inherited from UISystemBase
    +    private bool saveRecording;
    +
    +
    +    // Use this for initialization
    +    void Start() {
    +               Debug.Log (isTraining);
    +        InitTopSpeed(carController.MaxSpeed);
    +        recording = false;
    +        RecordingPause.SetActive(false);
    +               RecordStatus_Text.text = "RECORD";
    +               DriveStatus_Text.text = "";
    +               SaveStatus_Text.text = "";
    +               SetAngleValue(0);
    +        SetMPHValue(0);
    +               if (!isTraining) {
    +                       DriveStatus_Text.text = "Mode: Autonomous";
    +                       RecordDisabled.SetActive (true);
    +                       RecordStatus_Text.text = "";
    +               }
    +    }
    +
    +    public void SetAngleValue(float value)
    +    {
    +        Angle_Text.text = value.ToString("N2") + "°";
    +    }
    +
    +    public void ToggleRecording()
    +    {
    +               // Don't record in autonomous mode
    +               if (!isTraining) {
    +                       return;
    +               }
    +
    +        if (!recording)
    +        {
    +                       if (carController.checkSaveLocation())
    +                       {
    +                               recording = true;
    +                               RecordingPause.SetActive (true);
    +                               RecordStatus_Text.text = "RECORDING";
    +                               carController.IsRecording = true;
    +                       }
    +        }
    +        else
    +        {
    +                       saveRecording = true;
    +                       carController.IsRecording = false;
    +        }
    +    }
    +
    +    void UpdateCarValues()
    +    {
    +        SetMPHValue(carController.CurrentSpeed);
    +        SetAngleValue(carController.CurrentSteerAngle);
    +    }
    +
    +    // Override Escape handling to auto-select Term menu
    +    protected override void OnEscapePressed()
    +    {
    +        ReturnToAppropriateMenu();
    +    }
    +
    +       // Update is called once per frame
    +       void Update () {
    +
    +        // Easier than pressing the actual button :-)
    +        // Should make recording training data more pleasant.
    +
    +               if (carController.getSaveStatus ()) {
    +                       SaveStatus_Text.text = "Capturing Data: " + (int)(100 * carController.getSavePercent ()) + "%";
    +                       //Debug.Log ("save percent is: " + carController.getSavePercent ());
    +               }
    +               else if(saveRecording)
    +               {
    +                       SaveStatus_Text.text = "";
    +                       recording = false;
    +                       RecordingPause.SetActive(false);
    +                       RecordStatus_Text.text = "RECORD";
    +                       saveRecording = false;
    +               }
    +
    +        if (Input.GetKeyDown(KeyCode.R))
    +        {
    +            ToggleRecording();
    +        }
    +
    +               if (!isTraining)
    +               {
    +                       if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S)))
    +                       {
    +                               DriveStatus_Text.color = Color.red;
    +                               DriveStatus_Text.text = "Mode: Manual";
    +                       }
    +                       else
    +                       {
    +                               DriveStatus_Text.color = Color.white;
    +                               DriveStatus_Text.text = "Mode: Autonomous";
    +                       }
    +               }
    +
    +           // Replace inline Escape handling with shared helper
    +        HandleEscapeKey();
    +
    +        if (Input.GetKeyDown(KeyCode.Return))
    +        {
    +            //Do Console Here
    +        }
    +
    +        UpdateCarValues();
    +    }
    +
    +    // Automatic Term Detection without MenuOptions
    +    void ReturnToAppropriateMenu()
    +    {
    +        bool isTerm1Scene = false;
    +        string currentScene = SceneManager.GetActiveScene().name;
    +
    +        if(currentScene == "LakeTrackTraining" || currentScene == "LakeTrackAutonomous") {
    +            isTerm1Scene = true;
    +        }
    +        else if(currentScene == "JungleTrackTraining" || currentScene == "JungleTrackAutonomous") {
    +            isTerm1Scene = true;
    +        }
    +
    +        Debug.Log($"isTerm1Scene = {isTerm1Scene}");
    +
    +        SceneManager.LoadScene(isTerm1Scene ? "MenuScene" : "MenuSceneTerm2");
    +    }
    +}

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/BrakeLight.cs:

    ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/BrakeLight.cs"
    diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/BrakeLight.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/BrakeLight.cs
    index 34a4ea0..ad31da7 100644
    --- a/Assets/Standard Assets/Vehicles/Car/Scripts/BrakeLight.cs
    +++ b/Assets/Standard Assets/Vehicles/Car/Scripts/BrakeLight.cs
    @@ -3,23 +3,28 @@ using UnityEngine;

    namespace UnityStandardAssets.Vehicles.Car
    {
    -    public class BrakeLight : MonoBehaviour
    +    public class BrakeLight : BrakeLightBase
        {
            public CarController car; // reference to the car controller, must be dragged in inspector

    -        private Renderer m_Renderer;
    +        // private Renderer m_Renderer;


    -        private void Start()
    -        {
    -            m_Renderer = GetComponent<Renderer>();
    -        }
    +        // private void Start()
    +        // {
    +        //     m_Renderer = GetComponent<Renderer>();
    +        // }
    +

    +        // private void Update()
    +        // {
    +        //     // enable the Renderer when the car is braking, disable it otherwise.
    +        //     m_Renderer.enabled = GetBrakeInput() > 0f;
    +        // }

    -        private void Update()
    +        protected override float GetBrakeInput()
            {
    -            // enable the Renderer when the car is braking, disable it otherwise.
    -            m_Renderer.enabled = car.BrakeInput > 0f;
    +            return car != null ? car.BrakeInput : 0f;
            }
        }
    }

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs
index e5e91fe..ba00e41 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/CarAIControl.cs
@@ -8,51 +8,15 @@ using System.Collections;
 namespace UnityStandardAssets.Vehicles.Car
 {
     [RequireComponent (typeof(CarController))]
-    public class CarAIControl : MonoBehaviour
+    public class CarAIControl : CarAIControlBase
     {
-        public enum BrakeCondition
-        {
-            NeverBrake,
-            // the car simply accelerates at full throttle all the time.
-            TargetDirectionDifference,
-            // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.
-            TargetDistance,
-            // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
-            // head for a stationary target and come to rest when it arrives there.
-        }
-
-        // This script provides input to the car controller in the same way that the user control script does.
+               // This script provides input to the car controller in the same way that the user control script does.
         // As such, it is really 'driving' the car, with no special physics or animation tricks to make the car behave properly.

         // "wandering" is used to give the cars a more human, less robotic feel. They can waver slightly
         // in speed and direction while driving towards their target.

-        [SerializeField] [Range (0, 1)] private float m_CautiousSpeedFactor = 0.05f;
-        // percentage of max speed to use when being maximally cautious
-        [SerializeField] [Range (0, 180)] private float m_CautiousMaxAngle = 50f;
-        // angle of approaching corner to treat as warranting maximum caution
-        [SerializeField] private float m_CautiousMaxDistance = 100f;
-        // distance at which distance-based cautiousness begins
-        [SerializeField] private float m_CautiousAngularVelocityFactor = 30f;
-        // how cautious the AI should be when considering its own current angular velocity (i.e. easing off acceleration if spinning!)
-        [SerializeField] private float m_SteerSensitivity = 0.05f;
-        // how sensitively the AI uses steering input to turn to the desired direction
-        [SerializeField] private float m_AccelSensitivity = 0.04f;
-        // How sensitively the AI uses the accelerator to reach the current desired speed
-        [SerializeField] private float m_BrakeSensitivity = 1f;
-        // How sensitively the AI uses the brake to reach the current desired speed
-        [SerializeField] private float m_LateralWanderDistance = 3f;
-        // how far the car will wander laterally towards its target
-        [SerializeField] private float m_LateralWanderSpeed = 0.1f;
-        // how fast the lateral wandering will fluctuate
-        [SerializeField] [Range (0, 1)] private float m_AccelWanderAmount = 0.1f;
-        // how much the cars acceleration will wander
-        [SerializeField] private float m_AccelWanderSpeed = 0.1f;
-        // how fast the cars acceleration wandering will fluctuate
-        [SerializeField] private BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance;
-        // what should the AI consider when accelerating/braking?
-        [SerializeField] private bool m_Driving;
-        // whether the AI is currently actively driving or stopped.
+               // whether the AI is currently actively driving or stopped.
                [SerializeField] private List<Transform> waypoints;

                public GameObject front_sensor;
@@ -72,17 +36,18 @@ namespace UnityStandardAssets.Vehicles.Car

                public GameObject mycar;

-        private float m_RandomPerlin;
-        // A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
         private CarController m_CarController;
+
+               // protected float m_RandomPerlin;
+
         // Reference to actual car controller we are controlling
-        private float m_AvoidOtherCarTime;
+        // private float m_AvoidOtherCarTime;
         // time until which to avoid the car we recently collided with
         private float m_AvoidOtherCarSlowdown;
         // how much to slow down due to colliding with another car, whilst avoiding
-        private float m_AvoidPathOffset;
+        // private float m_AvoidPathOffset;
         // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding
-        private Rigidbody m_Rigidbody;
+        // private Rigidbody m_Rigidbody;

                public CarController follow_car;
                // the car that we are looping with
@@ -162,6 +127,8 @@ namespace UnityStandardAssets.Vehicles.Car
                        //flag new data is ready to process
                        simulator_process = false;

+                       // Initialize shared AI state
+                       InitAICommon();
         }

                public void Spawn(List<GameObject> cars)

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarAudio.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/CarAudio.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/CarAudio.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/CarAudio.cs
index 550149c..b676cc8 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/CarAudio.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/CarAudio.cs
@@ -1,11 +1,12 @@
 using System;
 using UnityEngine;
 using Random = UnityEngine.Random;
+using UnityStandardAssets.Vehicles.Car;

 namespace UnityStandardAssets.Vehicles.Car
 {
     [RequireComponent(typeof (CarController))]
-    public class CarAudio : MonoBehaviour
+    public class CarAudio : CarAudioBase
     {
         // This script reads some of the car's current properties and plays sounds accordingly.
         // The engine sound can be a simple single clip which is looped and pitched, or it
@@ -23,162 +24,31 @@ namespace UnityStandardAssets.Vehicles.Car
         // For proper crossfading, the clips pitches should all match, with an octave offset between low and high.


-        public enum EngineAudioOptions // Options for the engine audio
-        {
-            Simple, // Simple style audio
-            FourChannel // four Channel audio
-        }
-
-        public EngineAudioOptions engineSoundStyle = EngineAudioOptions.FourChannel;// Set the default audio options to be four channel
-        public AudioClip lowAccelClip;                                              // Audio clip for low acceleration
-        public AudioClip lowDecelClip;                                              // Audio clip for low deceleration
-        public AudioClip highAccelClip;                                             // Audio clip for high acceleration
-        public AudioClip highDecelClip;                                             // Audio clip for high deceleration
-        public float pitchMultiplier = 1f;                                          // Used for altering the pitch of audio clips
-        public float lowPitchMin = 1f;                                              // The lowest possible pitch for the low sounds
-        public float lowPitchMax = 6f;                                              // The highest possible pitch for the low sounds
-        public float highPitchMultiplier = 0.25f;                                   // Used for altering the pitch of high sounds
-        public float maxRolloffDistance = 500;                                      // The maximum distance where rollof starts to take place
-        public float dopplerLevel = 1;                                              // The mount of doppler effect used in the audio
-        public bool useDoppler = true;                                              // Toggle for using doppler
+        private CarController m_CarController;

-        private AudioSource m_LowAccel; // Source for the low acceleration sounds
-        private AudioSource m_LowDecel; // Source for the low deceleration sounds
-        private AudioSource m_HighAccel; // Source for the high acceleration sounds
-        private AudioSource m_HighDecel; // Source for the high deceleration sounds
-        private bool m_StartedSound; // flag for knowing if we have started sounds
-        private CarController m_CarController; // Reference to car we are controlling

-
-        private void StartSound()
+        private void Awake()
         {
-            // get the carcontroller ( this will not be null as we have require component)
             m_CarController = GetComponent<CarController>();
-
-            // setup the simple audio source
-            m_HighAccel = SetUpEngineAudioSource(highAccelClip);
-
-            // if we have four channel audio setup the four audio sources
-            if (engineSoundStyle == EngineAudioOptions.FourChannel)
-            {
-                m_LowAccel = SetUpEngineAudioSource(lowAccelClip);
-                m_LowDecel = SetUpEngineAudioSource(lowDecelClip);
-                m_HighDecel = SetUpEngineAudioSource(highDecelClip);
-            }
-
-            // flag that we have started the sounds playing
-            m_StartedSound = true;
-        }
-
-
-        private void StopSound()
-        {
-            //Destroy all audio sources on this object:
-            foreach (var source in GetComponents<AudioSource>())
-            {
-                Destroy(source);
-            }
-
-            m_StartedSound = false;
         }


         // Update is called once per frame
         private void Update()
         {
-            // get the distance to main camera
-            float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;
-
-            // stop sound if the object is beyond the maximum roll off distance
-            if (m_StartedSound && camDist > maxRolloffDistance*maxRolloffDistance)
-            {
-                StopSound();
-            }
-
-            // start the sound if not playing and it is nearer than the maximum distance
-            if (!m_StartedSound && camDist < maxRolloffDistance*maxRolloffDistance)
-            {
-                StartSound();
-            }
-
-            if (m_StartedSound)
-            {
-                // The pitch is interpolated between the min and max values, according to the car's revs.
-                float pitch = ULerp(lowPitchMin, lowPitchMax, m_CarController.Revs);
-
-                // clamp to minimum pitch (note, not clamped to max for high revs while burning out)
-                pitch = Mathf.Min(lowPitchMax, pitch);
-
-                if (engineSoundStyle == EngineAudioOptions.Simple)
-                {
-                    // for 1 channel engine sound, it's oh so simple:
-                    m_HighAccel.pitch = pitch*pitchMultiplier*highPitchMultiplier;
-                    m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
-                    m_HighAccel.volume = 1;
-                }
-                else
-                {
-                    // for 4 channel engine sound, it's a little more complex:
-
-                    // adjust the pitches based on the multipliers
-                    m_LowAccel.pitch = pitch*pitchMultiplier;
-                    m_LowDecel.pitch = pitch*pitchMultiplier;
-                    m_HighAccel.pitch = pitch*highPitchMultiplier*pitchMultiplier;
-                    m_HighDecel.pitch = pitch*highPitchMultiplier*pitchMultiplier;
-
-                    // get values for fading the sounds based on the acceleration
-                    float accFade = Mathf.Abs(m_CarController.AccelInput);
-                    float decFade = 1 - accFade;
-
-                    // get the high fade value based on the cars revs
-                    float highFade = Mathf.InverseLerp(0.2f, 0.8f, m_CarController.Revs);
-                    float lowFade = 1 - highFade;
-
-                    // adjust the values to be more realistic
-                    highFade = 1 - ((1 - highFade)*(1 - highFade));
-                    lowFade = 1 - ((1 - lowFade)*(1 - lowFade));
-                    accFade = 1 - ((1 - accFade)*(1 - accFade));
-                    decFade = 1 - ((1 - decFade)*(1 - decFade));
-
-                    // adjust the source volumes based on the fade values
-                    m_LowAccel.volume = lowFade*accFade;
-                    m_LowDecel.volume = lowFade*decFade;
-                    m_HighAccel.volume = highFade*accFade;
-                    m_HighDecel.volume = highFade*decFade;
-
-                    // adjust the doppler levels
-                    m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
-                    m_LowAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
-                    m_HighDecel.dopplerLevel = useDoppler ? dopplerLevel : 0;
-                    m_LowDecel.dopplerLevel = useDoppler ? dopplerLevel : 0;
-                }
-            }
+            UpdateAudio();
         }


-        // sets up and adds new audio source to the gane object
-        private AudioSource SetUpEngineAudioSource(AudioClip clip)
+        protected override float GetRevs()
         {
-            // create the new audio source component on the game object and set up its properties
-            AudioSource source = gameObject.AddComponent<AudioSource>();
-            source.clip = clip;
-            source.volume = 0;
-            source.loop = true;
-
-            // start the clip from a random point
-            source.time = Random.Range(0f, clip.length);
-            source.Play();
-            source.minDistance = 5;
-            source.maxDistance = maxRolloffDistance;
-            source.dopplerLevel = 0;
-            return source;
+            return m_CarController != null ? m_CarController.Revs : 0f;
         }


-        // unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
-        private static float ULerp(float from, float to, float value)
+        protected override float GetAccelInput()
         {
-            return (1.0f - value)*from + value*to;
+            return m_CarController != null ? m_CarController.AccelInput : 0f;
         }
     }
 }

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs
index 8ca27ca..2962386 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/CarController.cs
@@ -5,8 +5,6 @@ using UnityEngine;
 using UnityEngine.EventSystems;
 using System.Collections.Generic;

-
-
 namespace UnityStandardAssets.Vehicles.Car
 {
     internal enum CarDriveType
@@ -22,7 +20,7 @@ namespace UnityStandardAssets.Vehicles.Car
         KPH
     }

-    public class CarController : MonoBehaviour
+    public class CarController : CarControllerBase
     {
         [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
         [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
@@ -48,12 +46,10 @@ namespace UnityStandardAssets.Vehicles.Car

         private Quaternion[] m_WheelMeshLocalRotations;
         private Vector3 m_Prevpos, m_Pos;
-        private float m_SteerAngle;
         private int m_GearNum;
         private float m_GearFactor;
         private float m_OldRotation;
         private float m_CurrentTorque;
-        private Rigidbody m_Rigidbody;
         private const float k_ReversingThreshold = 0.01f;


@@ -61,120 +57,16 @@ namespace UnityStandardAssets.Vehicles.Car

         public float BrakeInput { get; private set; }

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
-
-        public float CurrentSteerAngle {
-            get { return m_SteerAngle; }
-            set { m_SteerAngle = value; }
+        public float MaxSpeed
+        {
+            get { return m_Topspeed; }
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
+        public void setMaxSpeed(float Topspeed)
+        {
+            m_Topspeed = Topspeed;
+        }

-        // Use this for initialization
         private void Start ()
         {
             m_WheelMeshLocalRotations = new Quaternion[4];
@@ -185,7 +77,9 @@ namespace UnityStandardAssets.Vehicles.Car

             m_MaxHandbrakeTorque = float.MaxValue;

-            m_Rigidbody = GetComponent<Rigidbody> ();
+            // Initialize shared base state
+            InitControllerBase();
+
             m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);

                        lastSpeed = 0;
@@ -248,113 +142,13 @@ namespace UnityStandardAssets.Vehicles.Car

         public void FixedUpdate()
         {
-                       if (main_car)
+                       if (main_car)
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
+                               // Replace duplicated motion-sensing with base helper
+                               UpdateMainCarMotionSensing();
                        }
         }

-               public float AverageLastSpeed()
-               {
-
-                       float averaged_speed = 0.0f;
-
-                       for (int i = 0; i < averageSpeed.Count; i++)
-                       {
-                               averaged_speed += averageSpeed[i];
-                       }
-
-                       return averaged_speed / (float)(averageSpeed.Count);
-
-               }
-               public float AverageLastAcc()
-               {
-
-                       float averaged_acc = 0.0f;
-
-                       for (int i = 0; i < averageAcc.Count; i++)
-                       {
-                               averaged_acc += averageAcc[i];
-                       }
-
-                       return averaged_acc / (float)(averageAcc.Count);
-
-               }
-               public float SenseCurve()
-               {
-                       float averaged_curve = 0.0f;
-
-                       for (int i = 0; i < previous_pos.Count-2; i++)
-                       {
-
-
-                               float x1 = previous_pos [i].x;
-                               float x2 = previous_pos [i + 1].x;
-                               float x3 = previous_pos [i + 2].x;
-
-                               float y1 = previous_pos [i].z;
-                               float y2 = previous_pos [i + 1].z;
-                               float y3 = previous_pos [i + 2].z;
-
-                               Vector2 ray1 = new Vector2 (x2 - x1, y2 - y1);
-                               Vector2 ray2 = new Vector2 (x3 - x2, y3 - y2);
-
-                               if (ray1.magnitude != 0 && ray2.magnitude != 0)
-                               {
-
-                                       Vector2 ray3 = new Vector2 (x3 - x1, y3 - y1);
-
-                                       float corner_angle = Mathf.Abs (Vector2.Angle (ray1, ray2));
-
-                                       if (ray3.magnitude != 0 && corner_angle != 180)
-                                       {
-                                               averaged_curve += 2 * Mathf.Sin (corner_angle*Mathf.Deg2Rad) / ray3.magnitude;
-                                       }
-                                       else
-                                       {
-
-                                               //the curve is infinite, this move is totally illegal, just going to return 1000000
-                                               averaged_curve += 1000000;
-                                       }
-
-                               }
-                               //else skip and just say that curve is zero since its stopped
-                       }
-
-                       return averaged_curve / ( (float)(previous_pos.Count-2));
-
-               }
-
         public void Move (float steering, float accel, float footbrake, float handbrake)
         {
             for (int i = 0; i < 4; i++) {

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs
index d5baea2..391bf65 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/CarRemoteControl.cs
@@ -5,33 +5,20 @@ using UnityStandardAssets.CrossPlatformInput;
 namespace UnityStandardAssets.Vehicles.Car
 {
     [RequireComponent(typeof(CarController))]
-    public class CarRemoteControl : MonoBehaviour
+    public class CarRemoteControl : CarRemoteControlBase
     {
         private CarController m_Car; // the car controller we want to use

-        public float SteeringAngle { get; set; }
-        public float Acceleration { get; set; }
-        private Steering s;
-
         private void Awake()
         {
             // get the car controller
             m_Car = GetComponent<CarController>();
-            s = new Steering();
-            s.Start();
+            base.Awake();
         }

-        private void FixedUpdate()
+        protected override void ApplyMove(float h, float v)
         {
-            // If holding down W or S control the car manually
-            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
-            {
-                s.UpdateValues();
-                m_Car.Move(s.H, s.V, s.V, 0f);
-            } else
-            {
-                               m_Car.Move(SteeringAngle, Acceleration, Acceleration, 0f);
-            }
+            m_Car.Move(h, v, v, 0f);
         }
     }
 }

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/CarUserControl.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/CarUserControl.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/CarUserControl.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/CarUserControl.cs
index 308eddc..59eb6f4 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/CarUserControl.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/CarUserControl.cs
@@ -5,23 +5,19 @@ using System.Collections;
 namespace UnityStandardAssets.Vehicles.Car
 {
     [RequireComponent(typeof(CarController))]
-    public class CarUserControl : MonoBehaviour
+    public class CarUserControl : CarUserControlBase
     {
         private CarController m_Car;
-        private Steering s;

         private void Awake()
         {
             m_Car = GetComponent<CarController>();
-            s = new Steering();
-            s.Start();
+            base.Awake();
         }

-        private void FixedUpdate()
+        protected override void ApplyMove(float h, float v)
         {
-            s.UpdateValues();
-            m_Car.Move(s.H, s.V, s.V, 0f);
-
+            m_Car.Move(h, v, v, 0f);
         }
     }
 }

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/Mudguard.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/Mudguard.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/Mudguard.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/Mudguard.cs
index 895d49f..3174c14 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/Mudguard.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/Mudguard.cs
@@ -6,22 +6,27 @@ namespace UnityStandardAssets.Vehicles.Car
     // this script is specific to the supplied Sample Assets car, which has mudguards over the front wheels
     // which have to turn with the wheels when steering is applied.

-    public class Mudguard : MonoBehaviour
+    public class Mudguard : MudguardBase
     {
         public CarController carController; // car controller to get the steering angle

-        private Quaternion m_OriginalRotation;
+        // private Quaternion m_OriginalRotation;


-        private void Start()
-        {
-            m_OriginalRotation = transform.localRotation;
-        }
+        // private void Start()
+        // {
+        //     m_OriginalRotation = transform.localRotation;
+        // }
+

+        // private void Update()
+        // {
+        //     transform.localRotation = m_OriginalRotation*Quaternion.Euler(0, GetSteerAngle(), 0);
+        // }

-        private void Update()
+        protected override float GetSteerAngle()
         {
-            transform.localRotation = m_OriginalRotation*Quaternion.Euler(0, carController.CurrentSteerAngle, 0);
+            return carController != null ? carController.CurrentSteerAngle : 0f;
         }
     }
 }

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/project_4/WaypointTracker_pid.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/project_4/WaypointTracker_pid.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/project_4/WaypointTracker_pid.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/project_4/WaypointTracker_pid.cs
index 0a23561..f21e716 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/project_4/WaypointTracker_pid.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/project_4/WaypointTracker_pid.cs
@@ -28,7 +28,7 @@ namespace UnityStandardAssets.Vehicles.Car
                }

                // Compute the next waypoint we should go to
-               private int NextWaypoint(CarController cc) {
+               private int NextWaypoint(CarControllerTerm2 cc) {
                        Vector3 p = cc.transform.position;
                        float closestLen = 100000; // large number
                        int closestWaypoint = 0;
@@ -56,7 +56,7 @@ namespace UnityStandardAssets.Vehicles.Car
                        return closestWaypoint;
                }

-               public float CrossTrackError(CarController cc) {
+               public float CrossTrackError(CarControllerTerm2 cc) {
                        var next_wp = NextWaypoint (cc);
                        var pos = cc.transform.position;

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/project_5/WaypointTracker_mpc.cs:

ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/project_5/WaypointTracker_mpc.cs"
diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/project_5/WaypointTracker_mpc.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/project_5/WaypointTracker_mpc.cs
index 38fea5d..9834f11 100644
--- a/Assets/Standard Assets/Vehicles/Car/Scripts/project_5/WaypointTracker_mpc.cs
+++ b/Assets/Standard Assets/Vehicles/Car/Scripts/project_5/WaypointTracker_mpc.cs
@@ -30,7 +30,7 @@ namespace UnityStandardAssets.Vehicles.Car
         }

         // Compute the next waypoint we should go to
-        public int NextWaypoint(CarController cc)
+        public int NextWaypoint(CarControllerTerm2 cc)
         {
             Vector3 p = cc.transform.position;
             float closestLen = 100000; // large number
@@ -62,7 +62,7 @@ namespace UnityStandardAssets.Vehicles.Car
             return closestWaypoint;
         }

-        public float CrossTrackError(CarController cc)
+        public float CrossTrackError(CarControllerTerm2 cc)
         {
             next_wp = NextWaypoint(cc);
             var pos = cc.transform.position;

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAIControlTerm2.cs:

    ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAIControlTerm2.cs"
    diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAIControlTerm2.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAIControlTerm2.cs
    index 90c1eba..f47d3d7 100644
    --- a/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAIControlTerm2.cs
    +++ b/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAIControlTerm2.cs
    @@ -1,233 +1,142 @@
    -using System;
    -using UnityEngine;
    -using Random = UnityEngine.Random;
    -
    -namespace UnityStandardAssets.Vehicles.Car
    -{
    -    [RequireComponent (typeof(CarControllerTerm2))]
    -    public class CarAIControlTerm2 : MonoBehaviour
    -    {
    -        public enum BrakeCondition
    -        {
    -            NeverBrake,
    -            // the car simply accelerates at full throttle all the time.
    -            TargetDirectionDifference,
    -            // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.
    -            TargetDistance,
    -            // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
    -            // head for a stationary target and come to rest when it arrives there.
    -        }
    -
    -        // This script provides input to the car controller in the same way that the user control script does.
    -        // As such, it is really 'driving' the car, with no special physics or animation tricks to make the car behave properly.
    -
    -        // "wandering" is used to give the cars a more human, less robotic feel. They can waver slightly
    -        // in speed and direction while driving towards their target.
    -
    -        [SerializeField] [Range (0, 1)] private float m_CautiousSpeedFactor = 0.05f;
    -        // percentage of max speed to use when being maximally cautious
    -        [SerializeField] [Range (0, 180)] private float m_CautiousMaxAngle = 50f;
    -        // angle of approaching corner to treat as warranting maximum caution
    -        [SerializeField] private float m_CautiousMaxDistance = 100f;
    -        // distance at which distance-based cautiousness begins
    -        [SerializeField] private float m_CautiousAngularVelocityFactor = 30f;
    -        // how cautious the AI should be when considering its own current angular velocity (i.e. easing off acceleration if spinning!)
    -        [SerializeField] private float m_SteerSensitivity = 0.05f;
    -        // how sensitively the AI uses steering input to turn to the desired direction
    -        [SerializeField] private float m_AccelSensitivity = 0.04f;
    -        // How sensitively the AI uses the accelerator to reach the current desired speed
    -        [SerializeField] private float m_BrakeSensitivity = 1f;
    -        // How sensitively the AI uses the brake to reach the current desired speed
    -        [SerializeField] private float m_LateralWanderDistance = 3f;
    -        // how far the car will wander laterally towards its target
    -        [SerializeField] private float m_LateralWanderSpeed = 0.1f;
    -        // how fast the lateral wandering will fluctuate
    -        [SerializeField] [Range (0, 1)] private float m_AccelWanderAmount = 0.1f;
    -        // how much the cars acceleration will wander
    -        [SerializeField] private float m_AccelWanderSpeed = 0.1f;
    -        // how fast the cars acceleration wandering will fluctuate
    -        [SerializeField] private BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance;
    -        // what should the AI consider when accelerating/braking?
    -        [SerializeField] private bool m_Driving;
    -        // whether the AI is currently actively driving or stopped.
    -        [SerializeField] private Transform m_Target;
    -        // 'target' the target object to aim for.
    -        [SerializeField] private bool m_StopWhenTargetReached;
    -        // should we stop driving when we reach the target?
    -        [SerializeField] private float m_ReachTargetThreshold = 2;
    -        // proximity to target to consider we 'reached' it, and stop driving.
    -
    -        private float m_RandomPerlin;
    -        // A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
    -        private CarControllerTerm2 m_CarController;
    -        // Reference to actual car controller we are controlling
    -        private float m_AvoidOtherCarTime;
    -        // time until which to avoid the car we recently collided with
    -        private float m_AvoidOtherCarSlowdown;
    -        // how much to slow down due to colliding with another car, whilst avoiding
    -        private float m_AvoidPathOffset;
    -        // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding
    -        private Rigidbody m_Rigidbody;
    -
    -
    -        private void Awake ()
    -        {
    -            // get the car controller reference
    -            m_CarController = GetComponent<CarControllerTerm2> ();
    -
    -            // give the random perlin a random value
    -            m_RandomPerlin = Random.value * 100;
    -
    -            m_Rigidbody = GetComponent<Rigidbody> ();
    -        }
    -
    -
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
    -
    -                // decide the actual amount of accel/brake input to achieve desired speed.
    -                float accel = Mathf.Clamp ((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);
    -
    -                // add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
    -                // i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
    -                accel *= (1 - m_AccelWanderAmount) +
    -                (Mathf.PerlinNoise (Time.time * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);
    -
    -                // calculate the local-relative position of the target, to steer towards
    -                Vector3 localTarget = transform.InverseTransformPoint (offsetTargetPos);
    -
    -                // work out the local angle towards the target
    -                float targetAngle = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;
    -
    -                // get the amount of steering needed to aim the car towards the target
    -                float steer = Mathf.Clamp (targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign (m_CarController.CurrentSpeed);
    -
    -                // feed input to the car controller.
    -                m_CarController.Move (steer, accel, accel, 0f);
    -
    -                // if appropriate, stop driving when we're close enough to the target.
    -                if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold) {
    -                    m_Driving = false;
    -                }
    -            }
    -        }
    -
    -
    -        private void OnCollisionStay (Collision col)
    -        {
    -            // detect collision against other cars, so that we can take evasive action
    -            if (col.rigidbody != null) {
    -                var otherAI = col.rigidbody.GetComponent<CarAIControlTerm2> ();
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
    -        }
    -
    -
    -        public void SetTarget (Transform target)
    -        {
    -            m_Target = target;
    -            m_Driving = true;
    -        }
    -    }
    -}
    +using System;
    +using UnityEngine;
    +using Random = UnityEngine.Random;
    +
    +namespace UnityStandardAssets.Vehicles.Car
    +{
    +    [RequireComponent (typeof(CarControllerTerm2))]
    +    public class CarAIControlTerm2 : CarAIControlBase
    +    {
    +        // 'target' the target object to aim for.
    +        [SerializeField] private bool m_StopWhenTargetReached;
    +        // should we stop driving when we reach the target?
    +        [SerializeField] private float m_ReachTargetThreshold = 2;
    +
    +        // whether the AI is currently actively driving or stopped.
    +        [SerializeField] private Transform m_Target;
    +
    +        private CarControllerTerm2 m_CarController;
    +
    +        // protected float m_RandomPerlin;
    +
    +        // Reference to actual car controller we are controlling
    +        // private float m_AvoidOtherCarTime;
    +        // time until which to avoid the car we recently collided with
    +        private float m_AvoidOtherCarSlowdown;
    +        // how much to slow down due to colliding with another car, whilst avoiding
    +        // private float m_AvoidPathOffset;
    +        // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding
    +        // private Rigidbody m_Rigidbody;
    +
    +
    +        private void Awake ()
    +        {
    +            // get the car controller reference
    +            m_CarController = GetComponent<CarControllerTerm2> ();
    +
    +            // Initialize shared AI state
    +            InitAICommon ();
    +        }
    +
    +
    +        private void FixedUpdate ()
    +        {
    +            if (m_Target == null || !m_Driving) {
    +                // Car should not be moving,
    +                // use handbrake to stop
    +                    m_CarController.Move (0, 0, -1f, 1f);
    +                }
    +            else
    +            {
    +                Vector3 fwd = transform.forward;
    +                if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed * 0.1f) {
    +                    fwd = m_Rigidbody.velocity;
    +                }
    +
    +                float desiredSpeed = m_CarController.MaxSpeed;
    +
    +                // now it's time to decide if we should be slowing down...
    +                switch (m_BrakeCondition) {
    +                case BrakeCondition.TargetDirectionDifference:
    +                    desiredSpeed = DesiredSpeedByDirection(fwd, m_Target, m_CarController.MaxSpeed);
    +                    break;
    +                case BrakeCondition.TargetDistance:
    +                    desiredSpeed = DesiredSpeedByDistance(m_Target, transform.position, m_CarController.MaxSpeed);
    +                    break;
    +                case BrakeCondition.NeverBrake:
    +                    break;
    +                }
    +
    +                // our target position starts off as the 'real' target position
    +                Vector3 offsetTargetPos = m_Target.position;
    +
    +                // if are we currently taking evasive action to prevent being stuck against another car:
    +                if (Time.time < m_AvoidOtherCarTime) {
    +                    // slow down if necessary (if we were behind the other car when collision occured)
    +                    desiredSpeed *= m_AvoidOtherCarSlowdown;
    +
    +                    // and veer towards the side of our path-to-target that is away from the other car
    +                    offsetTargetPos += m_Target.right * m_AvoidPathOffset;
    +                } else {
    +                    // no need for evasive action, we can just wander across the path-to-target in a random way,
    +                    // which can help prevent AI from seeming too uniform and robotic in their driving
    +                    offsetTargetPos += m_Target.right *
    +                    (Mathf.PerlinNoise (Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) *
    +                    m_LateralWanderDistance;
    +                }
    +
    +                // use different sensitivity depending on whether accelerating or braking:
    +                float accel = ComputeAccel(desiredSpeed, m_CarController.CurrentSpeed);
    +
    +                // calculate the local-relative position of the target, to steer towards
    +                Vector3 localTarget = transform.InverseTransformPoint (offsetTargetPos);
    +
    +                // get the amount of steering needed to aim the car towards the target
    +                float steer = ComputeSteer(localTarget, m_CarController.CurrentSpeed);
    +
    +                // feed input to the car controller.
    +                m_CarController.Move (steer, accel, accel, 0f);
    +
    +                // if appropriate, stop driving when we're close enough to the target.
    +                if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold) {
    +                    m_Driving = false;
    +                }
    +            }
    +        }
    +
    +
    +        private void OnCollisionStay (Collision col)
    +        {
    +            // detect collision against other cars, so that we can take evasive action
    +            if (col.rigidbody != null) {
    +                var otherAI = col.rigidbody.GetComponent<CarAIControlTerm2> ();
    +                if (otherAI != null) {
    +                    // we'll take evasive action for 1 second
    +                    m_AvoidOtherCarTime = Time.time + 1;
    +
    +                    // but who's in front?...
    +                    if (Vector3.Angle (transform.forward, otherAI.transform.position - transform.position) < 90) {
    +                        // the other ai is in front, so it is only good manners that we ought to brake...
    +                        m_AvoidOtherCarSlowdown = 0.5f;
    +                    } else {
    +                        // we're in front! ain't slowing down for anybody...
    +                        m_AvoidOtherCarSlowdown = 1;
    +                    }
    +
    +                    // both cars should take evasive action by driving along an offset from the path centre,
    +                    // away from the other car
    +                    var otherCarLocalDelta = transform.InverseTransformPoint (otherAI.transform.position);
    +                    float otherCarAngle = Mathf.Atan2 (otherCarLocalDelta.x, otherCarLocalDelta.z);
    +                    m_AvoidPathOffset = m_LateralWanderDistance * -Mathf.Sign (otherCarAngle);
    +                }
    +            }
    +        }
    +
    +
    +        public void SetTarget (Transform target)
    +        {
    +            m_Target = target;
    +            m_Driving = true;
    +        }
    +    }
    +}

### modified:   Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs:

    ubuntu@DESKTOP-B7FMDT9:/mnt/c/src/self-driving-car-sim$ git diff "Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs"
    diff --git a/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs b/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs
    index 0040157..81097f9 100644
    --- a/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs
    +++ b/Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarControllerTerm2.cs
    @@ -1,753 +1,622 @@
    -using System;
    -using System.Collections;
    -using System.IO;
    -using UnityEngine;
    -using UnityEngine.EventSystems;
    -using System.Collections.Generic;
    -
    -
    -
    -namespace UnityStandardAssets.Vehicles.Car
    -{
    -    internal enum CarDriveTypeTerm2
    -    {
    -        FrontWheelDrive,
    -        RearWheelDrive,
    -        FourWheelDrive
    -    }
    -
    -    internal enum SpeedTypeTerm2
    -    {
    -        MPH,
    -        KPH
    -    }
    -
    -    public class CarControllerTerm2 : MonoBehaviour
    -    {
    -        [SerializeField] private CarDriveTypeTerm2 m_CarDriveType = CarDriveTypeTerm2.FourWheelDrive;
    -        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    -        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
    -        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
    -        [SerializeField] private Vector3 m_CentreOfMassOffset;
    -        [SerializeField] private float m_MaximumSteerAngle;
    -        [Range (0, 1)] [SerializeField] private float m_SteerHelper;
    -        // 0 is raw physics , 1 the car will grip in the direction it is facing
    -        [Range (0, 1)] [SerializeField] private float m_TractionControl;
    -        // 0 is no traction control, 1 is full interference
    -        [SerializeField] private float m_FullTorqueOverAllWheels;
    -        [SerializeField] private float m_ReverseTorque;
    -        [SerializeField] private float m_MaxHandbrakeTorque;
    -        [SerializeField] private float m_Downforce = 100f;
    -        [SerializeField] private SpeedTypeTerm2 m_SpeedType;
    -        [SerializeField] private float m_Topspeed = 200;
    -        [SerializeField] private static int NoOfGears = 5;
    -        [SerializeField] private float m_RevRangeBoundary = 1f;
    -        [SerializeField] private float m_SlipLimit;
    -        [SerializeField] private float m_BrakeTorque;
    -
    -        public const string CSVFileName = "driving_log.csv";
    -        public const string DirFrames = "IMG";
    -
    -        [SerializeField] private Camera CenterCamera;
    -        [SerializeField] private Camera LeftCamera;
    -        [SerializeField] private Camera RightCamera;
    -
    -        private Quaternion[] m_WheelMeshLocalRotations;
    -        private Vector3 m_Prevpos, m_Pos;
    -        private float m_SteerAngle;
    -        private int m_GearNum;
    -        private float m_GearFactor;
    -        private float m_OldRotation;
    -        private float m_CurrentTorque;
    -        private Rigidbody m_Rigidbody;
    -        private const float k_ReversingThreshold = 0.01f;
    -        private string m_saveLocation = "";
    -        private Queue<CarSampleTerm2> carSamples;
    -               private int TotalSamples;
    -               private bool isSaving;
    -               private Vector3 saved_position;
    -               private Quaternion saved_rotation;
    -
    -        public bool Skidding { get; private set; }
    -
    -        public float BrakeInput { get; private set; }
    -
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
    -        }
    -
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
    -
    -        // Use this for initialization
    -        private void Start ()
    -        {
    -            m_WheelMeshLocalRotations = new Quaternion[4];
    -            for (int i = 0; i < 4; i++) {
    -                m_WheelMeshLocalRotations [i] = m_WheelMeshes [i].transform.localRotation;
    -            }
    -            m_WheelColliders [0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
    -
    -            m_MaxHandbrakeTorque = float.MaxValue;
    -
    -            m_Rigidbody = GetComponent<Rigidbody> ();
    -            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);
    -
    -                       lastSpeed = 0;
    -                       lastAcc = 0;
    -                       Jerk = 0;
    -                       AccelerationT = 0;
    -                       AccelerationN = 0;
    -        }
    -
    -        private void GearChanging ()
    -        {
    -            float f = Mathf.Abs (CurrentSpeed / MaxSpeed);
    -            float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
    -            float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;
    -
    -            if (m_GearNum > 0 && f < downgearlimit) {
    -                m_GearNum--;
    -            }
    -
    -            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1))) {
    -                m_GearNum++;
    -            }
    -        }
    -
    -
    -        // simple function to add a curved bias towards 1 for a value in the 0-1 range
    -        private static float CurveFactor (float factor)
    -        {
    -            return 1 - (1 - factor) * (1 - factor);
    -        }
    -
    -
    -        // unclamped version of Lerp, to allow value to exceed the from-to range
    -        private static float ULerp (float from, float to, float value)
    -        {
    -            return (1.0f - value) * from + value * to;
    -        }
    -
    -
    -        private void CalculateGearFactor ()
    -        {
    -            float f = (1 / (float)NoOfGears);
    -            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
    -            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
    -            var targetGearFactor = Mathf.InverseLerp (f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs (CurrentSpeed / MaxSpeed));
    -            m_GearFactor = Mathf.Lerp (m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
    -        }
    -
    -
    -        private void CalculateRevs ()
    -        {
    -            // calculate engine revs (for display / sound)
    -            // (this is done in retrospect - revs are not used in force/power calculations)
    -            CalculateGearFactor ();
    -            var gearNumFactor = m_GearNum / (float)NoOfGears;
    -            var revsRangeMin = ULerp (0f, m_RevRangeBoundary, CurveFactor (gearNumFactor));
    -            var revsRangeMax = ULerp (m_RevRangeBoundary, 1f, gearNumFactor);
    -            Revs = ULerp (revsRangeMin, revsRangeMax, m_GearFactor);
    -        }
    -
    -        public void FixedUpdate()
    -        {
    -                       if (main_car)
    -                       {
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
    -                       }
    -        }
    -
    -
    -        public void Update()
    -        {
    -            if (IsRecording)
    -            {
    -                //Dump();
    -            }
    -        }
    -
    -               public float AverageLastSpeed()
    -               {
    -
    -                       float averaged_speed = 0.0f;
    -
    -                       for (int i = 0; i < averageSpeed.Count; i++)
    -                       {
    -                               averaged_speed += averageSpeed[i];
    -                       }
    -
    -                       return averaged_speed / (float)(averageSpeed.Count);
    -
    -               }
    -               public float AverageLastAcc()
    -               {
    -
    -                       float averaged_acc = 0.0f;
    -
    -                       for (int i = 0; i < averageAcc.Count; i++)
    -                       {
    -                               averaged_acc += averageAcc[i];
    -                       }
    -
    -                       return averaged_acc / (float)(averageAcc.Count);
    -
    -               }
    -               public float SenseCurve()
    -               {
    -                       float averaged_curve = 0.0f;
    -
    -                       for (int i = 0; i < previous_pos.Count-2; i++)
    -                       {
    -
    -
    -                               float x1 = previous_pos [i].x;
    -                               float x2 = previous_pos [i + 1].x;
    -                               float x3 = previous_pos [i + 2].x;
    -
    -                               float y1 = previous_pos [i].z;
    -                               float y2 = previous_pos [i + 1].z;
    -                               float y3 = previous_pos [i + 2].z;
    -
    -                               Vector2 ray1 = new Vector2 (x2 - x1, y2 - y1);
    -                               Vector2 ray2 = new Vector2 (x3 - x2, y3 - y2);
    -
    -                               if (ray1.magnitude != 0 && ray2.magnitude != 0)
    -                               {
    -
    -                                       Vector2 ray3 = new Vector2 (x3 - x1, y3 - y1);
    -
    -                                       float corner_angle = Mathf.Abs (Vector2.Angle (ray1, ray2));
    -
    -                                       if (ray3.magnitude != 0 && corner_angle != 180)
    -                                       {
    -                                               averaged_curve += 2 * Mathf.Sin (corner_angle*Mathf.Deg2Rad) / ray3.magnitude;
    -                                       }
    -                                       else
    -                                       {
    -
    -                                               //the curve is infinite, this move is totally illegal, just going to return 1000000
    -                                               averaged_curve += 1000000;
    -                                       }
    -
    -                               }
    -                               //else skip and just say that curve is zero since its stopped
    -                       }
    -
    -                       return averaged_curve / ( (float)(previous_pos.Count-2));
    -
    -               }
    -
    -        public void Move (float steering, float accel, float footbrake, float handbrake)
    -        {
    -            for (int i = 0; i < 4; i++) {
    -                Quaternion quat;
    -                Vector3 position;
    -                m_WheelColliders [i].GetWorldPose (out position, out quat);
    -                m_WheelMeshes [i].transform.position = position;
    -                m_WheelMeshes [i].transform.rotation = quat;
    -            }
    -
    -            //clamp input values
    -            steering = Mathf.Clamp (steering, -1, 1);
    -            AccelInput = accel = Mathf.Clamp (accel, 0, 1);
    -            BrakeInput = footbrake = -1 * Mathf.Clamp (footbrake, -1, 0);
    -            handbrake = Mathf.Clamp (handbrake, 0, 1);
    -
    -
    -            //Set the steer on the front wheels.
    -            //Assuming that wheels 0 and 1 are the front wheels.
    -            m_SteerAngle = steering * m_MaximumSteerAngle;
    -            m_WheelColliders [0].steerAngle = m_SteerAngle;
    -            m_WheelColliders [1].steerAngle = m_SteerAngle;
    -
    -
    -
    -            SteerHelper ();
    -            ApplyDrive (accel, footbrake);
    -            CapSpeed ();
    -
    -            //Set the handbrake.
    -            //Assuming that wheels 2 and 3 are the rear wheels.
    -            if (handbrake > 0f)
    -            {
    -                var hbTorque = handbrake * m_MaxHandbrakeTorque;
    -                m_WheelColliders [2].brakeTorque = hbTorque;
    -                m_WheelColliders [3].brakeTorque = hbTorque;
    -            }
    -
    -            CalculateRevs ();
    -            GearChanging ();
    -
    -            AddDownForce ();
    -            CheckForWheelSpin ();
    -            TractionControl ();
    -        }
    -
    -
    -        private void CapSpeed ()
    -        {
    -            float speed = m_Rigidbody.velocity.magnitude;
    -            switch (m_SpeedType) {
    -            case SpeedTypeTerm2.MPH:
    -
    -                speed *= 2.23693629f;
    -                if (speed > m_Topspeed)
    -                    m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
    -                break;
    -
    -            case SpeedTypeTerm2.KPH:
    -                speed *= 3.6f;
    -                if (speed > m_Topspeed)
    -                    m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
    -                break;
    -            }
    -        }
    -
    -
    -        private void ApplyDrive (float accel, float footbrake)
    -        {
    -
    -                       for (int i = 0; i < 4; i++)
    -                       {
    -                               m_WheelColliders [i].motorTorque = 0f;
    -                               m_WheelColliders [i].brakeTorque = 0f;
    -                       }
    -
    -            float thrustTorque;
    -            switch (m_CarDriveType) {
    -            case CarDriveTypeTerm2.FourWheelDrive:
    -                thrustTorque = accel * (m_CurrentTorque / 4f);
    -                for (int i = 0; i < 4; i++) {
    -                    m_WheelColliders [i].motorTorque = thrustTorque;
    -                }
    -                break;
    -
    -            case CarDriveTypeTerm2.FrontWheelDrive:
    -                thrustTorque = accel * (m_CurrentTorque / 2f);
    -                m_WheelColliders [0].motorTorque = m_WheelColliders [1].motorTorque = thrustTorque;
    -                break;
    -
    -            case CarDriveTypeTerm2.RearWheelDrive:
    -                thrustTorque = accel * (m_CurrentTorque / 2f);
    -                m_WheelColliders [2].motorTorque = m_WheelColliders [3].motorTorque = thrustTorque;
    -                break;
    -
    -            }
    -
    -            for (int i = 0; i < 4; i++) {
    -                if (CurrentSpeed > 5 && Vector3.Angle (transform.forward, m_Rigidbody.velocity) < 50f) {
    -                    m_WheelColliders [i].brakeTorque = m_BrakeTorque * footbrake;
    -                } else if (footbrake > 0) {
    -                    m_WheelColliders [i].brakeTorque = 0f;
    -                    m_WheelColliders [i].motorTorque = -m_ReverseTorque * footbrake;
    -                }
    -            }
    -        }
    -
    -
    -        private void SteerHelper ()
    -        {
    -            for (int i = 0; i < 4; i++) {
    -                WheelHit wheelhit;
    -                m_WheelColliders [i].GetGroundHit (out wheelhit);
    -                if (wheelhit.normal == Vector3.zero)
    -                    return; // wheels arent on the ground so dont realign the rigidbody velocity
    -            }
    -
    -            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
    -            if (Mathf.Abs (m_OldRotation - transform.eulerAngles.y) < 10f) {
    -                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
    -                Quaternion velRotation = Quaternion.AngleAxis (turnadjust, Vector3.up);
    -                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
    -            }
    -            m_OldRotation = transform.eulerAngles.y;
    -        }
    -
    -
    -        // this is used to add more grip in relation to speed
    -        private void AddDownForce ()
    -        {
    -            m_WheelColliders [0].attachedRigidbody.AddForce (-transform.up * m_Downforce *
    -            m_WheelColliders [0].attachedRigidbody.velocity.magnitude);
    -        }
    -
    -
    -        // checks if the wheels are spinning and is so does three things
    -        // 1) emits particles
    -        // 2) plays tiure skidding sounds
    -        // 3) leaves skidmarks on the ground
    -        // these effects are controlled through the WheelEffects class
    -        private void CheckForWheelSpin ()
    -        {
    -            // loop through all wheels
    -            for (int i = 0; i < 4; i++) {
    -                WheelHit wheelHit;
    -                m_WheelColliders [i].GetGroundHit (out wheelHit);
    -
    -                // is the tire slipping above the given threshhold
    -                if (Mathf.Abs (wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs (wheelHit.sidewaysSlip) >= m_SlipLimit) {
    -                    m_WheelEffects [i].EmitTyreSmoke ();
    -                    continue;
    -                }
    -
    -                // if it wasnt slipping stop all the audio
    -                if (m_WheelEffects [i].PlayingAudio) {
    -                    m_WheelEffects [i].StopAudio ();
    -                }
    -                // end the trail generation
    -                m_WheelEffects [i].EndSkidTrail ();
    -            }
    -        }
    -
    -        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    -        private void TractionControl ()
    -        {
    -            WheelHit wheelHit;
    -            switch (m_CarDriveType) {
    -            case CarDriveTypeTerm2.FourWheelDrive:
    -                    // loop through all wheels
    -                for (int i = 0; i < 4; i++) {
    -                    m_WheelColliders [i].GetGroundHit (out wheelHit);
    -
    -                    AdjustTorque (wheelHit.forwardSlip);
    -                }
    -                break;
    -
    -            case CarDriveTypeTerm2.RearWheelDrive:
    -                m_WheelColliders [2].GetGroundHit (out wheelHit);
    -                AdjustTorque (wheelHit.forwardSlip);
    -
    -                m_WheelColliders [3].GetGroundHit (out wheelHit);
    -                AdjustTorque (wheelHit.forwardSlip);
    -                break;
    -
    -            case CarDriveTypeTerm2.FrontWheelDrive:
    -                m_WheelColliders [0].GetGroundHit (out wheelHit);
    -                AdjustTorque (wheelHit.forwardSlip);
    -
    -                m_WheelColliders [1].GetGroundHit (out wheelHit);
    -                AdjustTorque (wheelHit.forwardSlip);
    -                break;
    -            }
    -        }
    -
    -
    -        private void AdjustTorque (float forwardSlip)
    -        {
    -            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0) {
    -                m_CurrentTorque -= 10 * m_TractionControl;
    -            } else {
    -                m_CurrentTorque += 10 * m_TractionControl;
    -                if (m_CurrentTorque > m_FullTorqueOverAllWheels) {
    -                    m_CurrentTorque = m_FullTorqueOverAllWheels;
    -                }
    -            }
    -        }
    -
    -
    -               //Changed the WriteSamplesToDisk to a IEnumerator method that plays back recording along with percent status from UISystem script
    -               //instead of showing frozen screen until all data is recorded
    -               public IEnumerator WriteSamplesToDisk()
    -               {
    -                       yield return new WaitForSeconds(0.000f); //retrieve as fast as we can but still allow communication of main thread to screen and UISystem
    -                       if (carSamples.Count > 0) {
    -                               //pull off a sample from the que
    -                               CarSampleTerm2 sample = carSamples.Dequeue();
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
    -                CarSampleTerm2 sample = new CarSampleTerm2();
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
    -    internal class CarSampleTerm2
    -    {
    -        public Quaternion rotation;
    -        public Vector3 position;
    -        public float steeringAngle;
    -        public float throttle;
    -        public float brake;
    -        public float speed;
    -        public string timeStamp;
    -    }
    -
    -}
    +using System;
    +using System.Collections;
    +using System.IO;
    +using UnityEngine;
    +using UnityEngine.EventSystems;
    +using System.Collections.Generic;
    +
    +
    +
    +namespace UnityStandardAssets.Vehicles.Car
    +{
    +    internal enum CarDriveTypeTerm2
    +    {
    +        FrontWheelDrive,
    +        RearWheelDrive,
    +        FourWheelDrive
    +    }
    +
    +    internal enum SpeedTypeTerm2
    +    {
    +        MPH,
    +        KPH
    +    }
    +
    +    public class CarControllerTerm2 : CarControllerBase
    +    {
    +        [SerializeField] private CarDriveTypeTerm2 m_CarDriveType = CarDriveTypeTerm2.FourWheelDrive;
    +        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    +        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
    +        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
    +        [SerializeField] private Vector3 m_CentreOfMassOffset;
    +        [SerializeField] private float m_MaximumSteerAngle;
    +        [Range (0, 1)] [SerializeField] private float m_SteerHelper;
    +        // 0 is raw physics , 1 the car will grip in the direction it is facing
    +        [Range (0, 1)] [SerializeField] private float m_TractionControl;
    +        // 0 is no traction control, 1 is full interference
    +        [SerializeField] private float m_FullTorqueOverAllWheels;
    +        [SerializeField] private float m_ReverseTorque;
    +        [SerializeField] private float m_MaxHandbrakeTorque;
    +        [SerializeField] private float m_Downforce = 100f;
    +        [SerializeField] private SpeedTypeTerm2 m_SpeedType;
    +        [SerializeField] private float m_Topspeed = 200;
    +        [SerializeField] private static int NoOfGears = 5;
    +        [SerializeField] private float m_RevRangeBoundary = 1f;
    +        [SerializeField] private float m_SlipLimit;
    +        [SerializeField] private float m_BrakeTorque;
    +
    +        public const string CSVFileName = "driving_log.csv";
    +        public const string DirFrames = "IMG";
    +
    +        [SerializeField] private Camera CenterCamera;
    +        [SerializeField] private Camera LeftCamera;
    +        [SerializeField] private Camera RightCamera;
    +
    +        private Quaternion[] m_WheelMeshLocalRotations;
    +        private Vector3 m_Prevpos, m_Pos;
    +        private int m_GearNum;
    +        private float m_GearFactor;
    +        private float m_OldRotation;
    +        private float m_CurrentTorque;
    +        private const float k_ReversingThreshold = 0.01f;
    +        private string m_saveLocation = "";
    +        private Queue<CarSampleTerm2> carSamples;
    +               private int TotalSamples;
    +               private bool isSaving;
    +               private Vector3 saved_position;
    +               private Quaternion saved_rotation;
    +
    +        public bool Skidding { get; private set; }
    +
    +        public float BrakeInput { get; private set; }
    +
    +        public float MaxSpeed
    +        {
    +            get { return m_Topspeed; }
    +        }
    +
    +        public List<float> getSensors()
    +        {
    +                return sensor_values;
    +        }
    +
    +        private bool m_isRecording = false;
    +
    +        public bool IsRecording {
    +            get
    +            {
    +                return m_isRecording;
    +            }
    +
    +            set
    +            {
    +                m_isRecording = value;
    +                if(value == true)
    +                {
    +                    Debug.Log("Starting to record");
    +                    carSamples = new Queue<CarSampleTerm2>();
    +                    StartCoroutine(Sample());
    +                }
    +                                else
    +                {
    +                    Debug.Log("Stopping record");
    +                    StopCoroutine(Sample());
    +                    Debug.Log("Writing to disk");
    +                    //save the cars coordinate parameters so we can reset it to this properly after capturing data
    +                    saved_position = transform.position;
    +                    saved_rotation = transform.rotation;
    +                    //see how many samples we captured use this to show save percentage in UISystem script
    +                    TotalSamples = carSamples.Count;
    +                    isSaving = true;
    +                    StartCoroutine(WriteSamplesToDisk());
    +                };
    +            }
    +
    +        }
    +
    +
    +        public bool checkSaveLocation()
    +        {
    +            if (m_saveLocation != "")
    +            {
    +                return true;
    +            }
    +            else
    +            {
    +                SimpleFileBrowser.ShowSaveDialog (OpenFolder, null, true, null, "Select Output Folder", "Select");
    +            }
    +            return false;
    +        }
    +
    +
    +        // Use this for initialization
    +        private void Start ()
    +        {
    +            m_WheelMeshLocalRotations = new Quaternion[4];
    +            for (int i = 0; i < 4; i++) {
    +                m_WheelMeshLocalRotations [i] = m_WheelMeshes [i].transform.localRotation;
    +            }
    +            m_WheelColliders [0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
    +
    +            m_MaxHandbrakeTorque = float.MaxValue;
    +
    +            // Initialize shared base state
    +            InitControllerBase();
    +
    +            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);
    +
    +                       lastSpeed = 0;
    +                       lastAcc = 0;
    +                       Jerk = 0;
    +                       AccelerationT = 0;
    +                       AccelerationN = 0;
    +        }
    +
    +        private void GearChanging ()
    +        {
    +            float f = Mathf.Abs (CurrentSpeed / MaxSpeed);
    +            float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
    +            float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;
    +
    +            if (m_GearNum > 0 && f < downgearlimit) {
    +                m_GearNum--;
    +            }
    +
    +            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1))) {
    +                m_GearNum++;
    +            }
    +        }
    +
    +
    +        // simple function to add a curved bias towards 1 for a value in the 0-1 range
    +        private static float CurveFactor (float factor)
    +        {
    +            return 1 - (1 - factor) * (1 - factor);
    +        }
    +
    +
    +        // unclamped version of Lerp, to allow value to exceed the from-to range
    +        private static float ULerp (float from, float to, float value)
    +        {
    +            return (1.0f - value) * from + value * to;
    +        }
    +
    +
    +        private void CalculateGearFactor ()
    +        {
    +            float f = (1 / (float)NoOfGears);
    +            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
    +            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
    +            var targetGearFactor = Mathf.InverseLerp (f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs (CurrentSpeed / MaxSpeed));
    +            m_GearFactor = Mathf.Lerp (m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
    +        }
    +
    +
    +        private void CalculateRevs ()
    +        {
    +            // calculate engine revs (for display / sound)
    +            // (this is done in retrospect - revs are not used in force/power calculations)
    +            CalculateGearFactor ();
    +            var gearNumFactor = m_GearNum / (float)NoOfGears;
    +            var revsRangeMin = ULerp (0f, m_RevRangeBoundary, CurveFactor (gearNumFactor));
    +            var revsRangeMax = ULerp (m_RevRangeBoundary, 1f, gearNumFactor);
    +            Revs = ULerp (revsRangeMin, revsRangeMax, m_GearFactor);
    +        }
    +
    +        public void FixedUpdate()
    +        {
    +                       if (main_car)
    +                       {
    +                               // Replace duplicated motion-sensing with base helper
    +                               UpdateMainCarMotionSensing();
    +                       }
    +        }
    +
    +
    +        public void Update()
    +        {
    +            if (IsRecording)
    +            {
    +                //Dump();
    +            }
    +        }
    +
    +        public float AverageLastSpeed()
    +               {
    +
    +                       float averaged_speed = 0.0f;
    +
    +                       for (int i = 0; i < averageSpeed.Count; i++)
    +                       {
    +                               averaged_speed += averageSpeed[i];
    +                       }
    +
    +                       return averaged_speed / (float)(averageSpeed.Count);
    +
    +               }
    +               public float AverageLastAcc()
    +               {
    +
    +                       float averaged_acc = 0.0f;
    +
    +                       for (int i = 0; i < averageAcc.Count; i++)
    +                       {
    +                               averaged_acc += averageAcc[i];
    +                       }
    +
    +                       return averaged_acc / (float)(averageAcc.Count);
    +
    +               }
    +               public float SenseCurve()
    +               {
    +                       float averaged_curve = 0.0f;
    +
    +                       for (int i = 0; i < previous_pos.Count-2; i++)
    +                       {
    +
    +
    +                               float x1 = previous_pos [i].x;
    +                               float x2 = previous_pos [i + 1].x;
    +                               float x3 = previous_pos [i + 2].x;
    +
    +                               float y1 = previous_pos [i].z;
    +                               float y2 = previous_pos [i + 1].z;
    +                               float y3 = previous_pos [i + 2].z;
    +
    +                               Vector2 ray1 = new Vector2 (x2 - x1, y2 - y1);
    +                               Vector2 ray2 = new Vector2 (x3 - x2, y3 - y2);
    +
    +                               if (ray1.magnitude != 0 && ray2.magnitude != 0)
    +                               {
    +
    +                                       Vector2 ray3 = new Vector2 (x3 - x1, y3 - y1);
    +
    +                                       float corner_angle = Mathf.Abs (Vector2.Angle (ray1, ray2));
    +
    +                                       if (ray3.magnitude != 0 && corner_angle != 180)
    +                                       {
    +                                               averaged_curve += 2 * Mathf.Sin (corner_angle*Mathf.Deg2Rad) / ray3.magnitude;
    +                                       }
    +                                       else
    +                                       {
    +
    +                                               //the curve is infinite, this move is totally illegal, just going to return 1000000
    +                                               averaged_curve += 1000000;
    +                                       }
    +
    +                               }
    +                               //else skip and just say that curve is zero since its stopped
    +                       }
    +
    +                       return averaged_curve / ( (float)(previous_pos.Count-2));
    +
    +               }
    +
    +        public void Move (float steering, float accel, float footbrake, float handbrake)
    +        {
    +            for (int i = 0; i < 4; i++) {
    +                Quaternion quat;
    +                Vector3 position;
    +                m_WheelColliders [i].GetWorldPose (out position, out quat);
    +                m_WheelMeshes [i].transform.position = position;
    +                m_WheelMeshes [i].transform.rotation = quat;
    +            }
    +
    +            //clamp input values
    +            steering = Mathf.Clamp (steering, -1, 1);
    +            AccelInput = accel = Mathf.Clamp (accel, 0, 1);
    +            BrakeInput = footbrake = -1 * Mathf.Clamp (footbrake, -1, 0);
    +            handbrake = Mathf.Clamp (handbrake, 0, 1);
    +
    +
    +            //Set the steer on the front wheels.
    +            //Assuming that wheels 0 and 1 are the front wheels.
    +            m_SteerAngle = steering * m_MaximumSteerAngle;
    +            m_WheelColliders [0].steerAngle = m_SteerAngle;
    +            m_WheelColliders [1].steerAngle = m_SteerAngle;
    +
    +
    +
    +            SteerHelper ();
    +            ApplyDrive (accel, footbrake);
    +            CapSpeed ();
    +
    +            //Set the handbrake.
    +            //Assuming that wheels 2 and 3 are the rear wheels.
    +            if (handbrake > 0f)
    +            {
    +                var hbTorque = handbrake * m_MaxHandbrakeTorque;
    +                m_WheelColliders [2].brakeTorque = hbTorque;
    +                m_WheelColliders [3].brakeTorque = hbTorque;
    +            }
    +
    +            CalculateRevs ();
    +            GearChanging ();
    +
    +            AddDownForce ();
    +            CheckForWheelSpin ();
    +            TractionControl ();
    +        }
    +
    +
    +        private void CapSpeed ()
    +        {
    +            float speed = m_Rigidbody.velocity.magnitude;
    +            switch (m_SpeedType) {
    +            case SpeedTypeTerm2.MPH:
    +
    +                speed *= 2.23693629f;
    +                if (speed > m_Topspeed)
    +                    m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
    +                break;
    +
    +            case SpeedTypeTerm2.KPH:
    +                speed *= 3.6f;
    +                if (speed > m_Topspeed)
    +                    m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
    +                break;
    +            }
    +        }
    +
    +
    +        private void ApplyDrive (float accel, float footbrake)
    +        {
    +
    +                       for (int i = 0; i < 4; i++)
    +                       {
    +                               m_WheelColliders [i].motorTorque = 0f;
    +                               m_WheelColliders [i].brakeTorque = 0f;
    +                       }
    +
    +            float thrustTorque;
    +            switch (m_CarDriveType) {
    +            case CarDriveTypeTerm2.FourWheelDrive:
    +                thrustTorque = accel * (m_CurrentTorque / 4f);
    +                for (int i = 0; i < 4; i++) {
    +                    m_WheelColliders [i].motorTorque = thrustTorque;
    +                }
    +                break;
    +
    +            case CarDriveTypeTerm2.FrontWheelDrive:
    +                thrustTorque = accel * (m_CurrentTorque / 2f);
    +                m_WheelColliders [0].motorTorque = m_WheelColliders [1].motorTorque = thrustTorque;
    +                break;
    +
    +            case CarDriveTypeTerm2.RearWheelDrive:
    +                thrustTorque = accel * (m_CurrentTorque / 2f);
    +                m_WheelColliders [2].motorTorque = m_WheelColliders [3].motorTorque = thrustTorque;
    +                break;
    +
    +            }
    +
    +            for (int i = 0; i < 4; i++) {
    +                if (CurrentSpeed > 5 && Vector3.Angle (transform.forward, m_Rigidbody.velocity) < 50f) {
    +                    m_WheelColliders [i].brakeTorque = m_BrakeTorque * footbrake;
    +                } else if (footbrake > 0) {
    +                    m_WheelColliders [i].brakeTorque = 0f;
    +                    m_WheelColliders [i].motorTorque = -m_ReverseTorque * footbrake;
    +                }
    +            }
    +        }
    +
    +
    +        private void SteerHelper ()
    +        {
    +            for (int i = 0; i < 4; i++) {
    +                WheelHit wheelhit;
    +                m_WheelColliders [i].GetGroundHit (out wheelhit);
    +                if (wheelhit.normal == Vector3.zero)
    +                    return; // wheels arent on the ground so dont realign the rigidbody velocity
    +            }
    +
    +            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
    +            if (Mathf.Abs (m_OldRotation - transform.eulerAngles.y) < 10f) {
    +                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
    +                Quaternion velRotation = Quaternion.AngleAxis (turnadjust, Vector3.up);
    +                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
    +            }
    +            m_OldRotation = transform.eulerAngles.y;
    +        }
    +
    +
    +        // this is used to add more grip in relation to speed
    +        private void AddDownForce ()
    +        {
    +            m_WheelColliders [0].attachedRigidbody.AddForce (-transform.up * m_Downforce *
    +            m_WheelColliders [0].attachedRigidbody.velocity.magnitude);
    +        }
    +
    +
    +        // checks if the wheels are spinning and is so does three things
    +        // 1) emits particles
    +        // 2) plays tiure skidding sounds
    +        // 3) leaves skidmarks on the ground
    +        // these effects are controlled through the WheelEffects class
    +        private void CheckForWheelSpin ()
    +        {
    +            // loop through all wheels
    +            for (int i = 0; i < 4; i++) {
    +                WheelHit wheelHit;
    +                m_WheelColliders [i].GetGroundHit (out wheelHit);
    +
    +                // is the tire slipping above the given threshhold
    +                if (Mathf.Abs (wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs (wheelHit.sidewaysSlip) >= m_SlipLimit) {
    +                    m_WheelEffects [i].EmitTyreSmoke ();
    +                    continue;
    +                }
    +
    +                // if it wasnt slipping stop all the audio
    +                if (m_WheelEffects [i].PlayingAudio) {
    +                    m_WheelEffects [i].StopAudio ();
    +                }
    +                // end the trail generation
    +                m_WheelEffects [i].EndSkidTrail ();
    +            }
    +        }
    +
    +        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    +        private void TractionControl ()
    +        {
    +            WheelHit wheelHit;
    +            switch (m_CarDriveType) {
    +            case CarDriveTypeTerm2.FourWheelDrive:
    +                    // loop through all wheels
    +                for (int i = 0; i < 4; i++) {
    +                    m_WheelColliders [i].GetGroundHit (out wheelHit);
    +
    +                    AdjustTorque (wheelHit.forwardSlip);
    +                }
    +                break;
    +
    +            case CarDriveTypeTerm2.RearWheelDrive:
    +                m_WheelColliders [2].GetGroundHit (out wheelHit);
    +                AdjustTorque (wheelHit.forwardSlip);
    +
    +                m_WheelColliders [3].GetGroundHit (out wheelHit);
    +                AdjustTorque (wheelHit.forwardSlip);
    +                break;
    +
    +            case CarDriveTypeTerm2.FrontWheelDrive:
    +                m_WheelColliders [0].GetGroundHit (out wheelHit);
    +                AdjustTorque (wheelHit.forwardSlip);
    +
    +                m_WheelColliders [1].GetGroundHit (out wheelHit);
    +                AdjustTorque (wheelHit.forwardSlip);
    +                break;
    +            }
    +        }
    +
    +
    +        private void AdjustTorque (float forwardSlip)
    +        {
    +            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0) {
    +                m_CurrentTorque -= 10 * m_TractionControl;
    +            } else {
    +                m_CurrentTorque += 10 * m_TractionControl;
    +                if (m_CurrentTorque > m_FullTorqueOverAllWheels) {
    +                    m_CurrentTorque = m_FullTorqueOverAllWheels;
    +                }
    +            }
    +        }
    +
    +
    +               //Changed the WriteSamplesToDisk to a IEnumerator method that plays back recording along with percent status from UISystem script
    +               //instead of showing frozen screen until all data is recorded
    +               public IEnumerator WriteSamplesToDisk()
    +               {
    +                       yield return new WaitForSeconds(0.000f); //retrieve as fast as we can but still allow communication of main thread to screen and UISystem
    +                       if (carSamples.Count > 0) {
    +                               //pull off a sample from the que
    +                               CarSampleTerm2 sample = carSamples.Dequeue();
    +
    +                               //pysically moving the car to get the right camera position
    +                               transform.position = sample.position;
    +                               transform.rotation = sample.rotation;
    +
    +                               // Capture and Persist Image
    +                               string centerPath = WriteImage (CenterCamera, "center", sample.timeStamp);
    +                               string leftPath = WriteImage (LeftCamera, "left", sample.timeStamp);
    +                               string rightPath = WriteImage (RightCamera, "right", sample.timeStamp);
    +
    +                               string row = string.Format ("{0},{1},{2},{3},{4},{5},{6}\n", centerPath, leftPath, rightPath, sample.steeringAngle, sample.throttle, sample.brake, sample.speed);
    +                               File.AppendAllText (Path.Combine (m_saveLocation, CSVFileName), row);
    +                       }
    +                       if (carSamples.Count > 0) {
    +                               //request if there are more samples to pull
    +                               StartCoroutine(WriteSamplesToDisk());
    +                       }
    +                       else
    +                       {
    +                               //all samples have been pulled
    +                               StopCoroutine(WriteSamplesToDisk());
    +                               isSaving = false;
    +
    +                               //need to reset the car back to its position before ending recording, otherwise sometimes the car ended up in strange areas
    +                               transform.position = saved_position;
    +                               transform.rotation = saved_rotation;
    +                               m_Rigidbody.velocity = new Vector3(0f,-10f,0f);
    +                               Move(0f, 0f, 0f, 0f);
    +
    +                       }
    +               }
    +
    +               public float getSavePercent()
    +               {
    +                       return (float)(TotalSamples-carSamples.Count)/TotalSamples;
    +               }
    +
    +               public bool getSaveStatus()
    +               {
    +                       return isSaving;
    +               }
    +
    +
    +        public IEnumerator Sample()
    +        {
    +            // Start the Coroutine to Capture Data Every Second.
    +            // Persist that Information to a CSV and Perist the Camera Frame
    +            yield return new WaitForSeconds(0.0666666666666667f);
    +
    +            if (m_saveLocation != "")
    +            {
    +                CarSampleTerm2 sample = new CarSampleTerm2();
    +
    +                sample.timeStamp = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
    +                sample.steeringAngle = m_SteerAngle / m_MaximumSteerAngle;
    +                sample.throttle = AccelInput;
    +                sample.brake = BrakeInput;
    +                sample.speed = CurrentSpeed;
    +                sample.position = transform.position;
    +                sample.rotation = transform.rotation;
    +
    +                carSamples.Enqueue(sample);
    +
    +                sample = null;
    +                //may or may not be needed
    +            }
    +
    +            // Only reschedule if the button hasn't toggled
    +            if (IsRecording)
    +            {
    +                StartCoroutine(Sample());
    +            }
    +
    +        }
    +
    +        private void OpenFolder(string location)
    +        {
    +            m_saveLocation = location;
    +            Directory.CreateDirectory (Path.Combine(m_saveLocation, DirFrames));
    +        }
    +
    +        private string WriteImage (Camera camera, string prepend, string timestamp)
    +        {
    +            //needed to force camera update
    +            camera.Render();
    +            RenderTexture targetTexture = camera.targetTexture;
    +            RenderTexture.active = targetTexture;
    +            Texture2D texture2D = new Texture2D (targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
    +            texture2D.ReadPixels (new Rect (0, 0, targetTexture.width, targetTexture.height), 0, 0);
    +            texture2D.Apply ();
    +            byte[] image = texture2D.EncodeToJPG ();
    +            UnityEngine.Object.DestroyImmediate (texture2D);
    +            string directory = Path.Combine(m_saveLocation, DirFrames);
    +            string path = Path.Combine(directory, prepend + "_" + timestamp + ".jpg");
    +            File.WriteAllBytes (path, image);
    +            image = null;
    +            return path;
    +        }
    +    }
    +
    +    internal class CarSampleTerm2
    +    {
    +        public Quaternion rotation;
    +        public Vector3 position;
    +        public float steeringAngle;
    +        public float throttle;
    +        public float brake;
    +        public float speed;
    +        public string timeStamp;
    +    }
    +
    +}



## Untracked files:

Assets/1_SelfDrivingCar/Term3_Scenes/ControlMenuTerm3.unity
Assets/1_SelfDrivingCar/Term3_Scenes/ControlMenuTerm3.unity.meta

### Assets/1_SelfDrivingCar/Scripts/term3/UISystemTerm3.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.SceneManagement;

public class UISystemTerm2 : UISystemBase {

    public CarControllerTerm2 carController;
    public string GoodCarStatusMessage;
    public string BadSCartatusMessage;
    // MPH_Text and MPH_Animation are now inherited from UISystemBase
    public Text Angle_Text;
    public Text RecordStatus_Text;
    public Text DriveStatus_Text;
    public Text SaveStatus_Text;
    public GameObject RecordingPause; 
    public GameObject RecordDisabled;
    public bool isTraining = false;

    private bool recording;
    // topSpeed is now inherited from UISystemBase
    private bool saveRecording;


    // Use this for initialization
    void Start() {
		Debug.Log (isTraining);
        InitTopSpeed(carController.MaxSpeed);
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

    // Override Escape handling to auto-select Term menu
    protected override void OnEscapePressed()
    {
        ReturnToAppropriateMenu();
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

	    // Replace inline Escape handling with shared helper
        HandleEscapeKey();

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

        SceneManager.LoadScene(isTerm1Scene ? "MenuScene" : "MenuSceneTerm2");
    }
}


### Assets/Standard Assets/Vehicles/Car/Scripts/term2/BrakeLightTerm2.cs

using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class BrakeLightTerm2 : BrakeLightBase
    {
        public CarControllerTerm2 car; // drag Term2 controller here

        // private Renderer m_Renderer;


        // private void Start()
        // {
        //     m_Renderer = GetComponent<Renderer>();
        // }


        // private void Update()
        // {
        //     // enable the Renderer when the car is braking, disable it otherwise.
        //     m_Renderer.enabled = GetBrakeInput() > 0f;
        // }

        protected override float GetBrakeInput()
        {
            return car != null ? car.BrakeInput : 0f;
        }
    }
}

### Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarAudioTerm2.cs

using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarControllerTerm2))]
    public class CarAudioTerm2 : CarAudioBase
    {
        private CarControllerTerm2 m_CarController;

        private void Awake()
        {
            m_CarController = GetComponent<CarControllerTerm2>();
        }

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


### Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarRemoteControlTerm2.cs

using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarControllerTerm2))]
    public class CarRemoteControlTerm2 : CarRemoteControlBase
    {
        private CarControllerTerm2 m_Car; // the car controller we want to use

        private void Awake()
        {
            m_Car = GetComponent<CarControllerTerm2>();
            base.Awake();
        }

        protected override void ApplyMove(float h, float v)
        {
            m_Car.Move(h, v, v, 0f);
        }
    }
}

### Assets/Standard Assets/Vehicles/Car/Scripts/term2/CarUserControlTerm2.cs

using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarControllerTerm2))]
    public class CarUserControlTerm2 : CarUserControlBase
    {
        private CarControllerTerm2 m_Car;

        private void Awake()
        {
            m_Car = GetComponent<CarControllerTerm2>();
            base.Awake();
        }

        protected override void ApplyMove(float h, float v)
        {
            m_Car.Move(h, v, v, 0f);
        }
    }
}

### Assets/Standard Assets/Vehicles/Car/Scripts/term2/MudguardTerm2.cs

using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    // [RequireComponent(typeof(CarControllerTerm2))]
    public class MudguardTerm2 : MudguardBase
    {
        public CarControllerTerm2 carController; // car controller to get the steering angle

        // private Quaternion m_OriginalRotation;


        // private void Start()
        // {
        //     m_OriginalRotation = transform.localRotation;
        // }


        // private void Update()
        // {
        //     transform.localRotation = m_OriginalRotation*Quaternion.Euler(0, carController.CurrentSteerAngle, 0);
        // }

        protected override float GetSteerAngle()
        {
            return carController != null ? carController.CurrentSteerAngle : 0f;
        }
    }
}


