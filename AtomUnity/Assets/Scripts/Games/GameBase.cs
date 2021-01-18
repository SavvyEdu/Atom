using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameBase : MonoBehaviour
{
    [SerializeField] protected GameTutorialUI tutorialUI;

    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        tutorialUI.ShowTutorialMessage();
    }
}
