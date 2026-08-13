using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField, ReadOnly] private Dialogue currentDialogue;
    [SerializeField, ReadOnly] private StringTable currentStringTable;
    private int currentLineIndex;

    //Will be set by the scene
    private TMP_Text dialogueText;
    private Image characterPortrait;

    public void SetDialogueUI(TMP_Text dialogueText, Image characterPortrait)
    {
        this.dialogueText = dialogueText;
        this.characterPortrait = characterPortrait;
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void Update()
    {
        if (currentDialogue == null) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            AdvanceLine();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        currentStringTable = LocalizationSettings.StringDatabase.GetTable(currentDialogue.tableCollectionName);
        currentLineIndex = 0;
        DisplayCurrentLine();
    }

    private void AdvanceLine()
    {
        currentLineIndex++;
        if (currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }
        DisplayCurrentLine();
    }

    private void EndDialogue()
    {
        currentDialogue = null;
        currentStringTable = null;
        dialogueText.text = string.Empty;
    }

    private void OnLocaleChanged(Locale locale)
    {
        if (currentDialogue == null) return;
        currentStringTable = LocalizationSettings.StringDatabase.GetTable(currentDialogue.tableCollectionName);
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        string key = $"{currentDialogue.keyPrefix}_{currentLineIndex + 1:00}";
        dialogueText.text = LocalizationSettings.StringDatabase.GetLocalizedString(currentDialogue.tableCollectionName, key);

        DialogueLine line = currentDialogue.lines[currentLineIndex];
        if (line.characterPortrait != null) characterPortrait.sprite = line.characterPortrait;
    }
}
