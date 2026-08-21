using System;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class IntroDirector : MonoBehaviour
{
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Image tutorialImage;

    [SerializeField] private GameObject languageSelectionContainer;
    [SerializeField] private GameObject skipTutorialButton;

    private Dialogue dialogue;

    void OnEnable()
    {
        EventManager.TutorialOver += OnTutorialOver;
    }

    void OnDisable()
    {
        EventManager.TutorialOver -= OnTutorialOver;
    }

    void Start()
    {
        dialogue = Resources.Load<Dialogue>("Dialogues/Tutorial");
        DialogueManager.Instance.SetDialogueUI(dialogueContainer, dialogueText, characterPortrait, tutorialImage);
        DialogueManager.Instance.StartDialogue(dialogue);
    }
    //-------------------------------------------------------------------------------------
    private void OnTutorialOver()
    {
        SceneLoader.Instance.LoadScene("GameBoard");
    }

    public void SkipTutorial()
    {
        EventManager.TutorialCompleted();
    }

    public void ChooseGerman()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("de"));
        EnterTutorial();
    }

    public void ChooseEnglish()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("en"));
        EnterTutorial();
    }

    private void EnterTutorial()
    {
        languageSelectionContainer.SetActive(false);
        dialogueContainer.SetActive(true);
        skipTutorialButton.SetActive(true);
    }

    public void NextLine()
    {
        DialogueManager.Instance.AdvanceLine();
    }





}
