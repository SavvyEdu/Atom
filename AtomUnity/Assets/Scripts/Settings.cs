﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    [Header("Simulation")]

    [SerializeField] private Toggle shakeToggle;
    public static bool SHAKE = true;

    [SerializeField] private Toggle orbitToggle;
    public static bool ORBIT = true;

    [SerializeField] private Toggle colorToggle;
    public static bool COLOR = true;

    [Header("Audio")]

    [SerializeField] private Toggle muteToggle;
    public static bool MUTE = false;

    [SerializeField] private Slider sfxSlider;
    public static float SFX_VOLUME = 0.5f;

    [SerializeField] private Slider musicSlider;
    public static float MUSIC_VOLUME = 0.5f;

    [Header("Advanced")]
    [SerializeField] private Toggle orbitalsToggle;
    public static bool ORBITALS = true;

    [SerializeField] private Button restoreButton;


    private void Awake()
    {
        //set defaluts 
        shakeToggle.isOn = SHAKE;
        orbitToggle.isOn = ORBIT;
        colorToggle.isOn = COLOR;

        muteToggle.isOn = MUTE;
        sfxSlider.value = SFX_VOLUME;
        musicSlider.value = MUSIC_VOLUME;

        orbitalsToggle.isOn = ORBITALS;

        //update settings
        shakeToggle.onValueChanged.AddListener((bool v) => SHAKE = v);
        orbitToggle.onValueChanged.AddListener((bool v) => ORBIT = v);
        colorToggle.onValueChanged.AddListener((bool v) => COLOR = v);

        muteToggle.onValueChanged.AddListener((bool v) => MUTE = v);
        sfxSlider.onValueChanged.AddListener((float v) => SFX_VOLUME = v);
        musicSlider.onValueChanged.AddListener((float v) => MUSIC_VOLUME = v);

        orbitalsToggle.onValueChanged.AddListener((bool v) => ORBITALS = v);
        restoreButton.onClick.AddListener(() =>
        {
            shakeToggle.isOn = SHAKE = true;
            orbitToggle.isOn = ORBIT = true;
            colorToggle.isOn = COLOR = true;

            muteToggle.isOn = MUTE = false;
            sfxSlider.value = SFX_VOLUME = 0.5f;
            musicSlider.value = MUSIC_VOLUME = 0.5f;

            orbitalsToggle.isOn = ORBITALS = true;
        });
    }

    private void Start()
    {
        //update the music manager
        MusicManager mm = FindObjectOfType<MusicManager>();
        if (mm)
        {
            mm.SetVolume();
            muteToggle.onValueChanged.AddListener((bool v) => mm.SetVolume());
            musicSlider.onValueChanged.AddListener((float v) => mm.SetVolume());
        }
    }
}
