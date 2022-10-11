using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using System.Security.Cryptography;
using System.Text;
using Random = UnityEngine.Random;
using System.Net;


// everything that needs to be sent via netwrok needs to be serializable 


// this is what a match will be 
[System.Serializable]


// need a way to update the player from the server with the one in the client side prob need a quick function or somthin


// so the reason why there have been isseus with not the same data on both players is because the data is coming from this players on the server which are the same but not the same data
// is ssotred thats why we need to reset everything


public class Match
{
    public string matchID;

    public bool publicMatch;
    public bool inMatch;
    public bool matchFull;

    public int lordsWagered = 0;
    public string matchHostName = "name";

    public GameObject gameManager;

    public List<Player> players = new List<Player>();
    public Match(string matchID, Player player,bool publicMatch, int lordsWagered, string hoster)
    {
        this.publicMatch = publicMatch;
        this.lordsWagered = lordsWagered;
        this.matchID = matchID;
        players.Add(player);
        this.matchHostName = hoster;

    }

    public Match() { }

}


[System.Serializable]
public class PlayerData
{
    public string address;
    public string key;
    public bool music;

    public PlayerData() { }
}

[System.Serializable]
public class PlayerList
{
    public List<PlayerData> list;

    public PlayerList() {
        list = new List<PlayerData>();
    }

}



[System.Serializable]
public class MatchMaker : NetworkBehaviour
{
    public static MatchMaker instance;

    public PlayerList playerList;

    //[SerializeField] private JSONWriter jsonWriter;

    public SyncList<Match> matches = new SyncList<Match>();

    public SyncList<string> matchesIDs = new SyncList<string>();

    public SyncList<string> connectedAddresses = new SyncList<string>();

    [SerializeField] GameObject turnManagerPrefab;

    public MatchMaker() { }

    private void Start()
    {
        instance = this;
        // this.gameObject.AddComponent<JSONWriter>();
        // JSONWriter.instance.OutputJson();
        playerList = new PlayerList();

    }


    public bool HostGame(string _matchID, Player _player,string _name, bool publicMatch, int lordsWagered, SelectedTeam realm)
    {

        if (matchesIDs.Contains(_matchID))  // if the match Id already exists we cant have a same on so we send a false and ready for a new call
        {
            Debug.Log("Match ID already Exists");
            return false;
        }
        else
        {
            Debug.Log($"match created");
            matchesIDs.Add(_matchID);
            Match match = new Match(_matchID, _player, publicMatch, lordsWagered, _name);
            matches.Add(match);
            _player.wager = lordsWagered;
            _player.playerName = _name;
            _player.currentMatch = match;
            _player.selectedRealm = realm;
            return true;
        }
    }

