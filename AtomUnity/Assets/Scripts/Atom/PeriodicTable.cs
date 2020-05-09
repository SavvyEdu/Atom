using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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

                    b.image.color = ColorFromType(element.Type); //TODO: better color scheme
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

        private Color ColorFromType(ElementType type)
        {
            switch (type)
            {
                case ElementType.Alkali_Metal: return new Color(0.0f, 0.2f, 0.2f);
                case ElementType.Alkaline_Earth_Metal: return new Color(0.1f, 0.35f, 0.35f);

                case ElementType.Lanthanide: return new Color(0.0f, 0.7f, 0.7f); 
                case ElementType.Actinide: return new Color(0.1f, 0.6f, 0.6f); 

                case ElementType.Transition_Metal: return new Color(0, 0.8f, 0.5f);

                case ElementType.Metal: return new Color(0.0f, 0.7f, 1.0f);
                case ElementType.Metalloid: return new Color(0.1f, 0.5f, 0.9f);
                case ElementType.Nonmetal: return new Color(0.2f, 0.3f, 0.9f);
                case ElementType.Halogen: return new Color(0.1f, 0.15f, 0.7f);
                case ElementType.Noble_Gas: return new Color(0.0f, 0.0f, 0.5f);
               
                default: return new Color(0.0f, 0.0f, 0.0f);
            }
        }
    }
}
