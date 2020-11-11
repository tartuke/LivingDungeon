using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHeathBar : MonoBehaviour
{

    private Transform bar;
    private SpriteRenderer barSprite;

    private StatControler sC;
    HealthStat health;

    public GameObject gameOver;


    private Color lowHealth = Color.red;
    private Color maxHealth = Color.green;

    // Start is called before the first frame update
    void Start()
    {
        sC = transform.parent.GetComponent<StatControler>();
        bar = transform.Find("Bar");
        barSprite = bar.Find("BarSprite").gameObject.GetComponent<SpriteRenderer>();
        barSprite.color = maxHealth;
    }

    private void Update()
    {
        if (health == null)
        {
            health = sC.GetStatOfType(StatType.HealthStat) as HealthStat;
            health.OnStatDepletedCallBack += Die;
        }
        

        this.updateBar(health.PrecentValue);

    }

    private void updateBar(float normalizedHealth)
    {
        barSprite.color = Color.Lerp(lowHealth, maxHealth, normalizedHealth);
        bar.localScale = new Vector3(normalizedHealth, 1f);
    }

    public void Die()
    {
        Debug.Log("Player has died");
        Time.timeScale = 0;
        gameOver.SetActive(true);

    }

}
