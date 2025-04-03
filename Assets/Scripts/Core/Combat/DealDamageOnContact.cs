using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private int damage = 10;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == null) return;

        if(projectile.TeamIndex != -1)
        {
            if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)
            {
                if(player.TeamIndex.Value == projectile.TeamIndex)
                {
                    return;
                }
            }
        }
        

        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
