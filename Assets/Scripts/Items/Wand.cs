using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wand", menuName = "Inventory/Wand")]
public class Wand : Item
{
    [SerializeField] private float maxMana = 100;
    [SerializeField] private float mana = 100;
    [SerializeField] private float manaRegen = 10;
    [SerializeField] private float baseCastDelay = .5f;

    public float MaxMana { get { return maxMana; } }
    public float Mana { get { return mana; } }
    public float ManaRegen { get { return manaRegen; } }

    public Spell[] spells;


    public int nextSpellIndex;

    float timeWhenReadyToUse;
    float lastCastTime;

    // what happens when you use a wand
    public override void Use(Vector2 pos, Vector2 direction, GameObject user)
    {
        if (spells.Length == 0) return;

        // reset needed because scriptableObjects save data
        if (lastCastTime > Time.time)
        {
            lastCastTime = 0;
            timeWhenReadyToUse = 0;
            mana = MaxMana;
        }

        // get next spell
        Spell spell = spells[nextSpellIndex++];

        nextSpellIndex %= spells.Length;

        if (spell == null) return;

        // regen mana
        mana = Mathf.Min((mana + (Time.time - lastCastTime) * ManaRegen), MaxMana);

        lastCastTime = Time.time;

        // cast spell
        if (Time.time > timeWhenReadyToUse && spell.ManaCost < mana)
        {
            mana -= spell.ManaCost;

            timeWhenReadyToUse = Time.time + baseCastDelay + spell.CastDelay;

            spell.Cast(this, pos, direction, user);
        }       
    }
}
