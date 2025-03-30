using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{

    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null); // dedicate server dont have a graphic card
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if(isDedicatedServer)
        {
            ServerSingleton serverSingleton = Instantiate(serverPrefab); // if we are a dedicated server, we instantiate the server prefab
            await serverSingleton.CreateServer();

            // Logic for able people to connect to the server
            await serverSingleton.GameManager.StartGameServerAsync();
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticate = await clientSingleton.CreateClient();

            // Go to main menu
            if (authenticate)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }
}
