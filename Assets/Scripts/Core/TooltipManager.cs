using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    private TMP_Text textComponent;
    private bool isVisible;

    private void Awake()
    {
        CacheReferences();

        if (textComponent != null)
        {
            textComponent.text = string.Empty;
        }

        isVisible = gameObject.activeSelf;
    }

    void Update()
    {
        if (!isVisible)
        {
            return;
        }

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
            Vector3 offset = new Vector3(17f, -10f, 0f);
            transform.position = worldPoint + offset;
        }
    }

    public void ShowTooltip(string tooltipText)
    {
        CacheReferences();

        if (textComponent == null)
        {
            Debug.LogWarning("TooltipManager could not find a TMP_Text child.");
            return;
        }

        gameObject.SetActive(true);
        isVisible = true;
        textComponent.text = tooltipText;

        textComponent.ForceMeshUpdate();

        RectTransform tooltipRect = GetComponent<RectTransform>();
        Vector2 preferredSize = textComponent.GetPreferredValues(tooltipText, 280f, 0f);

        float padding = 20f;
        tooltipRect.sizeDelta = new Vector2(300f, preferredSize.y + padding);
    }

    public void HideTooltip()
    {
        CacheReferences();

        if (textComponent == null)
        {
            gameObject.SetActive(false);
            isVisible = false;
            return;
        }

        isVisible = false;
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }

    private void CacheReferences()
    {
        if (textComponent == null)
        {
            textComponent = GetComponentInChildren<TMP_Text>(true);
        }
    }
}
