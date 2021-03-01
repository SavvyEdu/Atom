using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    public class LocalizedText : MonoBehaviour
    {
        public string key;

        // Use this for initialization
        void Start()
        {
            Text text = GetComponent<Text>();
            text.text = LocalizationManager.GetLocalizedValue(key);
        }
    }
}