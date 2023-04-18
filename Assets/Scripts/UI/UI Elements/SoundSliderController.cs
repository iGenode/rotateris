using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SoundSliderController : MonoBehaviour
{
    [Header("Audio Mixer Settings")]
    [SerializeField]
    private AudioMixer _audioMixer;
    [Tooltip("Audio Mixer's exposed parameter name")]
    [SerializeField]
    private string _exposedName;
    [Tooltip("PlayerPref's name")]
    [SerializeField]
    private string _prefName;

    [Header("UI Label")]
    [SerializeField]
    private TextMeshProUGUI _label;

    private Slider _slider;

    public void SaveValueToPrefs()
    {
        _audioMixer.GetFloat(_exposedName, out float volume);
        PlayerPrefs.SetFloat(_prefName, volume);
    }

    private void Start()
    {
        _slider = GetComponent<Slider>();

        _slider.onValueChanged.AddListener(SetSoundSliderLabel);

        // Getting current mixer volume
        _audioMixer.GetFloat(_exposedName, out float volume);
        // Normalyzing current mixer volume to user friendly numbers and setting slider value
        _slider.value = Utils.NormalizeForUISlider(volume);
    }

    private void SetSoundSliderLabel(float value)
    {
        _label.text = Utils.NormalizeForAudioLabel(value).ToString();

        _audioMixer.SetFloat(_exposedName, Utils.NormalizeForMixer(value));
    }

    private void OnEnable()
    {
        if (_slider)
        {
            _slider.onValueChanged.AddListener(SetSoundSliderLabel);
        }
        SaveSettingsEvent.OnSaveSettingsAction += SaveValueToPrefs;
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(SetSoundSliderLabel);
        SaveSettingsEvent.OnSaveSettingsAction += SaveValueToPrefs;
    }
}
