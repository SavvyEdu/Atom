using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Atom;

public class MatchGame : MonoBehaviour
{
    //Game: generate a target element (from first 3 rows) and then start a timer to match the element

    [SerializeField] private Atom.Atom atom;
    [SerializeField] private AtomicSymbol targetSymbol;
    [SerializeField] private AtomicSymbol atomSymbol;

    int targetProtons, targetNeutrons, targetElectrons;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();


        SetRandomTarget();

        targetSymbol.SetUIColors(Colors.blue, true, true, true, true, true);
    }

    private void Update()
    {
        bool pMatch = atom.Nucleus.ProtonCount == targetProtons;
        bool nMatch = atom.Nucleus.NeutronCount == targetNeutrons;
        bool eMatch = atom.ElectronCount == targetElectrons;

        atomSymbol.SetUIColors( 
            c: Colors.blue,
            name: pMatch && nMatch && eMatch, 
            abb: pMatch && nMatch && eMatch, 
            number: pMatch, 
            mass: pMatch && nMatch, 
            charge: eMatch);

        if (pMatch && nMatch && eMatch)
        {
            // Play the completed SFX
            audioSource.Play();

            //Visual glow


            SetRandomTarget();
        }
    }

    private void SetRandomTarget()
    {
        targetProtons = Random.Range(0, 18) + 1;
        Element targetElement = Elements.GetElement(targetProtons);

        int mass = Random.Range(targetElement.MinIsotope, targetElement.MaxIsotope + 1);
        targetNeutrons = mass - targetProtons;
        targetElectrons = targetProtons;

        targetSymbol.SetSymbol(targetElement, targetProtons, mass, targetElectrons);
    }


}
