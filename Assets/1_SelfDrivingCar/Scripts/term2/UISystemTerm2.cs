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
