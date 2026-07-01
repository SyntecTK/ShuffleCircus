using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Transform originalParent;
    private Image image;
    private Canvas canvas;
    private CardSlot sourceSlot;
    private bool wasDropped;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        // Prefer nearest parent canvas, fallback to tagged canvas for older scene setups.
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            GameObject taggedCanvas = GameObject.FindGameObjectWithTag("Canvas");
            if (taggedCanvas != null)
            {
                canvas = taggedCanvas.GetComponent<Canvas>();
            }
        }

        if (canvas == null)
        {
            Debug.LogError("CardDrag: No Canvas found for drag calculations.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameOver) return;

        originalParent = rectTransform.parent;
        sourceSlot = originalParent.GetComponent<CardSlot>();
        wasDropped = false;
        image.raycastTarget = false;

        CardData card = GetComponent<CardData>();
        if (sourceSlot != null && card != null)
        {
            sourceSlot.RemoveCard(card);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameOver) return;

        if (canvas == null)
        {
            return;
        }

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameOver) return;

        image.raycastTarget = true;
        if (!wasDropped && sourceSlot != null)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            CardData cardData = GetComponent<CardData>();
            if (sourceSlot != null && cardData != null)
            {
                sourceSlot.RestoreCard(cardData);
            }
        }

    }

    public void MarkDropped()
    {
        wasDropped = true;
    }

    public void RestoreToOriginalPosition()
    {
        rectTransform.SetParent(originalParent);
        rectTransform.localPosition = Vector3.zero;
    }
}
