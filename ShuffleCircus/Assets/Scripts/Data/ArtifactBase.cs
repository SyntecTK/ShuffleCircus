using UnityEngine;
using System;

[Serializable]
public abstract class ArtifactEffect
{
    public virtual int ModifyRowScore(int currentScore, PokerHand hand) => currentScore;
    public virtual int ModifyTotalScore(int currentTotalScore, GameBoard board) => currentTotalScore;
}