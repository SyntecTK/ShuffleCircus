using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    private TMP_Text textComponent;
    private void Start()
    {
        textComponent = GetComponentInChildren<TMP_Text>();
        HideTooltip();
    }

    void Update()
    {
        RectTransform parentRect = transform.parent as RectTransform;
        Canvas canvas = GetComponentInParent<Canvas>();

        if (parentRect == null || canvas == null) return;

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            parentRect,
            Input.mousePosition,
            uiCamera,
            out Vector3 worldPoint))
        {
            Vector3 offset = new Vector3(17f, -15f, 0f);
            transform.position = worldPoint + offset;
        }
    }

    public void ShowTooltip(string tooltipText)
    {
        gameObject.SetActive(true);
        textComponent.text = tooltipText;

        textComponent.ForceMeshUpdate();

        RectTransform tooltipRect = GetComponent<RectTransform>();
        Vector2 preferredSize = textComponent.GetPreferredValues(tooltipText, 280f, 0f);

        float padding = 20f;
        tooltipRect.sizeDelta = new Vector2(300f, preferredSize.y + padding);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}
