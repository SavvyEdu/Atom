using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))] //not necessary but prefered to be on canvas
[RequireComponent(typeof(AudioSource))]
public class ButtonClickManager : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip toggleClickSFX;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //get ALL the buttons
        Button[] buttons = GetComponentsInChildren<Button>(true); 
        foreach(Button b in buttons)
        {
            //play button SFX onClick
            b.onClick.AddListener(() => source.PlayOneShot(buttonClickSFX));
        }

        //get ALL the toggles
        Toggle[] toggles = GetComponentsInChildren<Toggle>(true);
        foreach (Toggle t in toggles)
        {
            //play toggle SFX onClick
            t.onValueChanged.AddListener((bool b) => source.PlayOneShot(toggleClickSFX));
        }
    }
}
