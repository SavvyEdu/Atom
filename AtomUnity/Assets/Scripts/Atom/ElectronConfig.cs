using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atom
{
    public class ElectronConfig : MonoBehaviour
    {
        private Text[] texts;
        private Atom atom;

        private bool showConfig = true;

        private void Awake()
        {
            atom = FindObjectOfType<Atom>();
            texts = GetComponentsInChildren<Text>();
        }

        public void ToggleConfig()
        {
            showConfig = !showConfig;
            transform.localPosition = Vector2.left * (showConfig ? 30 : GetComponent<RectTransform>().rect.width - 30);
        }

        private void Update()
        {
            Shell s = atom.OuterShell;
            for (int i = 0; i < texts.Length; i++)
            {
                if (s != null)
                {
                    texts[atom.ShellCount-1 - i].text = s.ElectronCount.ToString();
                    s = s.NextShell;
                }
                else
                {
                    texts[i].text = "";
                }
            }
        }
    }
}
