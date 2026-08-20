using System;

[Serializable]
public class DistortedMirror : ArtifactEffect
{
    public override int ModifyRowScore(int currentScore, PokerHand hand)
    {
        if(hand == PokerHand.OnePair || hand == PokerHand.TwoPair)
        {
            return currentScore * 2;
        }
        return currentScore;
    }
}