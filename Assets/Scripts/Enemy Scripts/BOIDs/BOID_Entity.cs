using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOID_Entity : MonoBehaviour
{
    // Settings
    public float minSpeed = 2;
    public float maxSpeed = 5;
    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1;
    public float maxSteerForce = 3;

    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float seperateWeight = 1;

    public float targetWeight = 1;

    [Header("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;
    public int avoidencePrecision = 5;

    // State
    [HideInInspector]
    public Vector2 position;
    [HideInInspector]
    public Vector2 forward;
    Vector2 velocity2D;

    public int index;

    // To update:
    Vector2 acceleration;
    [HideInInspector]
    public Vector2 avgFlockHeading;
    [HideInInspector]
    public Vector2 avgAvoidanceHeading;
    [HideInInspector]
    public Vector2 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;
    [HideInInspector]
    public Transform target = null;

    private void Start()
    {
        float startSpeed = (minSpeed + maxSpeed) / 2;
        velocity2D = new Vector2(-startSpeed, -startSpeed);

        forward = velocity2D;
        position = transform.position;
    }

    public void UpdateBoid()
    {
        acceleration = Vector2.zero;

        if (target != null)
        {
            Vector2 target2D = target.position;
            Vector2 offsetToTarget = (target2D - position);
            acceleration = SteerTowards(offsetToTarget) * targetWeight;
        }

        if (numPerceivedFlockmates != 0)
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector2 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards(avgFlockHeading) * alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * cohesionWeight;
            var seperationForce = SteerTowards(avgAvoidanceHeading) * seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (Physics2D.Raycast(transform.position, forward, collisionAvoidDst, obstacleMask))
        {
            Debug.DrawRay(transform.position, forward/forward.magnitude * collisionAvoidDst, Color.yellow, 5f);
            //Debug.Log("Collision");
            Vector2 collisionAvoidDir = ObstacleRays();
            Vector2 collisionAvoidForce = SteerTowards(collisionAvoidDir) * avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        // Change to AddForce2D later
        velocity2D += acceleration * Time.deltaTime;

        float speed = velocity2D.magnitude;
        Vector2 direction = velocity2D / speed;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        velocity2D = direction * speed;

        // Convert from vertor 2 to a vector 3 with frozen z
        // Set the position and heading
        transform.position += new Vector3(velocity2D.x, velocity2D.y, 0) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, ToDegree(Mathf.Atan(velocity2D.x/velocity2D.y))); // cashe this offset?
        //transform.forward = new Vector3(velocity2D.x/speed, velocity2D.y/speed, 1);
        //Debug.Log(transform.position + " " + transform.forward + " " + position + " " + forward);

        // Set the public variables for the fleet
        position = transform.position; // check position stays 0
        forward = velocity2D;
    }

    Vector2 ObstacleRays()
    {
        Vector2[] rayDirections = GenerateDirections();

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Debug.DrawRay(transform.position, rayDirections[i], Color.red, .5f);

            if (!Physics2D.Raycast(transform.position, rayDirections[i], collisionAvoidDst, obstacleMask))
            {
                Debug.DrawRay(transform.position, rayDirections[i], Color.green, 5f);
                return rayDirections[i];
            }
        }
        //Debug.Log("No Excape");
        return forward;
    }

    Vector2[] GenerateDirections()
    {
        Vector2[] octArray = new Vector2[avoidencePrecision];
        Vector2[] finalArray = new Vector2[avoidencePrecision * 8];

        int c = 0;
        float rad1;
        float rad2;
        float offset = Mathf.Atan(velocity2D.x / velocity2D.y); //offset in rad

        // generate the quadrant values
        for (int degrees = 0; degrees < 180; degrees += 180/avoidencePrecision)
        { 
            rad1 = (ToRad(degrees) + offset) % ToRad(360);
            rad2 = (offset - ToRad(degrees)) % ToRad(360);

            //Debug.Log("Degrees: " + degrees + " Offset: " + ToDegree(offset) + " First: " + ToDegree(rad1) + " Second " + ToDegree(rad2));

            finalArray[c++] = (new Vector2(Mathf.Cos(rad1), Mathf.Sin(rad1))) * collisionAvoidDst;
            finalArray[c++] = (new Vector2(Mathf.Cos(rad2), Mathf.Sin(rad2))) * collisionAvoidDst;
        }

        return finalArray;
    }

    Vector2 SteerTowards(Vector2 vector)
    {
        Vector2 v = vector.normalized * maxSpeed - velocity2D;
        return Vector2.ClampMagnitude(v, maxSteerForce);
    }

    float ToRad(float degrees)
    {
        return degrees * (Mathf.PI / 180);
    }

    float ToDegree(float rad)
    {
        return rad * (180 / Mathf.PI);
    }
}
