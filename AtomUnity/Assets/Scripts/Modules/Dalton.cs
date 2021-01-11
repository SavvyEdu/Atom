using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Atom;
using DUI;

public class Dalton : ModuleBase
{
    [Header("Element View")]
    [SerializeField] private GameObject buttonGroup;

    [SerializeField] private Image dElementImage;
    [SerializeField] private Text dElementName;
    [SerializeField] private Text dElementWeight;
    [SerializeField] private AtomicSymbol dElementAtomicSymbol;

    [Header("Conservation of Mass Sim")]
    [SerializeField] private GameObject cofmSim;
    [SerializeField] private GameObject[] cofmSimWalls;
    private DUIAnchor CofMSimAnchor;
    [SerializeField] private GameObject CofMSimTemplate;
    [SerializeField] private Color c1, c2;
    private bool simulateVoid = true;
    private List<Rigidbody2D> cofmSimObjects;
    private const int SIM_COUNT = 50;

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
    protected override void Awake()
    {
        base.Awake();

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
        CofMSimAnchor = cofmSim.GetComponent<DUIAnchor>();
        cofmSimObjects = new List<Rigidbody2D>();
        CofMSimTemplate.SetActive(false);

        foreach (GameObject wall in cofmSimWalls)
        {
            BoxCollider collider = wall.GetComponent<BoxCollider>();
            DUIAnchor anchor = wall.GetComponent<DUIAnchor>();

            wall.transform.localScale = anchor.Bounds.size;
        }

        StartCoroutine(RunConservationOfMassSim());

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



    private IEnumerator RunConservationOfMassSim()
    {
        cofmSim.SetActive(true);
        for (int i = 0; i < SIM_COUNT; i++)
        {
            GameObject obj;
            //get existing object
            if (cofmSimObjects.Count == SIM_COUNT)
            {
                obj = cofmSimObjects[i].gameObject;
            }
            //make new object 
            else
            {
                Vector2 randPos = new Vector2(Random.Range(CofMSimAnchor.Bounds.min.x, CofMSimAnchor.Bounds.max.x),
                                              Random.Range(CofMSimAnchor.Bounds.min.y, CofMSimAnchor.Bounds.max.y)) * 0.95f;

                obj = Instantiate(CofMSimTemplate, randPos, Quaternion.identity, cofmSim.transform);
                cofmSimObjects.Add(obj.GetComponent<Rigidbody2D>());
                obj.SetActive(true);
            }

            //apply a random force
            //obj.GetComponent<Rigidbody2D>().velocity = Random.insideUnitCircle * 10;
        }

        while (simulateVoid)
        {
            Debug.DrawLine(Vector2.zero, (Vector2)DUI.DUI.inputPos, Color.red, 0.5f);
            foreach (Rigidbody2D rb2d in cofmSimObjects)
            {
                Vector2 diff = rb2d.transform.position - DUI.DUI.inputPos;
                Vector2 force = new Vector2(1 / (diff.x+0.5f), 1 / (diff.y+0.5f)) * 0.5f;
                rb2d.AddForce(force, ForceMode2D.Force);

                
                //Keep in Bounds of DUI Anchor
                Vector2 pos = rb2d.transform.position;
                pos.x = Mathf.Clamp(pos.x, CofMSimAnchor.Bounds.min.x, CofMSimAnchor.Bounds.max.x);
                pos.y = Mathf.Clamp(pos.y, CofMSimAnchor.Bounds.min.y, CofMSimAnchor.Bounds.max.y);
                rb2d.transform.position = pos;

            }

            yield return new WaitForEndOfFrame();
        }
        cofmSim.SetActive(false);
    }
}
