using MixedReality.Toolkit;
using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region Properties

    [Tooltip("Information sampling framerate (e.g. 30")]
    public int samplingRate = 30;
    [Tooltip("Resulting sampling interval between samples")]
    private float samplingInterval;
    [Tooltip("Timer used for measuring time between samples")]
    private float samplingTimer = 0f;

    [Tooltip("Surface Magnetism component on game area used for surface-based setup")]
    public SurfaceMagnetism surfMag;
    [Tooltip("Component on game area to align it towards user")]
    public AlignPlayingField alignPlayField;
    [Tooltip("ARMeshManager component responsible for environment meshing")]
    public ARMeshManager meshManager;

    [Tooltip("Dirty area game element consisting of many smaller tiles (used for task 1 and 3)")]
    public GameObject dirtyArea;
    [Tooltip("Numbered dirty areas (task 2)")]
    public GameObject[] numberedDirtyAreas;
    [Tooltip("List of locations (parent objects) for dirty areas (task 1)")]
    public List<Transform> dirtyAreaLocations3x4;
    [Tooltip("List of locations (parent objects) for dirty areas (task 3)")]
    public List<Transform> dirtyAreaLocations2x2;
    [Tooltip("Current dirty area index")]
    private int currentDirtyAreaIndex = 0;
    [Tooltip("Stageplate with 3D objects to remember during task 3")]
    public GameObject objectsToRemember;

    [Tooltip("Shared material for dirty tiles")]
    private Material[] dirtyMats = new Material[2];
    [Tooltip("Fade duration for showing next dirty area")]
    private float fadeDuration = 1.5f;
    [Tooltip("Time numbered dirty tiles are shown in task 2 (seconds)")]
    public float showNumberedDirtyAreasTime;
    [Tooltip("Time 3d objects to remember are shown in task 3 (seconds)")]
    public float show3dObjectsTime;

    [Tooltip("Total number of tiles within a single dirty area")]
    private int tilesInOneArea = 0;
    [Tooltip("Count how many tiles have been cleaned within one dirty area")]
    private int cleanedTiles = 0;
    public int CleanedTiles
    {
        get => cleanedTiles;
        set
        {
            // Increment underlying var
            cleanedTiles = value;

            // Number of cleaned tiles has reached total tiles count in one area
            if (cleanedTiles == 1)
            {
                // This is the first tile of a single dirty area
                StartedCleaningArea();
            }
            else if (currentTask == 1 || currentTask == 3)
            {
                if (cleanedTiles >= tilesInOneArea)
                {
                    // This is the last tile of a single dirty area
                    FinishedCleaningSingleAreaT1T3();
                }
            }
            else if (currentTask == 2)
            {
                if (cleanedTiles >= tilesInOneArea * 4)
                {
                    // All numbered tiles cleaned before time limit
                    StopTask2();
                }
            }
        }
    }

    [Tooltip("Time used to clean single dirty area")]
    private float singleAreaTime = 0f;
    [Tooltip("List of single area times")]
    public List<float> singleAreaTimes = new List<float>();

    [Tooltip("Distanced travelled to clean single dirty area")]
    private float singleAreaDistance = 0f;
    [Tooltip("List of single area distances")]
    public List<float> singleAreaDistances = new List<float>();
    [Tooltip("Previous position of cleaner in last frame to calculate covered distance between frames")]
    public Vector3 prevPosCleaner;

    [Tooltip("Flag if game started/is running")]
    private bool taskIsRunning = false;
    [Tooltip("Currently running task (1,2,3). 0 if none going on")]
    public int currentTask = 0;
    [Tooltip("Dynamic max. game time depending on task in seconds (T1: 180, T2: 120, T3: 120)")]
    private float maxGameTime;
    [Tooltip("User specified max. game time for task 1")]
    public float task1Time = 180f;
    [Tooltip("User specified max. game time for task 2 (including time for showing fields)")]
    public float task2Time = 120f;
    [Tooltip("User specified max. game time for task 3 (including time for showing objects)")]
    public float task3Time = 120f;

    [Tooltip("Audio source used to provide acoustic feedback to user")]
    public AudioSource audioSource;
    [Tooltip("Acousctic feedabck: User finsished cleaning single dirty area")]
    public AudioClip cleanedSingleAreaClip;
    [Tooltip("Acousctic feedabck: User finsished cleaning all dirty areas")]
    public AudioClip taskCompletedSound;
    [Tooltip("Distracting sound 1 played during task 3")]
    public AudioClip distractSound1;
    [Tooltip("Distracting sound 2 played during task 3")]
    public AudioClip distractSound2;
    [Tooltip("Distracting sound 3 played during task 3")]
    public AudioClip distractSound3;

    [Tooltip("Directional indicator pointing to next area to clean")]
    public GameObject directionalIndicator;

    [Tooltip("List of sampled information frames")]
    private List<InformationFrame> informationFrames = new List<InformationFrame>();

    [Serializable, Tooltip("Structure used to store relevant information")]
    public struct InformationFrame
    {
        public float Timestamp;         // Time since start of game
        public int AreaID;              // Number of area being cleaned

        public int CleanerIsTracked;    // Whether cleaner is being tracked
        public Vector3 CleanerPos;      // Position of cleaner

        public Vector3 HeadPos;         // Position of user head
        public Quaternion HeadRot;      // Rotation of user head
        public Vector3 LeftWristPos;    // Position of user left wrist
        //public Quaternion LeftWristRot; // Rotation of user left wrist
        public Vector3 RightWristPos;    // Position of user right wrist
        //public Quaternion RightWristRot; // Rotation of user right wrist
    }

    // Information bits and necessary additional vars to determine them
    [Tooltip("Value measuring time since game start (timestamp)")]
    private float timeStamp = 0f;

    [Tooltip("ID of area being cleaned. Is 0 if not (started) cleaning an area")]
    private int areaID = 0;
    [Tooltip("Number of area being currently cleaned. Used to specify areaID if currently cleaning")]
    public int areaBeingCleaned = 1;
    [Tooltip("Flag whether currently cleaning area or not")]
    private bool cleaning = false;

    [Tooltip("Flag whether cleaner is currently being tracked or not. Is 0 if not tracked, 1 if tracked")]
    public int cleanerIsTracked = 0;
    [Tooltip("World position of cleaner")]
    private Vector3 cleanerPos = Vector3.zero;

    [Tooltip("World position of user head")]
    private Vector3 headPos = Vector3.zero;
    [Tooltip("World rotation of user head")]
    private Quaternion headRot = Quaternion.identity;

    [Tooltip("World position of the user's left wrist")]
    private Vector3 leftWristPos = Vector3.zero;
    //[Tooltip("World rotation of the user's left wrist")]
    //private Quaternion leftWristRot = Quaternion.identity;
    [Tooltip("World position of the user's right wrist")]
    private Vector3 rightWristPos = Vector3.zero;
    //[Tooltip("World rotation of the user's right wrist")]
    //private Quaternion rightWristRot = Quaternion.identity;
    [Tooltip("Hands aggergator subsystem used to access joint data e.g. wrist pose")]
    private HandsAggregatorSubsystem handAggregator;

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
        // Cache total nunmer of tiles in single dirty area (e.g. 100 if tiles are 0.1mx0.1m)
        tilesInOneArea = dirtyArea.transform.childCount;
        //Debug.Log(tilesInOneArea.ToString());

        // Cache dirty tile SHARED materials (2 different)
        dirtyMats[0] = dirtyArea.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;
        dirtyMats[1] = dirtyArea.transform.GetChild(1).GetComponent<Renderer>().sharedMaterial;

        // Calculate sampling interval based on specified sampling rate
        samplingInterval = 1f / samplingRate;

        // Cache hands aggergator for getting wrist poses later
        handAggregator = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();

        // Hide object plate of task 3
        objectsToRemember.SetActive(false);
    }

    private async void Update()
    {
        // Measure game time once it started
        if (taskIsRunning)
        {
            // Measure time between frames
            samplingTimer += Time.unscaledDeltaTime;
            // Sample at specified sampling rate
            if (samplingTimer >= samplingInterval)
            {
                // Sample information
                SampleInformation();

                // Reset timer
                samplingTimer = 0f;
            }

            // Check if maximum game time reached
            if (timeStamp >= maxGameTime)
            {
                // Stop game
                if (currentTask == 1)
                {
                    await StopTask1();
                }
                else if (currentTask == 2)
                {
                    await StopTask2();
                }
                else if (currentTask == 3)
                {
                    await StopTask3();
                }

                // Reset time
                timeStamp = 0f;

                // Update UI accordingly
                //UIManager.Instance.endReasonText.text = "Zeitlimit erreicht";
                //UIManager.Instance.finishMaxTimeDialog.SetActive(true);
                //UIManager.Instance.statsPanel.SetActive(true);
            }
        }

        // Measure time and distance (only for local display, not needed for actual evaluation)
        //if (cleaning)
        //{
        //    // Time
        //    singleAreaTime += Time.deltaTime;

        //    // Distance
        //    singleAreaDistance += Vector3.Distance(prevPosCleaner, ManageTracking.Instance.gameImageTarget.transform.position);
        //    prevPosCleaner = ManageTracking.Instance.gameImageTarget.transform.position;
        //}
    }

    #endregion


    #region Methods

    // Set up game using image target as anchor
    public void InitTargetBasedSetup()
    {
        // Start looking for session anchor
        ManageTracking.Instance.StartTrackingRefTarget();

        // Update UI
        UIManager.Instance.handmenuLeft.SetActive(false);
        UIManager.Instance.initMeshToggle.SetActive(false);
        UIManager.Instance.triggerEyeCalibButton.SetActive(false);
        UIManager.Instance.tutorialButton.SetActive(false);
        UIManager.Instance.anchorSessionButton.SetActive(false);
        //UIManager.Instance.surfaceBasedStartToggle.SetActive(false);
        UIManager.Instance.confirmRefTargetButton.SetActive(true);
        UIManager.Instance.initDialog.SetActive(false);
    }

    // Set up game using surface as anchor
    public void InitSurfaceBasedSetup()
    {
        // Start looking for surface using hand rays
        surfMag.enabled = true;
        alignPlayField.enabled = true;

        // Enable meshing
        meshManager.enabled = true;

        // Enable game area visualization
        foreach (var rend in ManageTracking.Instance.gameAreaVisualizer3x4.GetComponentsInChildren<MeshRenderer>())
        {
            rend.enabled = true;
        }
        ManageTracking.Instance.gameAreaVisualizer3x4.SetActive(true);

        // Update UI
        UIManager.Instance.initMeshToggle.SetActive(false);
        UIManager.Instance.triggerEyeCalibButton.SetActive(false);
        UIManager.Instance.tutorialButton.SetActive(false);
        UIManager.Instance.anchorSessionButton.SetActive(false);
        UIManager.Instance.surfaceBasedStartToggle.SetActive(false);
        UIManager.Instance.confirmRefSurfaceButton.SetActive(true);
        UIManager.Instance.initDialog.SetActive(false);
    }

    public void CompleteSurfaceBasedSetup()
    {
        // Disable surface magnetism logic
        surfMag.enabled = false;
        alignPlayField.enabled = false;
        meshManager.enabled = false;

        // Update UI
        UIManager.Instance.confirmRefSurfaceButton.SetActive(false);
        UIManager.Instance.launchTask1Dialog.SetActive(true);
    }
    
    // Start task 1
    public void StartTask1()
    {
        // Flag game as started
        taskIsRunning = true;
        currentTask = 1;

        // Set max. game time according to task 1
        maxGameTime = task1Time;

        // Disable far rays
        UIManager.Instance.leftHandRay.SetActive(false);
        UIManager.Instance.rightHandRay.SetActive(false);

        // Update UI
        UIManager.Instance.launchTask1Dialog.SetActive(false);
        UIManager.Instance.homeToggle.SetActive(true);
        UIManager.Instance.gameTargetToggle.SetActive(true);

        // Start tracking game target
        ManageTracking.Instance.StartTrackingGameTarget();
        // Enable cleaner outline by default
        UIManager.Instance.gameTargetToggle.GetComponent<PressableButton>().ForceSetToggled(true);

        // Make sure dirty tiles are visible on game start
        foreach (var component in dirtyArea.GetComponentsInChildren<Component>())
        {
            switch (component)
            {
                // Enable mesh renderers and colliders as these are the key components on dirty tiles
                case Renderer rendererComponent:
                    rendererComponent.enabled = true;
                    break;
                case Collider colliderComponent:
                    colliderComponent.enabled = true;
                    break;
                default:
                    // Ignore other components
                    break;
            }
        }

        // Choose dirty area to clean based on specified order array
        dirtyArea.transform.SetParent(dirtyAreaLocations3x4[currentDirtyAreaIndex]);

        // Set local pose to identity to match prelocation
        dirtyArea.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));

        // Fade in dirty area to be cleaned
        //StartCoroutine(FadeInDirtyArea(fadeDuration));
        dirtyArea.SetActive(true);

        // Reset previous data (local visualization)
        //singleAreaDistance = 0f;
        //singleAreaTime = 0f;
        //singleAreaDistances.Clear();
        //singleAreaTimes.Clear();
    }

    // Stop task 1
    public async Task StopTask1()
    {
        // Flag as stopped (terminates information sampling in Update)
        taskIsRunning = false;
        currentTask = 0;

        // Enable far rays again
        UIManager.Instance.leftHandRay.SetActive(true);
        UIManager.Instance.rightHandRay.SetActive(true);

        // Stop tracking game target
        ManageTracking.Instance.StopTrackingGameTarget();

        // Reset dirty area tiles
        dirtyArea.SetActive(false);
        for (int i = 0; i < dirtyArea.transform.childCount; i++)
        {
            // Enable all tiles again
            dirtyArea.transform.GetChild(i).gameObject.SetActive(true);
        }

        // Update UI
        UIManager.Instance.finishedTask1Panel.SetActive(true);
        UIManager.Instance.gameTargetToggle.SetActive(false);
        UIManager.Instance.handmenuRight.SetActive(true);

        //UIManager.Instance.cleanedAreasText.text = (areaBeingCleaned - 1).ToString();
        //UIManager.Instance.timesPerArea.text = string.Join("; ", singleAreaTimes.Select(t => t.ToString("0.00")));
        //UIManager.Instance.distancesPerArea.text = string.Join("; ", singleAreaDistances.Select(d => d.ToString("0.00")));
        //UIManager.Instance.resetGameToggle.SetActive(false);
        //UIManager.Instance.gameTargetToggle.SetActive(false);

        // Write sampled information to disk using current time in name
        await WriteInformationToDisk($"Task1_{DateTime.Now:dd-MM_HH-mm-ss}.csv");

        // Reset vars (data)
        cleanedTiles = 0;
        CleanedTiles = 0;
        currentDirtyAreaIndex = 0;
        areaBeingCleaned = 1;
        timeStamp = 0f;
        informationFrames = new();

        // Acoustic feedback that task ended
        audioSource.PlayOneShot(taskCompletedSound);
    }

    // Start task 2
    public void StartTask2()
    {
        // Flag game as started
        taskIsRunning = true;
        currentTask = 2;

        // Set max. game time according to task 1
        maxGameTime = task2Time;

        // Disable far rays
        UIManager.Instance.leftHandRay.SetActive(false);
        UIManager.Instance.rightHandRay.SetActive(false);

        // Update UI
        UIManager.Instance.launchTask2Dialog.SetActive(false);
        UIManager.Instance.homeToggle.SetActive(true);
        UIManager.Instance.gameTargetToggle.SetActive(true);

        // Make sure dirty tiles are visible on game start
        foreach (var area in numberedDirtyAreas)
        {
            foreach (var component in area.GetComponentsInChildren<Component>())
            {
                switch (component)
                {
                    // Enable mesh renderers and colliders as these are the key components on dirty tiles
                    case Renderer rendererComponent:
                        rendererComponent.enabled = true;
                        break;
                    case Collider colliderComponent:
                        colliderComponent.enabled = true;
                        break;
                    case Canvas canvasComponent:
                        canvasComponent.enabled = true;
                        break;
                    default:
                        // Ignore other components
                        break;
                }
            }
        }

        // Start coroutine which initiates task 2
        StartCoroutine(InitiateTask2());
    }

    private IEnumerator InitiateTask2()
    {
        // Show numbered dirty areas
        foreach (var area in numberedDirtyAreas)
        {
            area.SetActive(true);
        }

        // Show numbered dirty areas for specified amount of time
        yield return new WaitForSeconds(showNumberedDirtyAreasTime);

        // Hide numbered dirty areas again (only rendered part, still active)
        foreach (var area in numberedDirtyAreas)
        {
            foreach (var component in area.GetComponentsInChildren<Component>())
            {
                switch (component)
                {
                    // Enable mesh renderers and colliders as these are the key components on dirty tiles
                    case Renderer rendererComponent:
                        rendererComponent.enabled = false;
                        break;
                    case Canvas canvasComponent:
                        canvasComponent.enabled = false;
                        break;
                    default:
                        // Ignore other components
                        break;
                }
            }
        }

        // Start tracking game target
        ManageTracking.Instance.StartTrackingGameTarget();
        // Enable cleaner outline by default
        UIManager.Instance.gameTargetToggle.GetComponent<PressableButton>().ForceSetToggled(true);
    }

    // Stop task 2
    public async Task StopTask2()
    {
        // Flag as stopped (terminates information sampling in Update)
        taskIsRunning = false;
        currentTask = 0;
        cleaning = false;

        // Enable far rays again
        UIManager.Instance.leftHandRay.SetActive(true);
        UIManager.Instance.rightHandRay.SetActive(true);

        // Stop tracking game target
        ManageTracking.Instance.StopTrackingGameTarget();

        // Reset dirty area tiles
        foreach (var area in numberedDirtyAreas)
        {
            // Disable all numbered dirty areas
            area.SetActive(false);

            // Re-enable tiles within numbered dirty areas
            for (int i = 0; i < area.transform.childCount; i++)
            {
                area.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        // Update UI
        UIManager.Instance.finishedTask2Panel.SetActive(true);
        UIManager.Instance.gameTargetToggle.SetActive(false);
        UIManager.Instance.handmenuRight.SetActive(true);

        // Write sampled information to disk using current time in name
        await WriteInformationToDisk($"Task2_{DateTime.Now:dd-MM_HH-mm-ss}.csv");

        // Reset vars (data)
        cleanedTiles = 0;
        CleanedTiles = 0;
        currentDirtyAreaIndex = 0;
        areaBeingCleaned = 1;
        timeStamp = 0f;
        informationFrames = new();

        // Acoustic feedback that task ended
        audioSource.PlayOneShot(taskCompletedSound);
    }

    // Start task 3
    public void StartTask3()
    {
        // Flag game as started
        taskIsRunning = true;
        currentTask = 3;

        // Set max. game time according to task 1
        maxGameTime = task1Time;

        // Disable far rays
        UIManager.Instance.leftHandRay.SetActive(false);
        UIManager.Instance.rightHandRay.SetActive(false);

        // Update UI
        UIManager.Instance.launchTask3Dialog.SetActive(false);
        UIManager.Instance.homeToggle.SetActive(true);
        UIManager.Instance.gameTargetToggle.SetActive(true);

        // Make sure dirty tiles are visible on game start
        foreach (var component in dirtyArea.GetComponentsInChildren<Component>())
        {
            switch (component)
            {
                // Enable mesh renderers and colliders as these are the key components on dirty tiles
                case Renderer rendererComponent:
                    rendererComponent.enabled = true;
                    break;
                case Collider colliderComponent:
                    colliderComponent.enabled = true;
                    break;
                default:
                    // Ignore other components
                    break;
            }
        }

        // Start coroutine which initiates task 3
        StartCoroutine(InitiateTask3());
    }

    private IEnumerator InitiateTask3()
    {
        // Show user 3D objects to remember
        objectsToRemember.SetActive(true);
        objectsToRemember.GetComponent<Solver>().enabled = true;    // Will deactivate automatically after short amount of time

        // Continue with task 3 after specified amount of time
        yield return new WaitForSeconds(show3dObjectsTime);

        // Disable objects again
        objectsToRemember.SetActive(false);

        // Choose dirty area to clean based on specified order array
        dirtyArea.transform.SetParent(dirtyAreaLocations2x2[currentDirtyAreaIndex]);

        // Set local pose to identity to match prelocation
        dirtyArea.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));

        // Fade in dirty area to be cleaned
        //StartCoroutine(FadeInDirtyArea(fadeDuration));
        dirtyArea.SetActive(true);

        // Start tracking game target
        ManageTracking.Instance.StartTrackingGameTarget();
        // Enable cleaner outline by default
        UIManager.Instance.gameTargetToggle.GetComponent<PressableButton>().ForceSetToggled(true);

        // Set off coroutines which play distracting noises (at equal intervals based on total time for task 3)
        float interval = (task3Time - show3dObjectsTime) / 4f;
        StartCoroutine(PlaySound(interval, distractSound1));
        StartCoroutine(PlaySound(2*interval, distractSound2));
        StartCoroutine(PlaySound(3*interval, distractSound3));
    }

    // Coroutine for playing distracting sounds
    private IEnumerator PlaySound(float timeToPlay, AudioClip soundToPlay)
    {
        // Wait for specified amount of time before playing sound
        yield return new WaitForSeconds(timeToPlay);

        // Play sound
        audioSource.PlayOneShot(soundToPlay);
    }

    // Stop task 3
    public async Task StopTask3()
    {
        // Flag as stopped (terminates information sampling in Update)
        taskIsRunning = false;
        currentTask = 0;

        // Enable far rays again
        UIManager.Instance.leftHandRay.SetActive(true);
        UIManager.Instance.rightHandRay.SetActive(true);

        // Stop tracking game target
        ManageTracking.Instance.StopTrackingGameTarget();

        // Reset dirty area tiles
        dirtyArea.SetActive(false);
        for (int i = 0; i < dirtyArea.transform.childCount; i++)
        {
            // Enable all tiles again
            dirtyArea.transform.GetChild(i).gameObject.SetActive(true);
        }

        // Update UI
        UIManager.Instance.finishedTask3Panel.SetActive(true);
        UIManager.Instance.gameTargetToggle.SetActive(false);
        UIManager.Instance.handmenuRight.SetActive(true);

        // Write sampled information to disk using current time in name
        await WriteInformationToDisk($"Task3_{DateTime.Now:dd-MM_HH-mm-ss}.csv");

        // Reset vars (data)
        cleanedTiles = 0;
        CleanedTiles = 0;
        currentDirtyAreaIndex = 0;
        areaBeingCleaned = 1;
        timeStamp = 0f;
        informationFrames = new();

        // Acoustic feedback that task ended
        audioSource.PlayOneShot(taskCompletedSound);
    }

    // Return to home menu, abort game if running
    public async void ReturnHome()
    {
        if (taskIsRunning)
        {        
            // Stop game
            await StopTask1();
        }

        // Reset UI to start state (selection of game mode)
        UIManager.Instance.ResetUI();

        // Disable all tracking visuals by default
        ManageTracking.Instance.DisableVisualizers();

        // Go back to looking for reference target
        //ManageTracking.Instance.StartTrackingRefTarget();
    }

    // Callback for when user has started cleaning a single dirty area
    public void StartedCleaningArea()
    {
        // Start measuring time and distance
        //singleAreaTime = 0f;
        //singleAreaDistance = 0f;
        //prevPosCleaner = ManageTracking.Instance.gameImageTarget.transform.position;
        cleaning = true;

        Debug.Log("Reached firts dirty tile, started cleaning an area (any task)");
    }

    // Callback for when user has cleaned all tiles of a single dirty area
    public void FinishedCleaningSingleAreaT1T3()
    {
        // Acoustic feedback that single area has been cleaned
        audioSource.PlayOneShot(cleanedSingleAreaClip);

        // Reset dirty area tiles
        dirtyArea.SetActive(false);
        for (int i = 0; i < dirtyArea.transform.childCount; i++)
        {
            // Enable all tiles again
            dirtyArea.transform.GetChild(i).gameObject.SetActive(true);
        }

        // Count cleaned area
        areaBeingCleaned++;

        // Stop measuring time and distance for this dirty area
        cleaning = false;
        //singleAreaTimes.Add(singleAreaTime);
        //singleAreaDistances.Add(singleAreaDistance);

        Debug.Log("Finished cleaning an area");

        // Reset cleaned tiles count
        cleanedTiles = 0;
        CleanedTiles = 0;

        // Move to next dirty area if not completely finished
        if (currentTask == 1)
        {
            if (currentDirtyAreaIndex < dirtyAreaLocations3x4.Count - 1)
            {
                currentDirtyAreaIndex++;
            }
            else
            {
                // Callback for complete finish
                CleanedAllDirtyAreasT1();

                return;
            }

            // Attach relevant dirty area to next parent position
            dirtyArea.transform.SetParent(dirtyAreaLocations3x4[currentDirtyAreaIndex]);
        }
        else if (currentTask == 3)
        {
            if (currentDirtyAreaIndex < dirtyAreaLocations2x2.Count - 1)
            {
                currentDirtyAreaIndex++;
            }
            else
            {
                // Callback for complete finish
                CleanedAllDirtyAreasT3();

                return;
            }

            // Attach relevant dirty area to next parent position
            dirtyArea.transform.SetParent(dirtyAreaLocations2x2[currentDirtyAreaIndex]);
        }

        // Reset local orientation of dirty area
        dirtyArea.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));

        // Fade in next dirty area
        //StartCoroutine(FadeInDirtyArea(fadeDuration, 2f));
        dirtyArea.SetActive(true);
    }

    // Callback for when user has finished cleaning all dirty areas
    public async void CleanedAllDirtyAreasT1()
    {
        // Stop game
        await StopTask1();
    }

    // Callback for when user has finished cleaning all dirty areas
    public async void CleanedAllDirtyAreasT3()
    {
        // Stop game
        await StopTask3();
    }

    // Fade in dirty area
    public IEnumerator FadeInDirtyArea(float fadeDuration, float waitTimeBeforeFadeStart = 0f)
    {
        // Wait before initiating fade if specified
        yield return new WaitForSecondsRealtime(waitTimeBeforeFadeStart);

        // Set starting alpha to 0
        dirtyMats[0].color = new Color(dirtyMats[0].color.r, dirtyMats[0].color.g, dirtyMats[0].color.b, 0f);
        dirtyMats[1].color = new Color(dirtyMats[1].color.r, dirtyMats[1].color.g, dirtyMats[1].color.b, 0f);

        // Enable dirty area
        dirtyArea.SetActive(true);

        float passedTime = 0f;

        while (passedTime < fadeDuration)
        {
            // Set alpha
            dirtyMats[0].color = new Color(dirtyMats[0].color.r, dirtyMats[0].color.g, dirtyMats[0].color.b, Mathf.Lerp(0f, 1f, passedTime / fadeDuration));
            dirtyMats[1].color = new Color(dirtyMats[1].color.r, dirtyMats[1].color.g, dirtyMats[1].color.b, Mathf.Lerp(0f, 1f, passedTime / fadeDuration));

            // Increment passed time
            passedTime += Time.deltaTime;

            yield return null;
        }
    }

    // Method for sampling information. Called by Update at specified framerate
    private void SampleInformation()
    {
        // Determine timestamp
        timeStamp += samplingTimer;

        // Determine area id
        areaID = (cleaning) ? areaBeingCleaned : 0;

        // Tracking state of cleaner is set in/by ManageTracking

        // Determine cleaner position
        cleanerPos = ManageTracking.Instance.gameImageTarget.transform.position;

        // Determine head pose
        headPos = Camera.main.transform.position;
        headRot = Camera.main.transform.rotation;

        // Determine left wrist pose
        bool left = handAggregator.TryGetJoint(TrackedHandJoint.Wrist, XRNode.LeftHand, out HandJointPose leftWrist);
        leftWristPos = left ? leftWrist.Position : Vector3.zero;
        //leftWristRot = left ? leftWrist.Rotation : Quaternion.identity;

        // Determine right wrist pose
        bool right = handAggregator.TryGetJoint(TrackedHandJoint.Wrist, XRNode.RightHand, out HandJointPose rightWrist);
        rightWristPos = right ? rightWrist.Position : Vector3.zero;
        //rightWristRot = right ? rightWrist.Rotation : Quaternion.identity;

        // Create information frame based on current conditions
        InformationFrame informationFrame = new InformationFrame
        {
            Timestamp = timeStamp,
            AreaID = areaID,
            CleanerIsTracked = cleanerIsTracked,
            CleanerPos = cleanerPos,
            HeadPos = headPos,
            HeadRot = headRot,
            LeftWristPos = leftWristPos,
            //LeftWristRot = leftWristRot,
            RightWristPos = rightWristPos,
            //RightWristRot = rightWristRot
        };

        // Save information
        informationFrames.Add(informationFrame);
    }

    // Method for writing sampled information to disk
    public async Task WriteInformationToDisk(string filename)
    {
        // Determine storage location
        string filePath = Path.Combine(Application.persistentDataPath, filename);

        // Start building .cvs file using StringBuilder which is more efficient in terms of memory allocation for string building
        StringBuilder sb = new StringBuilder();

        // Define header
        sb.AppendLine("Timestamp,AreaID,CleanerIsTracked,CleanerX,CleanerY,CleanerZ," +
                      "HeadX,HeadY,HeadZ,HeadRotX,HeadRotY,HeadRotZ,HeadRotW," +
                      "LeftWristX,LeftWristY,LeftWristZ," +
                      "RightWristX,RightWristY,RightWristZ");

        // Build each line of information
        foreach (var info in informationFrames)
        {
            sb.AppendLine($"{info.Timestamp.ToString(CultureInfo.InvariantCulture)},{info.AreaID.ToString(CultureInfo.InvariantCulture)},{info.CleanerIsTracked.ToString(CultureInfo.InvariantCulture)}," +
                          $"{info.CleanerPos.x.ToString(CultureInfo.InvariantCulture)},{info.CleanerPos.y.ToString(CultureInfo.InvariantCulture)},{info.CleanerPos.z.ToString(CultureInfo.InvariantCulture)}," +
                          $"{info.HeadPos.x.ToString(CultureInfo.InvariantCulture)},{info.HeadPos.y.ToString(CultureInfo.InvariantCulture)},{info.HeadPos.z.ToString(CultureInfo.InvariantCulture)},{info.HeadRot.x.ToString(CultureInfo.InvariantCulture)},{info.HeadRot.y.ToString(CultureInfo.InvariantCulture)},{info.HeadRot.z.ToString(CultureInfo.InvariantCulture)},{info.HeadRot.w.ToString(CultureInfo.InvariantCulture)}," +
                          $"{info.LeftWristPos.x.ToString(CultureInfo.InvariantCulture)},{info.LeftWristPos.y.ToString(CultureInfo.InvariantCulture)},{info.LeftWristPos.z.ToString(CultureInfo.InvariantCulture)}," +
                          //$"{info.LeftWristRot.x.ToString(CultureInfo.InvariantCulture)},{info.LeftWristRot.y.ToString(CultureInfo.InvariantCulture)},{info.LeftWristRot.z.ToString(CultureInfo.InvariantCulture)},{info.LeftWristRot.w.ToString(CultureInfo.InvariantCulture)}," +
                          $"{info.RightWristPos.x.ToString(CultureInfo.InvariantCulture)},{info.RightWristPos.y.ToString(CultureInfo.InvariantCulture)},{info.RightWristPos.z.ToString(CultureInfo.InvariantCulture)}");
            //$"{info.RightWristRot.x.ToString(CultureInfo.InvariantCulture)},{info.RightWristRot.y.ToString(CultureInfo.InvariantCulture)},{info.RightWristRot.z.ToString(CultureInfo.InvariantCulture)},{info.RightWristRot.w.ToString(CultureInfo.InvariantCulture)}");
        }

        // Write information string to file asynchronously
        try
        {
            // Ensure writer is disposed when finished
            using StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);
            await writer.WriteAsync(sb.ToString());
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed wot write information to disk. Error: {e.Message}");
        }
    }

    #endregion
}