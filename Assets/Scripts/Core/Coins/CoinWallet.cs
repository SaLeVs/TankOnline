using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private BountyCoin bountyCoinPrefab;

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercentage = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minCoinValue = 5;
    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
       if(!IsServer) return;

        coinRadius = bountyCoinPrefab.GetComponent<CircleCollider2D>().radius;
        health.OnDie += Health_OnDie;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Coin coin)) return;

        int coinValue = coin.Collect();

        if (!IsServer) return;

        TotalCoins.Value += coinValue;
    }

    public void SpendCoins(int coins)
    {
        TotalCoins.Value -= coins;
    }

    private void Health_OnDie(Health health)
    {
        int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if(bountyCoinValue < minCoinValue) { return; }
        
        for(int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoin bountyCoinInstance = Instantiate(bountyCoinPrefab, GetSpawnPoint(), Quaternion.identity);
            bountyCoinInstance.SetValue(bountyCoinValue);
            bountyCoinInstance.NetworkObject.Spawn();
        }
    }

    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * coinSpread;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoint, coinRadius, layerMask); // we substitute this beacause we dont have more the method NonAlloc

            if (colliders.Length == 0)
            {
                return spawnPoint;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        health.OnDie -= Health_OnDie;
    }
}
