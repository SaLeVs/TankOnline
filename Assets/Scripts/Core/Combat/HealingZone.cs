using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image healBar;


    [Header("Settings")]
    [SerializeField] private int maxHealPower = 30;
    [SerializeField] private float healCooldown = 35f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private int coinsPerTick = 10;
    [SerializeField] private int healthPerTick = 10;

    private List<TankPlayer> playersInZone = new List<TankPlayer>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent(out TankPlayer player)) { return; }

        playersInZone.Add(player);

        Debug.Log($"Entered: {player.playerName.Value}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent(out TankPlayer player)) { return; }

        playersInZone.Remove(player);

        Debug.Log($"Left: {player.playerName.Value}");
    }
}
