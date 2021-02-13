using UnityEngine;
using UnityEngine.UI;

public abstract class GameBase : MonoBehaviour
{
    [SerializeField] private string ProgressKey;
    [SerializeField] protected GameTutorialUI tutorialUI;

    protected virtual void Awake()
    {
        //find and show the tutorial message
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
    /// 
    /// </summary>
    public virtual void CompleteGame()
    {
        ModuleProgress.SetCompleted(ProgressKey);
    }

    /// <summary>
    /// Called when the win condition is met (also by Start)
    /// </summary>
    public abstract void ResetGame();

    /// <summary>
    /// Called once the game is unpaused by tutorialUI
    /// </summary>
    public abstract void ContinueGame();
}
