using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SetDefault : MonoBehaviour
{
    public int DefaultValue = 3;
    private TMP_InputField _inputText;

    private void Start()
    {
        _inputText = GetComponent<TMP_InputField>();
    }

    public void SetDefaultIfEmpty()
    {
        if (string.IsNullOrEmpty(_inputText.text))
        {
            _inputText.text = DefaultValue.ToString();
        }
    }
}
