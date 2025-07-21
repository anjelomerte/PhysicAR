/// <summary>
/// This script provides callback functions for switching locales
/// </summary>

using UnityEngine;
using UnityEngine.Localization.Settings;

public class SwitchLocale : MonoBehaviour
{
    #region Class properties

    #endregion


    #region

    private void Start()
    {
        // German locale by default
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
    }

    #endregion

    #region Class methods

    public void ChangeLocaleToGerman()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
    }

    public void ChangeLocaleToEnglish()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
    }

    #endregion
}
