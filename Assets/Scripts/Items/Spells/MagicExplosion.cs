using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MagicExplosion", menuName = "Spell/MagicExplosion")]
public class MagicExplosion : Spell
{
    public int speed = 10;
    public int damage = 400;
    public float explosionRaidius = 4;
    private float sqrR;
    public bool breakWalls = true;
    [SerializeField] private int manaCost = 50;
    [SerializeField] private float castDelay = 5;
    [SerializeField] private float lifeTime = 2f;

    public override int ManaCost { get { return manaCost; } }
    public override float CastDelay { get { return castDelay; } }
    public override float LifeTime { get { return lifeTime; } }


    GameObject prefab;
    GameObject explosion;
    GameObject User;

    public override void Cast(Wand wand, Vector2 pos, Vector2 direction, GameObject user)
    {
        User = user;

        sqrR = explosionRaidius * explosionRaidius;
        // load projectile prefab
        if (prefab == null)
        {
            prefab = Resources.Load("Prefabs/Spells/MagicExplosion") as GameObject;
        }
        if (explosion == null)
        {
            explosion = Resources.Load("Prefabs/Spells/Particles/Explosion") as GameObject;
        }

        direction = direction.normalized;

        GameObject projectile;

        // spawn projectile with an offset
        projectile = Instantiate(prefab, pos + direction*.1f, new Quaternion());

        // add volocity to the projectile
        projectile.GetComponent<Rigidbody2D>().velocity = direction * speed;

        // turn on trail
        //projectile.GetComponent<ParticleControler>().ParticleSystems[0].Play();

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

                Instantiate(explosion, hitPos, new Quaternion());

                for (float i = -explosionRaidius; i <= explosionRaidius; ++i)
                    for (float j = -explosionRaidius; j <= explosionRaidius; ++j)
                    {
                        var d = i * i + j * j;
                        if (d <= sqrR)
                            Map.MapManager.instance.DamageTile(hitPos.x+i, hitPos.y+j, damage/d);
                    }

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