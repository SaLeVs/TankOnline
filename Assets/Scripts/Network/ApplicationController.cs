using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{

    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    [SerializeField] private NetworkObject playerPrefab; 
    private ApplicationData appData;

    private const string GameSceneName = "Game";

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null); // dedicate server dont have a graphic card
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if(isDedicatedServer)
        {
            Application.targetFrameRate = 60;
            appData = new ApplicationData();

            ServerSingleton serverSingleton = Instantiate(serverPrefab); // if we are a dedicated server, we instantiate the server prefab
            
            StartCoroutine(LoadGameSceneAsync(serverSingleton));
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost(playerPrefab);

            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticate = await clientSingleton.CreateClient();

            // Go to main menu
            if (authenticate)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }

    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameSceneName);

        while(!asyncOperation.isDone)
        {
            yield return null;
        }

        Task createServerTask = serverSingleton.CreateServer(playerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);

        // Logic for able people to connect to the server
        Task startServerTask = serverSingleton.GameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
    }
}
