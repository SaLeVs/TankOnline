using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text matchmakingStatus;
    [SerializeField] private TMP_Text matchmakingTime;
    [SerializeField] private TMP_Text matchmakingButtonText;
    [SerializeField] private TMP_InputField joinCodeField;


    private void Start()
    {
        if(ClientSingleton.Instance == null) { return; }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        matchmakingStatus.text = string.Empty;
        matchmakingTime.text = string.Empty;
    }
    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsysnc();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }

    public async void StartMatchmaking()
    {

    }
}
