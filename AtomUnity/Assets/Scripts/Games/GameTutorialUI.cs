﻿using UnityEngine;
using UnityEngine.UI;

public class GameTutorialUI : MonoBehaviour
{
    [SerializeField] private Text message;
    [SerializeField] private Text title;
    [SerializeField] public Button playButton;

    [TextArea] [SerializeField] private string tutorialText;
    [SerializeField] private string tutorialTitle;

    [TextArea] [SerializeField] private string winText;
    [SerializeField] private string winTitle;

    [TextArea] [SerializeField] private string loseText;
    [SerializeField] private string loseTitle;

    public void ShowTutorialMessage()
    {
        gameObject.SetActive(true); //show the tutorial
        title.text = tutorialTitle;
        message.text = tutorialText;
    }

    public void ShowWinMessage()
    {
        gameObject.SetActive(true); //show the tutorial
        title.text = winTitle;
        message.text = winText;
    }

    public void ShowLoseMessage()
    {
        gameObject.SetActive(true);
        title.text = loseTitle;
        message.text = loseText;
    }
}
