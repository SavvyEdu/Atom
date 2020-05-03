using UnityEngine;
using UnityEngine.UI;

namespace Atom
{
    public class AtomicAttributes : MonoBehaviour
    {
        [SerializeField] private Atom atom;
        [SerializeField] private Text formalNameUI;
        [SerializeField] private Text stableUI;

        [SerializeField] private Text electronCountsUI;

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

                string counts = "";
                Shell s = atom.OuterShell;
                while(s != null)
                {
                    counts += s.ElectronCount + ", ";
                    s = s.NextShell;
                }
                electronCountsUI.text = counts;
            }
            else
            {
                formalNameUI.text = "";
                stableUI.text = "";
                electronCountsUI.text = "";
            }
        }
    }
}

