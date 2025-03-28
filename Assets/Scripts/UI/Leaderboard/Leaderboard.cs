using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
        
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            leaderboardEntities.OnListChanged += LeaderboardEntities_OnListChanged;

            foreach(LeaderboardEntityState entity in leaderboardEntities)
            {
                LeaderboardEntities_OnListChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer)
        {
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

            foreach (TankPlayer _player in players)
            {
                PlayerSpawned(_player);
            }

            TankPlayer.OnPlayerSpawned += PlayerSpawned;
            TankPlayer.OnPlayerDespawned += PlayerDespawned;
        }
    }

    private void LeaderboardEntities_OnListChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        switch(changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                break;
        }
    }

    private void PlayerSpawned(TankPlayer player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.playerName.Value,
            Coins = 0
        });
    }

    private void PlayerDespawned(TankPlayer player)
    {
        if(leaderboardEntities == null) { return; }

        foreach (LeaderboardEntityState entity in leaderboardEntities)
        {
            if (entity.ClientId != player.OwnerClientId) { continue; }

            leaderboardEntities.Remove(entity);
            break;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= LeaderboardEntities_OnListChanged;
        }

        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= PlayerSpawned;
            TankPlayer.OnPlayerDespawned -= PlayerDespawned;
        }
    }
}
