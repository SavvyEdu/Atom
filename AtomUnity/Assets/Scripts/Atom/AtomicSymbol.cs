using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atom
{
    public class AtomicSymbol : MonoBehaviour
    {
        [SerializeField] private Atom atom;
        [SerializeField] private Text nameUI;
        [SerializeField] private Text abbreviationUI;
        [SerializeField] private Text atomicNumberUI;
        [SerializeField] private Text massNumberUI;
        [SerializeField] private Text chargeUI;

        public void SetUIColors(Color c, bool name, bool abb, bool number, bool mass, bool charge)
        {
            nameUI.color = name ? c : UIColors.darkGrey;
            abbreviationUI.color = abb ? c : UIColors.darkGrey;
            atomicNumberUI.color = number ? c : UIColors.darkGrey;
            massNumberUI.color = mass ? c : UIColors.darkGrey;
            chargeUI.color = charge ? c : UIColors.darkGrey;
        }

        private void Update()
        {
            if (atom) //update to match the current Atom
            {
                SetSymbol(atom.Element, atom.Nucleus.ProtonCount, atom.Nucleus.Mass, atom.ElectronCount);
            }
        }

        /// <summary>
        /// Sets the UI elements based on an element common data 
        /// </summary>
        /// <param name="element">Element data</param>
        public void SetSymbol(Element element)
        {
            SetSymbol(element, element.AtomicNumber, element.Common.Mass, element.AtomicNumber);
        }

        /// <summary>
        /// Sets the UI elements based atom data
        /// </summary>
        /// <param name="element">Element data</param>
        /// <param name="protonCount">number of protons in atom</param>
        /// <param name="mass">mass of the atom</param>
        /// <param name="electronCount">number of electrons in atom</param>
        public void SetSymbol(Element element, int protonCount, int mass, int electronCount)
        {
            if (element != null)
            {
                nameUI.text = element.Name;
                abbreviationUI.text = element.Abbreviation;
            }
            else
            {
                nameUI.text = "";
                abbreviationUI.text = "";
            }

            atomicNumberUI.text = protonCount > 0 ? protonCount.ToString() : "";
            massNumberUI.text = mass > 0 ? mass.ToString() : "";

            int charge = protonCount - electronCount;
            if (charge > 0)
                chargeUI.text = charge + "+";
            else if (charge < 0)
                chargeUI.text = -charge + "-";
            else
                chargeUI.text = "";
        }
    }
}