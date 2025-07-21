using Unity.VisualScripting;
using UnityEngine;
using Vuforia;

public class ManageTracking : MonoBehaviour
{
    public static ManageTracking Instance { get; private set; }

    #region Properties

    [Tooltip("Reference image target used to anchor AR sesssion")]
    public ImageTargetBehaviour referenceImageTarget;
    [Tooltip("Visualizer for reference image target")]
    public GameObject referenceTargetVisualizer;
    [Tooltip("Confirmation window for reference image target")]
    public GameObject confirmRefWindow;

    [Tooltip("Game image target used to track vaccumer")]
    public ImageTargetBehaviour gameImageTarget;
    [Tooltip("Visualizer for game target")]
    public GameObject gameTargetVisualizer;
    [Tooltip("Game area object. Contains game area visualizer and dirty areas")]
    public GameObject gameArea;
    [Tooltip("Visualizer for game area")]
    public GameObject gameAreaVisualizer;
    [Tooltip("Default offset of game area when in image target ref anchoring mode")]
    public Vector3 defaultOffset = Vector3.zero;

    [Tooltip("Total distance covered by game target (cleaner) during game running")]
    public float totalCleanDistance = 0f;
    [Tooltip("Flag to measure distance")]
    private bool measureDistance = false;
    [Tooltip("Previous position of game target (cleaner) to calculate covered distance between frames")]
    private Vector3 prevPosCleaner;

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
    }

    private void Update()
    {
        // Measure distance clenaer is covering if game started
        if (measureDistance)
        {
            // Calcuate distance
            totalCleanDistance += Vector3.Distance(prevPosCleaner, gameImageTarget.transform.position);

            // Update previous position
            prevPosCleaner = gameImageTarget.transform.position;
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

        // Align game area relative to ref target
        gameArea.transform.SetLocalPositionAndRotation(defaultOffset, Quaternion.identity);
    }

    // Stop tracking session reference target. User confirmed correct detection
    public void StopTrackingRefTarget()
    {
        // Disable reference image target
        referenceImageTarget.enabled = false;

        // Make sure reference target stays visible
        foreach (var mRenderer in referenceTargetVisualizer.GetComponentsInChildren<MeshRenderer>())
        {
            mRenderer.enabled = true;
        }
        referenceTargetVisualizer.SetActive(true);

        // Make sure game area stays visible
        gameAreaVisualizer.SetActive(true);
        foreach (var mRenderer in gameAreaVisualizer.GetComponentsInChildren<MeshRenderer>())
        {
            mRenderer.enabled = true;
        }

        //// Make sure dirty tiles are visible on game start
        //foreach (var component in GameManager.Instance.dirtyArea.GetComponentsInChildren<Component>())
        //{
        //    switch (component)
        //    {
        //        // Enable mesh renderers and colliders as these are the key components on dirty tiles
        //        case Renderer rendererComponent:
        //            rendererComponent.enabled = true;
        //            break;
        //        case Collider colliderComponent:
        //            colliderComponent.enabled = true;
        //            break;
        //        default:
        //            // Ignore other components
        //            break;
        //    }
        //}

        // Update UI
        UIManager.Instance.startDialog.SetActive(true);
        UIManager.Instance.confirmRefTargetButton.SetActive(false);
    }

    // Start tracking game target
    public void StartTrackingGameTarget()
    {
        // Enable game image target
        gameImageTarget.enabled = true;

        // Start measuring distance
        prevPosCleaner = gameImageTarget.transform.position;
        totalCleanDistance = 0f;
        measureDistance = true;
    }

    // Stop tracking game target
    public void StopTrackingGameTarget()
    {
        // Enable game image target
        gameImageTarget.enabled = false;

        // Stop measure distance
        measureDistance = false;
    }

    // Disable all tracking visualizers
    public void DisableVisualizers()
    {
        referenceTargetVisualizer.SetActive(false);
        gameTargetVisualizer.SetActive(false);
        gameAreaVisualizer.SetActive(false);
    }

    #endregion
}
