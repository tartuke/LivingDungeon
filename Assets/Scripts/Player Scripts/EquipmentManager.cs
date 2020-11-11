using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    public GameObject equipmentUI;

    EquipmentUISlots [] equipmentUISlots;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EquipmentManager found");
        }
        instance = this;
    }
    void Start()
    {
        equipmentUISlots = equipmentUI.GetComponentsInChildren<EquipmentUISlots>();
    }

    public GameObject player;
    StatControler statController;

    public Item[] items = new Item[Enum.GetNames(typeof(EquipmentSlot)).Length];

    private void Update()
    {
        if (Input.GetButtonDown("Equipment"))
        {
            equipmentUI.SetActive(!equipmentUI.activeSelf);
        }
        statController = player.GetComponent<StatControler>();
        
    }


    public void addItem(Item item)
    {
        Debug.Log("This got called");

        StatModifier armorMod = new StatModifier(item.armor, item.statModType, item.armorType); ;

        StatModifier healthMod = new StatModifier(item.health, item.statModType, item.healthType);

        StatModifier damageMod = new StatModifier(item.damage, item.statModType, item.damageType);

        HealthStat health = statController.GetStatOfType(StatType.HealthStat) as HealthStat;
        ArmorStat armor = statController.GetStatOfType(StatType.ArmorStat) as ArmorStat;
        DamageStat damage = statController.GetStatOfType(StatType.DamageStat) as DamageStat;

        statController.AddModifier(healthMod);
        statController.AddModifier(armorMod);
        statController.AddModifier(damageMod);

        Debug.Log("Health total is " + health.CalculateFinalValue());
        Debug.Log("Damage total is " + damage.CalculateFinalValue());
        Debug.Log("Armor total is " + armor.CalculateFinalValue());
        

        items[(int)item.equipmentSlot] = item;

        for (int i = 0; i < equipmentUISlots.Length; i++)
        {
            if (item.equipmentSlot == equipmentUISlots[i].equipmentSlot)
            {
                equipmentUISlots[i].addItem(item);
            }
        }
    }

    public void removeItem(Item item)
    {
        StatModifier armorMod = new StatModifier(item.armor, item.statModType, item.armorType); ;

        StatModifier healthMod = new StatModifier(item.health, item.statModType, item.healthType);

        StatModifier damageMod = new StatModifier(item.damage, item.statModType, item.damageType);

        HealthStat health = statController.GetStatOfType(StatType.HealthStat) as HealthStat;
        ArmorStat armor = statController.GetStatOfType(StatType.ArmorStat) as ArmorStat;
        DamageStat damage = statController.GetStatOfType(StatType.DamageStat) as DamageStat;

        statController.RemoveModifier(healthMod);
        statController.RemoveModifier(armorMod);
        statController.RemoveModifier(damageMod);

        Debug.Log("Health total is " + health.CalculateFinalValue());
        Debug.Log("Damage total is " + damage.CalculateFinalValue());
        Debug.Log("Armor total is " + armor.CalculateFinalValue());

        items[(int)item.equipmentSlot] = null;
    }

    public Item getItem(EquipmentSlot index)
    {
        return items[(int)index];
    }


}
