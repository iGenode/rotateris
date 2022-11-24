using TMPro;
using UnityEngine;

public class GreyOutText : MonoBehaviour
{
    private TMP_InputField _inputField;
    private TextMeshProUGUI _text;
    private bool _prevState;
    private Color32 _greyedOutColor = new(104,104, 104, 255);

    private void Start()
    {
        _inputField = GetComponent<TMP_InputField>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        SetColor();
    }

    void Update()
    {
        if (_inputField.interactable != _prevState)
        {
            SetColor();
        }
    }

    private void SetColor()
    {
        _prevState = _inputField.interactable;
        _text.color = _prevState ? Color.white : _greyedOutColor;
    }
}
