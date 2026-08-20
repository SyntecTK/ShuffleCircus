using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandDisplay handDisplay;
    [SerializeField] private Transform cardParent;
    public HandDisplay HandDisplay => handDisplay;

    private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] private bool isPlayer = true;
    [SerializeField] private bool populateOnStart = true;

    [Header("Hand Settings")]
    [SerializeField] private int handSize = 6;
    private List<RectTransform> handCards = new List<RectTransform>();

    private void OnEnable()
    {
        EventManager.OnTurnEnded += HandleTurnEnded;
    }

    private void OnDisable()
    {
        EventManager.OnTurnEnded -= HandleTurnEnded;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (populateOnStart)
        {
            bool initialHandIsPlayer = GameManager.Instance != null ? GameManager.Instance.IsPlayerTurn : isPlayer;
            isPlayer = initialHandIsPlayer;
            DrawHand(initialHandIsPlayer);
        }
    }

    private void DrawHand(bool drawForPlayer)
    {
        if (DeckManager.Instance == null || handDisplay == null)
        {
            Debug.LogError("HandManager is missing DeckManager or HandDisplay reference.");
            return;
        }

        handDisplay.ClearCards(handCards);

        for (int i = 0; i < handSize; i++)
        {
            CardIdentity drawnCard = DeckManager.Instance.DrawCard(drawForPlayer);
            if (!drawnCard.IsValid)
            {
                Debug.LogError("Failed to draw card for hand: " + i);
                continue;
            }

            RectTransform cardInstance = handDisplay.CreateCardView(cardParent, drawnCard, i);
            if (cardInstance == null)
            {
                Debug.LogError("Failed to create card view.");
                continue;
            }

            handCards.Add(cardInstance);
        }

        audioSource.Play();
        RearrangeCards();
        handDisplay.AnimateHandDraw(handCards, handSize);
        EventManager.BoardChanged();
    }

    public void RearrangeCards()
    {
        handDisplay.ArrangeCards(handCards, handSize);
    }

    public void RefreshHandVisuals()
    {
        if (handDisplay == null)
        {
            return;
        }

        handDisplay.RefreshHandCardVisuals(handCards);
    }

    /// <summary>
    /// Returns the actual CardData objects currently in hand (for AI evaluation, etc.)
    /// </summary>
    public List<CardData> GetHandCards()
    {
        List<CardData> result = new List<CardData>();
        foreach (RectTransform cardTransform in handCards)
        {
            if (cardTransform != null)
            {
                CardData cardData = cardTransform.GetComponent<CardData>();
                if (cardData != null)
                {
                    result.Add(cardData);
                }
            }
        }
        return result;
    }

    public void RemoveCardFromHand(RectTransform card)
    {
        if (card == null)
        {
            return;
        }

        if (handCards.Remove(card))
        {
            handDisplay.ArrangeCards(handCards, handSize);
        }
    }

    /// <summary>
    /// CardData-based overload used by AI to remove a card from hand tracking.
    /// Finds the matching RectTransform via its CardData component and removes it.
    /// </summary>
    public void RemoveCardFromHand(CardData card)
    {
        if (card == null) return;

        RectTransform match = null;
        foreach (RectTransform cardTransform in handCards)
        {
            if (cardTransform != null && cardTransform.GetComponent<CardData>() == card)
            {
                match = cardTransform;
                break;
            }
        }

        if (match != null)
        {
            RemoveCardFromHand(match);
        }
    }

    private void HandleTurnEnded()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.IsGameOver) return;

        bool previousHandIsPlayer = isPlayer;
        bool nextHandIsPlayer = GameManager.Instance.IsPlayerTurn;

        DiscardRemainingHandCards(previousHandIsPlayer);
        isPlayer = nextHandIsPlayer;
        DrawHand(nextHandIsPlayer);
    }

    private void DiscardRemainingHandCards()
    {
        DiscardRemainingHandCards(isPlayer);
    }

    private void DiscardRemainingHandCards(bool discardForPlayer)
    {
        if (DeckManager.Instance == null)
        {
            return;
        }

        for (int i = 0; i < handCards.Count; i++)
        {
            RectTransform cardTransform = handCards[i];
            if (cardTransform == null || cardTransform.parent != cardParent)
            {
                continue;
            }

            CardData cardData = cardTransform.GetComponent<CardData>();
            if (cardData != null)
            {
                DeckManager.Instance.AddToDiscard(cardData.Identity, discardForPlayer);
            }
        }

        handDisplay.ClearCards(handCards);
        EventManager.BoardChanged();
    }
}
