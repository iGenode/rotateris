using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldController : MonoBehaviour
{
    public int DefaultValue = 3;

    private TMP_InputField _inputField;
    private TextMeshProUGUI _text;
    private bool _prevState;
    private Color32 _greyedOutColor = new(104, 104, 104, 255);

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

    // Method to increase inputField's value
    public void IncreaseValue()
    {
        var currentNumber = int.Parse(_inputField.text);
        _inputField.text = currentNumber < 99 ? (int.Parse(_inputField.text) + 1).ToString() : "99";
    }

    // Method to decrease inputField's value
    public void DecreaseValue()
    {
        var currentNumber = int.Parse(_inputField.text);
        _inputField.text = currentNumber > 1 ? (int.Parse(_inputField.text) - 1).ToString() : "1";
    }

    // Method for keeping default value in case the inputField is left empty
    public void SetDefaultIfEmpty()
    {
        if (string.IsNullOrEmpty(_inputField.text))
        {
            _inputField.text = DefaultValue.ToString();
        }
    }

    // Method for greying out the inputField if it's not interactable
    private void SetColor()
    {
        _prevState = _inputField.interactable;
        _text.color = _prevState ? Color.white : _greyedOutColor;
    }
}
