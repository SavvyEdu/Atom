using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Atom;

public class Dalton : ModuleBase
{
    [Header("Element View")]
    [SerializeField] private GameObject buttonGroup;

    [SerializeField] private Image dElementImage;
    [SerializeField] private Text dElementName;
    [SerializeField] private Text dElementWeight;
    [SerializeField] private AtomicSymbol dElementAtomicSymbol;

    private struct DaltonElement { 

        public string name; //Dalton's name for element
        public int weight; //Daltons's proposed relative weight
        public Element modern; //Modern Element equivalent

        public DaltonElement(string name, int weight, Element modern)
        {
            this.name = name;
            this.weight = weight;
            this.modern = modern;
        }
    }

    DaltonElement[] daltonElements;
    private void Awake()
    {
        //get the dalton element information (called in Awake because of GetElement)
        daltonElements = new DaltonElement[20]{
            new DaltonElement("Hydrogen",   1, Elements.GetElement(1)),
            new DaltonElement("Azote",      5, Elements.GetElement(7)),
            new DaltonElement("Carbone",    5, Elements.GetElement(6)),
            new DaltonElement("Oxygen",     7, Elements.GetElement(8)),
            new DaltonElement("Phosphorus", 9, Elements.GetElement(15)),
            new DaltonElement("Sulfur",     13, Elements.GetElement(16)),
            new DaltonElement("Magnesia",   20, Elements.GetElement(12)),
            new DaltonElement("Lime",       23, Elements.GetElement(20)),
            new DaltonElement("Soda",       28, Elements.GetElement(11)),
            new DaltonElement("Potash",     42, Elements.GetElement(19)),
            new DaltonElement("Strontites", 46, Elements.GetElement(38)),
            new DaltonElement("Barytes",    68, Elements.GetElement(56)),
            new DaltonElement("Iron",       38, Elements.GetElement(26)),
            new DaltonElement("Zinc",       56, Elements.GetElement(30)),
            new DaltonElement("Copper",     56, Elements.GetElement(29)),
            new DaltonElement("Lead",       95, Elements.GetElement(82)),
            new DaltonElement("Silver",     100, Elements.GetElement(47)),
            new DaltonElement("Platina",    100, Elements.GetElement(78)),
            new DaltonElement("Gold",       140, Elements.GetElement(79)),
            new DaltonElement("Mercury",    167, Elements.GetElement(80)),
        };
    }


    private void Start()
    {
        //get all the element buttons
        Button[] daltonButtons = buttonGroup.GetComponentsInChildren<Button>();

        int length = daltonElements.Length; //use data length in case of extra buttons

        for (int i = 0; i < length; i++)
        {
            Button button = daltonButtons[i];
            DaltonElement dElement = daltonElements[i];

            //assign the sprite and info change event
            button.onClick.AddListener(() => {

                dElementImage.sprite = button.transform.GetChild(0).GetComponent<Image>().sprite;
                dElementName.text = dElement.name;
                dElementWeight.text = "mass: " + dElement.weight;
                dElementAtomicSymbol.SetSymbol(dElement.modern); //show modern equivalent
            });
        }

        //Invoke event for first element
        daltonButtons[0].onClick?.Invoke();
    }
}
