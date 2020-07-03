using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atom.Util;
using System;
using DUI;
using UnityEngine.AI;
using System.Data;

namespace Atom {

    [RequireComponent(typeof(DUIAnchor), typeof(DUIButton))]
    public class Orbitals : MonoBehaviour
    {
        [SerializeField] private Atom atom;
        [SerializeField] private Transform root;
        [SerializeField] private Transform[] axis;
        private DUIAnchor m_anchor; //ref to own DUI anchor
        private DUIButton m_button; //ref to own DUI button
        private BoxCollider m_collider; //ref to own DUI button

        [Header("Orbitals")]
        [SerializeField] private GameObject[] sOrbitals;
        [SerializeField] private GameObject[] pOrbitals;
        [SerializeField] private GameObject[] dOrbitals;
        [SerializeField] private GameObject[] fOrbitals;

        private GameObject currentOrbital = null;
        private int currentElectronCount = 0;

        private const float DRAG_SPEED = 1;
        private Vector3 AXIS_SCALE => new Vector3(0.025f, 1f, 0.025f);

        private Dictionary<int, OrbitalData> dict = new Dictionary<int, OrbitalData>();

        public static Action<int, int, int, int> OnUpdtae;

        private void Awake()
        {
            gameObject.SetActive(Settings.ORBITALS);

            m_anchor = GetComponent<DUIAnchor>();
            m_button = GetComponent<DUIButton>();
            m_collider = GetComponent<BoxCollider>();

            m_button.OnDrag += (Vector2 drag) =>
            {
                root.RotateAround(Vector3.up, drag.x * DRAG_SPEED);
                root.RotateAround(Vector3.right, -drag.y * DRAG_SPEED);
            };
        }

        private void Start()
        {
            CreateAllOrbitals();
            
            m_collider.size = m_anchor.Bounds.size;

            foreach (Transform a in axis) { a.gameObject.SetActive(Settings.AXIS); }
        }

        private void Update()
        {
            int electronCount = atom.ElectronCount;

            if (atom.Element != null && electronCount > 0)
            {
                ShowOrbital(electronCount);
            }
            else
            {
                currentElectronCount = 0;
            }
        }

        public void DisplayOrbitals(int electronCount)
        {
            ShowOrbital(electronCount);
        }

        public void ShowOrbital(int electronCount)
        {
            if (electronCount != currentElectronCount && electronCount > 0)
            {   /*
                int n, l, ml, ms;

                Element electronElement = Elements.GetElement(electronCount);
                bool inOuterShell = atom.OuterShell.ElectronCount > 0;
                Shell electronShell = inOuterShell ? atom.OuterShell : atom.OuterShell.NextShell;

                //calculate n
                n = atom.ShellCount;
                if (!inOuterShell) { n -= 1; }
                if (electronElement.Block == BlockType.dBlock) { n -= 1; }
                if (electronElement.Block == BlockType.fBlock) { n -= 2; }

                //calculate l
                l = (int)electronElement.Block; //spdf => 0123

                //calculate ml
                switch (electronElement.Block)
                {
                    default:
                    case BlockType.sBlock: ml = 0; break;
                    case BlockType.pBlock: ml = ((electronShell.ElectronCount - 3) % 3) - 1; break;//subtract s block+1, set range to [0,2], move range to [-1, 1]
                    case BlockType.dBlock: ml = ((electronShell.NextShell.ElectronCount - 9) % 5) - 2; break;//subtract sp blocks+1, set range to [0,4], move range to [-2, 2]
                    case BlockType.fBlock: ml = ((electronShell.NextShell.NextShell.ElectronCount - 19) % 7) - 3; break;//subtract spd block+1, set range to [0,6], move range to [-3, 3]
                }

                //calculate ms
                switch (electronElement.Block)
                {
                    default:
                    case BlockType.sBlock: ms = atom.ElectronCount <= 1 ? -1 : 1; break;
                    case BlockType.pBlock: ms = electronShell.ElectronCount - 2 <= 3 ? -1 : 1; break;//subtract s block+1, set range to [0,2], move range to [-1, 1]
                    case BlockType.dBlock: ms = electronShell.NextShell.ElectronCount - 8 <= 5 ? -1 : 1; break;//subtract sp blocks+1, set range to [0,4], move range to [-2, 2]
                    case BlockType.fBlock: ms = electronShell.NextShell.NextShell.ElectronCount - 18 < 7 ? -1 : 1; break;//subtract spd block+1, set range to [0,6], move range to [-3, 3]
                }*/

                OrbitalData data = dict[electronCount];

                //activate and deactivate all necessary orbitals
                currentOrbital?.SetActive(false); //deactive old orbital
                currentOrbital = data.obj;
                currentOrbital.SetActive(true); //activate new orbital

                //update the axis
                if (Settings.AXIS) 
                {
                    float axisLength = currentOrbital.transform.localScale.x * 1.15f;
                    foreach (Transform a in axis) { a.localScale = AXIS_SCALE * axisLength; }
                }

                //Adjust 
                AdjustScale();
                currentElectronCount = electronCount; //update current orbiatal value
                OnUpdtae?.Invoke(data.n, data.l, data.ml, data.ms);
            }
        }


