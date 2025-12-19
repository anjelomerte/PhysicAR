using UnityEngine;

public class CleanCollider : MonoBehaviour
{
    #region Properties

    [Tooltip("Possible tags of dirty tiles")]
    public string [] tileTags;


    #endregion


    #region Unity lifecycle


    #endregion


    #region Methods

    // Check if tracked cleaner overlaps with dirty tile completely
    private void OnTriggerStay(Collider objCollidedWith)
    {
        // First check if colliding with dirty tile
        if (objCollidedWith.CompareTag(tileTags[0]) || objCollidedWith.CompareTag(tileTags[1]) || objCollidedWith.CompareTag(tileTags[2]) || objCollidedWith.CompareTag(tileTags[3]) || objCollidedWith.CompareTag(tileTags[4]))
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

                // Only for task 2 (static dirty areas)
                if (GameManager.Instance.currentTask == 2)
                {
                    // Set area id of currently cleaned one (based on tag. E.g. "area_3")
                    GameManager.Instance.areaBeingCleaned = int.Parse(objCollidedWith.tag.Split("_")[1]);
                }
            }
        }
    }

    #endregion
}
