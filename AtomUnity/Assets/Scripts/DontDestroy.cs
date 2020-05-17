using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    //tracks objects that area in "Don't Destroy"
    private static List<string> objects;

    static DontDestroy(){
        objects = new List<string>();
    }

    private void Awake()
    {
        if (!objects.Contains(gameObject.name)){
            objects.Add(gameObject.name);
            DontDestroyOnLoad(gameObject);
        }
    }
}
