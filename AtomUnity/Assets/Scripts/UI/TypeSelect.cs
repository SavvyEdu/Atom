using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TypeSelect : MonoBehaviour
{
    private List<string> options = new List<string>();

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Text text;

    private int index = 0;
    public T GetValue<T>() => (T)(object)index;
    public void SetValue<T>(T value) => index = (int)(object)value;

    public UnityEvent onValueChanged;

    private void Awake()
    {
        leftButton.onClick.AddListener(() => {
            index = (index + 1) % options.Count;
            text.text = options[index];
            onValueChanged?.Invoke();
        });

        rightButton.onClick.AddListener(() => {
            index = (index + options.Count - 1) % options.Count;
            text.text = options[index];
            onValueChanged?.Invoke();
        });
    }

    public void SetType<T>()
    {
        options.Clear();
        foreach(var x in Enum.GetValues(typeof(T)))
        {
            options.Add(x.ToString());
        }
    }

}
