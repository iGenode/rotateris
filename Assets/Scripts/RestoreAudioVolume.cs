using System;
using UnityEngine;
using UnityEngine.Audio;

public class RestoreAudioVolume : MonoBehaviour
{
    [SerializeField]
    private AudioMixer _auioMixer;
    [SerializeField]
    private VolumePref[] _volumePrefs;

    [Serializable]
    private struct VolumePref
    {
        public string ExposedPropertyName;
        public string PrefName;
        public float DefaultValue;
    }

    private void Start()
    {
        foreach (var pref in _volumePrefs)
        {
            _auioMixer.SetFloat(pref.ExposedPropertyName, PlayerPrefs.GetFloat(pref.PrefName, pref.DefaultValue));
        }
    }
}
