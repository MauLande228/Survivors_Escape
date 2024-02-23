using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Spawner : NetworkBehaviour
{
    
    public static Spawner Instace { get; private set; }

    public SpawnableList _spawnableList;
    private Transform _transform;

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
    }

    [ServerRpc(RequireOwnership = false)]
     public void SpawnObjectServerRpc(int itemSOIndex, int stackSize, float x, float y, float z)
     {
        var item = GetItemFromIndex(itemSOIndex);

        if (item == null)
        {
            Debug.Log("Prefab null");
        }

        Debug.Log(stackSize);
        GameObject itDropModel = item.itPrefab;

        transform.position = new Vector3(x, y, z);

        INV_PickUp pickup = Instantiate(itDropModel, transform).AddComponent<INV_PickUp>();
        pickup.transform.position = transform.position;
        //pickup.transform.SetParent(null);

        NetworkObject pickupNetworkObject = pickup.GetComponent<NetworkObject>();
        if (pickupNetworkObject != null)
        {
            pickupNetworkObject.Spawn(true);
        }

        pickup.data = item;
        pickup.stackSize = stackSize;
     }

     [ServerRpc(RequireOwnership = false)]
     public void SpawnBulletServerRpc(float x1, float y1, float z1, float x2, float y2, float z2)
     {

         //Vector3 aimDir = (_mouseWorldPosition - _spawnBulletPosition.position).normalized;
         //Instantiate(_bullerProjectile, _spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));

        var item = GetItemFromIndex(47);

         if (item == null)
         {
             Debug.Log("Prefab null");
         }

         GameObject itModel = item.itPrefab;

         Vector3 pos = new Vector3(x1, y1, z1);
         Vector3 aimDir = new Vector3(x2, y2, z2);

         Instantiate(itModel, pos, Quaternion.LookRotation(aimDir, Vector3.up));


         NetworkObject networkObject = itModel.GetComponent<NetworkObject>();
         if (networkObject != null)
         {
             networkObject.Spawn(true);
         }
     }

    public int GetItemIndex(Inv_itemSO itemSO)
    {
        return _spawnableList._itemsList.IndexOf(itemSO);
    }

    public Inv_itemSO GetItemFromIndex(int i)
    {
        return _spawnableList._itemsList[i];
    }
}
