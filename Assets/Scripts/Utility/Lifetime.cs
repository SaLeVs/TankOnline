using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float lifetime;

    private void Start()
    {
        DestroyGameObject();
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject, lifetime);
    }
}
