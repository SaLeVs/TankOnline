using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameUI : MonoBehaviour
{

    [SerializeField] private TankPlayer player;
    [SerializeField] private TMP_Text playerNameText;
    private void Start()
    {
       Player_UpdatePlayerName(string.Empty, player.playerName.Value);
       player.playerName.OnValueChanged += Player_UpdatePlayerName;
    }

    private void Player_UpdatePlayerName(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();
    }

    private void OnDestroy()
    {
        player.playerName.OnValueChanged -= Player_UpdatePlayerName;
    }
}
