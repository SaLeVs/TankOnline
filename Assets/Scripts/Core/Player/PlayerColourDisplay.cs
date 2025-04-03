using System;
using UnityEngine;

public class PlayerColourDisplay : MonoBehaviour
{
    [SerializeField] private TeamColouLookUp teamColouLookUp;
    [SerializeField] private TankPlayer player;
    [SerializeField] private SpriteRenderer[] playerSprites;

    private void Start()
    {
        Player_OnPlayerTeamChange(-1, player.TeamIndex.Value);
        player.TeamIndex.OnValueChanged += Player_OnPlayerTeamChange;
    }

    private void Player_OnPlayerTeamChange(int oldTeamIndex, int newTeamIndex)
    {
        Color teamColour = teamColouLookUp.GetTeamColour(newTeamIndex);

        foreach (SpriteRenderer sprite in playerSprites)
        {
            sprite.color = teamColour;
        }
    }

    private void OnDestroy()
    {
        player.TeamIndex.OnValueChanged -= Player_OnPlayerTeamChange;
    }
}
