using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DUI;
using UnityEngine.UI;
using Atom;

public class Transition : MonoBehaviour
{
    /// <summary>
    /// Groups together transitions between modes
    /// Not great but it works
    /// </summary>
    [SerializeField] private Trans[] transitions;

    //Lerp variables
    [SerializeField] private float lerpTime = 1.0f;
    private float currLerpTime = 0.0f;

    public bool Transitioning { get; private set; } = false;

    private void Start()
    {
        SetTo(0);
    }

    public void StartTransition(int index)
    {
        if (Transitioning) { return; }
        Transitioning = true;
        StartCoroutine(LerpTo(index));
    }

    private IEnumerator LerpTo(int index)
    {
        currLerpTime = 0;
        foreach (PanelTrans UITrans in transitions[index].panelTransitions)
        {
            UITrans.SetTarget();
        }
        while (currLerpTime < lerpTime)
        {
            currLerpTime += Time.deltaTime;
            if (currLerpTime > lerpTime)
            {
                currLerpTime = lerpTime;
            }
            float p = currLerpTime / lerpTime;
            p = p * p * (3f - 2f * p); //smooth step
            //p = p*p*p*(p*(p*6-15)+10); //smoother step
            //p = Mathf.Pow(p, 1f / 6f);
            foreach (DUITrans DUITrans in transitions[index].duiTransitions)
            {
                DUITrans.Update(p);
            }
            foreach (PanelTrans UITrans in transitions[index].panelTransitions)
            {
                UITrans.Update(p);
            }

            transitions[index].atomTransition.atom.AdjustScale();
            transitions[index].atomTransition.orbitals.AdjustScale();
            
            yield return new WaitForEndOfFrame();
        }

        transitions[index].atomTransition.atom.Interactable = transitions[index].atomTransition.interactable;
        Transitioning = false;
    }

    private void SetTo(int index)
    {
        foreach (DUITrans DUITrans in transitions[index].duiTransitions)
        {
            DUITrans.Update(1);
        }
        foreach (PanelTrans UITrans in transitions[index].panelTransitions)
        {
            UITrans.SetTarget();
            UITrans.Update(1);
        }

        transitions[index].atomTransition.atom.AdjustScale();
        transitions[index].atomTransition.orbitals.AdjustScale();
        transitions[index].atomTransition.atom.Interactable = transitions[index].atomTransition.interactable;
        Transitioning = false;
    }
}

[System.Serializable]
public class Trans
{
    public string name;

    public DUITrans[] duiTransitions;
    public PanelTrans[] panelTransitions;
    public AtomTrans atomTransition;
}

[System.Serializable]
public class AtomTrans
{
    public Atom.Atom atom;
    public Orbitals orbitals;
    public bool interactable;
}

[System.Serializable]
public class DUITrans
{
    public DUIAnchor anchor;
    public Vector2 min;
    public Vector2 max;
    
    public void Update(float p)
    {
        anchor.MinMax = new Vector2[] { Vector2.Lerp(anchor.MinMax[0], min , p),
                                        Vector2.Lerp(anchor.MinMax[1], max, p) };

    }
}

public enum Direction
{
    Up, Down, Left, Right, Center
}

[System.Serializable]
public class PanelTrans
{
    public RectTransform rectTransform;
    public Direction dir;
    private Vector2 target;

    public void SetTarget()
    { 
        switch (dir)
        {
            case Direction.Up: target = Vector2.up * Screen.height; break;
            case Direction.Down: target = Vector2.down * Screen.height; break;
            case Direction.Left: target = Vector2.left * Screen.width; break;
            case Direction.Right: target = Vector2.right * Screen.width; break;
            case Direction.Center: target = Vector2.zero; break;
        }  
    }

    public void Update(float p)
    {
        rectTransform.localPosition = Vector2.Lerp(rectTransform.localPosition, target, p);
    }
}