using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using MixedReality.Toolkit.Subsystems;
using UnityEngine.XR;
using MixedReality.Toolkit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
using System.IO;

public class SetupManager : MonoBehaviour
{
    #region Properties

    [Tooltip("Visible wireframe mesh filter")]
    public MeshFilter wireframeMesh;
    [Tooltip("Visible wireframes mesh material")]
    public Material wireframeMeshMat;
    [Tooltip("Invisible mesh filter (no occlusion)")]
    public MeshFilter transparentMesh;

    [Tooltip("Hand calibration time in seconds")]
    public int calibrationTime;
    [Tooltip("Countdown time for hand calibration in seconds")]
    public int countdownTime;
    [Tooltip("Countdown text for hand calibration")]
    public TMP_Text handCalibCountdownText;
    [Tooltip("Hand calibration instruction text")]
    public TMP_Text handCalibInstructText;

    [Tooltip("Global audio source")]
    public AudioSource audioSource;
    [Tooltip("Finished hand calibration audio sound")]
    public AudioClip finishedHandCalibSound;

    [Tooltip("Hands aggergator subsystem used to access joint data e.g. wrist pose")]
    private HandsAggregatorSubsystem handAggregator;

    [Tooltip("List of cached head positions during hand calibration")]
    private List<Vector3> headPositions;
    [Tooltip("List of cached game target positions during calibration")]
    private List<Vector3> gameTargetPositions;

    #endregion


    #region Unity lifecycle

    private void Start()
    {
        // Cache hands aggergator for getting wrist poses later
        handAggregator = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
    }

    #endregion


    #region Methods

    // Trigger eye calibration during setup/before tutorial or game
    public void TriggerEyeCalibration()
    {
#if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
            bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-hololenssetup://EyeTracking"));

        }, false);
#endif
    }

    // Trigger hand (max. distance) calibration
    public void TriggerHandCalibration()
    {
        // Start countdown to hand calibration
        StartCoroutine(CountdownToHandCalibration());
    }

    // Countdown routine for hand calibration
    private IEnumerator CountdownToHandCalibration()
    {
        // Disable handmenu
        UIManager.Instance.handmenuLeft.SetActive(false);

        // Initialize countdown
        int counter = countdownTime;
        handCalibCountdownText.text = counter.ToString();
        handCalibCountdownText.gameObject.SetActive(true);
        handCalibInstructText.gameObject.SetActive(false);

        while (counter > 0)
        {
            yield return new WaitForSeconds(1f);

            // Increase counter and change display
            counter--;
            handCalibCountdownText.text = counter.ToString();
        }

        // Update UI
        handCalibCountdownText.gameObject.SetActive(false);
        handCalibInstructText.gameObject.SetActive(true);

        // Initialize containers
        headPositions = new();
        gameTargetPositions = new();

        // Start hand calibration routine
        StartCoroutine(CalibrateHandDistance());
    }

    private IEnumerator CalibrateHandDistance()
    {
        // Initialize timer
        float timer = calibrationTime;

        while (timer > 0f)
        {
            yield return new WaitForSeconds(0.25f);

            // Adjust timer
            timer -= 0.25f;

            // Sample head and hand positions
            //bool left = handAggregator.TryGetJoint(TrackedHandJoint.Wrist, XRNode.LeftHand, out HandJointPose leftWrist);
            //Vector3 leftWristPos = left ? leftWrist.Position : Vector3.zero;
            //bool right = handAggregator.TryGetJoint(TrackedHandJoint.Wrist, XRNode.RightHand, out HandJointPose rightWrist);
            //Vector3 rightWristPos = right ? rightWrist.Position : Vector3.zero;
            Vector3 headPos = Camera.main.transform.position;
            Vector3 cleanerPos = ManageTracking.Instance.gameImageTarget.transform.position;

            // Cache these values
            headPositions.Add(headPos);
            gameTargetPositions.Add(cleanerPos);
        }

        // Finished calibration, play sound
        audioSource.PlayOneShot(finishedHandCalibSound);

        // Update UI
        handCalibInstructText.gameObject.SetActive(false);
        handCalibCountdownText.gameObject.SetActive(true);
        UIManager.Instance.handCalibCountDown.SetActive(false);

        // Enable handmenu again
        UIManager.Instance.handmenuLeft.SetActive(true);

        // Store information to disk
        WriteInformationToDisk($"HandCalibration_{DateTime.Now:dd-MM_HH-mm-ss}.csv");

        // Disable game target tracking again
        ManageTracking.Instance.gameImageTarget.enabled = false;
    }

    // Method for writing sampled information to disk
    public async Task WriteInformationToDisk(string filename)
    {
        // Determine storage location
        string filePath = Path.Combine(Application.persistentDataPath, filename);

        // Start building .cvs file using StringBuilder which is more efficient in terms of memory allocation for string building
        StringBuilder sb = new StringBuilder();

        // Define header
        sb.AppendLine("HeadX,HeadY,HeadZ,CleanerX,CleanerY,CleanerZ");

        // Build each line of information
        for (int i = 0; i < headPositions.Count; i++)
        {
            sb.AppendLine($"{headPositions[i].x.ToString(CultureInfo.InvariantCulture)},{headPositions[i].y.ToString(CultureInfo.InvariantCulture)},{headPositions[i].z.ToString(CultureInfo.InvariantCulture)}," +
                          $"{gameTargetPositions[i].x.ToString(CultureInfo.InvariantCulture)},{gameTargetPositions[i].y.ToString(CultureInfo.InvariantCulture)},{gameTargetPositions[i].z.ToString(CultureInfo.InvariantCulture)}");
                          //$"{info.LeftWristRot.x.ToString(CultureInfo.InvariantCulture)},{info.LeftWristRot.y.ToString(CultureInfo.InvariantCulture)},{info.LeftWristRot.z.ToString(CultureInfo.InvariantCulture)},{info.LeftWristRot.w.ToString(CultureInfo.InvariantCulture)}," +
                          //$"{rightHandPositions[i].x.ToString(CultureInfo.InvariantCulture)},{rightHandPositions[i].y.ToString(CultureInfo.InvariantCulture)},{rightHandPositions[i].z.ToString(CultureInfo.InvariantCulture)}");
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
            Debug.LogError($"Failed wot write hand calibration information to disk. Error: {e.Message}");
        }
    }

    // Toggle meshing during setup to generate initial mesh to use during tutorial
    public void ToggleMesh()
    {
        ARMeshManager mm = GameManager.Instance.meshManager;

        // Assign visible wireframe mesh prefab
        mm.meshPrefab = wireframeMesh;

        // Enable meshing
        mm.enabled = true;

        // Change material of every currently existing rendered mesh to wireframe material and enable renderer
        foreach (var mesh in mm.meshes)
        {
            mesh.GetComponent<MeshRenderer>().material = wireframeMeshMat;
            mesh.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    // Untoggle meshing again during setup
    public void UntoggleMesh()
    {
        ARMeshManager mm = GameManager.Instance.meshManager;

        // Assign invisble wireframe mesh prefab
        mm.meshPrefab = transparentMesh;

        // Disable renderer components
        foreach (var mesh in mm.meshes)
        {
            mesh.GetComponent<MeshRenderer>().enabled = false;
        }

        // Disable meshing
        mm.enabled = false;
    }

#endregion
}
