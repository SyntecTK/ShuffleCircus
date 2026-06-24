using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private HandDisplay handDisplay;
    [SerializeField] private bool isPlayer = true;
    [SerializeField] private bool populateOnStart = true;

    [Header("Hand Settings")]
    [SerializeField] private int handSize = 6;
    private List<RectTransform> handCards = new List<RectTransform>();

    private void Start()
    {
        if (populateOnStart)
        {
            DrawHand();
        }
    }

    public void DrawHand()
    {
        if (deckManager == null || handDisplay == null)
        {
            Debug.LogError("HandManager is missing DeckManager or HandDisplay reference.");
            return;
        }

        handDisplay.ClearCards(handCards);

        for (int i = 0; i < handSize; i++)
        {
            CardIdentity drawnCard = deckManager.DrawCard(isPlayer);
            if (!drawnCard.IsValid)
            {
                Debug.LogError("Failed to draw card for hand: " + i);
                continue;
            }

            RectTransform cardInstance = handDisplay.CreateCardView(transform, drawnCard, i);
            if (cardInstance == null)
            {
                Debug.LogError("Failed to create card view.");
                continue;
            }

            handCards.Add(cardInstance);
        }

        handDisplay.ArrangeCards(handCards, handSize);
        handDisplay.AnimateHandDraw(handCards, handSize);
    }
}