        /// <summary>
        /// Loop through all possible n, l, m to 
        /// </summary>
        public void CreateAllOrbitals()
        {
            int e = 0;
            //loop over all 7 energy levels
            for(int n = 1; n <= 7; n++)
            {
                int maxL = (int)(-Mathf.Abs(n - 4.5f) + 3.5f); //generates: 0 1 2 3 3 2 1
                //loop over appropriate spdf
                for (int l = 0; l <= maxL; l++)
                {
                    //loop over spin down(-1) then up(+1)
                    for(int ms = -1; ms <= 1; ms += 2)
                    {
                        //loop over possible magnetic [-l, l]
                        for (int ml = -l; ml <= l; ml++)
                        {
                            e += 1;
                            
                            //modify e based on n & l to get electron count
                            int electronCount = e;
                            if(l == 0 && n > 3) { electronCount -= 10; }
                            if(l == 0 && n > 4) { electronCount -= 14; }
                            if(l == 0 && n == 6) { electronCount -= 14; }
                            if(l == 1 && (n == 5 || n == 6)) { electronCount -= 14; }
                            if(l == 2) { electronCount += 2; }
                            if(l == 3) { electronCount += 10; }

                            //Debug.Log(electronCount + ": " + n + " " + l + " " + ml + " " + ms);


                            CreateOrbital(electronCount, n, l, ml, ms); //s1
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Instantiate an orbiatl with scale and color
        /// </summary>
        /// <param name="n">Shell [1, 7] </param>
        /// <param name="l">SPDF [0, 3] </param>
        /// <param name="ml">obital [-3, 3] </param>
        public GameObject CreateOrbital(int electronCount, int n, int l, int ml, int ms)
        {
            GameObject orbital = null;
            float scale = n;

            if (l == 0)
            {
                orbital = sOrbitals[ml];
                scale = 1f * n;
            }
            else if (l == 1)
            {
                orbital = pOrbitals[ml + 1];
                scale = 1.25f * n;
            }
            else if (l == 2)
            {
                orbital = dOrbitals[ml + 2];
                scale = 4f / 3 * n;
            }
            else if (l == 3)
            {
                orbital = fOrbitals[ml + 3];
                scale = 1.5f * n - 2;
            }

            GameObject obj = Instantiate(orbital, Vector3.zero, Quaternion.identity, root);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one * scale;

            //set the appropriate spdf color
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            Color color = BlockTypeUtil.ColorFromBlock((BlockType)l);
            foreach (Renderer r in renderers)
            {
                r.material.SetColor("_Color", color);
            }

            obj.SetActive(false);
            dict.Add(electronCount, new OrbitalData(obj, n, l, ml, ms));
            return obj;
        }

        private bool ValidConfig(int n, int l, int ml)
        {
            if (n < 1 || n > 7) return false; //validate n between 1 and 7
            if (l < 0 || l > -Mathf.Abs(n - 4.5f) + 3.5f) return false; //valudate l based on n
            if (Mathf.Abs(ml) > l) return false; //validate |m| is less than l 
            return true;
        }


        public void AdjustScale()
        {
            if (currentOrbital != null)
            {
                //calculate scale = maxRadius / baseRadius 
                float minAxis = Mathf.Min(m_anchor.Bounds.extents.x, m_anchor.Bounds.extents.y); //minor axis
                float scale = Mathf.Min(1, (minAxis * 0.9f) / currentOrbital.transform.localScale.x);
                root.localScale = Vector3.one * scale;
            }
            m_collider.size = m_anchor.Bounds.size;
        }

        private struct OrbitalData
        {
            public GameObject obj;
            public int n, l, ml, ms;

            public OrbitalData(GameObject obj, int n, int l, int ml, int ms)
            {
                this.obj = obj;
                this.n = n;
                this.l = l;
                this.ml = ml;
                this.ms = ms;
            }
        }

    }

}