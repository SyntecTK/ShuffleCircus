using System;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private RectTransform resultPanel;
    [SerializeField] private GameObject _uiContainer;
    private float _originalHeight;
    private bool _isMinimized = false;

    void Start()
    {
        _originalHeight = resultPanel.rect.height;
    }

    private void OnEnable()
    {
        EventManager.OnGameOver += ShowResultScreen;
    }

    private void OnDisable()
    {
        EventManager.OnGameOver -= ShowResultScreen;
    }

    private void ShowResultScreen()
    {
        resultPanel.gameObject.SetActive(true);
    }

    private void Minimize()
    {
        resultPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30f);
        _uiContainer.SetActive(false);
    }  

    private void Restore()
    {
        if (resultPanel == null) return;
        resultPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _originalHeight);
        _uiContainer.SetActive(true);
    }

    public void ToggleResultPanel()
    {
        if (_isMinimized)
        {
            Restore();
        }
        else
        {
            Minimize();
        }
        _isMinimized = !_isMinimized;
    }

    public void StartNextGame()
    {
        SceneLoader.Instance.LoadScene("GameBoard");
    }
}
