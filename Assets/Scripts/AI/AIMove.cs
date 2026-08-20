using UnityEngine;

/// <summary>
/// Represents a candidate move for the AI: place a specific card at a specific board slot.
/// </summary>
public struct AIMove
{
    public int row;
    public int col;
    public CardData card;
    public float evaluationScore;  // Higher is better
    
    public AIMove(int row, int col, CardData card)
    {
        this.row = row;
        this.col = col;
        this.card = card;
        this.evaluationScore = 0f;
    }
    
    public override string ToString()
    {
        return $"Move({row},{col}) Card:{card.RankValue} Score:{evaluationScore:F1}";
    }
}
