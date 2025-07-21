using UnityEngine;
using Vuforia;

public class CleanCollider : MonoBehaviour
{
    #region Properties

    [Tooltip("Default tag of dirty tiles")]
    public string tileTag = "dirtyTile";

    #endregion


    #region Unity lifecycle


    #endregion


    #region Methods

    // Check if tracked cleaner overlaps with dirty tile completely
    private void OnTriggerStay(Collider objCollidedWith)
    {
        // First check if colliding with dirty tile
        if (objCollidedWith.CompareTag(tileTag))
        {
            // Get current worldspace bounds of cleaner and dirty tile
            Bounds tileBounds = objCollidedWith.bounds;
            Bounds cleanerBounds = GetComponent<Collider>().bounds;

            // Check if cleaner bounds contains the dirty tile bounds
            if (cleanerBounds.Contains(tileBounds.min) && tileBounds.Contains(tileBounds.max))
            {
                //Debug.Log("Cleaner contains complete dirty tile");

                // Cleaner contains whole dirty tile -> Disable dirty tile
                objCollidedWith.gameObject.SetActive(false);

                // Increment number of cleaned tiles in GameManager
                GameManager.Instance.CleanedTiles++;
            }
        }
    }

    #endregion
}
