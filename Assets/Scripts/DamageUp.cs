using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUp : MonoBehaviour

{

    public CapsuleCollider2D playerCollider;
    public GameObject player;
    public int damageUp;
    StatControler playerSC;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player_v2");
        playerCollider = player.GetComponent<CapsuleCollider2D>();
        playerSC = player.GetComponent<StatControler>();
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision == playerCollider)
        {
            //PlayerAction.instance.heldItem.damage += damageUp;

            StatModifier damageMod = new StatModifier(damageUp, StatModType.Flat, StatType.DamageStat);

            playerSC.AddModifier(damageMod);

            Destroy(gameObject);
        }
    }
}
