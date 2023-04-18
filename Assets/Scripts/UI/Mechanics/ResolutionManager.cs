using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class ResolutionManager : MonoBehaviour
{
    [SerializeField]
    private Toggle _fullscreenToggle;
    private TMP_Dropdown _resolutionsDropdown;

    private List<Resolution> _filteredResolutions;
    private int _currentResolutionIndex = 0;

    private void Start()
    {
        _resolutionsDropdown = GetComponent<TMP_Dropdown>();
        _resolutionsDropdown.ClearOptions();

        _fullscreenToggle.isOn = Screen.fullScreen;

        // Filtering only the same refresh rate resolutions
        // Refresh rate is cast to int to prevent odd refresh rates from filtering off all resolutions
        _filteredResolutions =
            Screen.resolutions.Where(
                res => ((int)res.refreshRateRatio.value).Equals((int)Screen.currentResolution.refreshRateRatio.value)
            ).ToList();

        List<string> options = new();
        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            options.Add($"{_filteredResolutions[i].width}x{_filteredResolutions[i].height} {(int)_filteredResolutions[i].refreshRateRatio.value}hz");
            if (_filteredResolutions[i].width == Screen.width && _filteredResolutions[i].height == Screen.height)
            {
                _currentResolutionIndex = i;
            }
        }

        _resolutionsDropdown.AddOptions(options);
        _resolutionsDropdown.value = _currentResolutionIndex;
        _resolutionsDropdown.RefreshShownValue();
    }

    public void SetResolution(int dropdownIndex)
    {
        Resolution resolution = _filteredResolutions[dropdownIndex];
        Screen.SetResolution(resolution.width, resolution.height, _fullscreenToggle.isOn);
    }

    public void ToggleFullscreen(bool isOn)
    {
        Screen.fullScreen = isOn;
    }
}
