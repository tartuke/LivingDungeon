using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    Color healthyColor = Color.white;
    [SerializeField]
    Color damagedColor = Color.red;
    bool isAlive;

    // Added a quick little health stat so the slime is killable
    [SerializeField]
    float health;
    public float maxHealth = 10;

    [HideInInspector]
    public float healthPercent;

    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        TakeDamage(0); //Just for testing update not needed curently
    }

    // health setter
    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health > maxHealth) health = maxHealth;
        else if (health <= 0) Die();
        else
        {
            float normalized = Mathf.InverseLerp(0, maxHealth, health);
            sprite.color = Color.Lerp(damagedColor, healthyColor, normalized);
        }

        healthPercent = health / maxHealth;
    }

    public void Heal()
    {
        if (health < maxHealth) health += (health / maxHealth * Time.deltaTime); // Heal to full health gradually
    }

    private void Die()
    {
        isAlive = false;
        Destroy(gameObject);
        // instantiate loot
    }
}
