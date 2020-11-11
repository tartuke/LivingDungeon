using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SlimeAttack", menuName = "Attack/SlimeAttack")]
public class SlimeAttack : Attack
{
    public int sweepAngle = 90;
    public int damage = 20;
    [SerializeField] private float castDelay = 0;
    [SerializeField] private float lifeTime = 2f;

    public override float AttackDelay { get { return castDelay; } }
    public override float LifeTime { get { return lifeTime; } }


    public GameObject prefab;

    GameObject User;

    public override void Begin(Sword wand, Vector2 pos, Vector2 direction, GameObject user)
    {
        User = user;

        // load attack prefab
        if (prefab == null)
        {
            prefab = Resources.Load("Prefabs/Attacks/SlimeAttack") as GameObject;
        }


        GameObject attack;

        // spawn attack
        attack = Instantiate(prefab, pos, new Quaternion());

        attack.transform.right = direction;
        attack.transform.Rotate(0,0,-sweepAngle/2);

        Rigidbody2D rb = attack.GetComponent<Rigidbody2D>();

        rb.centerOfMass = new Vector2();
        // add volocity to the attack
        rb.angularVelocity = sweepAngle/lifeTime;


        // give the attack a refrence to this object
        attack.GetComponent<AttackControler>().source = this;
    }

    // do something on collision
    public override void onCollision(GameObject attack, Collision2D collider)
    {
        //Debug.Log(User.name + " hit " + collider.gameObject.name);

        if (collider.gameObject == User) return;

        StatControler sC;
        StatControler sCUser = User.GetComponent<StatControler>();
        if ((sC = collider.gameObject.GetComponent<StatControler>()) != null)
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
                health.addValue(-finalDamage);
                //Debug.Log("Players health is = " + health.CurrentValue);
            }
        }
    }

    // do something on Expiration
    public override void onExpiration(GameObject attack, Vector2 pos)
    {
        //Debug.Log("Expired at " + pos);

        Destroy(attack);
    }
}