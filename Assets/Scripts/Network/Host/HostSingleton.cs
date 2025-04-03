using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{

    public HostGameManager GameManager { get; private set; }


    private static HostSingleton instance;
    public static HostSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }

            instance = FindAnyObjectByType<HostSingleton>();

            if (instance == null)
            {
                Debug.LogError("No HostSingleton in the scene!");
                return instance;
            }

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost(NetworkObject playerPrefab)
    {
        GameManager = new HostGameManager(playerPrefab);  
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
