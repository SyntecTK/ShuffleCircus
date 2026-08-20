using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ICanvasRaycastFilter
{
    [Header("Hover Target")]
    [SerializeField] private Transform scaleTarget;

    [Header("Scale")]
    [SerializeField] private Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);
    [SerializeField] private Vector3 normalScale = Vector3.one;

    [Header("Hover Hit Area")]
    [Range(0.05f, 1f)]
    [SerializeField] private float hitAreaHeightPercent = 0.25f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (scaleTarget == null)
        {
            scaleTarget = transform.parent != null ? transform.parent : transform;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        scaleTarget.localScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        scaleTarget.localScale = normalScale;
    }

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (rectTransform == null)
        {
            return false;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var localPoint))
        {
            return false;
        }

        var rect = rectTransform.rect;
        var halfAllowedHeight = (rect.height * hitAreaHeightPercent) * 0.5f;

        // Full width is valid; only clamp Y to a centered horizontal strip.
        return Mathf.Abs(localPoint.y) <= halfAllowedHeight;
    }
}
