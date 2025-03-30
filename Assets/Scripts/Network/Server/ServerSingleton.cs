using System.Threading.Tasks;
using UnityEngine;

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


    public void CreateServer()
    {
        GameManager = new ServerGameManager();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
