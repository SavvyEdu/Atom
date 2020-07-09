using Atom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuantumNumbers : MonoBehaviour
{
    [SerializeField] private Text nText;
    [SerializeField] private Text lText;
    [SerializeField] private Text mlText;
    [SerializeField] private Text msText;

    private void Awake()
    {
        Orbitals.OnUpdate += UpdateNumbers;
    }

    public void UpdateNumbers(int n, int l, int ml, int ms)
    {
        nText.text = n.ToString();
        lText.text = l.ToString();
        mlText.text = ml.ToString();
        msText.text = ms == 0 ? "-1/2" : "1/2";
    }

    private void OnDestroy()
    {
        Orbitals.OnUpdate -= UpdateNumbers;
    }
}
