using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{

    private const int MAX_CONNECTIONS = 20;
    private const string GameSceneName = "Game";

    private Allocation allocation;
    private string joinCode;

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

        RelayServerData relayServerData = new RelayServerData(allocation, "udp");
        
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }
}
