using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private RectTransform cardPrefab;

    [Header("Hand Settings")]
    [SerializeField] private int handSize = 6;
    private List<RectTransform> cards = new List<RectTransform>();

    public void PopulateHand()
    {
        StopAllCoroutines();

        foreach(var c in cards)
        {
            if( c != null) Destroy(c.gameObject);
        }
        cards.Clear();

        for(int i = 0; i < handSize; i++)
        {
            CardIdentity drawnCard = deckManager.DrawCard(true); // TODO: Check for opponent
            if(!drawnCard.IsValid)
            {
                Debug.LogError("Failed to draw card for hand: " + i);
                continue;
            }

            RectTransform cardInstance = Instantiate(cardPrefab, transform);
            AssignCardValue(cardInstance, drawnCard, i);
        }
    }
    /// <summary>/ Weist der gezogenen Karte die Werte zu und aktualisiert das Sprite. </summary>
    private void AssignCardValue(RectTransform cardInstance, CardIdentity drawnCard, int index)
    {
       CardData cardData = cardInstance.GetComponent<CardData>(); 
       if(cardData == null)
       {
            cardInstance.name = $"Card_{index}";
            Debug.LogWarning("Spawned card is missing the Card component.");
            return;
       }

       cardData.SetCard(drawnCard);
       cardInstance.name = $"Card_{drawnCard.rank}_of_{drawnCard.suit}";

       CardDrag carDrag = cardInstance.GetComponent<CardDrag>();
       //cardDrag setOwnerHand (obsolete??)

       Image cardImage = cardInstance.GetComponent<Image>();
       //Sprite sprite = cardSpriteDatabase != null ? cardSpriteDatabase.GetSprite(drawnCard) : null;
    }
}
