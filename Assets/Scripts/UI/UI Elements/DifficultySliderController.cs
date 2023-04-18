using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySliderController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textToChange;
    [SerializeField]
    private PlayingFieldSettings _settings;
    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.value = _settings.DifficultyLevel;
        UpdateText();
    }

    public void UpdateText()
    {
        _textToChange.text = _slider.value.ToString();
    }
}
