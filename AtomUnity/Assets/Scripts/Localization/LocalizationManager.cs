using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        private static Dictionary<string, string> localizedText;
        public static bool IsLoaded { get; private set; } = false;
        private const string missingTextString = "[text not found]";

        public void Awake() {
            string savedLocalizationFileName = PlayerPrefs.GetString("localization", "localization_en.json");
            LoadLocalizedText(savedLocalizationFileName);
        }

        public void LoadLocalizedText(string fileName)
        {
            localizedText = new Dictionary<string, string>();
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

            //make sure data actually exists
            if (File.Exists(filePath))
            {
                //Load in JSON
                string dataAsJson = File.ReadAllText(filePath);
                LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

                //Convert to LoacalizedText Dictionary
                foreach(var item in loadedData.items)
                {
                    localizedText.Add(item.key, item.value);
                }

                Debug.Log($"Data loaded from {fileName}. Dictionary contains: {localizedText.Count} entries");
                PlayerPrefs.SetString("localization", fileName); //save the filename

                IsLoaded = true;
                UpdateLocalizationTextsInScene();
            }
            else
            {
                Debug.LogError("Cannot find file!");
            }
        }

        private void UpdateLocalizationTextsInScene()
        {
            var localizedTexts = FindObjectsOfType<LocalizedText>();
            foreach(var localizedText in localizedTexts)
            {
                localizedText.UpdateText();
            }
        }

        public static string GetLocalizedValue(string key)
        {
            if (!(IsLoaded && localizedText.ContainsKey(key))) //check for Loaded and known key
            {
                return missingTextString; //default to missing text
            }
            return localizedText[key];
        }

    }
}