using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifact", menuName = "Shuffle Circus/Artifact")]
public class ArtifactData : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string artifactID;
    [SerializeField] private string artifactName;
    [SerializeField] private string description;

    public Sprite Icon => icon;
    public string ID => artifactID;
    public string Name => artifactName;
    public string Description => description;

    [SerializeReference] private ArtifactEffect effect;

    public ArtifactEffect Effect => effect;
}
