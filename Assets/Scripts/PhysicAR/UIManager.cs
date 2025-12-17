using MixedReality.Toolkit.SpatialManipulation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    #region UI elements

    [Tooltip("Left hand far ray")]
    public GameObject leftHandRay;
    [Tooltip("Right hand far ray")]
    public GameObject rightHandRay;

    [Tooltip("Initial setup dialog displayed to user")]
    public GameObject initDialog;

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
        initMeshToggle.SetActive(true);

        // Left handmenu
        tutorialButton.SetActive(true);
        returnFromTutButton.SetActive(false);
        anchorSessionButton.SetActive(true);
        //surfaceBasedStartToggle.SetActive(true);
        confirmRefTargetButton.SetActive(false);
        //confirmRefSurfaceButton.SetActive(false);
        homeToggle.SetActive(false);
        gameAreaToggle.SetActive(false);
        gameTargetToggle.SetActive(false);

        initDialog.SetActive(true);
        launchTask1Dialog.SetActive(false);
        launchTask2Dialog.SetActive(false);
        launchTask3Dialog.SetActive(false);
        finishedTask1Panel.SetActive(false);
        finishedTask2Panel.SetActive(false);
        finishedTask3Panel.SetActive(false);
        //finishAllCleanedDialog.SetActive(false);
        //finishMaxTimeDialog.SetActive(false);
        //statsPanel.SetActive(false);

        startDialogTut.SetActive(false);
        buttonDialogTut.SetActive(false);
        solvedDialogTut.SetActive(false);
        anchoredDialogTut.SetActive(false);
        insituDialogTut.SetActive(false);
        finishedDialogTut.SetActive(false);

        // Enable all spatial solvers by default (because some panels disable solvers)
        launchTask1Dialog.GetComponent<Solver>().enabled = true;
        launchTask2Dialog.GetComponent<Solver>().enabled = true;
        launchTask3Dialog.GetComponent<Solver>().enabled = true;
        finishedTask1Panel.GetComponent<Solver>().enabled = true;
        finishedTask2Panel.GetComponent<Solver>().enabled = true;
        finishedTask3Panel.GetComponent<Solver>().enabled = true;
        //finishAllCleanedDialog.GetComponent<Solver>().enabled = true;
        //finishMaxTimeDialog.GetComponent<Solver>().enabled = true;
        //statsPanel.GetComponent<Solver>().enabled = true;
        anchoredDialogTut.GetComponent<Solver>().enabled = true;
        insituDialogTut.GetComponent<Solver>().enabled = true;
    }

    #endregion
}
