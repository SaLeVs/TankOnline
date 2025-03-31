using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text matchmakingStatus;
    [SerializeField] private TMP_Text matchmakingTime;
    [SerializeField] private TMP_Text matchmakingButtonText;
    [SerializeField] private TMP_InputField joinCodeField;


    private bool isMatchmaking;
    private bool isCancelling;


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
        if(isCancelling) { return; }

        if (isMatchmaking)
        {
            matchmakingStatus.text = "Canceling...";
            isCancelling = true;
            // Cancel matchmaking
            isCancelling = false;
            isMatchmaking = false;
            matchmakingButtonText.text = "FIND MATCH";
            matchmakingStatus.text = string.Empty;
            return;
        }
        // Start queue
        matchmakingButtonText.text = "CANCEL";
        matchmakingStatus.text = "Searching...";
        isMatchmaking = true;



    }
}
