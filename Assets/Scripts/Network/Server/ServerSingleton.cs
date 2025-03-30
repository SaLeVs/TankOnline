using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using Unity.Netcode;

public class ServerSingleton : MonoBehaviour
{

    public ServerGameManager GameManager { get; private set; }


    private static ServerSingleton instance;
    public static ServerSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }

            instance = FindAnyObjectByType<ServerSingleton>();

            if (instance == null)
            {
                Debug.LogError("No ServerSingleton in the scene!");
                return instance;
            }

            return instance;
        }
    }


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();
        GameManager = new ServerGameManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton
        );
        // here we are spawning the server and passing all the data that we need to start the server
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
