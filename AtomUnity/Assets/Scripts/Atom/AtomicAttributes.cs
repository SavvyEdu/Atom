using UnityEngine;
using UnityEngine.UI;

namespace Atom
{
    public class AtomicAttributes : MonoBehaviour
    {
        [SerializeField] private Atom atom;
        [SerializeField] private Text formalNameUI;
        [SerializeField] private Text stableUI;
        [SerializeField] private Text typeUI;
        [SerializeField] private Image typeImg;

        private void Update()
        {
            if (atom.Element != null)
            {
                Isotope isotope = atom.Element.GetIsotope(atom.Nucleus.Mass);
                if (isotope != null)
                {
                    formalNameUI.text = atom.Element.Name + "-" + atom.Nucleus.Mass;
                    stableUI.text = isotope.Stable ? "Stable" : "Radioactive";
                }
                else
                {
                    formalNameUI.text = "Not Isotope";
                    stableUI.text = "Radioactive";
                }
                typeUI.text = atom.Element.Type.ToString();
                typeImg.color = ElementTypeUtil.ColorFromType(atom.Element.Type);
            }
            else
            {
                formalNameUI.text = "";
                stableUI.text = "";
                typeUI.text = "";
                typeImg.color = Color.black;
            }
        }
    }
}

