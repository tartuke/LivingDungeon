using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject

{
    new public string name = "New Item";
    public Sprite icon = null;

    public EquipmentSlot equipmentSlot;

    public bool isDefault;

    public StatModType statModType;

    public StatType damageType = StatType.DamageStat;

    public StatType healthType = StatType.HealthStat;

    public StatType armorType = StatType.ArmorStat;

    public float damage;

    public float health;

    public float armor;

    public string description;

    public int goldCost;


    //public void Pickup(Inventory inventory)
    //{
    //    inventory.AddItem(this);
    //}

    public void Drop(Vector3 pos)
    {
        GameObject droppedItem = (GameObject)Instantiate(Resources.Load("Prefabs/Items/DroppedItem"),pos, new Quaternion());
        Debug.Log(droppedItem);
        droppedItem.GetComponent<DroppedItem>().item = this;
    }

    public virtual void Use(Vector2 pos, Vector2 direction, GameObject user) { }

}

public enum EquipmentSlot { WEAPON, CHEST, GLOVES, BOOTS, RELIC}