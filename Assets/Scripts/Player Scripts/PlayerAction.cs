using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{

    public static PlayerAction instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerAction found");
        }
        instance = this;
    }

    public Transform playerTransform;

    public Item heldItem;

    // Update is called once per frame
    void Update()
    {
        if (heldItem == null)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            UseItem();
        }
    }

    void UseItem()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)playerTransform.position;

        heldItem.Use(playerTransform.position,direction, gameObject);
    }
}
