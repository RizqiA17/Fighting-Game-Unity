using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void Scene(int level)
    {
        SceneManager.LoadScene(level);
        //print("Load" + level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
