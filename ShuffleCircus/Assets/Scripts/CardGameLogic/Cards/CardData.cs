using UnityEngine;

public class CardData : MonoBehaviour
{
    private int value;
    private CardSuit suit;
    public bool isLocked;

    public int RankValue => value;
    public int PointValue => GetPointValueFromRank(value);
    public CardSuit Suit => suit;
    public CardIdentity Identity => new CardIdentity(value, suit);
    public bool IsLocked => isLocked;

    private int GetPointValueFromRank(int rank)
    {
        if(rank >= 2 && rank <= 9) return rank;
        if(rank >=10 && rank <=13) return 10;
        if(rank == 14) return 11;
        return 0;
    }

    public void SetCard(CardIdentity identity)
    {
        if(identity.IsValid)
        {
            value = identity.rank;
            suit = identity.suit;
        }
        else
        {
            Debug.LogError("Invalid card identity: " + identity.rank + " of " + identity.suit);
        }
    }

    public void LockCard()
    {
        isLocked = true;
    }
}
