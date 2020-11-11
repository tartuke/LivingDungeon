using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  The Enemy.cs script is responsible attached to any hostile mob.
 * 
 *  Purpose:
 *      -- Control Movement (AI) of hostile Mobs
 *      -- Attack/Def AI of mobs
 *      -- Render HP bar above mobs when attacked/in attack-mode
 * 
 */

public class Enemy_Boss : MonoBehaviour
    
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
    public float forgetRadius = 5f;
    public float despawnRadius = 10f;
    public float attackRange = .5f;

    public float speed = 0.5f;
    public int maxSteps = 10;
    
    SpriteRenderer sprite;
    Rigidbody2D rb;

    GameObject playerObj; // target

    HealthStat health;

    public Item heldItem;

    Vector2 target;
    Vector2 dir;

    [SerializeField]
    Color fullColor = Color.white;
    [SerializeField]
    Color emptyColor = Color.red;
    [SerializeField]
    StatType statType = StatType.HealthStat;

    GameManager GM;

    StatControler statControler;

    ConsumableStat stat;

    void Start()
    {
        // hack: works for now - ask isaac
        heldItem = Instantiate(heldItem);

        statControler = GetComponent<StatControler>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (health == null)
        {
            health = GetComponent<StatControler>().GetStatOfType(StatType.HealthStat) as HealthStat;
        }

        StartCoroutine(EnemyFSM());
    }

    private void Update()
    {
        //Debug.Log(target);
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

        if (health == null)
        {
            health = GetComponent<StatControler>().GetStatOfType(StatType.HealthStat) as HealthStat;
        }

        if (stat == null)
        {
            stat = statControler.GetStatOfType(statType) as ConsumableStat;

            if (stat != null)
            {
                stat.OnStatChangeCallBack += UpdateColor;
                stat.OnStatDepletedCallBack += Die;
            }
        }
    }

    private void UpdateColor()
    {
        float normalized = Mathf.InverseLerp(0, stat.Value, stat.CurrentValue);
        sprite.color = Color.Lerp(emptyColor, fullColor, normalized);
    }

    private void Die()
    {
        Destroy(gameObject);
        GM.Win();
    }

    void UseItem()
    {
        Vector2 direction = playerObj.transform.position - transform.position;

        heldItem.Use(transform.position, direction, gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        dir = (collision.GetContact(0).normal + dir).normalized;
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

    void ChangeDirection()
    {
        var x = Random.Range(-2.5f, 2.5f);
        var y = Random.Range(-2.5f, 2.5f);

        dir = new Vector3(x, y, 0).normalized;
        FlipSprite(dir);
    }
    
    IEnumerator EnemyFSM()
    {
        while(true)
        {
            yield return StartCoroutine(CurrentState.ToString());
        }
    }
    
    IEnumerator PATROL()
    {
        target = transform.position;
        dir = new Vector3(0, 0, 0);
        var steps = 0;
        int numSteps = Random.Range(maxSteps/2, maxSteps);
        
        while(CurrentState == EnemyState.PATROL)
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

            if (steps > numSteps)
            {
                ChangeDirection();
                steps=0;
            }
            steps++;

            yield return null;
        }
    }
    
    IEnumerator ATTACK()
    {
        while (CurrentState == EnemyState.ATTACK)
        {
            if (health.PrecentValue < .5f)
            {
                CurrentState = EnemyState.RETREAT;
            }
            if (Vector3.Distance(playerObj.transform.position, transform.position) > forgetRadius)
            {
                CurrentState = EnemyState.PATROL; // Player Has left :(
                ChangeDirection();
            }

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
            

            target = -1*(playerObj.transform.position - transform.position); // Get out of the alert zone
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
