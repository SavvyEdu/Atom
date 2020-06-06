using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dalton : ModuleBase
{
    [SerializeField] private GameObject daltonElements;
    [SerializeField] private SpriteRenderer daltonElementImage;

    private void Start()
    {
        Button[] daltonButtons = daltonElements.GetComponentsInChildren<Button>();
        foreach(Button db in daltonButtons)
        {
            db.onClick.AddListener(() => {
                daltonElementImage.sprite =  db.transform.GetChild(0).GetComponent<SVGImage>().sprite;
            });
        }
    }
}
