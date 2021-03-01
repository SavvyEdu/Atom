using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Localization
{
    public class StartupManager : MonoBehaviour
    {
        // Use this for initialization
        private IEnumerator Start()
        {
            while (!LocalizationManager.IsLoaded)
            {
                yield return null;
            }

            SceneManager.LoadScene("Main");
        }
    }
}