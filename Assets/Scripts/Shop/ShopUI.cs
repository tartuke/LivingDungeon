using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{


    public GameObject shop;
    public GameObject shopUI;
    public GameObject player;

    public Transform shopItemsParent;
    ShopSlot[] shopSlots;

    Animator animator;

    ShopInventory shopInventory;

    // Start is called before the first frame update
    void Start()
    {
        shopUI.SetActive(false);
        animator = shop.GetComponent<Animator>();
        shopInventory = ShopInventory.instance;
        shopInventory.onItemChangedCallBack += UpdateUI;
        shopSlots = shopItemsParent.GetComponentsInChildren<ShopSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(shop.transform.position, player.transform.position);

        if (dist < 1 && Input.GetButtonDown("Interact"))
        {
            shopUI.SetActive(!shopUI.activeSelf);
            animator.SetBool("isPlayerInteracting", !animator.GetBool("isPlayerInteracting"));
        }

    }

    void UpdateUI()
    {

        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (i < shopInventory.items.Count)
            {
                shopSlots[i].addItem(shopInventory.items[i]);
            }
            else 
            {
                shopSlots[i].removeItem();
            }
        }

    }
}
