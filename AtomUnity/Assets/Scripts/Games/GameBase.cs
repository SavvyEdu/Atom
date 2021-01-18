using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBase : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private Text message;
    [SerializeField] private Text title;
    [TextArea] [SerializeField] private string tutorialText;
    [TextArea] [SerializeField] private string winText;
    [TextArea] [SerializeField] private string loseText;

    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        ShowTutorialMessage();
    }

    public void ShowTutorialMessage()
    {
        tutorialUI.gameObject.SetActive(true); //show the tutorial
        title.text = "SYMBOL MATCH";
        message.text = tutorialText;
    }

    protected void ShowWinMessage()
    {
        tutorialUI.gameObject.SetActive(true); //show the tutorial
        title.text = "Nice Work!";
        message.text = winText;
    }

    protected void ShowLoseMessage()
    {
        tutorialUI.gameObject.SetActive(true);
        title.text = "Out of Time";
        message.text = loseText;
    }
}
