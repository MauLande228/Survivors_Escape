using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INV_Collide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Touching something");
            INV_PickUp pickup = this.GetComponent<INV_PickUp>();
            //Debug.Log(pickup.stackSize);

            if (pickup != null)
            {
                other.GetComponentInChildren<INV_ScreenManager>().AddItem(pickup);
            }
        }
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
