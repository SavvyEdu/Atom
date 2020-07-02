using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwapButton : MonoBehaviour
{
    [SerializeField] private int t1;
    [SerializeField] private int t2;

    public UnityEvent t1Active;
    public UnityEvent t2Active;

    private int t = 0;  

    private void Awake()
    {
        gameObject.SetActive(Settings.ORBITALS);

        t = t1;
        t1Active?.Invoke();

        Transition transition = FindObjectOfType<Transition>();
        Button button = GetComponentInChildren<Button>();

        button.onClick.AddListener(() =>
        {
            //make sure transitioner is not already transitioning
            if (!transition.Transitioning)
            {
                if (t == t1)
                {
                    transition.StartTransition(t = t2); //start transition
                    t2Active?.Invoke();
                }
                else if (t == t2)
                {
                    transition.StartTransition(t = t1);
                    t1Active?.Invoke();
                }
            }
            
        });
    }
}
