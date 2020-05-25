using DUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtomsAndVoid : ModuleBase
{
    [SerializeField] private Image stickImage;

    [SerializeField] private GameObject KanadaAtom;

    [SerializeField] private GameObject VoidSim;
    private DUIAnchor VoidSimAnchor;
    [SerializeField] private GameObject VoidAtomTemplate;
    private bool simulateVoid = true;
    private List<GameObject> voidSimObjects;
    private const int SIM_COUNT = 50;

    private void Start()
    {
        VoidSimAnchor = VoidSim.GetComponent<DUIAnchor>();
        voidSimObjects = new List<GameObject>();
        VoidAtomTemplate.SetActive(false);

        KanadaAtom.SetActive(false);

        OnChange += Change;
    }

    public void CutClick()
    {
        stickImage.fillAmount = stickImage.fillAmount / 2;
    }

    private void Change(int index)
    {
        switch (index)
        {
            case 3:
                KanadaAtom.SetActive(false);
                break;
            case 4:
            case 5:
                KanadaAtom.SetActive(true);
                break;
            case 6:
                KanadaAtom.SetActive(false);
                simulateVoid = false;
                break;
            case 7:
                simulateVoid = true;
                StartCoroutine(RunVoidSim());
                break;
        }
    }

    private IEnumerator RunVoidSim()
    {
        VoidSim.SetActive(true);
        for (int i = 0; i< SIM_COUNT; i++)
        {
            GameObject obj;
            //get existing object
            if(voidSimObjects.Count == SIM_COUNT)
            {
                obj = voidSimObjects[i];
            }
            //make new object 
            else
            {
                obj = Instantiate(VoidAtomTemplate, Random.insideUnitCircle, Quaternion.identity, VoidSim.transform);
                voidSimObjects.Add(obj);
                obj.SetActive(true);
            }
            
            //apply a random force
            obj.GetComponent<Rigidbody2D>().velocity = Random.insideUnitCircle * 10;
        }

        while (simulateVoid)
        {
            foreach(GameObject obj in voidSimObjects)
            {
                //Keep in Bounds of DUI Anchor
                Vector2 pos = obj.transform.position;
                if (pos.y > VoidSimAnchor.Bounds.max.y + 0.5f)
                    pos.y = VoidSimAnchor.Bounds.min.y;
                if (pos.y < VoidSimAnchor.Bounds.min.y - 0.5f)
                    pos.y = VoidSimAnchor.Bounds.max.y;
                if (pos.x > VoidSimAnchor.Bounds.max.x + 0.5f)
                    pos.x = VoidSimAnchor.Bounds.min.x;
                if (pos.x < VoidSimAnchor.Bounds.min.x - 0.5f)
                    pos.x = VoidSimAnchor.Bounds.max.x;
                obj.transform.position = pos;
            }

            yield return new WaitForEndOfFrame();
        }
        VoidSim.SetActive(false);
    }
}
