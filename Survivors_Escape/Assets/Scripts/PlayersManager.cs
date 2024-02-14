using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayersManager : NetworkBehaviour
{
    private List<NetworkObject> playerObjects = new List<NetworkObject>();

    void Start()
    {
        
    }

    void Update()
    {
        //GetPlayersInSession();
        //
        //Debug.Log("LIST OH YES: ");
        //Debug.Log("LIST SIZE: " + playerObjects.Count);
    }

    public void GetPlayersInSession()
    {
        playerObjects.Clear(); // Clear the list to avoid duplicates

        // Iterate through all spawned objects
        foreach (var obj in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
        {
            // Check if the object is a player object
            if (obj.CompareTag("Player"))
            {
                // Add the player object to the list
                playerObjects.Add(obj);
            }
        }
    }
}
