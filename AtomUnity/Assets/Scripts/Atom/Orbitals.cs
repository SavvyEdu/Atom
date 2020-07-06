using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atom.Util;
using System;
using DUI;
using UnityEngine.AI;
using System.Data;
using System.Data.Common;

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

        [Header("Materials")]
        [SerializeField] private Material mat_transparent;
        [SerializeField] private Material mat_solid;

        private GameObject currentOrbital = null;
        private int currentElectronCount = 0;

        private const float DRAG_SPEED = 1;
        private Vector3 AXIS_SCALE => new Vector3(0.025f, 1f, 0.025f);

        private OrbitalData[] orbitalMap = new OrbitalData[119]; 

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

            //turn on/off the Axis
            foreach (Transform a in axis) 
                a.gameObject.SetActive(Settings.AXIS);
        }


        private void Update()
        {
            //get the number of electrons in the Atom
            int electronCount = atom.ElectronCount;
            //check if changed
            if (electronCount != currentElectronCount)
            {
                OrbitalData data = orbitalMap[currentElectronCount];

                if (Settings.ORBITALS_ALL)
                {
                    //Add in Orbitals
                    while (electronCount > currentElectronCount)
                    {
                        currentElectronCount++;
                        data = orbitalMap[currentElectronCount];
                        data.obj.SetActive(true);
                    }
                    //Remove Orbitals
                    while (electronCount < currentElectronCount)
                    {
                        data.obj.SetActive(false);
                        currentElectronCount--;
                        data = orbitalMap[currentElectronCount];
                    }
                }
                else // (Only show current orbital)
                {
                    //deactivate the current
                    if (currentElectronCount != 0)
                        data.obj.SetActive(false);

                    //activate the new
                    data = orbitalMap[electronCount];
                    if (electronCount > 0)
                        data.obj.SetActive(true);
                }

                //unpdate the orbital data
                currentOrbital = data.obj;
                currentElectronCount = electronCount;
                OnUpdtae?.Invoke(data.n, data.l, data.ml, data.ms);

                //Adjust the display
                AdjustScale();
                AdjustAxis();
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
                scale = 1.33f * n;
            }
            else if (l == 2)
            {
                orbital = dOrbitals[ml + 2];
                scale = 1.5f * n;
            }
            else if (l == 3)
            {
                orbital = fOrbitals[ml + 3];
                scale = 2.0f * n-1;
            }

            GameObject obj = Instantiate(orbital, Vector3.zero, Quaternion.identity, root);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one * scale;

            //set the appropriate spdf color
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            Color color = BlockTypeUtil.ColorFromBlock((BlockType)l);
            foreach (Renderer r in renderers)
            {
                switch (Settings.MATERIAL)
                {
                    case SettingsMaterial.Solid: 
                        r.material = mat_solid; 
                        break;
                    case SettingsMaterial.Transparent: 
                        r.material = mat_transparent;
                        color /= 4;
                        break;
                }
                r.material.SetColor("_Color", color);
            }

            obj.SetActive(false);
            orbitalMap[electronCount] = new OrbitalData(obj, n, l, ml, ms); //assign to array
            return obj;
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

        private void AdjustAxis()
        {
            //update the axis
            if (Settings.AXIS && currentOrbital != null)
            {
                float axisLength = currentOrbital.transform.localScale.x * 1.15f;
                foreach (Transform a in axis) { a.localScale = AXIS_SCALE * axisLength; }
            }
        }

        /// <summary>
        /// Associates Quantum numbers with gameobject
        /// </summary>
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