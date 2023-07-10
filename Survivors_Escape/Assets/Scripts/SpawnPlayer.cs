using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private void Start()
    {
        Invoke("InstatiateNewObject", 4);
    }

    public void InstatiateNewObject()
    {
        Instantiate(PlayerPrefab);
    }
}
