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
