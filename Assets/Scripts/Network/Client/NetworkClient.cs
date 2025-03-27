using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
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
        }
        
        if(networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if(networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }
}
