using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyCodeTxt;

    private NetworkVariable<FixedString32Bytes> lobbyCode = new NetworkVariable<FixedString32Bytes>("");

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            lobbyCode.OnValueChanged += OnLobbyCodeChanged;
            OnLobbyCodeChanged(string.Empty, lobbyCode.Value);
        }

        if(!IsHost) { return; }

        lobbyCode.Value = HostSingleton.Instance.GameManager.JoinCode;
    }

    private void OnLobbyCodeChanged(FixedString32Bytes oldCode, FixedString32Bytes newCode)
    {
        lobbyCodeTxt.text = newCode.ToString();
    }

    public void LeaveGame()
    {
        
        if(NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Shutdown();
        }

        ClientSingleton.Instance.GameManager.Disconnected();
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged -= OnLobbyCodeChanged;
        }
    }
}
