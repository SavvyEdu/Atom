using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmBox : MonoBehaviour
{
    /// <summary>
    /// Confirm user actions by showing standard confirm box
    /// </summary>

    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Text messageText;

    private void Awake()
    {
        cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
    public void ShowConfirm(UnityAction action, string message)
    {
        messageText.text = message;
        ShowConfirm(action);
    }

    public void ShowConfirm(UnityAction action)
    {
        gameObject.SetActive(true);
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(action);
    }
}
