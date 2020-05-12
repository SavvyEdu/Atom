using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("Simulation Settings")]

    [SerializeField] private Toggle shakeToggle;
    public static bool SHAKE = true;

    [SerializeField] private Toggle orbitToggle;
    public static bool ORBIT = true;

    [SerializeField] private Toggle colorToggle;
    public static bool COLOR = true;


    private void Awake()
    {
        //set defaluts 
        shakeToggle.isOn = SHAKE;
        orbitToggle.isOn = ORBIT;
        colorToggle.isOn = COLOR;

        //update settings
        shakeToggle.onValueChanged.AddListener((bool v) => SHAKE = v);
        orbitToggle.onValueChanged.AddListener((bool v) => ORBIT = v);
        colorToggle.onValueChanged.AddListener((bool v) => COLOR = v);
    }
}
