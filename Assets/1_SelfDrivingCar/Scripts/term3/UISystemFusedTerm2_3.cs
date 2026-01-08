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
