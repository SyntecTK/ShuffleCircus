using System;
using UnityEngine;

[Serializable]
public class Gum : ArtifactEffect
{
	[SerializeField] private int protectedRank = 5;

	public override bool CardCanBeStolen(CardData targetCard, int stolenRank, GameBoard targetBoard)
	{
		if (targetCard == null)
		{
			return true;
		}

		return targetCard.RankValue != protectedRank;
	}
}