/// <summary>
/// Switches objects within a list of objects so that only one is active. Cycles through
/// </summary>

using MixedReality.Toolkit.UX;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSliderBehaviors : MonoBehaviour
{
    #region Class properties

    [Tooltip("List of objects")]
    [SerializeField] private List<GameObject> objects = new List<GameObject>();

    [Tooltip("Current index of active object")]
    [SerializeField] private int currentObjIndex = 0;

    [Tooltip("Shared material for 3D objects")]
    [SerializeField] private Material objectMat;
    #endregion


    #region Unity lifecycle

    #endregion


    #region Class methods

    // Callback for button to switch object
    public void SwitchObject()
    {
        // Increase obj index to enable NEXT obj 
        if (currentObjIndex < objects.Count - 1)
        {
            // Regular advance through list
            currentObjIndex++;

            // Disable last object, enable new
            objects[currentObjIndex - 1].SetActive(false);
            objects[currentObjIndex].SetActive(true);
        }
        else
        {
            // Reached end of list, cycle through
            currentObjIndex = 0;

            // Disable last object, enable new
            objects[^1].SetActive(false);
            objects[currentObjIndex].SetActive(true);
        }
    }

    // Callback for slider to change object material color
    public void ChangeObjectColor(SliderEventData eventData)
    {
        // Get slider current value
        float sliderValue = eventData.NewValue;

        // Change material color (shared material)
        objectMat.color = new Color(2.0f * sliderValue, 2.0f * (1 - sliderValue), 0);
    }

    // Callback for button to change object material color
    public void ChangeObjColorButton()
    {
        // Generate random rgb combination
        float red = Random.Range(0f, 1f);
        float green = Random.Range(0f, 1f);
        float blue = Random.Range(0f, 1f);

        // Set material color
        objectMat.color = new Color(red, green, blue);
    }

    #endregion
}
