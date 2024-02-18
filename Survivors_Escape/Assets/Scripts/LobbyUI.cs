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
            SurvivorsEscapeMultiplayer.Instance.StartHost();
            SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
        });

        _joinGameButton.onClick.AddListener(() =>
        {
            SurvivorsEscapeMultiplayer.Instance.StartClient();
        });
    }
}
