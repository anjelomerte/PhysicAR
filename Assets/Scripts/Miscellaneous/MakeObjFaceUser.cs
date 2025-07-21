/// <summary>
/// This script makes a given object face the user on Start but on the height of the object itself
/// </summary>

using UnityEngine;

public class MakeObjFaceUser : MonoBehaviour
{
    #region Class properties

    [SerializeField, Tooltip("Rotate object only around local y-axis. This means the object faces along the shortest path from object to user plane")]
    private bool yRotOnly = false;
    [SerializeField, Tooltip("Update continuously to face user all the time")]
    private bool continuous = false;

    #endregion


    #region Unity lifecycle

    // Face user on enabling
    private void OnEnable()
    {
        FaceUser();
    }

    // Face user constantly
    private void Update()
    {
        if (continuous)
        {
            FaceUser();
        }
    }

    #endregion


    #region Class methods

    // Method to make this object face user
    private void FaceUser()
    {
        // Differentiate
        if (yRotOnly)
        {
            // Make object face user along shortest path to user plane
            Transform targetTransform = Camera.main.transform;
            targetTransform.position = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
            //targetTransform.rotation = Quaternion.LookRotation(2 * transform.position - targetTransform.position);

            transform.LookAt(2 * transform.position - targetTransform.position);
        }
        else
        {
            // Make object face user directly
            transform.LookAt(Camera.main.transform);
            transform.Rotate(Vector3.up, 180f);
        }
    }

    #endregion
}
