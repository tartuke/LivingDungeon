using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    // can add tags to mark helmets, weapons, etc

    public modInfo[] modifiers;

    [Serializable]
    public class modInfo
    {
        public StatType statType;
        public StatModType type;
        public int value;
    }

    public void Equip(StatControler statControler)
    {
        foreach (modInfo mod in modifiers)
            statControler.AddModifier(new StatModifier(mod.value, mod.type, mod.statType, this));
    }

    public void Unequip(StatControler statControler)
    {
        statControler.RemoveAllModifiersFromSource(this);
    }
}

