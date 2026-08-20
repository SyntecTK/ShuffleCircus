using UnityEngine;
using System.Collections.Generic;
public class DeckManager : Singleton<DeckManager>
{
    [SerializeField] private List<CardIdentity> playerDeck = new List<CardIdentity>();
    [SerializeField] private List<CardIdentity> playerDiscardPile = new List<CardIdentity>();
    [SerializeField] private List<CardIdentity> opponentDeck = new List<CardIdentity>();
    [SerializeField] private List<CardIdentity> opponentDiscardPile = new List<CardIdentity>();
    private bool isInitialized = false;

    private void InitializeDecks()
    {
        if(isInitialized) return;

        playerDeck.Clear();
        opponentDeck.Clear();
        playerDiscardPile.Clear();
        opponentDiscardPile.Clear();

        List<CardIdentity> fullDeck = CreateDeck();
        ShuffleDeck(fullDeck);
        SplitDeck(fullDeck);

        isInitialized = true;

    }

    private List<CardIdentity> CreateDeck()
    {
        List<CardIdentity> deck = new List<CardIdentity>();
        for (int rank = 2; rank <= 14; rank++)
        {
            for (int suit = 0; suit < 4; suit++)
            {
                deck.Add(new CardIdentity(rank, (CardSuit)suit));
            }
        }
        return deck;
    }

    private void ShuffleDeck(List<CardIdentity> deck)
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
        }
    }

    private void SplitDeck(List<CardIdentity> fullDeck)
    {
        int halfSize = fullDeck.Count / 2;
        playerDeck = fullDeck.GetRange(0, halfSize);
        opponentDeck = fullDeck.GetRange(halfSize, halfSize);
    }

    /// <summary>
    /// Zieht eine Karte. Gibt die Kartenidentitaet zurueck.
    /// Mischt automatisch den Ablagestapel ins Deck, wenn das Deck leer ist.
    /// </summary>
    public CardIdentity DrawCard(bool isPlayer)
    {
        InitializeDecks();

        List<CardIdentity> drawDeck = isPlayer ? playerDeck : opponentDeck;
        if (drawDeck.Count == 0)
        {
            ShuffleDiscardIntoDeck(isPlayer);
        }
        if (drawDeck.Count > 0)
        {
            CardIdentity value = drawDeck[0];
            drawDeck.RemoveAt(0);
            return value;
        }
        return default; // Keine Karten mehr
    }

    /// <summary>
    /// Fuegt eine Karte zum Ablagestapel hinzu.
    /// </summary>
    public void AddToDiscard(CardIdentity card, bool isPlayer)
    {
        if (!card.IsValid) return;

        if (isPlayer)
        {
            playerDiscardPile.Add(card);
        }
        else
        {
            opponentDiscardPile.Add(card);
        }
    }

    private void ShuffleDiscardIntoDeck(bool isPlayer)
    {
        List<CardIdentity> discard = isPlayer ? playerDiscardPile : opponentDiscardPile;
        List<CardIdentity> drawDeck = isPlayer ? playerDeck : opponentDeck;
        drawDeck.AddRange(discard);
        ShuffleDeck(drawDeck);
        discard.Clear();
    }

    public int GetDeckCount(bool isPlayer)
    {
        return isPlayer ? playerDeck.Count : opponentDeck.Count;
    }

    public int GetDiscardCount(bool isPlayer)
    {
        return isPlayer ? playerDiscardPile.Count : opponentDiscardPile.Count;
    }

    public List<CardIdentity> GetDiscardPile(bool isPlayer)
    {
        return isPlayer ? playerDiscardPile : opponentDiscardPile;
    }

    public void ResetDecks()
    {
        isInitialized = false;
        InitializeDecks();
    }


}
