using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;


public class HostGameManager
{

    private const int MAX_CONNECTIONS = 20;
    private const string GameSceneName = "Game";

    private Allocation allocation;
    private string joinCode;
    private string lobbyId;

    public async Task StartHostAsysnc()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(MAX_CONNECTIONS);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to start server: {e.Message}");
            return;
        }

        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join code: {joinCode}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to start server: {e.Message}");
            return;
        }

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false; // we can add something here for create private lobbies
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                { "JoinCode", new DataObject(
                visibility: DataObject.VisibilityOptions.Member,
                value: joinCode)
                }
            };

           Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("My Lobby", MAX_CONNECTIONS, lobbyOptions);
           lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15f));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }


    private IEnumerator HeartBeatLobby(float waitTimeSeconds) // we use this for say to ugs, that the lobby is still active
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds); // we use this for not create a new object every time

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

}
