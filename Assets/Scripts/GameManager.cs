using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum DepthLevel
    {
        ONE, //slimes
        TWO, //bugs
        THREE, //both
        FOUR //boss
    }

    public int[] depthBounds = { 0, 10, 20, 30, 40 };

    public DepthLevel CurrentLevel = DepthLevel.ONE;

    [SerializeField]
    Text depthText;

    [SerializeField]
    Spawner_VFP SlimeSpawner;
    [SerializeField]
    Spawner_Boids BugSpawner;
    [SerializeField]
    Spawner_VFP BossSlime; //actually a spawner JK
    [SerializeField]
    GameObject WinScreen;

    int depth;
    int minBound;
    int maxBound;

    Vector2 initialPos;
    GameObject player;

    //Singlton

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        initialPos = player.transform.position;

        StartCoroutine(CurrentLevel.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 tempPos = player.transform.position;
        tempPos -= initialPos;
        depth = (int)Mathf.Sqrt(Mathf.Pow(tempPos.x, 2) + Mathf.Pow(tempPos.y, 2));

        if (depthText != null) depthText.text = "Depth: " + depth.ToString();

        //if (ChangeColorWithStat.slimeDeaths == 20)
        //{
        //    
        //    ChangeColorWithStat.slimeDeaths = 0;
        //}
    }

    IEnumerator ONE()
    {
        //setup
        minBound = depthBounds[0];
        maxBound = depthBounds[1];
        SlimeSpawner.StartSpawn();
        BugSpawner.isPaused = true;

        //wait
        yield return new WaitForSeconds(1);

        //exit
        if (depth > maxBound) StartCoroutine(TWO());
        else StartCoroutine(ONE());
    }

    IEnumerator TWO()
    {
        //setup
        minBound = depthBounds[1];
        maxBound = depthBounds[2];
        SlimeSpawner.isPaused = true;
        BugSpawner.StartSpawn();

        //wait
        yield return new WaitForSeconds(1);

        //exit
        if (depth > maxBound) StartCoroutine(THREE());
        else if (depth < minBound) StartCoroutine(ONE());
        else StartCoroutine(TWO());
    }

    IEnumerator THREE()
    {
        //setup
        minBound = depthBounds[2];
        maxBound = depthBounds[3];
        SlimeSpawner.StartSpawn();
        BugSpawner.StartSpawn();

        //wait
        yield return new WaitForSeconds(1);

        //exit
        if (depth > maxBound) StartCoroutine(FOUR());
        else if (depth < minBound) StartCoroutine(TWO());
        else StartCoroutine(THREE());
    }

    // Boss Wave
    IEnumerator FOUR()
    {
        //setup
        //minBound = depthBounds[3];
        //maxBound = depthBounds[4];
        SlimeSpawner.isPaused = true;
        BugSpawner.isPaused = true;
        BossSlime.StartSpawn();

        depthText.text = "Defeat The Boss";
        depthText = null;

        //wait
        yield return new WaitForSeconds(1);

        //exit
    }

    public void Win()
    {
        Time.timeScale = 0;
        WinScreen.SetActive(true);
    }
}
