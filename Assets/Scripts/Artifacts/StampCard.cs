using System;

[Serializable]
public class StampCard : ArtifactEffect
{
    public override int ModifyTotalScore(int currentTotalScore, GameBoard board)
    {
        if (board == null)
        {
            return currentTotalScore;
        }

        int bonus = CalculateVerticalThreeOfAKindBonus(board);
        return currentTotalScore + bonus;
    }

    private int CalculateVerticalThreeOfAKindBonus(GameBoard board)
    {
        const int threeOfAKindMultiplier = 4;

        int rows = board.grid.GetLength(0);
        int columns = board.grid.GetLength(1);

        if (rows < 3)
        {
            return 0;
        }

        int bonus = 0;

        for (int col = 0; col < columns; col++)
        {
            CardData top = board.GetCard(0, col);
            CardData mid = board.GetCard(1, col);
            CardData bot = board.GetCard(2, col);

            if (top == null || mid == null || bot == null)
            {
                continue;
            }

            int rank = top.RankValue;
            if (mid.RankValue == rank && bot.RankValue == rank)
            {
                int tripleBase = top.PointValue + mid.PointValue + bot.PointValue;
                bonus += tripleBase * threeOfAKindMultiplier;
            }
        }

        return bonus;
    }
}