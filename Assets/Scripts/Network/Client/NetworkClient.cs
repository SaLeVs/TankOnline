using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient
{
    private NetworkManager networkManager;
    private const string MENU_NAME = "Menu";


    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId != 0 && clientId != networkManager.LocalClientId) { return; }
        

        if(SceneManager.GetActiveScene().name != MENU_NAME)
        {
            SceneManager.LoadScene(MENU_NAME);
            Debug.Log("Disconnected from server. Returning to menu.");
        }
        
        if(networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }
}
