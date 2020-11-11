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

public class Enemy_Stealth : MonoBehaviour
    
{
    public enum EnemyState
    {
        HIDE,
        ATTACK,
        RETREAT
    }

    public EnemyState CurrentState;
    
    public int viewDistance = 1; // How far an enemy can see; used for attack range calculations
    public float alertRadius = 2.5f; // What is the "cone" that the enemy can see
    public float excapeRadius = 5f; // What is the "cone" that the enemy can see
    public float attackRange = .5f;

    public float speed = 0.5f;
    
    SpriteRenderer sprite;
    Rigidbody2D rb;

    GameObject playerObj; // target

    public Item heldItem;

    Vector2 target;
    Vector2 dir;

    // color 
    [SerializeField]
    Color a = Color.white;
    [SerializeField]
    Color b = Color.red;
    [SerializeField]
    StatType statType = StatType.HealthStat;

    StatControler statControler;

    public static int slimeDeaths = 0;

    ConsumableStat health;

    [SerializeField]
    GameObject LootPrefab;

    void Start()
    {
        heldItem = Instantiate(heldItem);

        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        statControler = GetComponent<StatControler>();
        playerObj = GameObject.Find("Player_v2");

        sprite.color = Color.Lerp(a, b, 0);

        StartCoroutine(SetHealth());
    }

    IEnumerator SetHealth()
    {
        yield return new WaitForEndOfFrame();

        if (health == null)
        {
            health = statControler.GetStatOfType(statType) as ConsumableStat;

            if (health != null)
            {
                health.OnStatDepletedCallBack += Die;
            }

            StartCoroutine(SetHealth());
        }
        else
        {
            StartCoroutine(EnemyFSM());
        }
    }

    private void Update()
    {
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    void UseItem()
    {
        Vector2 direction = playerObj.transform.position - transform.position;
        heldItem.Use(transform.position, direction, gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //dir = (collision.GetContact(0).normal + dir).normalized;
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
        while(true) yield return StartCoroutine(CurrentState.ToString());
    }
    
    IEnumerator HIDE()
    {
        Debug.Log("Hide");
        dir = new Vector3(0, 0, 0);
        
        while(CurrentState == EnemyState.HIDE)
        {
            var dist = Vector2.Distance(playerObj.transform.position, transform.position);

            // sits still until player is close
            if (dist < alertRadius)
            {
                CurrentState = EnemyState.ATTACK; // In Attack Range
            }

            // if damaged attack
            if (health.PrecentValue < .7f)
            {
                CurrentState = EnemyState.ATTACK;
            }

            yield return null;
        }
    }
    
    IEnumerator ATTACK()
    {
        //float n = Mathf.InverseLerp(0, 100, 100);
        //Debug.Log(n);
        sprite.color = Color.Lerp(a, b, 1);

        while (CurrentState == EnemyState.ATTACK)
        {
            if (health.PrecentValue < .3f)
            {
                CurrentState = EnemyState.RETREAT;
            }

            if ((Vector2.Distance(playerObj.transform.position, transform.position) <= attackRange))
            {
                dir = Vector3.zero;
                yield return new WaitForSeconds(1);
                UseItem();
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
        sprite.color = Color.Lerp(a, b, .5f);

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

            if (Vector2.Distance(playerObj.transform.position, transform.position) > excapeRadius)
            {
                // excape
                Destroy(this.gameObject);
            }

            if (health.PrecentValue > .7f)
            {
                CurrentState = EnemyState.ATTACK;
            }

            yield return null;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Instantiate(LootPrefab, transform.position, transform.rotation);
        //Debug.Log("Died?");
    }
}
