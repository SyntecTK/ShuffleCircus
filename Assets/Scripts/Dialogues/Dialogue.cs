using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Shuffle Circus/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string tableCollectionName;
    public string keyPrefix;
    public List<DialogueLine> lines = new List<DialogueLine>();
}
