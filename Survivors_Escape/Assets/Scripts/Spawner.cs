using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Spawner : NetworkBehaviour
{
    public static Spawner Instace { get; private set; }

    private void Awake()
    {
        Instace = this;
    }

    public void DropItem(Slot slot)
    {
        if (slot == null)
        {
            Debug.Log("Prefab null");
            Debug.Log(slot.stackSize.ToString());
        }

        SpawnObjectServerRpc(slot);
    }

    [ServerRpc(RequireOwnership = false)]
     public void SpawnObjectServerRpc(Slot slot)
     {
        if (slot == null)
        {
            Debug.Log("Prefab null");
            Debug.Log(slot.stackSize.ToString());
        }

        GameObject itDropModel = slot.data.itPrefab;
        INV_PickUp pickup = Instantiate(itDropModel, slot.GetCurrentDropPos()).AddComponent<INV_PickUp>();

        NetworkObject pickupNetworkObject = pickup.GetComponent<NetworkObject>();
        if (pickupNetworkObject != null)
        {
            pickupNetworkObject.Spawn(true);
        }

        //Inv_itemSO data;

        pickup.data = slot.data;
        pickup.stackSize = slot.stackSize;

        slot.Clean();
     }
}
