/// <summary>
/// This script can be used to convert text to speech. 
/// Needs: Audio source, text
/// </summary>

using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;
using UnityEngine;
using TMPro;

public class InstructionToSpeech : MonoBehaviour
{
    public static InstructionToSpeech instance;

    #region Class properties

    [Tooltip("Text-To-Speech sub")]
    private TextToSpeechSubsystem textToSpeechSubsystem;

    [SerializeField, Tooltip("Audio source for interactable cubes panel")]
    private AudioSource cubesAudioSource;
    [SerializeField, Tooltip("Audio source for button slider panel")]
    private AudioSource buttonSliderAudioSource;
    [SerializeField, Tooltip("Audio source for piano panel")]
    private AudioSource pianoAudioSource;

    [SerializeField, Tooltip("Text of interactable cubes panel")]
    private TMP_Text cubesText;
    [SerializeField, Tooltip("Text of interactable UI panel")]
    private TMP_Text buttonSliderText;
    [SerializeField, Tooltip("Text of interactable piano panel")]
    private TMP_Text pianoText;

    #endregion


    #region Unity lifecycle

    void Awake()
    {
        // Make sure only one instance of this class exists
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        // Get TTS subsystem initially
         textToSpeechSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<TextToSpeechSubsystem>();
    }

    #endregion


    #region Class methods

    // Callbacks (on gaze)
    public void SpeakCubesInstruction()
    {
        if (textToSpeechSubsystem != null)
        {
            // Speak message
            textToSpeechSubsystem.TrySpeak(cubesText.text, cubesAudioSource);
        }

    }

    public void SpeakPianoInstruction()
    {
        if (textToSpeechSubsystem != null)
        {
            // Speak message
            textToSpeechSubsystem.TrySpeak(pianoText.text, pianoAudioSource);
        }
    }

    public void SpeakButtonSliderInstruction()
    {
        if (textToSpeechSubsystem != null)
        {
            // Speak message
            textToSpeechSubsystem.TrySpeak(buttonSliderText.text, buttonSliderAudioSource);
        }
    }

    #endregion
}
