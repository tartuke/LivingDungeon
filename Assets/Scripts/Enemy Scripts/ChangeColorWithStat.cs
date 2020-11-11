using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorWithStat : MonoBehaviour
{
    [SerializeField]
    Color fullColor = Color.white;
    [SerializeField]
    Color emptyColor = Color.red;
    [SerializeField]
    StatType statType = StatType.HealthStat;

    SpriteRenderer sprite;
    StatControler statControler;

    public static int slimeDeaths = 0;

    ConsumableStat stat;

    // Start is called before the first frame update
    void Start()
    {

        //Debug.Log("Starting");
        sprite = GetComponent<SpriteRenderer>();
        statControler = GetComponent<StatControler>();
    }

    private void Update()
    {
        if (stat == null)
        {
            stat = statControler.GetStatOfType(statType) as ConsumableStat;

            if (stat != null)
            {
                stat.OnStatChangeCallBack += UpdateColor;
                stat.OnStatDepletedCallBack += Die;
            }
        }
    }

    private void UpdateColor()
    {
        float normalized = Mathf.InverseLerp(0, stat.Value, stat.CurrentValue);
        sprite.color = Color.Lerp(emptyColor, fullColor, normalized);
    }

    // TODO: should be moved to a new script
    private void Die()
    {
        Destroy(gameObject);
        
        // instantiate loot
        //Debug.Log("Died?");
    }
}
