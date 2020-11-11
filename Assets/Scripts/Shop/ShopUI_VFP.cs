using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI_VFP : MonoBehaviour
{


    public GameObject shop;
    public GameObject prompt;
    public GameObject shopUI;
    public GameObject player;

    public Transform shopItemsParent;
    public ShopSlot_VFP[] shopSlots;

    public AudioClip chest_open;
    public AudioClip chest_close;
    AudioSource FX;

    Animator animator;

    ShopInventory_VFP shopInventory;

    bool open = false;

    // Start is called before the first frame update
    void Start()
    {
        FX = GetComponent<AudioSource>();
        shopUI.SetActive(false);
        animator = shop.GetComponent<Animator>();
        shopInventory = ShopInventory_VFP.instance;
        shopInventory.onItemChangedCallBack += UpdateUI;
        shopSlots = shopItemsParent.GetComponentsInChildren<ShopSlot_VFP>();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(shop.transform.position, player.transform.position);
        
        if (dist < 1 && Input.GetButtonDown("Interact"))
        {
            FX.clip = chest_open;
            FX.Play();
            shopUI.SetActive(!shopUI.activeSelf);
            prompt.SetActive(false);
            animator.SetBool("isPlayerInteracting", !animator.GetBool("isPlayerInteracting"));
            open = !open;
        } else if (open && dist > 2)
        {
            FX.clip = chest_close;
            FX.Play();
            shopUI.SetActive(false);
            animator.SetBool("isPlayerInteracting", !animator.GetBool("isPlayerInteracting"));
            open = false;
        }

    }

    void UpdateUI(Item item, InventoryAction action)
    {
        switch (action)
        {
            case InventoryAction.AddItem:
                for (int i = 0; i < shopSlots.Length; i++)
                {
                    if (shopSlots[i].IsEmpty())
                    {
                        shopSlots[i].addItem(item);
                        return;
                    }
                }
                break;
            case InventoryAction.RemoveItem:
                for (int i = 0; i < shopSlots.Length; i++)
                {
                    if (shopSlots[i].Contains(item))
                    {
                        shopSlots[i].removeItem();
                        return;
                    }
                }
                break;
            default:
                break;
        }
    }
}
