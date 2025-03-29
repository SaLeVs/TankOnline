using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
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
    private float reaminingHealCooldown;
    private float tickTimer;    

    private List<TankPlayer> playersInZone = new List<TankPlayer>();

    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            HealPower.OnValueChanged += OnHealPowerChanged;
            OnHealPowerChanged(0, HealPower.Value);
        }
        if(IsServer)
        {
            HealPower.Value = maxHealPower;
            
        }
    }

    private void Update()
    {
        if(!IsServer) { return; }

        if(reaminingHealCooldown > 0)
        {
            reaminingHealCooldown -= Time.deltaTime;
            
            if(reaminingHealCooldown <= 0)
            {
                HealPower.Value = maxHealPower;
            }
            else
            {
                return;
            }
        }

        tickTimer += Time.deltaTime;

        if(tickTimer >=  1 / healTickRate)
        {
            foreach(TankPlayer player in playersInZone)
            {
               if(HealPower.Value == 0) { break; }

               if(player.Health.currentHealth.Value == player.Health.MaxHealth) { continue; }

               if(player.Wallet.TotalCoins.Value < coinsPerTick) { continue; }

               player.Wallet.SpendCoins(coinsPerTick);
               player.Health.RestoreHealth(healthPerTick);

               HealPower.Value -= 1;

                if(HealPower.Value == 0)
                {
                    reaminingHealCooldown = healCooldown;
                    break;
                }
            }

            tickTimer = tickTimer % (1 / healTickRate);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent(out TankPlayer player)) { return; }

        playersInZone.Add(player);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent(out TankPlayer player)) { return; }

        playersInZone.Remove(player);

    }

    private void OnHealPowerChanged(int oldHealPower, int newHealPower)
    {
        healBar.fillAmount = (float)newHealPower / maxHealPower;
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= OnHealPowerChanged;
        }
        
    }
}
