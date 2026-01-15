#if ENABLE_WINMD_SUPPORT
using System;
#endif
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
