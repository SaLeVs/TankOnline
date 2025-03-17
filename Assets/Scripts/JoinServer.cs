using Unity.Netcode; // we need to add this namespace to use the NetworkManager
using UnityEngine;

public class JoinServer : MonoBehaviour
{
    public void Join()
    {
        NetworkManager.Singleton.StartClient(); // we can use netowrk manager because the NetworkManager is a singleton
    }
}
