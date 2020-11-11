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

public class Enemy_VFP : MonoBehaviour
    
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
    
    SpriteRenderer sprite;
    Rigidbody2D rb;

    GameObject playerObj; // target

    HealthStat health;

    public Item heldItem;

    Vector2 target;
    Vector2 dir;
    
    void Start()
    {
        // hack: works for now - ask isaac
        heldItem = Instantiate(heldItem);

        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.Find("Player");
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
    }

    void UseItem()
    {
        Vector2 direction = playerObj.transform.position - transform.position;

        heldItem.Use(transform.position, direction, gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("collision");
        dir = (collision.GetContact(0).normal + dir).normalized;
        //ChangeDirection();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Player in the area of the enemy ==> Go attack it
        //CurrentState = EnemyState.ATTACK;
        
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Player leaves the area of the enemy ==> Return to patrolling
        //CurrentState = EnemyState.PATROL;
    }

    void FlipSprite(Vector3 normalized)
    {
        var angle = Mathf.Atan2(normalized.x, normalized.y);
        //Debug.Log(angle);
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
        //target = transform.position + new Vector3(x, y, 0);
        //var normalized = target;
        //normalized.Normalize();
        FlipSprite(dir);
        //dir = normalized * speed;
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
        //Debug.Log("Enemy enterd patrol state");
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
            //else if (dist > despawnRadius)
            //{
            //    Destroy(gameObject);
            //}

            
            // Update new patrol area if we reached our target area.
            if (steps > numSteps)
            {
                //Debug.Log("Update Target Position");
                ChangeDirection();
                steps=0;
                
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
        //Debug.Log("Enemy exited patrol state");
    }
    
    IEnumerator ATTACK()
    {
        //Debug.Log("Enemy enterd attack state");


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

                

            // Move to player Location
            /*
            transform.position = Vector3.MoveTowards(transform.position, 
                                                     target,
                                                     2 * speed * Time.deltaTime);
            */
            yield return null;
        }
        
        //Debug.Log("Enemy exited attack state");
    }
    
    IEnumerator RETREAT()
    {
        //Debug.Log("Enemy enterd retreat state");

        
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
        
        //Debug.Log("Enemy exited retreat state");
    }
    
}
