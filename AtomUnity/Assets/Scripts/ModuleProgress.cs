using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleProgress : MonoBehaviour
{
    //gameobject is part of the main scene, contains key and GameObject
    [SerializeField] private ModuleData[] moduleData;

    private void Awake()
    {
        foreach (ModuleData data in moduleData)
        {
            //PlayerPrefs.GetInt(data.key, 0) == 1;
            
        }
    }


    //TODO: should probably loop over keys 
    //TODO: hook up to settings panel UI 
    public void EraseAllProgress() {
        PlayerPrefs.DeleteAll();
    }
}


public struct ModuleData
{
    public string key;
    public GameObject UI;
    public bool hasBonus;
}