using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))] //not necessary but prefered to be on canvas
[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip toggleClickSFX;

    private AudioSource source = null;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Adjust();

        //get ALL the buttons
        Button[] buttons = GetComponentsInChildren<Button>(true); 
        foreach(Button b in buttons)
        {
            //play button SFX onClick
            b.onClick.AddListener(ButtonClick);
        }

        //get ALL the toggles
        Toggle[] toggles = GetComponentsInChildren<Toggle>(true);
        foreach (Toggle t in toggles)
        {
            //play toggle SFX onClick
            t.onValueChanged.AddListener(ToggleClick);
        }
    }

    private void ButtonClick()
    {
        Adjust();
        source.PlayOneShot(buttonClickSFX);
    }

    private void ToggleClick(bool b)
    {
        Adjust();
        source.PlayOneShot(toggleClickSFX);
    }

    private void Adjust()
    {
        source.volume = Settings.MUTE ? 0 : 0.1f * Settings.SFX_VOLUME;
    }
}
