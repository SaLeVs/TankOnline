using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;

    [Space(1.5f)]

    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;

    [Space(5f)]

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;

    private bool shouldFire;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent += InputReader_LaunchProjectile;
    }

    private void InputReader_LaunchProjectile(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    private void Update()
    {
        if(!IsOwner) return;

        if(!shouldFire) return;

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }

    private void SpawnDummyProjectile(Vector3 spwanPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spwanPos, Quaternion.identity);
        projectileInstance.transform.up = direction;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spwanPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spwanPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        SpawnDummyProjectileClientRpc(spwanPos, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spwanPos, Vector3 direction)
    {
        if(IsOwner) return;

        SpawnDummyProjectile(spwanPos, direction);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent -= InputReader_LaunchProjectile;
    }
}
