using UnityEngine;

public class ResetObj : MonoBehaviour
{
    #region Class properties

    private Vector3 startingPos;
    private Quaternion startingRot;
    private Vector3 startingScale;

    #endregion


    #region Unity lifecycle

    // Start is called before the first frame update
    void Start()
    {
        // Chache starting pose
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        startingScale = transform.localScale;
    }

    #endregion


    #region Class methods

    public void OnReset()
    {
        // Reset manipulation object (this)
        transform.SetLocalPositionAndRotation(startingPos, startingRot);
        transform.localScale = startingScale;
    }

    #endregion
}
