using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DescriptionDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public bool isHovering;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        Debug.Log("Mouse enter");
        
        

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        Debug.Log("Mouse Exit");


    }

}
