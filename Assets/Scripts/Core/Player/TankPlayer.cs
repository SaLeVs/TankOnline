using System;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet Wallet { get; private set; }
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Texture2D crosshair;

    [Header("Settings")]
    [SerializeField] private int cameraPriority = 15;
    [SerializeField] private Color myColor;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;   

    public override void OnNetworkSpawn()
    {

        if(IsServer)
        {
            UserData userData = 
                HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            playerName.Value = userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }
        if (IsOwner)
        {
            cinemachineCamera.Priority = cameraPriority;
            spriteRenderer.color = myColor;
            Cursor.SetCursor(crosshair, new Vector2(crosshair.width / 2, crosshair.height / 2), CursorMode.Auto);
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
        
    }
}
