using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    public void Scene(int level)
    {
        SceneManager.LoadScene(level);
        loadingPanel.SetActive(true);
        //print("Load" + level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
