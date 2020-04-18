using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            for(int i = 0; i < texts.Length; i++)
            {
                int protonCount = -1;
                if (i < 56) { protonCount = i + 1; } //elements up to Lathanoids
                else if (i > 56 && i < 74) { protonCount = i + 15; } //elements after Lathanoids and before Actinoids
                else if (i > 74 && i < 90) { protonCount = i + 29; } //elements after Actinoids and before END
                else if (i >= 90 && i < 105) { protonCount = i - 33; } //Lathanoids
                else if (i >= 105 && i< 120) { protonCount = i - 16; } //Actinoids

                Element element = Elements.GetElement(protonCount); //get bound element
                if (element != null)
                {
                    texts[i].text = element.Abbreviation;

                    //Hook up button to show the element data
                    Button b = texts[i].GetComponentInParent<Button>();
                    if(b != null)
                    {
                        b.onClick.AddListener(() => SetElement(protonCount));
                    }
                }
            }
        }

        

        /// <summary>
        /// Set the display element to the common form of whatever was just selected
        /// </summary>
        /// <param name="protonCount">proton count of element to set</param>
        private void SetElement(int protonCount)
        {
            Debug.Log("Show element: " + Elements.GetElement(protonCount).Name);

            if(atom != null)
            {
                atom.ForceToCommon(protonCount);
            }
        }
    }
}
