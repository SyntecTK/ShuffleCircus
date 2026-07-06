using System.Collections.Generic;
using System.Linq;

public static class ScoringSystem
{
    private static readonly Dictionary<PokerHand, int> handMultipliers = new Dictionary<PokerHand, int>
    {
        { PokerHand.HighCard,       1 },
        { PokerHand.OnePair,        2 },
        { PokerHand.TwoPair,        3 },
        { PokerHand.ThreeOfAKind,   4 },
        { PokerHand.Straight,       5 },
        { PokerHand.Flush,          6 },
        { PokerHand.FullHouse,      7 },
        { PokerHand.FourOfAKind,    8 },
        { PokerHand.StraightFlush,  9 },
        { PokerHand.RoyalFlush,    10 }
    };

    public static PokerHand GetPokerhand(CardData[] rowCards)
    {
        List<CardData> cards = rowCards.Where(c => c != null).ToList();
        List<int> ranks = cards.Select(c => c.RankValue).Where(v => v > 0).ToList();

        if(ranks.Count == 0) return PokerHand.HighCard;

        Dictionary<int, int> amount = new Dictionary<int, int>();
        foreach (int rank in ranks)
        {
            amount.TryGetValue(rank, out int count);
            amount[rank] = count + 1;
        }

        List<int> groupSizes = amount.Values.OrderByDescending(v => v).ToList();
        if(groupSizes[0] == 4) return PokerHand.FourOfAKind;
        if(groupSizes[0] == 3 && groupSizes.Count > 1 && groupSizes[1] == 2) return PokerHand.FullHouse;

        bool isFiveCards = cards.Count == 5;
        bool isFlush = isFiveCards && cards.Select(c => c.Suit).Distinct().Count() == 1;

        bool isStraight = false;
        List<int> ordered = null;

        if(isFiveCards && groupSizes[0] == 1)
        {
            ordered = cards.Select(c => c.RankValue).ToList();

            bool isNormalStraight = true;
            for(int i = 1; i < ordered.Count; i++)
            {
                if(ordered[i] != ordered[i - 1] + 1)
                {
                    isNormalStraight = false;
                    break;
                }
            }

            if(isNormalStraight)
            {
                isStraight = true;
            }
            else
            {
                // Allow Ace to be used as 1 for A-2-3-4-5 in placed order.
                List<int> aceLowOrdered = ordered.Select(v => v == 14 ? 1 : v).ToList();
                isStraight = true;
                for(int i = 1; i < aceLowOrdered.Count; i++)
                {
                    if(aceLowOrdered[i] != aceLowOrdered[i - 1] + 1)
                    {
                        isStraight = false;
                        break;
                    }
                }
            }
        }

        if (isFlush && isStraight)
        {
            bool isRoyal = ordered != null && ordered[0] == 10 && ordered[1] == 11 && ordered[2] == 12 && ordered[3] == 13 && ordered[4] == 14;
            return isRoyal ? PokerHand.RoyalFlush : PokerHand.StraightFlush;
        }

        if(isFlush) return PokerHand.Flush;
        if(isStraight) return PokerHand.Straight;

        if(groupSizes[0] == 3) return PokerHand.ThreeOfAKind;
        if(groupSizes.Count >= 2 && groupSizes[0] == 2 && groupSizes[1] == 2) return PokerHand.TwoPair;
        if(groupSizes[0] == 2) return PokerHand.OnePair;

        return PokerHand.HighCard;
    }

    public static string GetHandName(PokerHand hand) => hand switch
    {
        PokerHand.HighCard      => "High Card",
        PokerHand.OnePair       => "One Pair",
        PokerHand.TwoPair       => "Two Pair",
        PokerHand.ThreeOfAKind  => "Three of a Kind",
        PokerHand.Straight      => "Straight",
        PokerHand.Flush         => "Flush",
        PokerHand.FullHouse     => "Full House",
        PokerHand.FourOfAKind   => "Four of a Kind",
        PokerHand.StraightFlush => "Straight Flush",
        PokerHand.RoyalFlush    => "Royal Flush",
        _                       => "Unknown Hand"
    };

    public static int CalculateRowScore(GameBoard board, int row)
    {
        CardData[] cards = new CardData[5];
        for(int i = 0; i < 5; i++)
        {
            cards[i] = board.GetCard(row, i);
        }
        return CalculateRowScore(cards, board);
    }

    /// <summary>
    /// Array-based overload — works on a snapshot (e.g. for AI simulation) without a GameBoard instance.
    /// </summary>
    public static int CalculateRowScore(CardData[] rowCards)
    {
        return CalculateRowScore(rowCards, null);
    }

    private static int CalculateRowScore(CardData[] rowCards, GameBoard board)
    {
        PokerHand hand = GetPokerhand(rowCards);
        int baseScore = GetHandScore(rowCards, hand);
        int finalScore = baseScore * handMultipliers[hand];

        if (ArtifactManager.Instance != null)
        {
            finalScore = ArtifactManager.Instance.ApplyScoreEffects(finalScore, hand, board);
        }

        return finalScore;
    }

    private static int GetHandScore(CardData[] rowCards, PokerHand hand)
    {
        List<CardData> cards = rowCards.Where(c => c != null).ToList();
        if(cards.Count == 0) return 0;

        Dictionary<int, int> counts = new Dictionary<int, int>();
        foreach(var card in cards)
        {
            counts.TryGetValue(card.RankValue, out int c);
            counts[card.RankValue] = c + 1;
        }

        switch (hand)
        {
            case PokerHand.HighCard:
                int maxRank = cards.Max(c => c.RankValue);
                return cards.Where(c => c.RankValue == maxRank).Max(c => c.PointValue);

            case PokerHand.OnePair:
                int pairRank = counts.First(kvp => kvp.Value == 2).Key;
                return cards.Where(c => c.RankValue == pairRank).Sum(c => c.PointValue);

            case PokerHand.TwoPair:
                var pairRanks = new HashSet<int>(counts.Where(kvp => kvp.Value == 2).Select(kvp => kvp.Key));
                return cards.Where(c => pairRanks.Contains(c.RankValue)).Sum(c => c.PointValue);

            case PokerHand.ThreeOfAKind:
                int threeRank = counts.First(kvp => kvp.Value == 3).Key;
                return cards.Where(c => c.RankValue == threeRank).Sum(c => c.PointValue);

            case PokerHand.FourOfAKind:
                int fourRank = counts.First(kvp => kvp.Value == 4).Key;
                return cards.Where(c => c.RankValue == fourRank).Sum(c => c.PointValue);

            default:
                return cards.Sum(c => c.PointValue);
        }

    }

    public static int CalculateTotalScore(GameBoard board)
    {
        int total = 0;
        for(int row = 0; row < 3; row++)
        {
            total += CalculateRowScore(board, row);
        }

        if (ArtifactManager.Instance != null)
        {
            total = ArtifactManager.Instance.ApplyTotalScoreEffects(total, board);
        }

        return total;
    }

}
