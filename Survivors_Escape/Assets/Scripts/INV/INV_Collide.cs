using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            else
            {
                STR_Main storage = this.GetComponent<STR_Main>();
                if (storage != null)
                {
                    if(!storage.opened)
                    {
                        storage.Open(other.GetComponentInChildren<STR_UI>());
                    }
                }
            }

        }
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
