using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Atom.Util;

namespace Atom
{
    public enum PeriodicTableDisplayType { 
        Block, // abbreviation, color from s/p/d/f block type, yes numbers 
        Type, // abbreviation, color from type, yes numbers 
        NoNum, // abbreviation, color from type, no numbers 
        BlockCount // s/p/d/f, color from s/p/d/f block type, no numbers 
    }

    public class PeriodicTable : MonoBehaviour
    {
        [SerializeField] private Atom atom;
        [SerializeField] private PeriodicTableDisplayType displayType;
        /// <summary>
        /// Invoked when an element in the table is selected 
        /// </summary>
        public Action<int> OnElementSelect;

        private List<Button> buttons = new List<Button>();

        private void Awake()
        {
            Text[] texts = GetComponentsInChildren<Text>();

            //loop over the text elements in the table
            for (int t = 0; t < texts.Length; t += 2)
            {
                int i = t / 2; //index every 2 elements -> (number + abb)

                //get proton count from index
                int protonCount = -1;
                if (i < 56) { protonCount = i + 1; } //elements up to Lathanoids
                else if (i > 56 && i < 74) { protonCount = i + 15; } //elements after Lathanoids and before Actinoids
                else if (i > 74 && i < 90) { protonCount = i + 29; } //elements after Actinoids and before END
                else if (i >= 90 && i < 105) { protonCount = i - 33; } //Lathanoids
                else if (i >= 105 && i < 120) { protonCount = i - 16; } //Actinoids

                Element element = Elements.GetElement(protonCount); //get bound element
                if (element != null)
                {
                    //Hook up button to show the element data
                    Button b = texts[t + 1].GetComponentInParent<Button>();
                    b.onClick.AddListener(() => OnElementSelect?.Invoke(protonCount));
                    buttons.Add(b);

                    switch (displayType) //see enum for description
                    {
                        case PeriodicTableDisplayType.BlockCount:
                            texts[t].text = "";
                            texts[t + 1].text = BlockTypeUtil.BlockTypeToString[element.Block];
                            b.image.color = BlockTypeUtil.ColorFromBlock(element.Block);
                            break;
                        case PeriodicTableDisplayType.Block:
                            texts[t].text = protonCount.ToString();
                            texts[t + 1].text = element.Abbreviation;
                            b.image.color = BlockTypeUtil.ColorFromBlock(element.Block);
                            break;
                        case PeriodicTableDisplayType.Type:
                            texts[t].text = protonCount.ToString();
                            texts[t + 1].text = element.Abbreviation;
                            b.image.color = ElementTypeUtil.ColorFromType(element.Type);
                            break;
                        case PeriodicTableDisplayType.NoNum:
                            texts[t].text =  "";
                            texts[t + 1].text = element.Abbreviation;
                            b.image.color = ElementTypeUtil.ColorFromType(element.Type);
                            break;
                    }
                }
            }

            if(atom)
            {
                //update the atom to match selected element
                OnElementSelect += atom.ForceToCommon;
            }
        }

        public void ShowAtomicNumber(int protonCount)
        {
            buttons[protonCount-1].gameObject.GetComponentInChildren<Text>().text = protonCount.ToString();
        }

        public void HideAtomicNumber(int protonCount)
        {
            buttons[protonCount - 1].gameObject.GetComponentInChildren<Text>().text = "";
        }
    }
}
