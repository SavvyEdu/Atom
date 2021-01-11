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

    private void Awake()
    {
        SetRandomTarget();

        targetSymbol.atomicNumberColor = Colors.blue;
        targetSymbol.massNumberColor = Colors.blue;
        targetSymbol.chargeColor = Colors.blue;
    }

    private void Update()
    {
        bool pMatch = atom.Nucleus.ProtonCount == targetProtons;
        bool nMatch = atom.Nucleus.NeutronCount == targetNeutrons;
        bool eMatch = atom.ElectronCount == targetElectrons;

        atomSymbol.atomicNumberColor = pMatch ? Colors.blue : Colors.darkGrey;
        atomSymbol.massNumberColor = pMatch && nMatch ? Colors.blue : Colors.darkGrey;
        atomSymbol.chargeColor = eMatch ? Colors.blue : Colors.darkGrey;

        if (pMatch && nMatch && eMatch)
        {
            Debug.Log("NICE!");
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
