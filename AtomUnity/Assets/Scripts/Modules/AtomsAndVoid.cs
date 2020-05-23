using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtomsAndVoid : MonoBehaviour
{
    [SerializeField] private Button BackButton;
    [SerializeField] private Button NextButton;

    private int index = 0;
    [SerializeField] private GameObject[] sequence;

    private void Awake()
    {
        BackButton.onClick.AddListener(BackClick);
        NextButton.onClick.AddListener(NextClick);

        BackButton.gameObject.SetActive(false);

        index = 0;
    }

    public void BackClick()
    {
        sequence[index].SetActive(false);
        sequence[index-1].SetActive(true);
        index--;
        if(index == 0) { BackButton.gameObject.SetActive(false); }
    }

    public void NextClick()
    {
        if (index == 0) { BackButton.gameObject.SetActive(true); }
        if(index == sequence.Length - 1) { return; }
        sequence[index].SetActive(false);
        sequence[index+1].SetActive(true);
        index++;
    }
}
