using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandDisplay : MonoBehaviour
{
    private struct CardAnimData
    {
        public Vector2 StartPos;
        public Vector2 EndPos;

        public Quaternion StartRot;
        public Quaternion EndRot;

        public float TargetBezierT;
    }

    private struct AnimatedCard
    {
        public RectTransform Rect;
        public CanvasGroup Canvas;
    }

    [Header("Card View")]
    [SerializeField] private RectTransform cardPrefab;
    [SerializeField] private CardSpriteDataBase cardSpriteDataBase;

    [Header("Layout")]
    [SerializeField] private bool useBezierLayout = false;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform controlPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float cardSpacing = 140f;

    [Header("Animation")]
    [SerializeField] private float cardAnimDuration = 0.3f;
    [SerializeField] private float cardAnimDelay = 0.08f;
    [SerializeField] private float drawYOffset = 120f;

    public void ClearCards(List<RectTransform> cards)
    {
        StopAllCoroutines();
        foreach (RectTransform card in cards)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        cards.Clear();
    }

    public RectTransform CreateCardView(Transform parent, CardIdentity card, int index)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("HandDisplay is missing cardPrefab reference.");
            return null;
        }

        RectTransform cardInstance = Instantiate(cardPrefab, parent);
        ApplyCardVisual(cardInstance, card, index);
        return cardInstance;
    }

    public void ArrangeCards(IReadOnlyList<RectTransform> cards, int maxSlots)
    {
        List<RectTransform> activeCards = GetActiveCards(cards);
        int count = activeCards.Count;
        if (count == 0)
        {
            return;
        }

        if (UseBezier())
        {
            ArrangeAlongBezier(activeCards, maxSlots);
            UpdateHoverBasePoses(activeCards);
            return;
        }

        ArrangeLinear(activeCards);
        UpdateHoverBasePoses(activeCards);
    }

    public void AnimateHandDraw(IReadOnlyList<RectTransform> cards, int maxSlots)
    {
        StopAllCoroutines();
        EventManager.HandDrawStarted();
        StartCoroutine(AnimateHandDrawRoutine(cards, maxSlots));
    }

    public void RevealCard(RectTransform cardInstance, CardIdentity card)
    {
        if (cardInstance == null || cardSpriteDataBase == null)
        {
            return;
        }

        Image cardImage = cardInstance.GetComponentInChildren<Image>();
        if (cardImage != null)
        {
            Sprite sprite = cardSpriteDataBase.GetSprite(card);
            if (sprite != null)
            {
                cardImage.sprite = sprite;
            }
        }
    }

    public void RefreshHandCardVisuals(IReadOnlyList<RectTransform> cards)
    {
        if (cards == null || cardSpriteDataBase == null)
        {
            return;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform card = cards[i];
            if (card == null)
            {
                continue;
            }

            CardData cardData = card.GetComponent<CardData>();
            if (cardData == null)
            {
                continue;
            }

            ApplyCardVisual(card, cardData.Identity, i);
        }
    }

    private void ApplyCardVisual(RectTransform cardInstance, CardIdentity drawnCard, int index)
    {
        if (cardInstance == null)
        {
            return;
        }

        CardData cardData = cardInstance.GetComponent<CardData>();
        if (cardData == null)
        {
            cardInstance.name = $"Card_{index}";
            Debug.LogWarning("Spawned card is missing the CardData component.");
            return;
        }

        cardData.SetCard(drawnCard);
        cardInstance.name = $"Card_{drawnCard.rank}_of_{drawnCard.suit}";

        Image cardImage = cardInstance.GetComponentInChildren<Image>();
        Sprite sprite = cardSpriteDataBase != null ? cardSpriteDataBase.GetSprite(drawnCard) : null;

        if (cardImage != null && sprite != null)
        {
            if(GameManager.Instance.IsPlayerTurn || GameManager.Instance.State.GameMode == GameMode.Multiplayer)
            {
                cardImage.sprite = sprite;
            }
            else
            {
                cardImage.sprite = cardSpriteDataBase.GetEnemyBackSprite();
            }
        }
    }

    private List<RectTransform> GetActiveCards(IReadOnlyList<RectTransform> cards)
    {
        List<RectTransform> activeCards = new List<RectTransform>();
        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform card = cards[i];
            if (card != null)
            {
                activeCards.Add(card);
            }
        }
        return activeCards;
    }

    private bool UseBezier()
    {
        return useBezierLayout && startPoint != null && controlPoint != null && endPoint != null;
    }

    private void ArrangeLinear(List<RectTransform> cards)
    {
        int count = cards.Count;
        float startX = -((count - 1) * cardSpacing) * 0.5f;
        for (int i = 0; i < count; i++)
        {
            RectTransform card = cards[i];
            card.anchoredPosition = new Vector2(startX + i * cardSpacing, 0f);
            card.localRotation = Quaternion.identity;
        }
    }

    private void ArrangeAlongBezier(List<RectTransform> cards, int maxSlots)
    {
        int count = cards.Count;
        float spacing = maxSlots > 1 ? 1f / (maxSlots - 1) : 0f;
        float tStart = 0.5f - (count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float t = count == 1 ? 0.5f : tStart + i * spacing;
            cards[i].anchoredPosition = transform.InverseTransformPoint(GetBezierPoint(t));

            Vector2 tangent = GetBezierTangent(t);
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            cards[i].localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private IEnumerator AnimateHandDrawRoutine(IReadOnlyList<RectTransform> cards, int maxSlots)
    {
        List<RectTransform> activeCards = GetActiveCards(cards);
        int count = activeCards.Count;
        if (count == 0)
        {
            EventManager.DrawnHand();
            yield break;
        }

        AnimatedCard[] animatedCards = PrepareCards(activeCards);
        bool useBezierForAnimation = UseBezier();
        CardAnimData[] animData = CalculateLayout(count, maxSlots, useBezierForAnimation);
        ApplyStartState(animatedCards, animData);

        float totalDuration = cardAnimDuration + cardAnimDelay * Mathf.Max(0, count - 1);
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            AnimateCards(animatedCards, animData, elapsed, useBezierForAnimation);

            elapsed += Time.deltaTime;
            yield return null;
        }

        FinalizeCards(animatedCards, animData);
        EventManager.DrawnHand();
    }

    private AnimatedCard[] PrepareCards(List<RectTransform> activeCards)
    {
        AnimatedCard[] animatedCards = new AnimatedCard[activeCards.Count];
        for (int i = 0; i < activeCards.Count; i++)
        {
            RectTransform cardRect = activeCards[i];
            CanvasGroup canvasGroup = cardRect.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = cardRect.gameObject.AddComponent<CanvasGroup>();
            }

            animatedCards[i] = new AnimatedCard
            {
                Rect = cardRect,
                Canvas = canvasGroup
            };
        }

        return animatedCards;
    }

    private CardAnimData[] CalculateLayout(int count, int maxSlots, bool useBezierForAnimation)
    {
        CardAnimData[] animData = new CardAnimData[count];
        if (useBezierForAnimation)
        {
            CalculateBezierLayout(animData, maxSlots);
        }
        else
        {
            CalculateLinearLayout(animData);
        }

        return animData;
    }

    private void CalculateBezierLayout(CardAnimData[] animData, int maxSlots)
    {
        int count = animData.Length;
        float spacing = maxSlots > 1 ? 1f / (maxSlots - 1) : 0f;
        float tStart = 0.5f - (count - 1) * spacing * 0.5f;
        Vector2 leftPoint = transform.InverseTransformPoint(GetBezierPoint(0f));
        Vector2 leftTangent = GetBezierTangent(0f);
        float leftAngle = Mathf.Atan2(leftTangent.y, leftTangent.x) * Mathf.Rad2Deg;
        Quaternion leftRotation = Quaternion.Euler(0f, 0f, leftAngle);

        for (int i = 0; i < count; i++)
        {
            float t = count == 1 ? 0.5f : tStart + i * spacing;
            Vector2 targetPosition = transform.InverseTransformPoint(GetBezierPoint(t));
            Vector2 tangent = GetBezierTangent(t);
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

            animData[i] = new CardAnimData
            {
                StartPos = leftPoint,
                EndPos = targetPosition,
                StartRot = leftRotation,
                EndRot = Quaternion.Euler(0f, 0f, angle),
                TargetBezierT = t
            };
        }
    }

    private void CalculateLinearLayout(CardAnimData[] animData)
    {
        int count = animData.Length;
        float startX = -((count - 1) * cardSpacing) * 0.5f;
        float offscreenLeftX = startX - cardSpacing * 1.75f;

        for (int i = 0; i < count; i++)
        {
            animData[i] = new CardAnimData
            {
                StartPos = new Vector2(offscreenLeftX, drawYOffset),
                EndPos = new Vector2(startX + i * cardSpacing, 0f),
                StartRot = Quaternion.identity,
                EndRot = Quaternion.identity,
                TargetBezierT = 0f
            };
        }
    }

    private void ApplyStartState(AnimatedCard[] animatedCards, CardAnimData[] animData)
    {
        for (int i = 0; i < animatedCards.Length; i++)
        {
            AnimatedCard card = animatedCards[i];
            if (card.Rect == null || card.Canvas == null)
            {
                continue;
            }

            card.Canvas.alpha = 0f;
            card.Rect.anchoredPosition = animData[i].StartPos;
            card.Rect.localRotation = animData[i].StartRot;
        }
    }

    private void AnimateCards(AnimatedCard[] animatedCards, CardAnimData[] animData, float elapsed, bool useBezierForAnimation)
    {
        for (int i = 0; i < animatedCards.Length; i++)
        {
            AnimatedCard card = animatedCards[i];
            if (card.Rect == null || card.Canvas == null)
            {
                continue;
            }

            float localElapsed = elapsed - i * cardAnimDelay;
            if (localElapsed <= 0f)
            {
                continue;
            }

            float tNorm = Mathf.Clamp01(localElapsed / cardAnimDuration);
            float eased = Mathf.SmoothStep(0f, 1f, tNorm);

            if (useBezierForAnimation)
            {
                AnimateBezierCard(card, animData[i], eased);
            }
            else
            {
                AnimateLinearCard(card, animData[i], eased);
            }

            card.Canvas.alpha = eased;
        }
    }

    private void AnimateBezierCard(AnimatedCard card, CardAnimData data, float eased)
    {
        float curveT = Mathf.Lerp(0f, data.TargetBezierT, eased);
        card.Rect.anchoredPosition = transform.InverseTransformPoint(GetBezierPoint(curveT));

        Vector2 tangent = GetBezierTangent(curveT);
        float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
        card.Rect.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void AnimateLinearCard(AnimatedCard card, CardAnimData data, float eased)
    {
        card.Rect.anchoredPosition = Vector2.Lerp(data.StartPos, data.EndPos, eased);
        card.Rect.localRotation = Quaternion.Slerp(data.StartRot, data.EndRot, eased);
    }

    private void FinalizeCards(AnimatedCard[] animatedCards, CardAnimData[] animData)
    {
        for (int i = 0; i < animatedCards.Length; i++)
        {
            AnimatedCard card = animatedCards[i];
            if (card.Rect == null)
            {
                continue;
            }

            card.Rect.anchoredPosition = animData[i].EndPos;
            card.Rect.localRotation = animData[i].EndRot;

            if (card.Canvas != null)
            {
                card.Canvas.alpha = 1f;
            }
        }

        UpdateHoverBasePoses(animatedCards);
    }

    private void UpdateHoverBasePoses(IReadOnlyList<RectTransform> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform card = cards[i];
            if (card == null)
            {
                continue;
            }

            CardHover hover = card.GetComponent<CardHover>();
            if (hover != null)
            {
                hover.UpdateBasePose();
            }
        }
    }

    private void UpdateHoverBasePoses(AnimatedCard[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            RectTransform cardRect = cards[i].Rect;
            if (cardRect == null)
            {
                continue;
            }

            CardHover hover = cardRect.GetComponent<CardHover>();
            if (hover != null)
            {
                hover.UpdateBasePose();
            }
        }
    }


    private Vector2 GetBezierPoint(float t)
    {
        Vector2 p0 = startPoint.position;
        Vector2 p1 = controlPoint.position;
        Vector2 p2 = endPoint.position;

        return Mathf.Pow(1f - t, 2f) * p0 + 2f * (1f - t) * t * p1 + Mathf.Pow(t, 2f) * p2;
    }

    private Vector2 GetBezierTangent(float t)
    {
        Vector2 p0 = startPoint.position;
        Vector2 p1 = controlPoint.position;
        Vector2 p2 = endPoint.position;

        return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
    }

    private void OnDrawGizmosSelected()
    {
        if (!UseBezier())
        {
            return;
        }

        Gizmos.color = Color.green;
        Vector3 previous = startPoint.position;
        for (int i = 1; i <= 20; i++)
        {
            float t = i / 20f;
            Vector3 point = GetBezierPoint(t);
            Gizmos.DrawLine(previous, point);
            previous = point;
        }
    }
}
