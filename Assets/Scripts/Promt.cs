using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Promt : MonoBehaviour
{
    public GameObject prompt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            if (prompt != null) prompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (prompt != null) prompt.SetActive(false);
        }
    }
}
