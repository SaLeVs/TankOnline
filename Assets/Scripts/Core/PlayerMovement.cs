using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;


    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turningRate = 30f;
    private Vector2 lastMovementInput;
    private float zRotation;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) // here we are checking who was spawn, if is us, is owner, so can do the code
        {
            return;
        }

        inputReader.MoveEvent += HandleMovement;

    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        inputReader.MoveEvent -= HandleMovement;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        zRotation = lastMovementInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        
        rb.linearVelocity = (Vector2)bodyTransform.up * lastMovementInput.y * moveSpeed;

    }

    private void HandleMovement(Vector2 movement)
    {
        lastMovementInput = movement;
    }
}
