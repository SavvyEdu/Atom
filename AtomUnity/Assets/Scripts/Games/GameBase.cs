using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public abstract class GameBase : MonoBehaviour
{
    protected GameTutorialUI tutorialUI;
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        //find and show the tutorial message
        tutorialUI = FindObjectOfType<GameTutorialUI>(true);
        tutorialUI.playButton.onClick.AddListener(StartGame);
        tutorialUI.continueButton.onClick.AddListener(ContinueGame);
        tutorialUI.ShowTutorialMessage();
    }

    protected virtual void Start()
    {
        ResetGame();
    }

    /// <summary>
    /// Called when the game is started
    /// </summary>
    public abstract void StartGame();

    /// <summary>
    /// Called when the win condition is met (also by Start)
    /// </summary>
    public abstract void ResetGame();

    /// <summary>
    /// Called once the game is unpaused by tutorialUI
    /// </summary>
    public abstract void ContinueGame();
}
