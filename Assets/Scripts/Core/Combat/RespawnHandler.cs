using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

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
        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId)
    {
        yield return null;
        NetworkObject playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstance.SpawnAsPlayerObject(ownerClientId);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        TankPlayer.OnPlayerSpawned -= TankPlayer_PlayerSpawned;
        TankPlayer.OnPlayerDespawned -= TankPlayer_OnPlayerDespawned;
    }

}
