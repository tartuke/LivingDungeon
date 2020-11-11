using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackControler : MonoBehaviour
{

    public Attack source;

    float expireTime;

    private void Start()
    {
        // set time at which the projectile will expire
        expireTime = Time.time + source.LifeTime;
    }


    private void Update()
    {
        // check if time to expire
        if (Time.time > expireTime)
        {
            // pass object and position back to the source
            source.onExpiration(gameObject, transform.position);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // pass object and colision data back to the source
        source.onCollision(gameObject, collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // pass object and trigger data back to the source
        source.onTrigger(gameObject, collision);
    }
}
