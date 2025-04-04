using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;
    [SerializeField] private int entitiesToDisplay = 8;

    [SerializeField] private Transform teamLeaderboardEntityHolder;
    [SerializeField] private GameObject teamLeaderboardBackground;
    [SerializeField] private Color ownerColour;
    [SerializeField] private string[] teamNames;
    [SerializeField] private TeamColouLookUp teamColourLookUp;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();
    private List<LeaderboardEntityDisplay> teamEntityDisplays = new List<LeaderboardEntityDisplay>();

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            if(ClientSingleton.Instance.GameManager.userData.userGamePreferences.gameQueue == GameQueue.Team)
            {
                teamLeaderboardBackground.gameObject.SetActive(true);

                for (int i = 0; i < teamNames.Length; i++)
                {
                    LeaderboardEntityDisplay teamDisplay = Instantiate(leaderboardEntityPrefab, teamLeaderboardEntityHolder);
                    teamDisplay.Initialise(i, teamNames[i], 0);
                    Color teamColour = teamColourLookUp.GetTeamColour(i);
                    teamDisplay.SetColour(teamColour);

                    teamEntityDisplays.Add(teamDisplay);
                }

                teamLeaderboardBackground.SetActive(true);

            }
            else
            {
                leaderboardEntityHolder.gameObject.SetActive(true);
                teamLeaderboardEntityHolder.gameObject.SetActive(false);
            }

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
        if(!gameObject.scene.isLoaded) { return; }

        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                if(!entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    LeaderboardEntityDisplay leaderEntityDisplay = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);

                    if (NetworkManager.Singleton.LocalClientId == changeEvent.Value.ClientId)
                    {
                        leaderEntityDisplay.SetColour(ownerColour);
                    }

                    leaderEntityDisplay.Initialise(changeEvent.Value.ClientId, changeEvent.Value.PlayerName, changeEvent.Value.Coins);
                    entityDisplays.Add(leaderEntityDisplay);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntityDisplay displayToRemove = entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    entityDisplays.Remove(displayToRemove);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntityDisplay displayToUpdate = entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if(displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;  

        }

        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

        for(int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText(); 

            entityDisplays[i].gameObject.SetActive(i <= entitiesToDisplay - 1);
        }

        LeaderboardEntityDisplay myDisplay = entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        if(myDisplay != null)
        {
           if(myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
           {
                leaderboardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
           }
        }

        if (!teamLeaderboardBackground.activeSelf) return;

        LeaderboardEntityDisplay teamDisplay = teamEntityDisplays.FirstOrDefault(x => x.TeamIndex == changeEvent.Value.TeamIndex);

        if(teamDisplay != null)
        {
            if(changeEvent.Type == NetworkListEvent<LeaderboardEntityState>.EventType.Remove)
            {
                teamDisplay.UpdateCoins(teamDisplay.Coins - changeEvent.Value.Coins);
            }
            else
            {
                teamDisplay.UpdateCoins(teamDisplay.Coins + (changeEvent.Value.Coins - changeEvent.PreviousValue.Coins));
            }

            teamEntityDisplays.Sort((x,y) => y.Coins.CompareTo(x.Coins));

            for(int i = 0; i < teamEntityDisplays.Count; i++)
            {
                teamEntityDisplays[i].transform.SetSiblingIndex(i);
                teamEntityDisplays[i].UpdateText();

            }
        }
    }

    private void PlayerSpawned(TankPlayer player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.playerName.Value,
            TeamIndex = player.TeamIndex.Value,
            Coins = 0
        });

        player.Wallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) => OnCoinsChanged(player.OwnerClientId, newCoins);
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

        player.Wallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) => OnCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void OnCoinsChanged(ulong clientId, int newCoins)
    {
        for(int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientId != clientId) { continue; }

            leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientId = leaderboardEntities[i].ClientId,
                PlayerName = leaderboardEntities[i].PlayerName,
                TeamIndex = leaderboardEntities[i].TeamIndex,
                Coins = newCoins
            };

            return;
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
