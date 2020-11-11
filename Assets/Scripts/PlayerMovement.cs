using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool canMove = true;

    public Rigidbody2D rb;
    public Animator animator;
    
    
    public AudioClip footStepClip;
    
    private AudioSource playerAudio;

    Vector2 movement;

    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        playerAudio.clip = footStepClip;
        playerAudio.loop = true;
        playerAudio.volume = 0.75f;
        
    }
    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        
        if (isWalking() != 0)
        {
            if(!playerAudio.isPlaying)
            {
                //Debug.Log("Audio Playing");
                playerAudio.PlayOneShot(footStepClip, 0.1f);
            }
        }
        else
        {
            playerAudio.Stop();
        }
        
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
    
    int isWalking()
    {
        return (int) Mathf.Abs(Input.GetAxisRaw("Horizontal")) + (int) Mathf.Abs(Input.GetAxisRaw("Vertical"));
        
    }
}
