using UnityEngine;
using System.Collections.Generic;

public class ArtifactManager : Singleton<ArtifactManager>
{
    private const string ArtifactResourcesPath = "Artifacts";

    [SerializeField] private List<ArtifactData> activeArtifacts = new List<ArtifactData>();
    [SerializeField] private List<ArtifactData> allArtifacts = new List<ArtifactData>();
    [Header("Debug")]
    [SerializeField] private bool addTestArtifactOnStart;
    [SerializeField] private ArtifactData testArtifact;

    public IReadOnlyList<ArtifactData> ActiveArtifacts => activeArtifacts;
    public IReadOnlyList<ArtifactData> AllArtifacts => allArtifacts;

    private void Start()
    {
        ReloadAllArtifacts();

        AddActiveArtifact(allArtifacts[0]);
    }

    public void ReloadAllArtifacts()
    {
        allArtifacts.Clear();

        ArtifactData[] artifacts = Resources.LoadAll<ArtifactData>(ArtifactResourcesPath);
        allArtifacts.AddRange(artifacts);
    }

    public void AddActiveArtifact(ArtifactData artifact)
    {
        if (artifact == null || activeArtifacts.Contains(artifact))
        {
            return;
        }

        activeArtifacts.Add(artifact);
        EventManager.AddedArtifact();
    }

    public void RemoveActiveArtifact(ArtifactData artifact)
    {
        activeArtifacts.Remove(artifact);
    }

    public void ClearActiveArtifacts()
    {
        activeArtifacts.Clear();
    }

    public int ApplyScoreEffects(int currentScore, PokerHand hand, GameBoard board)
    {
        int modifiedScore = currentScore;

        foreach (ArtifactData artifact in activeArtifacts)
        {
            if (artifact == null || artifact.Effect == null)
            {
                continue;
            }

            modifiedScore = artifact.Effect.ModifyRowScore(modifiedScore, hand);
        }

        return modifiedScore;
    }

    public int ApplyTotalScoreEffects(int currentTotalScore, GameBoard board)
    {
        int modifiedTotalScore = currentTotalScore;

        foreach (ArtifactData artifact in activeArtifacts)
        {
            if (artifact == null || artifact.Effect == null)
            {
                continue;
            }

            modifiedTotalScore = artifact.Effect.ModifyTotalScore(modifiedTotalScore, board);
        }

        return modifiedTotalScore;
    }

    public bool CanCardBeStolen(CardData targetCard, int stolenRank, GameBoard targetBoard)
    {
        foreach (ArtifactData artifact in activeArtifacts)
        {
            if (artifact == null || artifact.Effect == null)
            {
                continue;
            }

            if (!artifact.Effect.CardCanBeStolen(targetCard, stolenRank, targetBoard))
            {
                return false;
            }
        }

        return true;
    }
}