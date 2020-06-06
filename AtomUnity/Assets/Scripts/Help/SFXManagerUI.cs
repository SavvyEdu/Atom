using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))] //not necessary but prefered to be on canvas
[RequireComponent(typeof(AudioSource))]
public class SFXManagerUI : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip toggleClickSFX;
    [SerializeField] private AudioClip sliderClickSFX;

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

        //get ALL the toggles
        Slider[] slider = GetComponentsInChildren<Slider>(true);
        foreach (Slider s in slider)
        {
            //play toggle SFX onClick
            PointerHandle fx = s.gameObject.AddComponent<PointerHandle>();
            fx.upAction += SliderClick;
            fx.downAction += SliderClick;
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

    private void SliderClick()
    {
        Adjust();
        source.PlayOneShot(sliderClickSFX);
        
    }

    private void Adjust()
    {
        source.volume = Settings.MUTE ? 0 : 0.1f * Settings.SFX_VOLUME;
    }

    private class PointerHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action downAction;
        public Action upAction;

        public void OnPointerDown(PointerEventData eventData)
        {
            downAction?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            upAction?.Invoke();
        }
    }

}
