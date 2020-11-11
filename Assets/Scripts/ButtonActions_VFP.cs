using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions_VFP : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1;

    }

    public void Quit()
    {
        Application.Quit();
    }
}
