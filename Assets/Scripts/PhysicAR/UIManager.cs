using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    #region UI elements

    [Tooltip("Left hand far ray")]
    public GameObject leftHandRay;
    [Tooltip("Right hand far ray")]
    public GameObject rightHandRay;

    // Tutorial
    [Tooltip("Starting dialog of tutorial")]
    public GameObject startDialogTut;
    [Tooltip("Dialog showcasing how to press buttons")]
    public GameObject buttonDialogTut;
    [Tooltip("Dialog showcasing user attached panels")]
    public GameObject solvedDialogTut;
    [Tooltip("Dialog showcasing static panels")]
    public GameObject anchoredDialogTut;
    [Tooltip("Dialog showcasing in-situ capabilities of HoloLens")]
    public GameObject insituDialogTut;
    [Tooltip("Dialog at the end of tutorial")]
    public GameObject finishedDialogTut;

    // Game
    [Tooltip("Dialog to start first task")]
    public GameObject launchTask1Dialog;
    [Tooltip("Dialog to start second task")]
    public GameObject launchTask2Dialog;
    [Tooltip("Dialog to start third task")]
    public GameObject launchTask3Dialog;
    [Tooltip("Panel shown after task 1 finished")]
    public GameObject finishedTask1Panel;
    [Tooltip("Panel shown after task 2 finished")]
    public GameObject finishedTask2Panel;
    [Tooltip("Panel shown after task 3 finished")]
    public GameObject finishedTask3Panel;
    [Tooltip("Dialog when finished cleaning all dirty areas")]
    public GameObject finishAllCleanedDialog;
    [Tooltip("Dialog when finished due to maximum time reached")]
    public GameObject finishMaxTimeDialog;

    [Tooltip("Stats panel showing information such as total time, cleaned areas etc.")]
    public GameObject statsPanel;
    [Tooltip("Reason for end of game (e.g. cleaned all areas, time limit surpassed)")]
    public TMP_Text endReasonText;
    [Tooltip("Number of areas cleaned displayed in stats panel")]
    public TMP_Text cleanedAreasText;
    [Tooltip("Times needed to clean each single area in stats panel")]
    public TMP_Text timesPerArea;
    [Tooltip("Distances travelled to clean each single area in stats panel")]
    public TMP_Text distancesPerArea;

    // Hand calibration
    [Tooltip("Dialog before calibrating hand distance")]
    public GameObject handCalibDialog;
    [Tooltip("Countdown panel for hand calibration")]
    public GameObject handCalibCountDown;

    // Handmenu
    [Tooltip("Handmenu left")]
    public GameObject handmenuLeft;
    [Tooltip("Menu canvas on left handmenu. Needs to be disabled and is automatically enabled on detection")]
    public GameObject handmenuCanvasLeft;
    [Tooltip("Handmenu right")]
    public GameObject handmenuRight;
    [Tooltip("Menu canvas on right handmenu. Needs to be disabled and is automatically enabled on detection")]
    public GameObject handmenuCanvasRight;
    [Tooltip("Button for triggering eye calibration")]
    public GameObject triggerEyeCalibButton;
    [Tooltip("Toggle for initial environmental meshing")]
    public GameObject initMeshToggle;
    [Tooltip("Button to launch tutorial")]
    public GameObject tutorialButton;
    [Tooltip("Button to return from tutorial back to main scenario")]
    public GameObject returnFromTutButton;
    [Tooltip("Toggle to anchor game session")]
    public GameObject anchorSessionButton;
    [Tooltip("Toggle to start game using surface as anchor")]
    public GameObject surfaceBasedStartToggle;
    [Tooltip("Button to confirm session anchor in target-based game setup")]
    public GameObject confirmRefTargetButton;
    [Tooltip("Toggle to confirm session anchor in surface-based game setup")]
    public GameObject confirmRefSurfaceButton;
    [Tooltip("Go home toggle on handmenu")]
    public GameObject homeToggle;
    [Tooltip("Toggle for game area outline")]
    public GameObject gameAreaToggle;
    [Tooltip("Toggle which controls visualization of game target (cleaner). On handmenu")]
    public GameObject gameTargetToggle;

    // Settings handmenu
    [Tooltip("Handmenu for settings (currently only task times)")]
    public GameObject settingsMenu;
    [Tooltip("Time value label")]
    public TMP_Text timeValueLabel;
    [Tooltip("Toggle group of time settings (Index -> 0: Task 1 time, 1: Task 2 time, 2: Task 2 show fields time, 3: Task 3 time, 4: Task 3 show objects time)")]
    public ToggleCollection timeSettingsToggles;


    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of UIManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this); // Destroy duplicate instances
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Reset UI elements to default state
        ResetUI();

        // Set internal task times based on player prefs
        if (PlayerPrefs.HasKey("task1Time"))
        {
            GameManager.Instance.task1Time = PlayerPrefs.GetInt("task1Time");
        }
        else
        {
            PlayerPrefs.SetInt("task1Time", (int)GameManager.Instance.task1Time);
            timeValueLabel.text = GameManager.Instance.task1Time.ToString();
        }
        if (PlayerPrefs.HasKey("task2Time"))
        {
            GameManager.Instance.task2Time = PlayerPrefs.GetInt("task2Time");
        }
        else
        {
            PlayerPrefs.SetInt("task2Time", (int)GameManager.Instance.task2Time);
            timeValueLabel.text = GameManager.Instance.task2Time.ToString();
        }
        if (PlayerPrefs.HasKey("task2ShowTime"))
        {
            GameManager.Instance.showNumberedDirtyAreasTime = PlayerPrefs.GetInt("task2ShowTime");
        }
        else
        {
            PlayerPrefs.SetInt("task2ShowTime", (int)GameManager.Instance.showNumberedDirtyAreasTime);
            timeValueLabel.text = GameManager.Instance.showNumberedDirtyAreasTime.ToString();
        }
        if (PlayerPrefs.HasKey("task3Time"))
        {
            GameManager.Instance.task3Time = PlayerPrefs.GetInt("task3Time");
        }
        else
        {
            PlayerPrefs.SetInt("task3Time", (int)GameManager.Instance.task3Time);
            timeValueLabel.text = GameManager.Instance.task3Time.ToString();
        }
        if (PlayerPrefs.HasKey("task3ShowTime"))
        {
            GameManager.Instance.show3dObjectsTime = PlayerPrefs.GetInt("task3ShowTime");
        }
        else
        {
            PlayerPrefs.SetInt("task3ShowTime", (int)GameManager.Instance.show3dObjectsTime);
            timeValueLabel.text = GameManager.Instance.show3dObjectsTime.ToString();
        }
    }

    #endregion


    #region Methods

    public void ResetUI()
    {
        // Set initially active dialog and disable all other
        handmenuLeft.SetActive(true);
        handmenuCanvasLeft.SetActive(false);
        handmenuRight.SetActive(false);
        handmenuCanvasRight.SetActive(false);

        //Right handmenu
        triggerEyeCalibButton.SetActive(true);
        initMeshToggle.SetActive(false);

        // Left handmenu
        tutorialButton.SetActive(false);
        returnFromTutButton.SetActive(false);
        anchorSessionButton.SetActive(true);
        //surfaceBasedStartToggle.SetActive(true);
        confirmRefTargetButton.SetActive(false);
        //confirmRefSurfaceButton.SetActive(false);
        homeToggle.SetActive(false);
        gameAreaToggle.SetActive(false);
        //gameTargetToggle.SetActive(false);
        settingsMenu.SetActive(false);

        launchTask1Dialog.SetActive(false);
        launchTask2Dialog.SetActive(false);
        launchTask3Dialog.SetActive(false);
        finishedTask1Panel.SetActive(false);
        finishedTask2Panel.SetActive(false);
        finishedTask3Panel.SetActive(false);
        //finishAllCleanedDialog.SetActive(false);
        //finishMaxTimeDialog.SetActive(false);
        //statsPanel.SetActive(false);

        handCalibDialog.SetActive(false);
        handCalibCountDown.SetActive(false);

        startDialogTut.SetActive(false);
        buttonDialogTut.SetActive(false);
        solvedDialogTut.SetActive(false);
        anchoredDialogTut.SetActive(false);
        insituDialogTut.SetActive(false);
        finishedDialogTut.SetActive(false);

        // Enable all spatial solvers by default (because some panels disable solvers)
        handCalibDialog.GetComponent<Solver>().enabled = true;
        handCalibCountDown.GetComponent<Solver>().enabled = true;
        GameManager.Instance.objectsToRemember.GetComponent<Solver>().enabled = true;
        anchoredDialogTut.GetComponent<Solver>().enabled = true;
        insituDialogTut.GetComponent<Solver>().enabled = true;
    }

    #endregion


    #region Callbacks

    // Called when a task time setting toggle is toggled
    public void ToggledTaskTimeSetting(int value)
    {
        // Display currently internally set time
        switch (value)
        {
            case 0:
                timeValueLabel.text = GameManager.Instance.task1Time.ToString();
                break;
            case 1:
                timeValueLabel.text = GameManager.Instance.task2Time.ToString();
                break;
            case 2:
                timeValueLabel.text = GameManager.Instance.showNumberedDirtyAreasTime.ToString();
                break;
            case 3:
                timeValueLabel.text = GameManager.Instance.task3Time.ToString();
                break;
            case 4:
                timeValueLabel.text = GameManager.Instance.show3dObjectsTime.ToString();
                break;
            default:
                break;
        }
    }

    // Called when user increases task time
    public void IncreasedTaskTime()
    {
        // Calculate new value
        int value = int.Parse(timeValueLabel.text) + 5;

        // Upper limit
        if (value > 1000)
        {
            return;
        }

        // Update visually
        timeValueLabel.text = value.ToString();

        // Update internally
        switch (timeSettingsToggles.CurrentIndex)
        {
            case 0:
                GameManager.Instance.task1Time = value;
                break;
            case 1:
                GameManager.Instance.task2Time = value;
                break;
            case 2:
                GameManager.Instance.showNumberedDirtyAreasTime = value;
                break;
            case 3:
                GameManager.Instance.task3Time = value;
                break;
            case 4:
                GameManager.Instance.show3dObjectsTime = value;
                break;
            default:
                break;
        }
    }

    // Called when user decreases task time
    public void DecreasedTaskTime()
    {
        // Calculate new value
        int value = int.Parse(timeValueLabel.text) - 5;

        // Lower limit
        if (value < 5)
        {
            return;
        }

        // Update visually
        timeValueLabel.text = value.ToString();

        // Update internally
        switch (timeSettingsToggles.CurrentIndex)
        {
            case 0:
                GameManager.Instance.task1Time = value;
                break;
            case 1:
                GameManager.Instance.task2Time = value;
                break;
            case 2:
                GameManager.Instance.showNumberedDirtyAreasTime = value;
                break;
            case 3:
                GameManager.Instance.task3Time = value;
                break;
            case 4:
                GameManager.Instance.show3dObjectsTime = value;
                break;
            default:
                break;
        }
    }

    // Called if user saves new task times configured via settings handmenu in main menu
    public void SaveTaskTimes()
    {
        // Update in player prefs
        PlayerPrefs.SetInt("task1Time", (int)GameManager.Instance.task1Time);
        PlayerPrefs.SetInt("task2Time", (int)GameManager.Instance.task2Time);
        PlayerPrefs.SetInt("task2ShowTime", (int)GameManager.Instance.showNumberedDirtyAreasTime);
        PlayerPrefs.SetInt("task3Time", (int)GameManager.Instance.task3Time);
        PlayerPrefs.SetInt("task3ShowTime", (int)GameManager.Instance.show3dObjectsTime);
        PlayerPrefs.Save();
    }

    #endregion
}
