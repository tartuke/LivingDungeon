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

public class Enemy : MonoBehaviour
    
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
    
    public float speed = 0.5f;
    public int maxSteps = 25;

    Health health;
    
    SpriteRenderer sprite;
    Rigidbody2D rb;

    GameObject playerObj; // target

    Vector3 target;
    Vector3 dir;
    
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.Find("Player");
        health = GetComponent<Health>();
        StartCoroutine(EnemyFSM());
    }

    private void Update()
    {
        //Debug.Log(target);
        rb.velocity = dir;
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
    
    
    IEnumerator EnemyFSM()
    {
        while(true)
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
        
        while(CurrentState == EnemyState.PATROL)
        {

            health.Heal();
            
            if (Vector3.Distance(playerObj.transform.position, transform.position) < alertRadius)
            {
                CurrentState = EnemyState.ATTACK; // In Attack Range
            }
            
            // Update new patrol area if we reached our target area.
            if (steps > maxSteps)
            {
                //Debug.Log("Update Target Position");
                var x = Random.Range(-2.5f, 2.5f);
                var y = Random.Range(-2.5f, 2.5f);
            
                target = transform.position + new Vector3(x, y, 0);
                var normalized = target;
                normalized.Normalize();
                FlipSprite(normalized);
                dir = normalized * speed;
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
        Debug.Log("Enemy exited patrol state");
    }
    
    IEnumerator ATTACK()
    {
        Debug.Log("Enemy enterd attack state");
        var playerObj = GameObject.Find("Player");
        
        while (CurrentState == EnemyState.ATTACK)
        {
            if (health.healthPercent < .5f)
            {
                CurrentState = EnemyState.RETREAT;
            }
            if (Vector3.Distance(playerObj.transform.position, transform.position) > alertRadius)
            {
                CurrentState = EnemyState.PATROL; // Player Has left :(
            }

            Debug.Log(Vector3.Distance(playerObj.transform.position, transform.position));
            if ((Vector3.Distance(playerObj.transform.position, transform.position) <= 0.75))
            {
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
        
        Debug.Log("Enemy exited attack state");
    }
    
    IEnumerator RETREAT()
    {
        var playerObj = GameObject.Find("Player");
        Debug.Log("Enemy enterd retreat state");
        
        while (CurrentState == EnemyState.RETREAT)
        {
            var away = alertRadius + 0.5f;
            var away_x = away;
            var away_y = away;
            var x = playerObj.transform.position.x;
            var y = playerObj.transform.position.y;
            
            if (x < transform.position.x)
            {
                away_x = -1 * away;
            }
            if (y < transform.position.y)
            {
                away_y = -1 * away;
            }
            
            target = playerObj.transform.position - new Vector3(away_x, away_y, 0); // Get out of the alert zone
            var normalized = target;
            normalized.Normalize();
            FlipSprite(normalized);
            dir = normalized * speed;

            /*
            transform.position = Vector3.MoveTowards(transform.position,
                                                     target,
                                                     speed * Time.deltaTime);
            */
            if (Vector3.Distance(playerObj.transform.position, transform.position) > alertRadius)
            {
                CurrentState = EnemyState.PATROL; // We got away, now chill
            }
            
            yield return null;
        }
        
        Debug.Log("Enemy exited retreat state");
    }
    
}
