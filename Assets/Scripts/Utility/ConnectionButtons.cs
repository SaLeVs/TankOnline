using Unity.Netcode; // we need to add this namespace to use the NetworkManager
using UnityEngine;

public class ConnectionButtons : MonoBehaviour
{
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient(); // we can use netowrk manager because the NetworkManager is a singleton
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}
