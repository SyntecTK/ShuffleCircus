using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
    [SerializeField] private Image _p1TicketCut;
    [SerializeField] private Image _p2TicketCut;
    [SerializeField] private TMP_InputField _p1NameInput;
    [SerializeField] private TMP_InputField _p2NameInput;

    private string p1Name;
    private string p2Name;
    private bool p1Ready;
    private bool p2Ready;

    private void OnEnable()
    {
        _p1NameInput.onSubmit.AddListener(OnInputEnd);
        _p2NameInput.onSubmit.AddListener(OnInputEnd);
    }

    private void OnDisable()
    {
        _p1NameInput.onSubmit.RemoveListener(OnInputEnd);
        _p2NameInput.onSubmit.RemoveListener(OnInputEnd);
    }

    private void OnInputEnd(string input)
    {
        if(_p1NameInput.isFocused)
        {
            _p1TicketCut.GetComponent<Animator>().Play("TicketRip");
            p1Name = string.IsNullOrEmpty(_p1NameInput.text) ? "Spieler 1" : _p1NameInput.text;
            Debug.Log("Entered on Ticket 1");
            p1Ready = true;
        }
        else if(_p2NameInput.isFocused)
        {
            _p2TicketCut.GetComponent<Animator>().Play("TicketRip");
            p2Name = string.IsNullOrEmpty(_p2NameInput.text) ? "Spieler 2" : _p2NameInput.text;
            Debug.Log("Entered on Ticket 2");

            p2Ready = true;
        }

        if(p1Ready && p2Ready)
        {
            StartCoroutine(LoadGameBoardRoutine());
            GameManager.Instance.State.SetPlayerNames(p1Name, p2Name);
        }
    }

    IEnumerator LoadGameBoardRoutine()
    {
        yield return new WaitForSeconds(2f);
        SceneLoader.Instance.LoadScene("GameBoard");
    }
}
