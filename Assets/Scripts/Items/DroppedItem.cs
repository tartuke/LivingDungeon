using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{

    public Item item;
    public bool passive = true;
    public bool useOnPickup = false;

    void Start()
    {
        if (item == null)
        {
            SetUp();
        } else
        {
            SpriteRenderer sR = gameObject.GetComponent<SpriteRenderer>();

            sR.sprite = item.icon;
        }
    }

    IEnumerator SetUp()
    {
        yield return new WaitForEndOfFrame();

        if (item == null)
        {
            SetUp();
            yield return null;
        }

        SpriteRenderer sR = gameObject.GetComponent<SpriteRenderer>();

        sR.sprite = item.icon;

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (passive)
                Interact(collision);
            else if (Input.GetButtonDown("Interact"))
                Interact(collision);
        }
    }

    private void Interact(Collider2D collision)
    {
        if (useOnPickup)
            item.Use(collision.transform.position, new Vector2(), collision.gameObject);
        else
            Inventory_VFP.instance.Add(item);
        Destroy(gameObject);
    }

}
