using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIMatch : MonoBehaviour
{

    [SerializeField] TMP_Text hostName;
    [SerializeField] TMP_Text lordsWagered;
    public int LordsWagered;
    [SerializeField] TMP_Text gameId;
    [SerializeField] Button join_game;

    public void setData(Match _match) 
    {
        hostName.text = _match.matchHostName;
        LordsWagered = _match.lordsWagered;
        lordsWagered.text = _match.lordsWagered.ToString();
        gameId.text = _match.matchID;
    }

    public void StartGame() 
    {
        UILobby.instance.Join(gameId.text);
    }

    public void setData(UIMatch _match)
    {
        hostName.text = _match.hostName.text;
        LordsWagered = _match.LordsWagered;
        lordsWagered.text = _match.lordsWagered.text;
        gameId.text = _match.gameId.text;
    }


}
