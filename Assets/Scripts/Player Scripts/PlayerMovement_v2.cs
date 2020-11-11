using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerMovement_v2 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool canMove = true;

    public Rigidbody2D rb;
    public Animator animator;
    public AudioClip footStepClip;
    public AudioClip fallDown;

    private AudioSource playerAudio;

    Vector2 movement;

    void Start()
    {
        playerAudio = GetComponent<AudioSource>();

    }
    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 facing = (mousePosition - rb.position).normalized;

        animator.SetFloat("FacingX", facing.x);
        animator.SetFloat("FacingY", facing.y);

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);

        animator.SetFloat("Speed", movement.sqrMagnitude);

        //Debug.Log(Input.GetMouseButton(0));

        animator.SetBool("Attacking", Input.GetMouseButton(0));

        //Debug.Log(animator.GetBool("Attacking"));

        movement.Normalize();

        if (isWalking() != 0)
            if (!playerAudio.isPlaying)
                playerAudio.PlayOneShot(footStepClip, 0.1f);
    }

    void FixedUpdate()
    {
        if (!Map.MapManager.instance.loadingNextLevel && Map.MapManager.instance.IsHole(transform.position.x, transform.position.y))
        {
            animator.SetBool("Falling", true);
            playerAudio.PlayOneShot(fallDown, 0.15f);
            Map.MapManager.instance.loadingNextLevel = true;
            movement = new Vector3((int)Math.Floor(transform.position.x) + .5f, (int)Math.Floor(transform.position.y) + .5f) - transform.position;
        }

        if (animator.GetBool("Falling"))
            movement = new Vector3((int)Math.Floor(transform.position.x) + .5f, (int)Math.Floor(transform.position.y) + .5f) - transform.position;

        if (!Map.MapManager.instance.IsFloor(transform.position.x, transform.position.y) && !animator.GetBool("Falling")) return;

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    int isWalking()
    {
        return (int)Mathf.Abs(Input.GetAxisRaw("Horizontal")) + (int)Mathf.Abs(Input.GetAxisRaw("Vertical"));

    }
}
