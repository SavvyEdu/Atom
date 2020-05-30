using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using Atom.Util;

namespace Atom
{
    public class PeriodicTable : MonoBehaviour
    {
        [SerializeField] private Atom atom;
        private Text[] texts;
        public Element Element { get; private set; }

        private void Awake()
        {
            texts = GetComponentsInChildren<Text>();

            //loop over the text elements in the table
            for(int t = 0; t < texts.Length; t+=2)
            {
                int i = t / 2;
                int protonCount = -1;
                if (i < 56) { protonCount = i + 1; } //elements up to Lathanoids
                else if (i > 56 && i < 74) { protonCount = i + 15; } //elements after Lathanoids and before Actinoids
                else if (i > 74 && i < 90) { protonCount = i + 29; } //elements after Actinoids and before END
                else if (i >= 90 && i < 105) { protonCount = i - 33; } //Lathanoids
                else if (i >= 105 && i< 120) { protonCount = i - 16; } //Actinoids

                Element element = Elements.GetElement(protonCount); //get bound element
                if (element != null)
                {
                    texts[t].text = protonCount.ToString();
                    texts[t+1].text = element.Abbreviation;

                    //Hook up button to show the element data
                    Button b = texts[t+1].GetComponentInParent<Button>();
                    if(b != null)
                    {
                        b.onClick.AddListener(() => SetElement(protonCount));
                    }

                    b.image.color = BlockTypeUtil.ColorFromBlock(element.Block);
                    //b.image.color = ElementTypeUtil.ColorFromType(element.Type); //TODO: better color scheme
                }
            }
        }

        /// <summary>
        /// Set the display element to the common form of whatever was just selected
        /// </summary>
        /// <param name="protonCount">proton count of element to set</param>
        private void SetElement(int protonCount)
        {
            if(atom != null)
            {
                atom.ForceToCommon(protonCount);
            }
        }
    }
}
