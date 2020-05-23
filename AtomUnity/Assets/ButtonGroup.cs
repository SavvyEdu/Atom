using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helps indicate which button in a group has been clicked
/// </summary>
public class ButtonGroup : MonoBehaviour
{
    private Button[] buttons;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        foreach(Button b_self in buttons)
        {
            b_self.onClick.AddListener(() =>
            {
                foreach(Button b_other in buttons)
                {
                    b_other.interactable = b_other != b_self; //set all other to interactable except self
                }
            });
        }
    }
}
