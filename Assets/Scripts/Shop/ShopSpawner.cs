using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSpawner : MonoBehaviour
{

    public GameObject chest;
    public AddShopItems_VFP add;
    public int moveSpeed = 3;
    public float lightRadius;
    int distance = 0;
    int chestCount = 0;
    int maxChests = 3;
    bool isThereAChest = false;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    //make function to spawn at specific x,y coordinate
    void Update()
    {
         if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
         {
            distance += moveSpeed;
         }
         if (distance == 12 && chestCount < maxChests)
         {
            chest.transform.position = transform.position + new Vector3(1, 0, 0);
            add.newInventory();
            chestCount++;
         }
        

    }

    void spawn(Vector3 vector)
    {
        chest.transform.position = vector;
        chest.GetComponent<SpriteRenderer>().enabled = true;
    }

    void spawn(float x, float y)
    {
        chest.transform.position.Set(x, y, chest.transform.position.z);
        chest.GetComponent<SpriteRenderer>().enabled = true;
    }

    void unrender()
    {
        if (distanceToPlayer() > lightRadius)
        {
            chest.GetComponent<SpriteRenderer>().enabled = false;
        }
        
    }

    float distanceToPlayer()
    {
        return Vector3.Distance(chest.transform.position, transform.position);
    }


}
