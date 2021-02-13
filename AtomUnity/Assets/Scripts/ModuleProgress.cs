using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ModuleProgress : MonoBehaviour
{
    //gameobject is part of the main scene, contains key and GameObject
    [SerializeField] private ModuleData[] moduleData;

    private void Awake()
    {
        foreach (ModuleData data in moduleData)
        {
            bool completed = PlayerPrefs.GetInt(data.key, 0) == 1;
            Color color = completed ? Color.white : new Color(1, 1, 1, 0);

            data.moduleUI.GetComponentInChildren<SVGImage>().color = color;
        }
    }

    public static void SetCompleted(string key)
    {
        PlayerPrefs.SetInt(key, 1);
    }

    //TODO: should probably loop over keys 
    //TODO: hook up to settings panel UI 
    public void EraseAllProgress() {
        PlayerPrefs.DeleteAll();
    }
}

[System.Serializable]
public struct ModuleData
{
    public string key;
    public GameObject moduleUI;
}