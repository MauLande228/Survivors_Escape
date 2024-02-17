using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _createGameButton;
    [SerializeField] private Button _joinGameButton;

    private void Awake()
    {
        _createGameButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Starting HOST");
        });

        _joinGameButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Starting CLIENT");
        });
    }
}
