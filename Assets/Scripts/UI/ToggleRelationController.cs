using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for implementing custom relation logic between menu toggles
/// </summary>
public class ToggleRelationController : MonoBehaviour
{
    [SerializeField]
    private Toggle[] _presetToggles;
    [SerializeField]
    private Toggle _customSizeToggle;

    // Switching on one of the presets
    public void OnPresetToggledOn()
    {
        // Check if customSize is on, if so disable it
        DisableIfOn(_customSizeToggle);
    }

    // Switching off one of the presets
    public void OnPresetToggledOff()
    {

    }

    // Switching on custom size toggle
    public void OnCustomSizeToggledOn()
    {
        foreach (Toggle presetToggle in _presetToggles)
        {
            DisableIfOn(presetToggle);
        }
    }

    // Switching off custom size toggle
    public void OnCustomSizeToggledOff()
    {
        // If any of the preset toggles is on when cutsom size was just disabled, then do nothing
        foreach (Toggle presetToggle in _presetToggles)
        {
            if (presetToggle.isOn)
            {
                return;
            }
        }
        // Else toggle one of the presets on
        _presetToggles[_presetToggles.Length / 2].isOn = true;
    }

    private void DisableIfOn(Toggle toggle)
    {
        if (toggle.isOn)
        {
            toggle.isOn = false;
        }
    }
}
