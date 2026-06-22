using UnityEngine;

public class CardData : MonoBehaviour
{
    public int value;
    public CardSuit suit;

    public int RankValue => value;
    public int PointValue => GetPointValueFromRank(value);
    public CardSuit Suit => suit;
    public CardIdentity Identity => new CardIdentity(value, suit);

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CardSpriteDataBase spriteDataBase;

    private int GetPointValueFromRank(int rank)
    {
        if(rank >= 2 && rank <= 9) return rank;
        if(rank >=10 && rank <=14) return 10;
        return 0;
    }

    public void SetCard(CardIdentity identity)
    {
        if(identity.IsValid)
        {
            value = identity.rank;
            suit = identity.suit;
            UpdateSprite();
        }
        else
        {
            Debug.LogError("Invalid card identity: " + identity.rank + " of " + identity.suit);
        }
    }

    private void UpdateSprite()
    {
        if(spriteRenderer != null && spriteDataBase != null)
        {
            spriteRenderer.sprite = spriteDataBase.GetSprite(Identity);
        }
    }
}
