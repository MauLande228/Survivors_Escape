using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class INV_Collide : NetworkBehaviour
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

            GameObject go = other.gameObject;
            SurvivorsEscape.CharacterController cc = go.GetComponent<SurvivorsEscape.CharacterController>();

            if (pickup != null && cc != null)
            {
                if (!cc.IsOwner) return;
                Debug.Log("PICK UP");
                other.GetComponentInChildren<INV_ScreenManager>().AddItem(pickup);

                /*var agents = GameObject.FindGameObjectsWithTag("Player");
                foreach (var a in agents)
                {
                    SurvivorsEscape.CharacterController cc = a.GetComponent<SurvivorsEscape.CharacterController>();
                    if (cc != null)
                    {
                        if(cc.IsPlayerOwner())
                        {
                            Debug.Log("PICK UP");
                            other.GetComponentInChildren<INV_ScreenManager>().AddItem(pickup);
                        }
                    }
                }*/
            }
        }
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
