using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sword", menuName = "Inventory/Sword")]
public class Sword : Item
{

    [SerializeField] private float baseAttackDelay = .5f;

    public Attack[] attacks;

    public int nextIndex;

    float timeWhenReadyToUse;
    float lastAtackTime;

    // what happens when you use a sword
    public override void Use(Vector2 pos, Vector2 direction, GameObject user)
    {
        if (attacks.Length == 0) return;

        // reset needed because scriptableObjects save data
        if (lastAtackTime > Time.time)
        {
            lastAtackTime = 0;
            timeWhenReadyToUse = 0;
        }

        // get next attack
        Attack Attack = attacks[nextIndex++];

        nextIndex %= attacks.Length;

        if (Attack == null) return;


        lastAtackTime = Time.time;

        // begin attack
        if (Time.time > timeWhenReadyToUse)
        {
            timeWhenReadyToUse = Time.time + baseAttackDelay + Attack.AttackDelay;

            Attack.Begin(this, pos, direction, user);
        }
    }
}
