using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuOptions : MonoBehaviour
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
    public Sprite project_6;

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
        Debug.Log($"IsTerm3 = {IsTerm3}");
        if (IsTerm1) {
            SceneManager.LoadScene("ControlMenu");
        }
        else if (IsTerm2) {
            SceneManager.LoadScene("ControlMenuTerm2");
        }
        else if (IsTerm3) {
            SceneManager.LoadScene("ControlMenuTerm3"); // Term 3 uses ControlMenu
        }
	}

	public void MainMenu()
	{
        Debug.Log("Going to main menu scene");
        Debug.Log($"IsTerm1 = {IsTerm1}");
        Debug.Log($"IsTerm2 = {IsTerm2}");
        Debug.Log($"IsTerm3 = {IsTerm3}");
        if (IsTerm1) {
            SceneManager.LoadScene("MenuScene");
        }
        else if (IsTerm2) {
            SceneManager.LoadScene("MenuSceneTerm2");
        }
        else if (IsTerm3) {
            SceneManager.LoadScene("MenuSceneTerm3"); // Term 3 uses MenuScene
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
            case 5:
                SceneManager.LoadScene("PathPlanning");
                break;
        }
    }

	public void Next()
	{
		project = (project + 1) % 6;
        UpdateProjectDisplay();
    }

    public void Previous()
    {
        project = (project == 0) ? 5 : project - 1;
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
            5 => "Project 6: Path Planning",
            _ => "Invalid Project"
        };

        projectImage.sprite = project switch
        {
            0 => project_1,
            1 => project_2,
            2 => project_3,
            3 => project_4,
            4 => project_5,
            5 => project_6,
            _ => null
        };
    }
    #endregion // end of Term 2 Methods
}
