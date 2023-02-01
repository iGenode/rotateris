using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class OnToggled : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _onToggledOn;
    [SerializeField]
    private UnityEvent _onToggledOff;

    void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(ProcessToggleState);
    }

    private void ProcessToggleState(bool isOn)
    {
        if (isOn)
        {
            _onToggledOn.Invoke();
        } 
        else
        {
            _onToggledOff.Invoke();
        }
    }

}
