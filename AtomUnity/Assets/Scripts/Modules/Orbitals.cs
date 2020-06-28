using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atom.Util;
using System;

namespace Atom {
    public class Orbitals : MonoBehaviour
    {
        [SerializeField] private Atom atom;

        [Header("Orbitals")]
        [SerializeField] private GameObject[] sOrbitals;
        [SerializeField] private GameObject[] pOrbitals;
        [SerializeField] private GameObject[] dOrbitals;
        [SerializeField] private GameObject[] fOrbitals;


        private void Start()
        {
            CreateAllOrbitals();
            transform.localScale = Vector3.one * 0.1f;
        }

        private void Update()
        {
            int n = 0, l = 0, m = 0;

            if (atom.Element != null)
            {
                Element e = Elements.GetElement(atom.ElectronCount);
                Shell shell;

                if(atom.OuterShell.ElectronCount > 0)
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
                        m = 0;
                        break;
                    case BlockType.pBlock:
                        m = ((shell.ElectronCount - 2) % 3) - 1 ; //subtract s block, set range to [0,2], move range to [-1, 1]
                        break;
                    case BlockType.dBlock:
                        n -= 1;
                        m = ((shell.NextShell.ElectronCount - 8) % 5) - 2; //subtract sp block, set range to [0,4], move range to [-2, 2]
                        break;
                    case BlockType.fBlock:
                        n -= 2;
                        m = ((shell.NextShell.NextShell.ElectronCount - 18) % 7) - 3; //subtract spd block, set range to [0,6], move range to [-3, 3]
                        break;
                }
            }
        }


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

        /// <param name="n">Shell [1, 7] </param>
        /// <param name="l">SPDF [0, 3] </param>
        /// <param name="m">obital [-3, 3] </param>
        public void CreateOrbital(int n, int l, int m)
        {
            //if (!ValidConfig(n, l, m)) return;

            GameObject orbital = null;
            float scale = n;

            if (l == 0)
            {
                orbital = sOrbitals[m];
                scale = 1f * n;
            }
            else if (l == 1)
            {
                orbital = pOrbitals[m + 1];
                scale = 1.25f * n;
            }
            else if (l == 2)
            {
                orbital = dOrbitals[m + 2];
                scale = 4f / 3 * n;
            }
            else if (l == 3)
            {
                orbital = fOrbitals[m + 3];
                scale = 1.5f * n - 2;
            }

            GameObject obj = Instantiate(orbital, Vector3.zero, Quaternion.identity, transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one * scale;

            //set the appropriate spdf color
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            Color color = BlockTypeUtil.ColorFromBlock((BlockType)l);
            foreach (Renderer r in renderers)
            {
                r.material.SetColor("_Color", color);
            }

            obj.SetActive(true);
        }

        private bool ValidConfig(int n, int l, int m)
        {
            if (n < 1 || n > 7) return false; //validate n between 1 and 7
            if (l < 0 || l > -Mathf.Abs(n - 4.5f) + 3.5f) return false; //valudate l based on n
            if (Mathf.Abs(m) > l) return false; //validate |m| is less than l 
            return true;
        }
    }
}