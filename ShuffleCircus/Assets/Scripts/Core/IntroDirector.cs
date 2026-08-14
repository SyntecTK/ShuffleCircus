using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroDirector : MonoBehaviour
{
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Image tutorialImage;

    private Dialogue dialogue;

    void Start()
    {
        dialogue = Resources.Load<Dialogue>("Dialogues/Tutorial");
        DialogueManager.Instance.SetDialogueUI(dialogueContainer, dialogueText, characterPortrait, tutorialImage);
        DialogueManager.Instance.StartDialogue(dialogue);
    }

}
