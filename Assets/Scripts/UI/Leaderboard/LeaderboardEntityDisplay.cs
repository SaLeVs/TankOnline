using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;


    private FixedString32Bytes displayName;

    public int TeamIndex { get; private set; }
    public ulong ClientId { get; private set; }
    public int Coins {get; private set; }

    public void Initialise(ulong clientId, FixedString32Bytes displayName, int coins)
    {
        ClientId = clientId;
        this.displayName = displayName;
        
        UpdateCoins(coins);
    }
    public void Initialise(int teamIndex, FixedString32Bytes displayName, int coins)
    {
        TeamIndex = teamIndex;
        this.displayName = displayName;

        UpdateCoins(coins);
    }

    public void SetColour(Color color)
    {
        playerNameText.color = color;
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }

    public void UpdateText()
    {
        playerNameText.text = $"{transform.GetSiblingIndex() + 1}. {displayName}  ({Coins})";
    }
}
