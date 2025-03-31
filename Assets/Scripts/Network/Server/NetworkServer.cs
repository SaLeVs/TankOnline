using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    public Action<UserData> OnUserJoined;
    public Action<UserData> OnUserLeft;

    public event Action<string> OnClientLeft;

    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += NetworkManager_ApprovalCheck;
        networkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    public bool OpenConnection(string ip, int port)
    {
        UnityTransport unityTransport = networkManager.gameObject.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ip, (ushort)port);

        return networkManager.StartServer();
    }

    private void NetworkManager_ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payLoad = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payLoad);

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;

        OnUserJoined?.Invoke(userData);

        response.Approved = true;
        response.Position = SpawnPoint.GetRandomSpawnPos();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
    }

    private void NetworkManager_OnServerStarted()
    {
        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback; 
        
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            OnUserLeft?.Invoke(authIdToUserData[authId]);
            authIdToUserData.Remove(authId);
            OnClientLeft?.Invoke(authId);
        }
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
       if(clientIdToAuth.TryGetValue(clientId, out string authId))
       {
            if (authIdToUserData.TryGetValue(authId, out UserData userData))
            {
                return userData;
            }
            return null;
       }
       return null;
    }

    public void Dispose()
    {
        if(networkManager != null) { return; }

        networkManager.ConnectionApprovalCallback -= NetworkManager_ApprovalCheck;
        networkManager.OnClientConnectedCallback -= NetworkManager_OnClientDisconnectCallback;
        networkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        
        if(networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }
}
