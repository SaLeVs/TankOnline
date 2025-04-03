using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text matchmakingStatus;
    [SerializeField] private TMP_Text matchmakingTime;
    [SerializeField] private TMP_Text matchmakingButtonText;
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private Toggle teamToggle;

    private bool isMatchmaking;
    private bool isCancelling;
    private bool isBusy;

    private float timeInQueue;



    private void Start()
    {
        if(ClientSingleton.Instance == null) { return; }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        matchmakingStatus.text = string.Empty;
        matchmakingTime.text = string.Empty;
    }

    private void Update()
    {
        if (isMatchmaking)
        {
            timeInQueue += Time.deltaTime;
            TimeSpan ts = TimeSpan.FromSeconds(timeInQueue);
            matchmakingTime.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            
        }
    }

    public async void StartHost()
    {
        if (isBusy) { return; }
        isBusy = true;

        await HostSingleton.Instance.GameManager.StartHostAsysnc();
        isBusy = false;
    }

    public async void StartClient()
    {
        if (isBusy) { return; }
        isBusy = true;

        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
        isBusy = false;
    }

    public async void StartMatchmaking()
    {
        if(isCancelling) { return; }

        if (isMatchmaking)
        {
            matchmakingStatus.text = "Canceling...";
            isCancelling = true;
            await ClientSingleton.Instance.GameManager.CancelMatchmaking();
            isCancelling = false;
            isMatchmaking = false;
            isBusy = false;
            matchmakingButtonText.text = "FIND MATCH";
            matchmakingStatus.text = string.Empty;
            matchmakingTime.text = string.Empty;
            return;
        }
        if (isBusy) { return; }

        ClientSingleton.Instance.GameManager.MatchmakeAsync(teamToggle.isOn, OnMatchMade);
        matchmakingButtonText.text = "CANCEL";
        matchmakingStatus.text = "Searching...";
        timeInQueue = 0;
        isMatchmaking = true;
        isBusy = true;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isBusy) return;
        isBusy = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        isBusy = false;
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                matchmakingStatus.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                matchmakingStatus.text = "TicketCreationError";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                matchmakingStatus.text = "TicketCancellationError";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                matchmakingStatus.text = "TicketRetrievalError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                matchmakingStatus.text = "MatchAssignmentError";
                break;
        }
    }
}
