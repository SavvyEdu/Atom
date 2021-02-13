using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleBase : MonoBehaviour
{
    [SerializeField] private string ProgressKey;
    [SerializeField] private Button BackButton;
    [SerializeField] private Button NextButton;

    private int index = 0;
    [SerializeField] private GameObject[] sequence;

    protected Action<int> OnChange;

    protected virtual void Awake()
    {
        BackButton.onClick.AddListener(BackClick);
        NextButton.onClick.AddListener(NextClick);
        index = 0;

        //set the first sequence active 
        foreach(GameObject g in sequence)
        {
            g.SetActive(false);
        }
        sequence[0].SetActive(true);

        BackButton.interactable = false;
    }

    public void BackClick()
    {
        sequence[index].SetActive(false);
        sequence[index - 1].SetActive(true);
        index--;
        OnChange?.Invoke(index);

        if (index == 0) { BackButton.interactable = false; }
        if (index == sequence.Length - 2) { NextButton.interactable = true; }
    }

    public void NextClick()
    {
        sequence[index].SetActive(false);
        sequence[index + 1].SetActive(true);
        index++;
        OnChange?.Invoke(index);

        if (index == 1) { BackButton.interactable = true; }
        if (index == sequence.Length - 1) { 
            NextButton.interactable = false;
            ModuleProgress.SetCompleted(ProgressKey);
        }
    }
}
