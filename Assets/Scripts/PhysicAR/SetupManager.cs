#if ENABLE_WINMD_SUPPORT
using System;
#endif
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

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

    #endregion


    #region Unity lifecycle

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

        // Start hand calibration routine
        StartCoroutine(CalibrateHandDistance());
    }

    private IEnumerator CalibrateHandDistance()
    {
        // Initialize timer
        float timer = calibrationTime;

        while (timer > 0f)
        {
            yield return new WaitForSeconds(0.5f);

            // Adjust timer
            timer -= 0.5f;

            // Sample head-hand distance
        }

        // Finished calibration, play sound
        audioSource.PlayOneShot(finishedHandCalibSound);

        // Update UI
        handCalibInstructText.gameObject.SetActive(false);
        handCalibCountdownText.gameObject.SetActive(true);
        UIManager.Instance.handCalibCountDown.SetActive(false);

        // Enable handmenu again
        UIManager.Instance.handmenuLeft.SetActive(true);
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
