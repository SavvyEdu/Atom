using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atom;

public class Subatomic : ModuleBase
{
    /*
    Show descriptions for each of the subatomic particples 
    - show workbench element
    - mass, charge, location

    Show table of the values ^^

    Show 'blank' atom
    - add proton 
    - add nucleus
    - add electron
    - show UI counters for # of each, charge (with equation), mass (with equation)

    */

    [SerializeField] private Atom.Atom atom;
    [SerializeField] private Workbench workbench;
    private GameObject[] workbenchParticles = new GameObject[3];


    private void Start()
    {
        OnChange += Change;

        workbenchParticles[0] = workbench.transform.GetChild(0).gameObject;
        workbenchParticles[1] = workbench.transform.GetChild(1).gameObject;
        workbenchParticles[2] = workbench.transform.GetChild(2).gameObject;
        ShowWorkbench(false, false, false);

        atom.gameObject.SetActive(false);
    }

    void ShowWorkbench(bool proton, bool neutron, bool electron)
    {
        workbenchParticles[0].SetActive(proton); 
        workbenchParticles[1].SetActive(neutron);
        workbenchParticles[2].SetActive(electron);
    }


    private void Change(int index)
    {
        switch (index)
        {
            case 1:
                ShowWorkbench(true, false, false); //Proton only
                break;
            case 2:
                ShowWorkbench(false, true, false); //Proton only
                break;
            case 3:
                ShowWorkbench(false, false, true); //Proton only
                break;
            case 4:
                ShowWorkbench(true, true, true); //Proton only
                break;
        }
    }

}
