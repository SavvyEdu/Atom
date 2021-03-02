using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        public string key;

        // Use this for initialization
        void Start() => UpdateText();
        public void UpdateText()
        {
            Text text = GetComponent<Text>();
            text.text = LocalizationManager.GetLocalizedValue(key);
        }
    }
}