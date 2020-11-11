using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Depth : MonoBehaviour
{
    [SerializeField]
    Text depthText;

    // Update is called once per frame
    void Update()
    {
        if (depthText != null) depthText.text = "Depth: " + Map.MapManager.instance.LevelData.levelNum;
    }
}
