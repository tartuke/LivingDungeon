using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_Base : MonoBehaviour
{
    public Animator mageAni;
    public float attackRange = 2.5f;
    public float speed = 1f;

    public Item heldItem;

    private enum DIR : int
    {
        IDLE,
        UP,
        DOWN,
        RIGHT,
        LEFT
    }

    DIR curDirection;
    Vector3 target;
    Transform player;

    private const float ThreePiOverFour = Mathf.PI * 3 / 4;
    private const float PiOverFour = Mathf.PI / 4;

    StatType statType = StatType.HealthStat;
    StatControler statControler;
    ConsumableStat stat;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        heldItem = Instantiate(heldItem);
        curDirection = DIR.IDLE;
        player = GameObject.Find("Player").transform;
        statControler = GetComponent<StatControler>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stat == null)
        {
            stat = statControler.GetStatOfType(statType) as ConsumableStat;
            if (stat != null)
                stat.OnStatDepletedCallBack += Die;
        }

        if (transform.position != target)
        {
            target = player.position + new Vector3(attackRange/2, attackRange/2, 0);

            var norm = target;
            norm.Normalize();
            var angle = Mathf.Atan2(norm.x, norm.y);
            //Debug.Log(angle);

            if (angle < ThreePiOverFour && angle > -PiOverFour)
            {
                mageAni.SetInteger("WalkState", (int)DIR.UP);
            }
            else if (angle < PiOverFour && angle > -PiOverFour )
            {
                mageAni.SetInteger("WalkState", (int)DIR.RIGHT);
            }

            else if (angle < -PiOverFour && angle > -ThreePiOverFour)
            {
                mageAni.SetInteger("WalkState", (int)DIR.DOWN);
            }
            else if (angle > ThreePiOverFour || angle < -ThreePiOverFour)
            {
                mageAni.SetInteger("WalkState", (int)DIR.LEFT);
            }

            //transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
        }
        else
        {
            mageAni.SetInteger("WalkState", (int)DIR.IDLE);
        }

        if (Vector3.Distance(player.position, transform.position) <= attackRange)
        {
            UseItem();
        }

        Vector2 dir = target.normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

    }

    void UseItem()
    {
        Vector2 direction = player.position - transform.position;
        heldItem.Use(transform.position, direction, gameObject);
    }

    // TODO: should be moved to a new script
    private void Die()
    {
        Destroy(gameObject);
        //GameObject.FindObjectOfType<Spawner_VFP>().freeNum();
        // instantiate loot
        //Debug.Log("Died?");
    }
}
