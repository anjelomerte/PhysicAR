using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    #region Properties

    #endregion


    #region Unity lifecycle

    #endregion


    #region Methods

    // Callback for initializing tutorial logic
    public void StartTutorial()
    {
        // Update UI
        UIManager.Instance.tutorialButton.SetActive(false);
        UIManager.Instance.surfaceBasedStartToggle.SetActive(false);
        UIManager.Instance.targetBasedStartToggle.SetActive(false);
        UIManager.Instance.returnFromTutButton.SetActive(true);

    }

    // Callback to go back to init scenario
    public void GoHome()
    {
        // Update UI
        UIManager.Instance.tutorialButton.SetActive(true);
        UIManager.Instance.surfaceBasedStartToggle.SetActive(true);
        UIManager.Instance.targetBasedStartToggle.SetActive(true);

        // Disable and reset all tutorial-related stuff/UI
    }

    #endregion
}
