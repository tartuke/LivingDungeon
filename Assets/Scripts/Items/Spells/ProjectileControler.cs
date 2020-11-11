using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileControler : MonoBehaviour
{

    public Spell source;

    float expireTime;
    Transform soundManager;

    private void Start()
    {
        // set time at which the projectile will expire
        expireTime = Time.time + source.LifeTime;
        soundManager = GameObject.Find("Sound_Manager").transform;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        try
        {
            StartCoroutine(soundManager.GetComponent<BMG_Manager>().PlayOneShotAudio(3));
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        
        // pass object and colision data back to the source
        source.onCollision(gameObject, collision);
        
    }
}
