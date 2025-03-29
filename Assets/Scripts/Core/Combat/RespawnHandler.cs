using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private float keptPercentage;

    private int totalCoinsAfterDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

        foreach(TankPlayer player in players)
        {
            TankPlayer_PlayerSpawned(player);
        }
            
        TankPlayer.OnPlayerSpawned += TankPlayer_PlayerSpawned;
        TankPlayer.OnPlayerDespawned += TankPlayer_OnPlayerDespawned;
    }


    private void TankPlayer_PlayerSpawned(TankPlayer player)
    {
        player.Health.OnDie += (health) => OnPlayerDie(player);
    }
    private void TankPlayer_OnPlayerDespawned(TankPlayer player)
    {
        player.Health.OnDie -= (health) => OnPlayerDie(player);
    }

    private void OnPlayerDie(TankPlayer player)
    {
        totalCoinsAfterDie = (int)(player.Wallet.TotalCoins.Value * (keptPercentage / 100f));

        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(player.OwnerClientId, totalCoinsAfterDie));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, int keptCoins)   
    {
        yield return null;
        TankPlayer playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
        playerInstance.Wallet.TotalCoins.Value += keptCoins;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        TankPlayer.OnPlayerSpawned -= TankPlayer_PlayerSpawned;
        TankPlayer.OnPlayerDespawned -= TankPlayer_OnPlayerDespawned;
    }

}
