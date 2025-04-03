using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ParticleSystem dustCloudParticleSystem;


    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turningRate = 30f;
    [SerializeField] private float particleEmisionValue = 10f;

    private ParticleSystem.EmissionModule emissionModule;
    private Vector2 lastMovementInput;
    private float zRotation;
    private Vector3 previousPos;
    private const float PARTICLE_STOP_TRESHOLD = 0.005f;

    private void Awake()
    {
        emissionModule = dustCloudParticleSystem.emission;
    }

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
        if((transform.position - previousPos).sqrMagnitude > PARTICLE_STOP_TRESHOLD)
        {
            emissionModule.rateOverTime = particleEmisionValue;
        }
        else
        {
            emissionModule.rateOverTime = 0;
        }

        previousPos = transform.position;

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
