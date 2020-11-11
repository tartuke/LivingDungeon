using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Direct Rip of Enemy Class rewritten for boid implimentation
public class Enemy_Bug : MonoBehaviour
{
    public enum EnemyState
    {
        PATROL,
        ATTACK,
        RETREAT
    }

    public EnemyState CurrentState;

    public int viewDistance = 1; // How far an enemy can see; used for attack range calculations
    public float alertRadius = 2.5f; // What is the "cone" that the enemy can see
    public float forgetRadius = 5f; // What is the "cone" that the enemy can see
    public float despawnRadius = 10f;
    public float attackRange = .5f;

    public float speed = 0.5f;
    public int maxSteps = 10;
    public int damage = 10;

    SpriteRenderer sprite;

    GameObject playerObj; // target

    HealthStat health;

    public Item heldItem;

    Vector2 target;
    Vector2 dir;

    [SerializeField]
    StatType statType = StatType.HealthStat;

    StatControler statControler;

    ConsumableStat stat;

    void Start()
    {
        // hack: works for now - ask isaac
        heldItem = Instantiate(heldItem);

        sprite = GetComponent<SpriteRenderer>();
        statControler = GetComponent<StatControler>();
        //rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (health == null)
        {
            health = GetComponent<StatControler>().GetStatOfType(StatType.HealthStat) as HealthStat;
        }
        //StartCoroutine(EnemyFSM());
    }

    void UseItem()
    {
        Vector2 direction = playerObj.transform.position - transform.position;

        heldItem.Use(transform.position, direction, gameObject);
    }

    //DO damage on colision like magic bolt
    private void OnCollisionEnter2D(Collision2D col)
    {
        StatControler sC;
        if ((sC = col.GetContact(0).collider.gameObject.GetComponent<StatControler>()) != null && col.gameObject.tag == "Player")
        {
            int finalDamage = 0;
            finalDamage += damage;

            HealthStat health = sC.GetStatOfType(StatType.HealthStat) as HealthStat;
            ArmorStat armor = sC.GetStatOfType(StatType.ArmorStat) as ArmorStat;

            if (armor != null)
            {
                finalDamage -= (int)armor.CalculateFinalValue();
            }

            if (health != null)
            {
                health.addValue(-finalDamage);
                Die();
            }
        }
    }

    void FlipSprite(Vector3 normalized)
    {
        var angle = Mathf.Atan2(normalized.x, normalized.y);

        if (angle >= 0)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }
    }

    private void Update()
    {
        if (stat == null)
        {
            stat = statControler.GetStatOfType(statType) as ConsumableStat;

            if (stat != null)
            {
                stat.OnStatDepletedCallBack += Die;
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        FindObjectOfType<BOID_Manager>().DestroyBoid(GetComponent<BOID_Entity>().index);
    }


    IEnumerator EnemyFSM()
    {
        while (true)
        {
            yield return StartCoroutine(CurrentState.ToString());
        }
    }

    IEnumerator PATROL()
    {
        Debug.Log("Enemy enterd patrol state");
        target = transform.position;
        dir = new Vector3(0, 0, 0);
        var steps = 0;
        int numSteps = Random.Range(maxSteps / 2, maxSteps);

        while (CurrentState == EnemyState.PATROL)
        {
            var dist = Vector3.Distance(playerObj.transform.position, transform.position);
            if (dist < alertRadius)
            {
                CurrentState = EnemyState.ATTACK; // In Attack Range
            }
            else if (dist > despawnRadius)
            {
                Destroy(gameObject);
            }


            // Update new patrol area if we reached our target area.
            if (steps > numSteps)
            {
                //Debug.Log("Update Target Position");
                //ChangeDirection();
                steps = 0;

            }
            steps++;

            // Move toward the target
            /*
            transform.position = Vector3.MoveTowards(transform.position, 
                                                     target,
                                                     speed * Time.deltaTime);
            */
            //rb.MovePosition(target * Time.deltaTime);
            yield return null;

        }
        Debug.Log("Enemy exited patrol state");
    }

    IEnumerator ATTACK()
    {
        Debug.Log("Enemy enterd attack state");


        while (CurrentState == EnemyState.ATTACK)
        {
            if (health.PrecentValue < .5f)
            {
                CurrentState = EnemyState.RETREAT;
            }
            if (Vector3.Distance(playerObj.transform.position, transform.position) > forgetRadius)
            {
                CurrentState = EnemyState.PATROL; // Player Has left :(
                //ChangeDirection();
            }

            //Debug.Log(Vector3.Distance(playerObj.transform.position, transform.position));
            if ((Vector3.Distance(playerObj.transform.position, transform.position) <= attackRange))
            {
                UseItem();
                dir = Vector3.zero;
            }
            else
            {
                // Get the current position of the player...
                target = playerObj.transform.position - transform.position;
                var normalized = target;
                normalized.Normalize();
                FlipSprite(normalized);
                dir = normalized * 2 * speed;
            }
            yield return null;
        }
    }

    IEnumerator RETREAT()
    {
        while (CurrentState == EnemyState.RETREAT)
        {
            var away = alertRadius + 0.5f;
            var away_x = away;
            var away_y = away;
            var x = playerObj.transform.position.x;
            var y = playerObj.transform.position.y;


            target = -1 * (playerObj.transform.position - transform.position); // Get out of the alert zone
            var normalized = target;
            normalized.Normalize();
            FlipSprite(normalized);
            dir = normalized * speed;

            if (Vector3.Distance(playerObj.transform.position, transform.position) > forgetRadius)
            {
                CurrentState = EnemyState.PATROL; // We got away, now chill
            }

            if (health.PrecentValue > .7f)
            {
                CurrentState = EnemyState.ATTACK;
            }

            yield return null;
        }
    }
}
