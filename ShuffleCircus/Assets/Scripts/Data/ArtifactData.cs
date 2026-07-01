using UnityEngine;

public enum ArtifactRarity
{
    Common,
    Uncommon,
    Rare,
    Boss,
    Legendary
}

public enum ArtifactTargetSide
{
    Player,
    Opponent,
    Both
}

[CreateAssetMenu(fileName = "NewArtifactData", menuName = "Artifacts/Artifact Data")]
public class ArtifactData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string displayName;

    [Header("Presentation")]
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    [Header("Config")]
    [SerializeField] private ArtifactRarity rarity = ArtifactRarity.Common;
    [SerializeField] private ArtifactTargetSide targetSide = ArtifactTargetSide.Player;

    public string Id => id;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public ArtifactRarity Rarity => rarity;
    public ArtifactTargetSide TargetSide => targetSide;

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogWarning($"ArtifactData '{name}' has no id set.", this);
        }
    }
}
