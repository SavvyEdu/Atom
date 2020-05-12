using UnityEngine;
using DUI;
namespace Atom
{
    [RequireComponent(typeof(DUIAnchor))]
    public class Workbench : MonoBehaviour
    {
        /// <summary>
        /// handles the 
        /// </summary>

        [SerializeField] private GameObject ProtonPrefab;
        [SerializeField] private GameObject NeutronPrefab;
        [SerializeField] private GameObject ElectronPrefab;

        public void NewAutoProton(int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                Proton obj = Instantiate(ProtonPrefab, transform.GetChild(0)).GetComponent<Proton>();
                obj.PhysicsObj.AddForce(Random.insideUnitSphere); //inject some random offset
                obj.OnDeselect?.Invoke();
            }
        }

        public void NewAutoNeutron(int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                Neutron obj = Instantiate(NeutronPrefab, transform.GetChild(1)).GetComponent<Neutron>();
                obj.PhysicsObj.AddForce(Random.insideUnitSphere); //inject some random offset
                obj.OnDeselect?.Invoke();
            }
        }

        public void NewAutoElectron(int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                Electron obj = Instantiate(ElectronPrefab, transform.GetChild(2)).GetComponent<Electron>();
                obj.PhysicsObj.AddForce(Random.insideUnitSphere); //inject some random offset
                obj.OnDeselect?.Invoke();
            }
        }
        /// <summary>
        /// create a new proton
        /// </summary>
        public void NewProton()
        {
            GameObject obj = Instantiate(ProtonPrefab, transform.GetChild(0));

            Proton proton = obj.GetComponent<Proton>();
            if (proton != null)
            {
                proton.OnSelect?.Invoke();
            }
        }

        /// <summary>
        /// create a new neutron
        /// </summary>
        public void NewNeutron()
        {
            GameObject obj = Instantiate(NeutronPrefab, transform.GetChild(1));

            Neutron neutron = obj.GetComponent<Neutron>();
            if (neutron != null)
            {
                neutron.OnSelect?.Invoke();
            }
        }

        /// <summary>
        /// create a new electron
        /// </summary>
        public void NewElectron()
        {
            GameObject obj = Instantiate(ElectronPrefab, transform.GetChild(2));

            Electron electron = obj.GetComponent<Electron>();
            if (electron != null)
            {
                electron.OnSelect?.Invoke();
            }
        }
    }
}