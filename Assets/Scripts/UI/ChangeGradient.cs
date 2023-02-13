using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeGradient : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TMP_ColorGradient _normalGradient;
    [SerializeField]
    private TMP_ColorGradient _highlightedGradient;
    private TextMeshProUGUI _text;

    private void Start()
    {
        //_text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //_text.colorGradientPreset = _highlightedGradient;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //_text.colorGradientPreset = _normalGradient;
    }

    private void OnDisable()
    {
        //_text.colorGradientPreset = _normalGradient;
    }
}
