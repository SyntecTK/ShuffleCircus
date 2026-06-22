using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cards/Card Sprite DataBase")]
public class CardSpriteDataBase : ScriptableObject
{
    [Serializable] public struct SuitSet
    {
        public Sprite[] ranks;
    }

    public SuitSet hearts;
    public SuitSet diamonds;
    public SuitSet clubs;
    public SuitSet spades;
    public Sprite cardBack;

    public Sprite GetSprite(CardIdentity id)
    {
        if (!id.IsValid) return null;
        int rankIndex = id.rank - 2; // 2..14 -> 0..12

        return id.suit switch
        {
            CardSuit.Hearts => hearts.ranks[rankIndex],
            CardSuit.Diamonds => diamonds.ranks[rankIndex],
            CardSuit.Clubs => clubs.ranks[rankIndex],
            CardSuit.Spades => spades.ranks[rankIndex],
            _ => null
        };
    }
}
