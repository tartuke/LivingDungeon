using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : ScriptableObject
{
    public virtual float AttackDelay { get; }
    public virtual float LifeTime { get; }

    // called by Sword to begin Attack
    public virtual void Begin(Sword wand, Vector2 pos, Vector2 direction, GameObject User) { }

    // called by AttackControler when attack collider collides with an object
    public virtual void onCollision(GameObject projectile, Collision2D collision) { }

    // called by AttackControler when attack trigger collides with an object
    public virtual void onTrigger(GameObject projectile, Collider2D collision) { }

    // called by AttackControler when attack lifeTime is finished
    public virtual void onExpiration(GameObject projectile, Vector2 pos) { }
}
