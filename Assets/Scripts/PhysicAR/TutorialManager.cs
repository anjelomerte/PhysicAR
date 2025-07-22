using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    #region Properties

    [Tooltip("List of cubes used for in-situ tutorial")]
    public GameObject[] cubes = new GameObject[3];
    [Tooltip("Directions for forces to apply to demo cubes on test")]
    private Vector3[] forceDirections = new Vector3[] { 0.5f * Vector3.left + 1.5f * Vector3.up, 1.5f * Vector3.up, 0.5f * Vector3.right + 1.5f * Vector3.up };
    [Tooltip("Default local positions of cubes for reset")]
    private Vector3[] defaultCubePositions = new Vector3[] { new(-0.25f, 0f, 0f), new(-0.125f, 0f, 0f), new(0f, 0f, 0f) };

    [Tooltip("Flag whether returning to tutorial level after already finished. Is enabled after completing tutorial once")]
    private bool finishedTutorial = false;

    #endregion


    #region Unity lifecycle

    #endregion


    #region Methods

    // Callback for initializing tutorial logic
    public void InitializeTutorial()
    {
        // Update UI
        UIManager.Instance.tutorialButton.SetActive(false);
        UIManager.Instance.surfaceBasedStartToggle.SetActive(false);
        UIManager.Instance.targetBasedStartToggle.SetActive(false);

        UIManager.Instance.returnFromTutButton.SetActive(true);
        UIManager.Instance.startDialogTut.SetActive(true);

        // Disable far ray interaction
        UIManager.Instance.leftHandRay.SetActive(false);
        UIManager.Instance.rightHandRay.SetActive(false);

        // Enable meshing for in-situ tutorial
        GameManager.Instance.meshManager.enabled = true;
    }

    // Callback for starting tutorial
    public void StartTutorial()
    {
        // Update UI
        UIManager.Instance.startDialogTut.SetActive(false);
        UIManager.Instance.buttonDialogTut.SetActive(true);

        // Disable meshing again. Existing meshes persist
        GameManager.Instance.meshManager.enabled = false;
    }

    // Callback for successful completion of press button tutorial
    public void CompletedButtonTutorial()
    {
        // Update UI
        UIManager.Instance.buttonDialogTut.SetActive(false);
        UIManager.Instance.finishedDialogTut.SetActive(false);

        // Check if returning after already having finished tutorial
        if (finishedTutorial)
        {
            UIManager.Instance.finishedDialogTut.SetActive(true);
        }
        else
        {
            UIManager.Instance.solvedDialogTut.SetActive(true);
        }
    }

    // Callback for successful completion of solved user panel tutorial
    public void CompletedSolvedPanelTutorial()
    {
        // Update UI
        UIManager.Instance.solvedDialogTut.SetActive(false);
        UIManager.Instance.finishedDialogTut.SetActive(false);

        // Check if returning after already having finished tutorial
        if (finishedTutorial)
        {
            UIManager.Instance.finishedDialogTut.SetActive(true);
        }
        else
        {
            UIManager.Instance.anchoredDialogTut.SetActive(true);
        }
    }

    // Callback for successful completion of anchored user panel tutorial
    public void CompletedAnchoredPanelTutorial()
    {
        // Update UI
        UIManager.Instance.anchoredDialogTut.SetActive(false);
        UIManager.Instance.finishedDialogTut.SetActive(false);

        // Check if returning after already having finished tutorial
        if (finishedTutorial)
        {
            UIManager.Instance.finishedDialogTut.SetActive(true);
        }
        else
        {
            UIManager.Instance.insituDialogTut.SetActive(true);
        }
    }

    // Callback for successful completion of in-situ tutorial
    public void CompletedInsituTutorial()
    {
        // Update UI
        UIManager.Instance.insituDialogTut.SetActive(false);
        UIManager.Instance.finishedDialogTut.SetActive(true);

        // Flag as completed (at least once)
        finishedTutorial = true;
    }

    // Callback to go back to init scenario
    public void GoHome()
    {
        // Update UI
        UIManager.Instance.tutorialButton.SetActive(true);
        UIManager.Instance.surfaceBasedStartToggle.SetActive(true);
        UIManager.Instance.targetBasedStartToggle.SetActive(true);

        // Disable and reset all tutorial-related stuff/UI
        UIManager.Instance.startDialogTut.SetActive(false);
        UIManager.Instance.buttonDialogTut.SetActive(false);
        UIManager.Instance.solvedDialogTut.SetActive(false);
        UIManager.Instance.anchoredDialogTut.SetActive(false);
        UIManager.Instance.insituDialogTut.SetActive(false);

        // Re-enable far rays
        UIManager.Instance.leftHandRay.SetActive(true);
        UIManager.Instance.rightHandRay.SetActive(true);
    }

    // Actionize cubes for in-situ tutorial
    public void ActionizeCubes()
    {
        // Iterate demo cubes
        for (int i = 0; i < cubes.Length; i++)
        {
            // Enable gravity
            Rigidbody rb = cubes[i].GetComponent<Rigidbody>();
            rb.useGravity = true;

            // Reset to default pose
            cubes[i].transform.SetLocalPositionAndRotation(defaultCubePositions[i], Quaternion.identity);
            // Reset any velocity
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Add little force for demonstration purpose
            rb.AddForce(forceDirections[i], ForceMode.Impulse);
            // Add torque as well
            rb.AddTorque(Vector3.up, ForceMode.Impulse);
        }
    }

    #endregion
}
