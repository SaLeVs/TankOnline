using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private const string MenuSceneName = "Menu";
    private JoinAllocation joinAllocation;

    public async Task<bool> InitAsync()
    {
       await UnityServices.InitializeAsync();
       AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join server: {e.Message}");
            return;
        }

        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
        unityTransport.SetRelayServerData(relayServerData);

        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Mssing name")
        };

        string payLoad = JsonUtility.ToJson(userData);
        byte[] payLoadBytes = Encoding.UTF8.GetBytes(payLoad);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payLoadBytes;

        NetworkManager.Singleton.StartClient();

    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }
}
