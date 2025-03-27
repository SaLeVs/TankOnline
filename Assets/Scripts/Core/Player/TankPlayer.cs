using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Settings")]
    [SerializeField] private int cameraPriority = 15;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            cinemachineCamera.Priority = cameraPriority;
        }
    }
}
