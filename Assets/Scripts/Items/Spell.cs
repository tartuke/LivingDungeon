using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : ScriptableObject
{
    public virtual int ManaCost { get; }
    public virtual float CastDelay { get; }
    public virtual float LifeTime { get; }

    // called by wand to cast spell
    public virtual void Cast(Wand wand, Vector2 pos, Vector2 direction, GameObject user) { }

    // called by ProjectileControler when spell collides with an object
    public virtual void onCollision(GameObject projectile, Collision2D collision) { }

    // called by ProjectileControler when spells lifeTime is finished
    public virtual void onExpiration(GameObject projectile, Vector2 pos) { }

}
