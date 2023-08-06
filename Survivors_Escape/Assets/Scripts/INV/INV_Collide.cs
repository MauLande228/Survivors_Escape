using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class INV_Collide : NetworkBehaviour
{
    static bool bDestroyed = true;

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Player"))
        {
            INV_PickUp pickup = this.GetComponent<INV_PickUp>();
            GameObject go = other.gameObject;
            SurvivorsEscape.CharacterController cc = go.GetComponent<SurvivorsEscape.CharacterController>();

            if (pickup != null && cc != null)
            {
                if (cc.IsOwner)
                {
                    Debug.Log("PICK UP");
                    bDestroyed = other.GetComponentInChildren<INV_ScreenManager>().AddItem(pickup);
                }
            }

            if(bDestroyed)
            {
                Destroy(pickup.gameObject);
            }
        }
    }
}
