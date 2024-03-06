using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Spawner : NetworkBehaviour
{
    public static Spawner Instace { get; private set; }

    public SpawnableList _spawnableList;
    private Transform _transform;
    public NetworkVariable<int> stackn = new NetworkVariable<int>();

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
        Debug.Log("SPAWN OBJECT GOT CALLED BY SOMEONE");

        var item = GetItemFromIndex(itemSOIndex);

        if (item == null) { Debug.Log("Prefab null"); }

        Debug.Log(stackSize.ToString());

        GameObject itDropModel = item.itPrefab;
        INV_PickUp pickup = itDropModel.GetComponent<INV_PickUp>();
        pickup.data = item;

        stackn.Value = stackSize;
        pickup.stackSize = stackn.Value;

        Debug.Log(pickup.stackSize.ToString());

        transform.position = new Vector3(x, y, z);
        pickup.transform.position = transform.position;

        //var instance = Instantiate(myPrefab);
        //var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        //instanceNetworkObject.Spawn();

        var pickNO = Instantiate(itDropModel);

        var refNO = pickNO.GetComponent<NetworkObject>();
        refNO.Spawn();

        //bool xd = refNO.IsOwnedByServer;
        //Debug.Log(xd.ToString());

        //PlayerData u0 = SurvivorsEscapeMultiplayer.Instance.GetPlayerDataFromPlayerIndex(0);
        //ulong id0 = u0.clientId;
        //Debug.Log(id0.ToString());
        //bool r0 = refNO.IsNetworkVisibleTo(id0);
        //Debug.Log(r0.ToString());

        //PlayerData u1 = SurvivorsEscapeMultiplayer.Instance.GetPlayerDataFromPlayerIndex(1);
        //ulong id1 = u1.clientId;
        //Debug.Log(id1.ToString());
        //bool r1 =  refNO.IsNetworkVisibleTo(id1);
        //Debug.Log(r1.ToString());

        //pickup.transform.SetParent(null);
        //NetworkObject pickupNetworkObject = pickup.GetComponent<NetworkObject>();
        //if (pickupNetworkObject != null) { pickupNetworkObject.Spawn(true); }
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
