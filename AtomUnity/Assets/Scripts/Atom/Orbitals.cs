using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atom.Util;
using System;
using DUI;

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

        private OrbitalMap map = new OrbitalMap();
        private GameObject currentOrbital = null;
        private int N = 0, L = 0, ML = 0, MS = 0;

        private const float DRAG_SPEED = 1;
        private Vector3 AXIS_SCALE => new Vector3(0.025f, 1f, 0.025f);

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
            int n = 0, l = 0, ml = 0, ms = 0;

            if (atom.Element != null && atom.ElectronCount > 0)
            {
                Element e = Elements.GetElement(atom.ElectronCount);
                Shell shell;

                if (atom.OuterShell.ElectronCount > 0)
                {
                    n = atom.ShellCount;
                    shell = atom.OuterShell;
                }
                else
                {
                    n = atom.ShellCount - 1;
                    shell = atom.OuterShell.NextShell;
                }

                l = (int)e.Block;

                switch (e.Block)
                {
                    case BlockType.sBlock:
                        ml = 0;
                        break;
                    case BlockType.pBlock:
                        ml = ((shell.ElectronCount - 3) % 3) - 1; //subtract s block+1, set range to [0,2], move range to [-1, 1]
                        break;
                    case BlockType.dBlock:
                        n -= 1;
                        ml = ((shell.NextShell.ElectronCount - 9) % 5) - 2; //subtract sp blocks+1, set range to [0,4], move range to [-2, 2]
                        break;
                    case BlockType.fBlock:
                        n -= 2;
                        ml = ((shell.NextShell.NextShell.ElectronCount - 19 ) % 7) - 3; //subtract spd block+1, set range to [0,6], move range to [-3, 3]
                        break;
                }

                ms = atom.ElectronCount % 2; //  0,1

                if (n != N || l != L || ml != ML || ms != MS)
                {
                    currentOrbital?.SetActive(false); //deactive old orbital
                    currentOrbital = map.Get(n, l, ml);
                    currentOrbital.SetActive(true); //activate new orbital

                    if (Settings.AXIS)
                    {
                        float axisLength = currentOrbital.transform.localScale.x * 1.15f;
                        foreach (Transform a in axis) { a.localScale = AXIS_SCALE * axisLength; }
                    }

                    AdjustScale();
                    N = n; L = l; ML = ml; MS = ms; //update current orbiatal value

                    OnUpdtae?.Invoke(n, l, ml, ms);
                }
            }
            else
            {
                currentOrbital?.SetActive(false);
                N = 0; L = 0; ML = 0;
            }
        }

        /// <summary>
        /// Loop through all possible n, l, m to 
        /// </summary>
        public void CreateAllOrbitals()
        {
            for(int n = 1; n <= 7; n++)
            {
                for(int l = 0; l < 4; l++)
                {
                    if (l > -Mathf.Abs(n - 4.5f) + 3.5f) continue; //validate l based on n
                    for(int m = -3; m <= 3; m++)
                    {
                        if (Mathf.Abs(m) > l) continue; //validate |m| is less than l 
                        CreateOrbital(n, l, m); //s1
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
        public GameObject CreateOrbital(int n, int l, int ml)
        {
            //if (!ValidConfig(n, l, m)) return;

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
            map.Add(n, l, ml, obj);
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

        /// <summary>
        /// helper class used for storing orbitals with n, l, m accessors
        /// </summary>
        private class OrbitalMap {

            private Dictionary<int, GameObject> dict = new Dictionary<int, GameObject>();

            private int Key(int n, int l, int ml) => (n * 100) + (l * 10) + (ml + l);

            public void Add(int n, int l, int ml, GameObject obj)
            {
                int k = Key(n, l, ml); //make key
                if (dict.ContainsKey(k)) //make sure dictionary doesn't contains key
                    throw new Exception($"Map {k} already exists");
                dict.Add(k, obj);
            }

            public GameObject Get(int n, int l, int ml)
            {
                int k = Key(n, l, ml); //make key
                if (dict.ContainsKey(k)) //make sure dictionary contains key
                    return dict[k];
                return null;
               
            }
        }

    }

}