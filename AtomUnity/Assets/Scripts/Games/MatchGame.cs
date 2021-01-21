using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using Atom;
using Atom.Util;

public class MatchGame : GameBase
{
    //Game: generate a target element (from first 3 rows) and then start a timer to match the element

    [SerializeField] private Atom.Atom atom;
    [SerializeField] private AtomicSymbol targetSymbol;
    [SerializeField] private AtomicSymbol atomSymbol;
    
    [Header("Effects")]
    [SerializeField] private GameObject stars;
    private SVGImage[] starImages;
    [SerializeField] private GlowFX glowFX;
    [SerializeField] private Text timerText;

    private int targetProtons = 1, targetNeutrons = 0, targetElectrons = 1;
    private int numCompleted = 0;
    private bool challenge = false;

    private bool gameRunning = false;

    protected override void Awake()
    {
        base.Awake();

        starImages = stars.GetComponentsInChildren<SVGImage>();
        
        //make the text color for target symbol blue
        targetSymbol.SetUIColors(UIColors.blue, true, true, true, true, true);

        atom.Interactable = false;

        Random.InitState((int)Time.time);
    }

    public override void StartGame()
    {
        if (gameRunning) return;
        gameRunning = true;

        atom.Interactable = true;
        StartCoroutine("Coutdown");

        //remove atom particles
        atom.ForceToCommon(1);
    }

    public override void ResetGame()
    {
        gameRunning = false;

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
        if (!glowFX.IsAnimating && pMatch && nMatch && eMatch) 
        {

            starImages[numCompleted].color = Color.yellow;
            numCompleted++;
            if(numCompleted == stars.transform.childCount)
            {
                StopCoroutine("Coutdown"); //stop the coutdown
                challenge = true;
                ResetGame();

                //display the win message 
                tutorialUI.ShowWinMessage();
                atom.Interactable = false;
            }
            
            // Play the completed SFX
            audioSource.Play();

            //Visual glow then Make a new target after
            glowFX.BeginAnimation(audioSource.clip.length, UIColors.yellow, callback: SetRandomTarget);
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

        //display the lose message 
        tutorialUI.ShowLoseMessage();
        atom.Interactable = false;
    }

    private void SetRandomTarget()
    {
        //determine target protons (randomly increase the count)
        targetProtons = Random.Range(targetProtons + 2, targetProtons + 5);
        Element targetElement = Elements.GetElement(targetProtons);

        //determine target neutrons (use common element)
        int mass = targetElement.Common.Mass;
        targetNeutrons = mass - targetProtons;

        //determine target electrons
        targetElectrons = targetProtons;
        switch (targetElement.Block)
        {
            case BlockType.sBlock:
                targetElectrons += Random.Range(0, 2); break;
            case BlockType.pBlock:
                targetElectrons -= Random.Range(0, 3); break;
            case BlockType.dBlock:
            case BlockType.fBlock:
            default:
                targetElectrons += Random.Range(-3, 2); break;
        }

        targetSymbol.SetSymbol(targetElement, targetProtons, mass, targetElectrons);
    }

    public override void ContinueGame()
    {
        throw new NotImplementedException();
    }
}
