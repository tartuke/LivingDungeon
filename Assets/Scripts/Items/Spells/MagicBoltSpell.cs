using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MagicBolt", menuName = "Spell/MagicBolt")]
public class MagicBoltSpell : Spell
{
    public int speed = 10;
    public int damage = 20;
    public bool breakWalls = true;
    [SerializeField] private int manaCost = 10;
    [SerializeField] private float castDelay = 0;
    [SerializeField] private float lifeTime = 2f;

    public override int ManaCost { get { return manaCost; } }
    public override float CastDelay { get { return castDelay; } }
    public override float LifeTime { get { return lifeTime; } }


    GameObject prefab;
    GameObject User;

    public override void Cast(Wand wand, Vector2 pos, Vector2 direction, GameObject user)
    {
        User = user;

        // load projectile prefab
        if (prefab == null)
        {
            prefab = Resources.Load("Prefabs/Spells/MagicBolt") as GameObject;
        }

        direction = direction.normalized;

        GameObject projectile;

        // spawn projectile with an offset of .5
        projectile = Instantiate(prefab, pos + direction*.1f, new Quaternion());

        // add volocity to the projectile
        projectile.GetComponent<Rigidbody2D>().velocity = direction * speed;

        // give the projectile a refrence to this object
        projectile.GetComponent<ProjectileControler>().source = this;
    }

    // do something on collision
    public override void onCollision(GameObject projectile, Collision2D collision)
    {
        Collider2D collider = collision.GetContact(0).collider;
        //Debug.Log(User.name + " hit " + collider.name);
        

        if (collider.gameObject == User) return;


        if (breakWalls)
        {
            Vector2 hitPos = collision.GetContact(0).point - collision.relativeVelocity.normalized * .1f;

            if (collision.GetContact(0).collider.name == "Walls")
            {
                FindObjectsOfType<Map.MapManager>()[0].DamageTile(hitPos.x, hitPos.y,damage);
                Destroy(projectile);
            }
        }

        StatControler sC;
        StatControler sCUser = User.GetComponent<StatControler>();
        if ((sC = collision.GetContact(0).collider.gameObject.GetComponent<StatControler>()) != null)
        {
            int finalDamage = 0;
            finalDamage += damage;

            HealthStat health = sC.GetStatOfType(StatType.HealthStat) as HealthStat;
            ArmorStat armor = sC.GetStatOfType(StatType.ArmorStat) as ArmorStat;
            DamageStat damageBonus = sCUser.GetStatOfType(StatType.DamageStat) as DamageStat;

            if (armor != null)
            {
                finalDamage -= (int)armor.CalculateFinalValue();
            }
            if (damageBonus != null)
            {
                finalDamage += (int)damageBonus.CalculateFinalValue();
            }

            //Debug.Log(health);

            if (health != null)
            {
                //Debug.Log(finalDamage);
                health.addValue(-finalDamage);
                Destroy(projectile);
            }
        }
    }

    // do something on Expiration
    public override void onExpiration(GameObject projectile, Vector2 pos)
    {
        //Debug.Log("Expired at " + pos);

        Destroy(projectile);
    }
}