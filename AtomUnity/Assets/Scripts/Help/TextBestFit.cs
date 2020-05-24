using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class TextBestFit : MonoBehaviour
{
    private Text[] texts;

    private void Awake()
    {
        //get ALL the buttons
        texts = GetComponentsInChildren<Text>(true);
        StartCoroutine(Resize());
    }
    private IEnumerator Resize()
    {
        while (texts[0].cachedTextGenerator.fontSizeUsedForBestFit == 0)
        {
            yield return new WaitForSeconds(1.0f);
            foreach (Text t in texts)
            {
                if (t.resizeTextForBestFit & t.cachedTextGenerator.fontSizeUsedForBestFit > 0)
                {
                    t.resizeTextForBestFit = false;
                    t.fontSize = t.cachedTextGenerator.fontSizeUsedForBestFit;
                }
            }
        }
    }
    
}
