using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour
{
    public int gold;
    public Text goldUI;

    // Start is called before the first frame update
    void Start()
    {
        goldUI.text = "GP: " + gold;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // return -1 failed
    // return current gold value if valid
    public int ChangeGold(int value) 
    {
        if (value < 0 && (gold + value < 0))
        {
            return -1;
        }
        else 
        {
            gold += value;
            goldUI.text = "GP: " + gold;
            return gold;
        }

    }
}
