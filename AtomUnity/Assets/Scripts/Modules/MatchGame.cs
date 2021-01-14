using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using Atom;
using Atom.Util;

public class MatchGame : MonoBehaviour
{
    //Game: generate a target element (from first 3 rows) and then start a timer to match the element

    [SerializeField] private Atom.Atom atom;
    [SerializeField] private AtomicSymbol targetSymbol;
    [SerializeField] private AtomicSymbol atomSymbol;
    
    [Header("Effects")]
    [SerializeField] private GameObject stars;
    private SVGImage[] starImages;
    [SerializeField] private SpriteRenderer glowEffect;
    private bool glowAnimating = false;
    [SerializeField] private Text timerText;

    [Header("Panel")]
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private Text message;
    [SerializeField] private Text title;
    [TextArea] [SerializeField] private string tutorialText;
    [TextArea] [SerializeField] private string winText;
    [TextArea] [SerializeField] private string loseText;

    private int targetProtons = 1, targetNeutrons = 0, targetElectrons = 1;
    private int numCompleted = 0;
    private bool challenge = false;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        starImages = stars.GetComponentsInChildren<SVGImage>();

        glowEffect.gameObject.SetActive(false); //hide the gloweffect
        
        tutorialUI.gameObject.SetActive(true); //show the tutorial
        title.text = "SYMBOL MATCH";
        message.text = tutorialText;

        //make the text color for target symbol blue
        targetSymbol.SetUIColors(UIColors.blue, true, true, true, true, true);

        atom.Interactable = false;

        Random.InitState((int)Time.time);
    }

    private void Start()
    {
        ResetGame();
    }

    public void StartGame()
    {
        atom.Interactable = true;
        StartCoroutine("Coutdown");
    }

    private void ResetGame()
    {
        //remove atom particles
        atom.ForceToCommon(1);

        //reset targets
        targetProtons = 1;
        SetRandomTarget();

        //reset stars
        numCompleted = 0;
        foreach (SVGImage star in starImages)
        {
            star.color = UIColors.lightGrey;
        }
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
        if (!glowAnimating && pMatch && nMatch && eMatch) 
        {
            glowAnimating = true;

            starImages[numCompleted].color = Color.yellow;
            numCompleted++;
            if(numCompleted == stars.transform.childCount)
            {
                StopCoroutine("Coutdown"); //stop the coutdown
                challenge = true;
                ResetGame();

                //display the win message 
                tutorialUI.gameObject.SetActive(true); //show the tutorial
                title.text = "Nice Work!";
                message.text = winText;
                atom.Interactable = false;
            }
            
            // Play the completed SFX
            audioSource.Play();

            //Visual glow then Make a new target after
            StartCoroutine(GlowFX(audioSource.clip.length, SetRandomTarget));
        }
    }

    private IEnumerator Coutdown()
    {
        int t = challenge ? 60 : 120;
        while(t > 0)
        {
            yield return new WaitForSeconds(1);
            t--;

            int second = t % 60;
            if(second < 10) 
                timerText.text = $"{t / 60}:0{second}";
            else
                timerText.text = $"{t / 60}:{second}";
        }
        ResetGame();

        //display the win message 
        tutorialUI.gameObject.SetActive(true); //show the tutorial
        title.text = "Out of Time";
        message.text = loseText;
        atom.Interactable = false;
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
        glowAnimating = false;

        //determine target protons (randomly increase the count)
        targetProtons = Random.Range(targetProtons + 2, targetProtons + 5);
        Element targetElement = Elements.GetElement(targetProtons);

        //determine target neutrons (use common element)
        int mass = targetElement.Common.Mass;
        targetNeutrons = mass - targetProtons;

        //determine target electrons
        targetElectrons = targetProtons;
        Debug.Log(targetElement.Block);
        switch (targetElement.Block)
        {
            case BlockType.sBlock:
                targetElectrons += Random.Range(0, 2); break;
            case BlockType.pBlock:
                targetElectrons -= Random.Range(0, 3); break;
        }

        targetSymbol.SetSymbol(targetElement, targetProtons, mass, targetElectrons);
    }


}
