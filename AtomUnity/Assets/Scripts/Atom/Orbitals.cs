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

        public static Action<int, int, int, int> OnUpdate;

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
                        //active andOr recolor
                        if(data.ms == -1)
                            data.obj.SetActive(true);
                        AdjustColor(data.obj, data.l, data.ms);
                    }
                    //Remove Orbitals
                    while (electronCount < currentElectronCount)
                    {
                        //deactive or recolor
                        if (data.ms == -1)
                            data.obj.SetActive(false);
                        else if(electronCount > 0)
                            AdjustColor(data.obj, data.l, -1); //revert color
                        currentElectronCount--;
                        data = orbitalMap[currentElectronCount];
                    }
                }
                else // (Only show current orbital)
                {
                    //deactivate the current
                    if (currentElectronCount != 0)
                        data.obj.SetActive(false);

                    //activate the new and recolor
                    data = orbitalMap[electronCount];
                    if (electronCount > 0)
                        data.obj.SetActive(true);
                    AdjustColor(data.obj, data.l, data.ms);
                }

                //unpdate the orbital data
                currentOrbital = data.obj;
                currentElectronCount = electronCount;
                OnUpdate?.Invoke(data.n, data.l, data.ml, data.ms);

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
        /// <param name="ms">spin [-1 1]</param>
        public GameObject CreateOrbital(int electronCount, int n, int l, int ml, int ms)
        {
            GameObject obj;

            //orbital has been added, assign reference
            if (ms == 1)
            {
                obj = orbitalMap[electronCount - (2 * l + 1)].obj; // 2L + 1 => 1,3,5,7
                orbitalMap[electronCount] = new OrbitalData(obj, n, l, ml, ms);
                return obj;
            }
            
            GameObject orbitalPrefab = null;
            float scale = n;
            switch (l)
            {
                case 0:
                    orbitalPrefab = sOrbitals[ml];
                    scale = 1f * n;
                    break;
                case 1:
                    orbitalPrefab = pOrbitals[ml + 1];
                    scale = 1.33f * n;
                    break;
                case 2:
                    orbitalPrefab = dOrbitals[ml + 2];
                    scale = 1.5f * n;
                    break;
                case 3:
                    orbitalPrefab = fOrbitals[ml + 3];
                    scale = 2.0f * n - 1;
                    break;
            }

            obj = Instantiate(orbitalPrefab, Vector3.zero, Quaternion.identity, root);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one * scale;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(); //get all the renderers
            foreach (Renderer r in renderers)
            {
                switch (Settings.MATERIAL)
                {
                    case SettingsMaterial.Solid: r.material = mat_solid; break;
                    case SettingsMaterial.Transparent: r.material = mat_transparent; break;
                }
            }

            obj.SetActive(false);
            orbitalMap[electronCount] = new OrbitalData(obj, n, l, ml, ms); //assign to array
            return obj;
        }

        /// <summary>
        /// Color the object based on l and ms
        /// </summary>
        private void AdjustColor(GameObject obj, int l, int ms)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(); //get all the renderers
            Color color = BlockTypeUtil.ColorFromBlock((BlockType)l); //get color from (l)
            if (Settings.MATERIAL == SettingsMaterial.Transparent) { color = Color.Lerp(color, new Color(1, 1, 1), 0.8f); color.a = 0.3f; } //tone down transparency
            //if (ms == -1) { color.r /= 2; color.g /= 2; color.b /= 2; } //tone down for one electron in shell
            if (ms == -1) { color -= new Color(0.3f, 0.3f, 0.3f, 0f); } //tone down for one electron in shell
            foreach (Renderer r in renderers)
            {
                r.material.SetColor("_Color", color);
            }
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