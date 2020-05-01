using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DUI;
using UnityEngine.UI;
using Atom;

public class Transition : MonoBehaviour
{
    [SerializeField] private Trans[] transitions;

    //Lerp variables
    [SerializeField] private float lerpTime = 1.0f;
    private float currLerpTime = 0.0f;

    private bool t = false;

    private void Start()
    {
        StartTransition(0);
    }

    public void StartTransition(int index)
    {
        if (t) { return; }
        t = true;
        StartCoroutine(LerpTo(index));
        /*
        foreach (DUITrans DUITrans in transitions[index].DUItransitions)
        {
            DUITrans.Update(1);
        }
        foreach(UITrans UITrans in transitions[index].UItransitions)
        {
            UITrans.Update(1);
        }

        if (transitions[index].atomTransition != null)
        {
            transitions[index].atomTransition.atom.AdjustScale();
            transitions[index].atomTransition.atom.Interactable = transitions[index].atomTransition.interactable;
        }*/


        if (transitions[index].atomTransition != null)
        {
            transitions[index].atomTransition.atom.Interactable = transitions[index].atomTransition.interactable;
        }
    }

    private IEnumerator LerpTo(int index)
    {
        currLerpTime = 0;

        while (currLerpTime < lerpTime)
        {
            currLerpTime += Time.deltaTime;
            if (currLerpTime > lerpTime)
            {
                currLerpTime = lerpTime;
            }
            float p = currLerpTime / lerpTime;
            //p = p * p * (3f - 2f * p); //smooth step
            //p = p*p*p*(p*(p*6-15)+10); //smoother step
            //p = Mathf.Pow(p, 1f / 6f);
            foreach (DUITrans DUITrans in transitions[index].DUItransitions)
            {
                DUITrans.Update(p);
            }
            foreach (UITrans UITrans in transitions[index].UItransitions)
            {
                UITrans.Update(p);
            }
            if (transitions[index].atomTransition != null)
            {
                transitions[index].atomTransition.atom.AdjustScale();
            }
            yield return new WaitForEndOfFrame();
        }
        t = false;
    }
}

[System.Serializable]
public class Trans
{
    public string name;

    public DUITrans[] DUItransitions;
    public UITrans[] UItransitions;
    public AtomTrans atomTransition;
}

[System.Serializable]
public class AtomTrans
{
    public Atom.Atom atom;
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

[System.Serializable]
public class UITrans
{
    public RectTransform rectTransform;

    public Vector2 position;

    public void Update(float p)
    {
        rectTransform.localPosition = Vector2.Lerp(rectTransform.localPosition, position, p);
    }

}