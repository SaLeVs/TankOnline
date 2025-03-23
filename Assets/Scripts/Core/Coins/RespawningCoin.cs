using System;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;

    private Vector3 previousPos;

    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (alreadyCollected)
        {
            return 0;
        }

        alreadyCollected = true;

        OnCollected?.Invoke(this);  
        return coinValue;

    }

    public void Update()
    {
        if (!IsServer) return;

        if(previousPos != transform.position)
        {
            Show(true);
        }
        previousPos = transform.position;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
