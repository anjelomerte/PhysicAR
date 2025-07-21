/// <summary>
/// This script initializes the scene. AR playground is placed in front of user on start. Handmenu is enabled by default
/// </summary>

using UnityEngine;

public class SceneSettings : MonoBehaviour
{
    #region Class properties

    [SerializeField, Tooltip("Parent object ARPlayground housing all interactables")]
    private GameObject arPlayground;
    [SerializeField, Tooltip("Default distance offset of playground to user")]
    private float distanceOffset;
    [SerializeField, Tooltip("Default height offset of playground to user head height")]
    private float heightOffset;

    [SerializeField, Tooltip("Handmenu left")]
    private GameObject handmenuLeft;

    #endregion


    #region Unity lifecycle

    #endregion


    #region Class methods
     
    // Start is called before the first frame update
    public void SpawnArPlayground()
    {
        // Get user head (main camera)
        Transform userHead = Camera.main.transform;

        // Parent playground to main camera
        arPlayground.transform.SetParent(userHead, false);

        // Put playground in front of user (gaze direction)
        arPlayground.transform.SetLocalPositionAndRotation(Vector3.forward, Quaternion.identity);

        // Put playground on same height as user head
        Vector3 modifiedPos = arPlayground.transform.position;
        arPlayground.transform.SetParent(userHead, false);
        modifiedPos.y = userHead.position.y;
        arPlayground.transform.position = modifiedPos;

        // Place playground at given distance from user
        arPlayground.transform.localPosition = arPlayground.transform.localPosition.normalized * distanceOffset;

        // Unparent playground
        arPlayground.transform.SetParent(null);

        // Make playground face user
        arPlayground.transform.LookAt(userHead);
        // Rotate playground about y-axis for 180� otherwise inverted orientation
        arPlayground.transform.Rotate(Vector3.up, 180);

        // Apply height offset
        arPlayground.transform.position += Vector3.up * heightOffset;

        // Enable playground
        arPlayground.SetActive(true);

        //// Get user head transform
        //Transform userHead = Camera.main.transform;

        //// Determine target position of playground (right in front of user based on offset)
        //Vector3 direction = userHead.position + userHead.forward;
        //direction = new Vector3(direction.x, userHead.position.y, direction.z);
        //Vector3 targetPos = direction * distanceOffset + Vector3.up * heightOffset;

        //// Set position of playground
        //arPlayground.transform.position = targetPos;

        //// Make playground face user
        //Transform targetTransform = userHead;
        //targetTransform.position = new Vector3(targetTransform.position.x, arPlayground.transform.position.y, targetTransform.position.z);
        //arPlayground.transform.LookAt(2 * arPlayground.transform.position - targetTransform.position);

        //// Enable playground
        //arPlayground.SetActive(true);
    }

    #endregion
}
