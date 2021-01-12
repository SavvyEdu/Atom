using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using Atom;

public class MatchGame : MonoBehaviour
{
    //Game: generate a target element (from first 3 rows) and then start a timer to match the element

    [SerializeField] private Atom.Atom atom;
    [SerializeField] private AtomicSymbol targetSymbol;
    [SerializeField] private AtomicSymbol atomSymbol;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private SpriteRenderer glowEffect;

    int targetProtons, targetNeutrons, targetElectrons;
    bool completed = false;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        glowEffect.gameObject.SetActive(false); //hide the gloweffect
        tutorialUI.gameObject.SetActive(true); //show the tutorial

        //make the text color for target symbol blue
        targetSymbol.SetUIColors(UIColors.blue, true, true, true, true, true);

        SetRandomTarget();
    }
    private void Update()
    {
        //check for each of the 3 particle counts
        bool pMatch = atom.Nucleus.ProtonCount == targetProtons;
        bool nMatch = atom.Nucleus.NeutronCount == targetNeutrons;
        bool eMatch = atom.ElectronCount == targetElectrons;

        //adjust the colors the be blue when matched
        atomSymbol.SetUIColors( 
            c: UIColors.blue,
            name: pMatch, 
            abb: pMatch, 
            number: pMatch, 
            mass: pMatch && nMatch, 
            charge: eMatch);

        //check for all matching
        if (!completed && pMatch && nMatch && eMatch) 
        {
            completed = true;

            // Play the completed SFX
            audioSource.Play();

            //Visual glow then Make a new target after
            StartCoroutine(GlowFX(audioSource.clip.length, SetRandomTarget));
        }
    }

    private IEnumerator GlowFX(float time, Action callback = null)
    {
        glowEffect.gameObject.SetActive(true);

        float t = 0, p = 0;
        Color c = glowEffect.color;
        while (t < time)
        {
            t += Time.deltaTime;
            p = t / time;

            c.a = 1 - (p*p); //alpha fade 1 -> 0
            glowEffect.color = c;
            glowEffect.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 10f, p*p); //scale up 

            yield return new WaitForEndOfFrame();
        }

        //invoke the callback function if there is one
        callback?.Invoke();
    }

    private void SetRandomTarget()
    {
        completed = false;

        targetProtons = Random.Range(0, 18) + 1;
        Element targetElement = Elements.GetElement(targetProtons);

        int mass = targetElement.Common.Mass;
        targetNeutrons = mass - targetProtons;
        targetElectrons = targetProtons;

        targetSymbol.SetSymbol(targetElement, targetProtons, mass, targetElectrons);
    }


}
