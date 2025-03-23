using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field : SerializeField] public int MaxHealth { get; private set; } = 100; // this dont will change, so can be a normal variable

    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(); // only server can change this value
    
    private bool isDead = false;


    public Action<Health> OnDie;


    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        currentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healthValue)
    {
        ModifyHealth(healthValue);
    }

    private void ModifyHealth(int value)
    {

        if(isDead) return;  

        int newHealth = currentHealth.Value + value;
        currentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        if(currentHealth.Value == 0)
        {
            isDead = true;
            OnDie?.Invoke(this);
        }
    }
}
