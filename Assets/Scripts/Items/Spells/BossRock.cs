using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : MonoBehaviour
{
    bool setup = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (!setup)
        //    Map.MapManager.instance?.tileGrowth.Equations.Add(new RaidialDisplacement(transform, 0, 1, .03, false));
        //gameObject.GetComponent<Rigidbody2D>()?.MovePosition(transform.position + new Vector3(.01f, .01f));
    }
}
