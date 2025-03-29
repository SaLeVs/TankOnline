using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    private FixedString32Bytes playerName;
    public ulong ClientId { get; private set; }
    public int Coins {get; private set; }

    public void Initialise(ulong clientId, FixedString32Bytes playerName, int coins)
    {
        ClientId = clientId;
        this.playerName = playerName;
        this.Coins = coins;

        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }

    private void UpdateText()
    {
        playerNameText.text = $"{1}. {playerName}  ({Coins})";
    }
}