    // i dont think this (turnmamnage) should be spawned in both of the players we need the turnmanager or techincally the game manage to do evertgtubg
    public void BeginGame(string _matchID)
    {
        GameObject newTurnManager = Instantiate(turnManagerPrefab);
        NetworkServer.Spawn(newTurnManager);
        newTurnManager.GetComponent<NetworkMatch>().matchId = _matchID.ToGuid();
        TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _matchID)
            {
                matches[i].gameManager = newTurnManager;

                int idx = 0;
                foreach (var player in matches[i].players)
                {

                    Debug.Log($"{player.name} is getting added to the turn manager");

                    Player _player = player.GetComponent<Player>();


                    if (idx == 0) 
                    {
                        turnManager.ChoosenCardsHost = _player.selectedRealm.TroopsIdList;
                    }
                    else if (idx == 1) 
                    {
                        turnManager.ChoosenCardsJoiner = _player.selectedRealm.TroopsIdList;
                    }

                    turnManager.AddPlayer(_player);
                    player.StartGame();

                    idx++;
                    //player.cardDeck = new Deck();

                }

                break;
            }
        }
    }


    public bool JoinGame(string _matchID, Player _player,string _name, out int wager, SelectedTeam savedTroops)
    {
        wager = -1;

        if (matchesIDs.Contains(_matchID))  // if the match Id exists we need to find the game
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID== _matchID) 
                {
                    if (!matches[i].matchFull)
                    {
                        matches[i].players.Add(_player);
                        matches[i].matchFull = true;
                        _player.currentMatch = matches[i];
                        _player.playerName = _name;
                        _player.wager = matches[i].lordsWagered;
                        _player.selectedRealm = savedTroops;
                        wager = matches[i].lordsWagered;
                    }
                    else { return false; }
                    
                    break;
                }
            }
            Debug.Log("Match found!!");
            
            return true;
        }
        else
        {
            Debug.Log("match never found");
            return false;
        }

    }

    public static string GetRandomMatchID()   // this creates the ID needed per room 
    {

        string _id = string.Empty;  // create an empty string

        for (int i = 0; i < 5; i++)       // we want it to be 5 big
        {
            int random = UnityEngine.Random.Range(0, 36);    // random number from 0 to 36    that is all char and int possible


            if (random < 26)
            {
                _id += (char)(random + 65);    // if less then 26 its a letter and turn the byte into char, the 65 is so we have capitals
            }
            else
            {
                _id += (random - 26).ToString();      // if bigger then 26 then its a normal number
            }
        }


        Debug.Log($"the random Id is: {_id}");

        return _id;
    }


    // complicated, if in game then destory if not in game just lobby stay but ofcours ebare in mind the after amoutn
    // need to check this with contract behaviours
    public void playerDisconnect(Player player, string _matchID) 
    {
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].matchID == _matchID) 
            {
                int playerIndex = matches[i].players.IndexOf(player);
                matches[i].players.RemoveAt(playerIndex);
                matches[i].matchFull = false;
                Debug.Log($"REMOVED PLAYER");

                if (matches[i].players.Count == 0) 
                {
                    Debug.Log($"no more players, termination match");
                    matches.RemoveAt(i);
                    matchesIDs.Remove(_matchID);
                
                }

                break;

            }
        }
    }

 




    public void GetInitialMatchData(string _matchID) 
    {
       
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    
                }
            }
          
        
    }

    public string GetKey(string address) 
    {
        foreach (var item in playerList.list)
        {
            if (address == item.address)
            {
                return item.key;
            }
        }
        return "-1";
    }

    public  bool PlayerConnectToServerCheck(string address)
    {

        if (connectedAddresses.Contains(address))  
        {
            return true;
        }
        else
        {
            connectedAddresses.Add(address);

            bool add = false;
           
                foreach (var item in playerList.list)
                {
                    if (address == item.address)
                    {
                        add = true;
                        break;
                    }
                }
            

            if (!add) 
            {
                PlayerData dataRef = new PlayerData();

                dataRef.address = address;
                dataRef.music = true;
                dataRef.key = GenKey();

                playerList.list.Add(dataRef);
            }



            return true;
        }
    }

 

    public void RemoveActivePlayer(string address) 
    {
        connectedAddresses.Remove(address);
    }

    public string GenKey()
    {
        string Key = string.Empty;  // create an empty string

        for (int i = 0; i < 16; i++)       // we want it to be 20 big
        {
            int random = Random.Range(0, 9);    // random number from 0 to 9    that is all char and int possible
            Key += (random).ToString();      // if bigger then 26 then its a normal number
            if (i == 15) 
            {
                
            }
            else 
            {
                Key += "-";
            }
       
            
        }

        Debug.Log($"genedkey {Key}");

        return Key;

    }





}



// with the fact that we have one server dealing with multiple lobbies we need a way to tell the server what obj is in which
// a GUID identifier lets us have this number which dictates everything happenign in the lobby
public static class MatchExtension
{

    // this is quite interesting so the static public means its accessible everywhere
    // plus the this in the args means we can used it as a . i think
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
