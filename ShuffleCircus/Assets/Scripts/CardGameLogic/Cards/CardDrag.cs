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
    private int originalSiblingIndex;
    private Vector2 originalAnchoredPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;
    private CardData card;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        card = GetComponent<CardData>();
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
        if(card.IsLocked) return;


        originalParent = rectTransform.parent;
        originalSiblingIndex = rectTransform.GetSiblingIndex();
        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalLocalRotation = rectTransform.localRotation;
        originalLocalScale = rectTransform.localScale;
        sourceSlot = originalParent.GetComponent<CardSlot>();
        wasDropped = false;
        image.raycastTarget = false;

        if (sourceSlot != null && card != null)
        {
            sourceSlot.RemoveCard(card);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameOver || canvas == null) return;
        if(card.IsLocked) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsGameOver) return;

        image.raycastTarget = true;
        if (!wasDropped)
        {
            rectTransform.SetParent(originalParent, false);
            rectTransform.SetSiblingIndex(originalSiblingIndex);
            rectTransform.anchoredPosition = originalAnchoredPosition;
            rectTransform.localRotation = originalLocalRotation;
            rectTransform.localScale = originalLocalScale;

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
        card.LockCard();
        enabled = false;
    }

    public void RestoreToOriginalPosition()
    {
        rectTransform.SetParent(originalParent, false);
        rectTransform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.anchoredPosition = originalAnchoredPosition;
        rectTransform.localRotation = originalLocalRotation;
        rectTransform.localScale = originalLocalScale;
    }
}
