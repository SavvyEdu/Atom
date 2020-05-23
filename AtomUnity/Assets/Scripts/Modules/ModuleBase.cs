using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ModuleBase : MonoBehaviour
{
    [SerializeField] private Button BackButton;
    [SerializeField] private Button NextButton;

    private int index = 0;
    [SerializeField] private GameObject[] sequence;

    private void Awake()
    {
        BackButton.onClick.AddListener(BackClick);
        NextButton.onClick.AddListener(NextClick);
        index = 0;
    }

    public void BackClick()
    {
        if (index == 0) { return; }
        sequence[index].SetActive(false);
        sequence[index - 1].SetActive(true);
        index--;
        
    }

    public void NextClick()
    {
        if (index == sequence.Length - 1) { return; }
        sequence[index].SetActive(false);
        sequence[index + 1].SetActive(true);
        index++;
    }
}
