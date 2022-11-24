using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextOnSlide : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textToChange;
    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
    }

    public void UpdateText()
    {
        _textToChange.text = _slider.value.ToString();
    }

}
