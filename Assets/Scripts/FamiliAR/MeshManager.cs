/// <summary>
/// This script toggles spatial and hand mesh visualization
/// </summary>
/// 
using MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MeshManager : MonoBehaviour
{
    #region Class properties

    [Tooltip("ARMeshManager Object")]
    public ARMeshManager arMeshManager;

    [Tooltip("Spatial mesh prefabs (0: occlusion, 1: wireframe)")]
    public MeshFilter[] spatialMeshes;

    [Tooltip("Occlusion (0), wireframe (1) material")]
    public Material[] materials;

    [Tooltip("Left hand xr controller")]
    public ArticulatedHandController lhController;

    [Tooltip("Right hand xr controller")]
    public ArticulatedHandController rhController;

    #endregion


    #region Unity lifecycle

    #endregion

    #region Class methods

    // Callback to enable spatial mesh visualization
    public void ToggledSpatialMesh()
    {
        // Set prefab to wireframe prefab
        arMeshManager.meshPrefab = spatialMeshes[1];

        // Change material of every currently existing rendered mesh to wireframe material
        foreach (var mesh in arMeshManager.meshes)
        {
            mesh.GetComponent<MeshRenderer>().material = materials[1];
        }
    }

    // Callback to disable spatial mesh visualization
    public void UntoggledSpatialMesh()
    {
        // Set prefab to occlusion prefab
        arMeshManager.meshPrefab = spatialMeshes[0];

        // Change material of every currently existing rendered mesh to occlusion material
        foreach (var mesh in arMeshManager.meshes)
        {
            mesh.GetComponent<MeshRenderer>().material = materials[0];
        }
    }

    // Callback to enable hand meshes
    public void ToggledHandMesh()
    {
        lhController.model.GetComponent<RiggedHandMeshVisualizer>().ShowHandsOnTransparentDisplays = true;
        rhController.model.GetComponent<RiggedHandMeshVisualizer>().ShowHandsOnTransparentDisplays = true;
    }

    // Callback to disable hand meshes
    public void UtoggledHandMesh()
    {
        lhController.model.GetComponent<RiggedHandMeshVisualizer>().ShowHandsOnTransparentDisplays = false;
        rhController.model.GetComponent<RiggedHandMeshVisualizer>().ShowHandsOnTransparentDisplays = false;
    }

    #endregion
}
