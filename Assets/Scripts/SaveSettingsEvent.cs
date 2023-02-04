using UnityEngine;
using UnityEngine.UI;

public class SaveSettingsEvent : MonoBehaviour
{
    public delegate void OnSaveSettings();
    public static event OnSaveSettings OnSaveSettingsAction;

    [SerializeField]
    private Button _backButton;

    private void Start()
    {
        _backButton.onClick.AddListener(delegate () { OnSaveSettingsAction?.Invoke(); });
    }
}
