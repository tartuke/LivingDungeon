using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public Rigidbody2D followRb;
    public Rigidbody2D rb;

    public float maxSpeed = 5;
    public float lag = .5f;
    public float offset = 2;
    public float jumpDist = 3;

    Vector2 movement;
    float speed;

    Vector2 fLastPos;
    Vector2 fDirection = new Vector2(-1, 1);


    // Start is called before the first frame update
    void Start()
    {
        fLastPos = followRb.position;
    }

    private void Update()
    {
        movement = (followRb.position + (fDirection).normalized * offset) - rb.position;
        speed = maxSpeed * movement.sqrMagnitude / (lag * lag);
        speed = (speed > maxSpeed) ? maxSpeed : speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movement.sqrMagnitude > jumpDist*jumpDist)
        {
            rb.gameObject.transform.position = followRb.position;
        }else 
            rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
        if ((followRb.position - fLastPos).sqrMagnitude > .001)
            fDirection = followRb.position - fLastPos;
        fLastPos = followRb.position;
    }

}
