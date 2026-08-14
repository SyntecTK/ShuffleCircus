using System;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class IntroDirector : MonoBehaviour
{
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Image tutorialImage;

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





}
