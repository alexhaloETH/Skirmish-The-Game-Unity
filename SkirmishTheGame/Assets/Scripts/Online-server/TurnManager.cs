using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class TurnManager : NetworkBehaviour
{
    // waht if in the match of the calss i attach this turn manager so i can get it 

    public List<Player> players = new List<Player> ();
    [SerializeField] Player currentPlayer;
    [SerializeField] int currentPlayerIndex;
    [SerializeField] bool gameInProgress = true;
     public List<int> ChoosenCardsHost;     // these are the choosen cards from the start of the game
     public List<int> ChoosenCardsJoiner;

    public List<CompressTroopData> HostCardsOnTheField;
    public List<CompressTroopData> JoinerCardsOnTheField;



    // for every "turn end or action" write to this array and then for the player who isnt the action gets called
    IEnumerator TrackTurns()
    {
        currentPlayerIndex = Random.Range(0, players.Count);
        currentPlayer = players[currentPlayerIndex];

        while (gameInProgress)
        {
            currentPlayer.SetTurn(true);
            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] != currentPlayer) players[i].SetTurn(false);
            }

            yield return new WaitForSeconds(10);
            NextPlayer();
        }
    }

    void NextPlayer()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;
        }

        currentPlayer = players[currentPlayerIndex];
    }

    
    public void StartTurnsGame() 
    {
        StartCoroutine(TrackTurns());
    }

    
    public void AddPlayer(Player player)
    {
        players.Add(player);

        if (players.Count == 2) 
        {
            StartCoroutine(TrackTurns());
        }
    }

    //i think its becausee in the matchmaker the reference is wrong and we are not using the singleton like we are meant to

   



}



[System.Serializable]
public class CompressTroopData
{
    public int type;
    public int vitality;



    public CompressTroopData()
    {
       
    }

    public CompressTroopData(int _type, int _vitality)
    {
        this.type = _type;
        this.vitality = _vitality;
    }
}


