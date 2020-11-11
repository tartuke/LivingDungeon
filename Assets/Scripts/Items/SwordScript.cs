using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    //bunch of variable declarations that are assigned in editor
    public Transform player;

    public CapsuleCollider2D enemy;
    public PolygonCollider2D swordTriggerBox;
    public SpriteRenderer playerSprite;
    public Animator animator;
    public Enemy slime;
    public GameObject slimeObject;
    public Vector3 box;
    public float width;
    public float height;
    public Equipment sword;
    
    public AudioSource audioSource;
    int frame;
    //basic damage value 
    public int damage = 5;

    bool isSwordReady = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        box = playerSprite.bounds.size;
        width = box.x;
        height = box.y;
        transform.position = player.position;

        swordTriggerBox.enabled = false;
        // was testing with the stats system  but have just implented something super basic for now
        // sword = new Equipment();
        // StatModifier mod1 = new StatModifier(10, StatModType.Flat, StatType.Strength);
        // StatControler controller = new StatControler();
        // controller.Stats.
        // controller.AddModifier(mod1);
        // mod1.Strength.calculateFinalValue();
    }

    // Update is called once per frame
    void Update()
    {
 
        animator.SetBool("isSwinging", false);
        
        // this else if string determines where the sword needs to be centered, can use some polishing to make it look natural
        // considering making it so the sword isn't even rendered until it needs to be swiped
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            Vector3 swordPos = new Vector3(playerSprite.bounds.center.x, playerSprite.bounds.center.y - (height/2));
            transform.eulerAngles = new Vector3(0, 0, -135);
            transform.position = swordPos;
        }
        else if (Input.GetAxisRaw("Vertical") > 0)
        {
            Vector3 swordPos = new Vector3(playerSprite.bounds.center.x, playerSprite.bounds.center.y + (height/2));
            transform.eulerAngles = new Vector3(0, 0, 45);
            transform.position = swordPos;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            Vector3 swordPos = new Vector3(playerSprite.bounds.center.x - (width/2) , playerSprite.bounds.center.y);
            transform.eulerAngles = new Vector3(0, 0, 135);
            transform.position = swordPos;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            Vector3 swordPos = new Vector3(playerSprite.bounds.center.x + (width / 2), playerSprite.bounds.center.y);
            transform.eulerAngles = new Vector3(0, 0, -45);
            transform.position = swordPos;
        }

        // animation and health check if sword swipes the slime
        if (Input.GetMouseButtonDown(0) && isSwordReady)
        {
            isSwordReady = false;
            animator.SetBool("isSwinging", true);
            audioSource.PlayOneShot(audioSource.clip, 0.5f);

            // rewrote to damage any object with a health scrpit
            StartCoroutine(ActivateSword());
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name);
        int x = (int)player.transform.position.x;
        int y = (int)player.transform.position.y;
        for(int i = -1; i < 1; i++)
        {
            for(int j = -1; j < 1; j++)
            {
                FindObjectOfType<MapManager_V2>().DestroyTile((x+i, y+j));
            }
        }
        
        if (col.tag == "Walls")
        {
            Vector2 point = col.ClosestPoint(transform.position);
            Debug.Log("wall hit + " + point);
            
        }

        Health health = col.GetComponent<Health>();
        if (health != null) health.TakeDamage(damage);
    }

    IEnumerator ActivateSword()
    {
        swordTriggerBox.enabled = true;
        yield return new WaitForSeconds(1);
        swordTriggerBox.enabled = false;
        isSwordReady = true;
    }
}
