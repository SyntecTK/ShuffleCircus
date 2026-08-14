using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField, ReadOnly] private Dialogue currentDialogue;
    [SerializeField, ReadOnly] private StringTable currentStringTable;
    private int currentLineIndex;

    [Header("UI Elements")]
    [SerializeField] private GameObject _dialogueContainer;
    [SerializeField] private Image _tutorialImage;

    [Header("Tutorial Image Animation")]
    [SerializeField] private Vector2 _dialogueContainerDownOffset = new Vector2(0f, -150f);
    [SerializeField] private float _tutorialAnimationDuration = 0.4f;
    [SerializeField] private AnimationCurve _tutorialAnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private RectTransform _dialogueContainerRect;
    private RectTransform _tutorialImageRect;
    private Vector2 _dialogueContainerDefaultPos;
    private Vector3 _tutorialImageSmallScale;
    private bool _tutorialImageVisible;
    private Coroutine _tutorialAnimationCoroutine;

    //Will be set by the scene
    private TMP_Text dialogueText;
    private Image characterPortrait;

    //-------------------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();

        _dialogueContainerRect = _dialogueContainer.GetComponent<RectTransform>();
        _dialogueContainerDefaultPos = _dialogueContainerRect.anchoredPosition;

        _tutorialImageRect = _tutorialImage.rectTransform;
        _tutorialImageSmallScale = _tutorialImageRect.localScale;

        _tutorialImage.gameObject.SetActive(false);
        Color c = _tutorialImage.color;
        c.a = 0f;
        _tutorialImage.color = c;
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
    //-------------------------------------------------------------------------------------
    public void SetDialogueUI(TMP_Text dialogueText, Image characterPortrait)
    {
        this.dialogueText = dialogueText;
        this.characterPortrait = characterPortrait;
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
        UpdateTutorialImage(null);
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

        UpdateTutorialImage(line.tutorialImage);
    }

    //-------------------------------------------------------------------------------------
    private void UpdateTutorialImage(Sprite tutorialImage)
    {
        if (tutorialImage != null)
        {
            _tutorialImage.sprite = tutorialImage;

            if (!_tutorialImageVisible)
            {
                _tutorialImageVisible = true;
                RestartTutorialAnimation(AnimateTutorialImageIn());
            }
        }
        else if (_tutorialImageVisible)
        {
            _tutorialImageVisible = false;
            RestartTutorialAnimation(AnimateTutorialImageOut());
        }
    }

    private void RestartTutorialAnimation(IEnumerator routine)
    {
        if (_tutorialAnimationCoroutine != null) StopCoroutine(_tutorialAnimationCoroutine);
        _tutorialAnimationCoroutine = StartCoroutine(routine);
    }

    private IEnumerator AnimateTutorialImageIn()
    {
        _tutorialImage.gameObject.SetActive(true);
        _tutorialImageRect.localScale = _tutorialImageSmallScale;

        Color color = _tutorialImage.color;
        color.a = 0f;
        _tutorialImage.color = color;

        Vector2 containerFrom = _dialogueContainerDefaultPos;
        Vector2 containerTo = _dialogueContainerDefaultPos + _dialogueContainerDownOffset;

        float t = 0f;
        while (t < _tutorialAnimationDuration)
        {
            t += Time.deltaTime;
            float progress = _tutorialAnimationCurve.Evaluate(Mathf.Clamp01(t / _tutorialAnimationDuration));

            _dialogueContainerRect.anchoredPosition = Vector2.LerpUnclamped(containerFrom, containerTo, progress);
            _tutorialImageRect.localScale = Vector3.LerpUnclamped(_tutorialImageSmallScale, Vector3.one, progress);
            color.a = progress;
            _tutorialImage.color = color;

            yield return null;
        }

        _dialogueContainerRect.anchoredPosition = containerTo;
        _tutorialImageRect.localScale = Vector3.one;
        color.a = 1f;
        _tutorialImage.color = color;

        _tutorialAnimationCoroutine = null;
    }

    private IEnumerator AnimateTutorialImageOut()
    {
        Vector2 containerFrom = _dialogueContainerRect.anchoredPosition;
        Vector3 scaleFrom = _tutorialImageRect.localScale;
        Color color = _tutorialImage.color;
        float alphaFrom = color.a;

        float t = 0f;
        while (t < _tutorialAnimationDuration)
        {
            t += Time.deltaTime;
            float progress = _tutorialAnimationCurve.Evaluate(Mathf.Clamp01(t / _tutorialAnimationDuration));

            _dialogueContainerRect.anchoredPosition = Vector2.LerpUnclamped(containerFrom, _dialogueContainerDefaultPos, progress);
            _tutorialImageRect.localScale = Vector3.LerpUnclamped(scaleFrom, _tutorialImageSmallScale, progress);
            color.a = Mathf.LerpUnclamped(alphaFrom, 0f, progress);
            _tutorialImage.color = color;

            yield return null;
        }

        _dialogueContainerRect.anchoredPosition = _dialogueContainerDefaultPos;
        _tutorialImageRect.localScale = _tutorialImageSmallScale;
        color.a = 0f;
        _tutorialImage.color = color;
        _tutorialImage.gameObject.SetActive(false);

        _tutorialAnimationCoroutine = null;
    }
}
