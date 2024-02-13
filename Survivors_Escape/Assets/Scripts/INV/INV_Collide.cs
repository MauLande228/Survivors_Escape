using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
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
                    bDestroyed = other.GetComponentInChildren<INV_ScreenManager>().AddItem(pickup, cc);
                }
            }

            if(!bDestroyed)
            {
                Destroy(pickup.gameObject);
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
}
