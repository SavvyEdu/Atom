using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private ConfirmBox confirm;

    public void ChangeScene()
    {
        ChangeScene(sceneName); //use editor value
    }

    public void ChangeScene(string sceneName)
    {
        if (confirm != null){
            confirm.ShowConfirm(() => SceneManager.LoadScene(sceneName, LoadSceneMode.Single));
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        
    }        
}
