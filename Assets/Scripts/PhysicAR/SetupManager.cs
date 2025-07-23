#if ENABLE_WINMD_SUPPORT
using System;
#endif
using UnityEngine;

public class SetupManager : MonoBehaviour
{
    #region Properties

    [Tooltip("Visible wireframe mesh prefab")]
    public MeshFilter wireframeMeshPrefab;
    [Tooltip("Invisible mesh prefab (without occlusion)")]
    public MeshFilter transparentMeshPrefab;

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
            bool result = await Windows.System.Launcher.LaunchUriAsync(new System.Uri("ms-hololenssetup://EyeTracking"));

        }, false);
#endif
    }

    // Toggle meshing during setup to generate initial mesh to use during tutorial
    public void ToggleMesh()
    {
        // Assign visible wireframe mesh prefab
        GameManager.Instance.meshManager.meshPrefab = wireframeMeshPrefab;

        // Enable meshing
        GameManager.Instance.meshManager.enabled = true;
    }

    // Untoggle meshing again during setup
    public void UntoggleMesh()
    {
        // Assign invisble wireframe mesh prefab
        GameManager.Instance.meshManager.meshPrefab = transparentMeshPrefab;

        // Disable meshing
        GameManager.Instance.meshManager.enabled = false;
    }

#endregion
}
