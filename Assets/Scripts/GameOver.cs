using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameOver : MonoBehaviour
{

    public GameObject gameOver;

    public void onPlayAgainButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1;

    }

}
