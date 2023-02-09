using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollDropdownWithNavigation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Rect transform of the container of the scrollRect
    public RectTransform MaskTransform;

    private ScrollRect _scrollRect;
    private RectTransform _scrollTransform;
    private RectTransform _content;
    private bool _mouseOver = false;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _scrollTransform = _scrollRect.transform as RectTransform;
        _content = _scrollRect.content;
    }

    /// <summary>
    /// Specify an item to position it in the middle of ScrollRect
    /// </summary>
    /// <param name="target">The target that needs to be located</param>
    public void CenterOnItem(RectTransform target)
    {
        if (_mouseOver)
        {
            return;
        }
        // Item is here
        var itemCenterPositionInScroll = GetWorldPointInWidget(_scrollTransform, GetWidgetWorldPoint(target));
        // But must be here
        var targetPositionInScroll = GetWorldPointInWidget(_scrollTransform, GetWidgetWorldPoint(MaskTransform));
        // So it has to move this distance
        var difference = targetPositionInScroll - itemCenterPositionInScroll;
        difference.z = 0f;

        //clear axis data that is not enabled in the scrollrect
        if (!_scrollRect.horizontal)
        {
            difference.x = 0f;
        }

        if (!_scrollRect.vertical)
        {
            difference.y = 0f;
        }

        //this is the wanted new position for the content
        var newAnchoredPosition = _content.anchoredPosition3D + difference;
        _content.anchoredPosition3D = newAnchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mouseOver = false;
    }

    private Vector3 GetWidgetWorldPoint(RectTransform target)
    {
        //pivot position + item size has to be included
        var pivotOffset = new Vector3(
            (0.5f - target.pivot.x) * target.rect.size.x,
            (0.5f - target.pivot.y) * target.rect.size.y,
            0f);
        var localPosition = target.localPosition + pivotOffset;
        return target.parent.TransformPoint(localPosition);
    }

    private Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
    {
        return target.InverseTransformPoint(worldPoint);
    }
}
