using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PokerHand
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush
}

public class ScoringSystem
{
    private static readonly Dictionary<PokerHand, int> handMultipliers = new Dictionary<PokerHand, int>
    {
        { PokerHand.HighCard, 1 },
        { PokerHand.OnePair, 2 },
        { PokerHand.TwoPair, 3 },
        { PokerHand.ThreeOfAKind, 4 },
        { PokerHand.Straight, 5 },
        { PokerHand.Flush, 6 },
        { PokerHand.FullHouse, 7 },
        { PokerHand.FourOfAKind, 8 },
        { PokerHand.StraightFlush, 9 },
        { PokerHand.RoyalFlush, 10 }
    };

    // public static PokerHand GetPokerhand(CardData[] rowCards)
    // {
    //     List<CardData> cards = rowCards.Where(c => c != null).ToList();
    //     List<int> ranks = cards.Select(c => c.RankValue).Where(v => v > 0).ToList();

    //     if(ranks.Count == 0) return PokerHand.HighCard;

    // }

}
