using UnityEngine;
using Vuforia;

public class ManageTracking : MonoBehaviour
{
    public static ManageTracking Instance { get; private set; }

    #region Properties

    [Tooltip("Reference image target used to anchor AR sesssion")]
    public ImageTargetBehaviour referenceImageTarget;
    [Tooltip("Shadow object copying reference image target transform")]
    public GameObject refTargetShadow;
    [Tooltip("Visualizer for reference image target")]
    public GameObject referenceTargetVisualizer;
    [Tooltip("Confirmation window for reference image target")]
    public GameObject confirmRefWindow;

    [Tooltip("Game image target used to track vaccumer")]
    public ImageTargetBehaviour gameImageTarget;
    [Tooltip("Visualizer for game target")]
    public GameObject gameTargetVisualizer;
    [Tooltip("3x4 game area object. Contains game area visualizer and dirty areas")]
    public GameObject gameArea3x4;
    [Tooltip("2x2 game area object. Contains game area visualizer and dirty areas")]
    public GameObject gameArea2x2;
    [Tooltip("Visualizer for 3x4 game area")]
    public GameObject gameAreaVisualizer3x4;
    [Tooltip("Visualizer for 2x2 game area")]
    public GameObject gameAreaVisualizer2x2;
    [Tooltip("Default offset of game area when in image target ref anchoring mode")]
    public Vector3 defaultOffset = Vector3.zero;

    [Tooltip("Total distance covered by game target (cleaner) during game running")]
    public float totalCleanDistance = 0f;

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

    private void Start()
    {
        // Disable image targets by default
        referenceImageTarget.enabled = false;
        gameImageTarget.enabled = false;

        // Don't show reference target shadow objects by default (enabled on image target enabled)
        refTargetShadow.SetActive(false);
    }

    private void Update()
    {
        // Update "shadow" of reference target if image target is currently tracked
        if (referenceImageTarget.enabled)
        {
            refTargetShadow.SetActive(true);
            refTargetShadow.transform.SetPositionAndRotation(referenceImageTarget.transform.position, referenceImageTarget.transform.rotation);
        }
    }

    #endregion


    #region Methods

    // Callback for session reference target is found
    public void SessionRefFound()
    {
        // Enable visualizer
        referenceTargetVisualizer.SetActive(true);

        // Enable confirmation window to anchor AR session
        confirmRefWindow.SetActive(true);
    }

    // Callback for when session reference target is lost
    public void SessionRefLost()
    {
        // Disable visualizer
        referenceTargetVisualizer.SetActive(false);
    }

    // Callback for when game target is found

    // Callback for when game target is lost


    // Start tracking session reference target (e.g. on reset)
    public void StartTrackingRefTarget()
    {
        // Enable reference image target
        referenceImageTarget.enabled = true;
    }

    // Stop tracking session reference target. User confirmed correct detection
    public void StopTrackingRefTarget()
    {
        // Disable reference image target
        referenceImageTarget.enabled = false;

        //// Make sure reference target stays visible
        //foreach (var mRenderer in referenceTargetVisualizer.GetComponentsInChildren<MeshRenderer>())
        //{
        //    mRenderer.enabled = true;
        //}
        //referenceTargetVisualizer.SetActive(true);

        //// Make sure game area(s) stay visible
        //gameAreaVisualizer3x4.SetActive(true);
        //foreach (var mRenderer in gameAreaVisualizer3x4.GetComponentsInChildren<MeshRenderer>())
        //{
        //    mRenderer.enabled = true;
        //}
        //gameAreaVisualizer2x2.SetActive(true);
        //foreach (var mRenderer in gameAreaVisualizer2x2.GetComponentsInChildren<MeshRenderer>())
        //{
        //    mRenderer.enabled = true;
        //}

        // Update UI
        UIManager.Instance.confirmRefTargetButton.SetActive(false);

        //UIManager.Instance.startDialog.SetActive(true);
        // Enable task selection via right hand menu
        UIManager.Instance.handmenuRight.SetActive(true);
    }

    // Start tracking game target
    public void StartTrackingGameTarget()
    {
        // Enable game image target
        gameImageTarget.enabled = true;

        // Start measuring distance
        //prevPosCleaner = gameImageTarget.transform.position;
        //totalCleanDistance = 0f;
        //measureDistance = true;
    }

    // Stop tracking game target
    public void StopTrackingGameTarget()
    {
        // Enable game image target
        gameImageTarget.enabled = false;

        // Stop measure distance
        //measureDistance = false;
    }

    // Game target (cleaner) is tracked
    public void GameTargetFound()
    {
        GameManager.Instance.cleanerIsTracked = 1;
    }

    // Game target (cleaner) is not tracked
    public void GameTargetLost()
    {
        GameManager.Instance.cleanerIsTracked = 0;
    }


    // Disable all tracking visualizers
    public void DisableVisualizers()
    {
        referenceTargetVisualizer.SetActive(false);
        gameTargetVisualizer.SetActive(false);
        gameAreaVisualizer3x4.SetActive(false);
    }

    #endregion
}
