using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += NetworkManager_ApprovalCheck;
        networkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    private void NetworkManager_ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payLoad = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payLoad);

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;

        response.Approved = true;
        response.CreatePlayerObject = true;
    }

    private void NetworkManager_OnServerStarted()
    {
        networkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback; 
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }

    public void Dispose()
    {
        if(networkManager != null) { return; }

        networkManager.ConnectionApprovalCallback -= NetworkManager_ApprovalCheck;
        networkManager.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
        networkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        
        if(networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }
}
