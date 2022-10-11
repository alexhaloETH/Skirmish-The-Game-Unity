using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class Player : NetworkBehaviour
{
    public static Player localPlayer;   // singleton

    NetworkMatch networkMatch;

    [SyncVar] public string matchID;

    [SyncVar] public int wager;

    [SyncVar] public string playerName = "no";

    [SyncVar] public Match currentMatch;

    GameObject playerLobbyUI;

    [SerializeField] public SelectedTeam selectedRealm;


    public bool myTurn { get; private set; }





    void Awake() { networkMatch = GetComponent<NetworkMatch>(); }



    public void ConfirmTeam(RealmUI realmsData) 
    {
        selectedRealm = new SelectedTeam();
        selectedRealm.setID(realmsData.RealmID);
        selectedRealm.setTroops(realmsData.TroopTypesListCurrentTeam);
        selectedRealm.setOrder((int)realmsData.RealmOrder);
    }


    public override void OnStartClient()
    {
        // this is called when the client gets added to the server


      
        if (isLocalPlayer)  // make the localplayer singleton = to this script, this if is needed in case multiple instances from the networking
        {
            localPlayer = this;
            playerName = PlayerPrefs.GetString("TestName");


        }
        else
        {
            playerLobbyUI = UILobby.instance.spawnPlayerUIPrefab(this);  // if they join the lobby and they are not us then we spawn a prefab for the UILobby
        }
    }












    private void Start()
    {
        CheckNames(PlayerPrefs.GetString("TestName"));
    }


    // to delete
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            NetworkManager.singleton.StopClient();
        }
    }




    public override void OnStopClient()
    {
        //this gets called when the player disconnects
   
        ClientDisconnect();
    }

    public override void OnStopServer()
    {
        Debug.Log($"whenis this called ons topserver");
        ServerDisconnect();
    }

    public override void OnStartServer()
    {
        // player is connected to server call
        Debug.Log($"wheneneneenen on start server");
    }







    #region Get key
    // we need to change the name here to a more global scope
    public void CallForData()
    {
        CmdGetKey(playerName);
    }



    [Command]
    void CmdGetKey(string address) // this happens in the server, also Cmd is important 
    {
        string key = MatchMaker.instance.GetKey(address);

        if (key == "-1") 
        {
            GeneralUtilScript.instance.SpawnMessagePrefab("there is an issue with your key", false);
        }
        else 
        {
            TargetGetKey(key);
        }
    }


    [TargetRpc]
    void TargetGetKey(string key)
    {
        DeckViewLogic.instance.RequestDataJs(key);
        
        //call something
    }








    public void CallForDatac()
    {
        CmdGetKeyc(playerName);
    }



    [Command]
    void CmdGetKeyc(string address) // this happens in the server, also Cmd is important 
    {
        string key = MatchMaker.instance.GetKey(address);

        if (key == "-1")
        {
            GeneralUtilScript.instance.SpawnMessagePrefab("there is an issue with your key", false);
        }
        else
        {
            TargetGetKeyc(key);
        }
    }


    [TargetRpc]
    void TargetGetKeyc(string key)
    {
        DeckViewLogic.instance.BuildSendData(key);

        //call something
    }









    #endregion



    #region Name check

    public void CheckNames(string address)
    {
        CmdCheckNames(address);
    }



    [Command]
    void CmdCheckNames(string address) // this happens in the server, also Cmd is important 
    {
        if (MatchMaker.instance.PlayerConnectToServerCheck(address)) 
        {
            TargetCheckNames(false);
        }
        else
        {
            TargetCheckNames(true);
        }
    }


    [TargetRpc]
    void TargetCheckNames(bool present)
    {
        if (present)
        NetworkManager.singleton.StopClient();
    }







    









    #endregion



    #region Host game

    public void HostGame(bool publicMatch, int lordsWagered)
    {
        Debug.Log($"Host game player");

        string matchID = MatchMaker.GetRandomMatchID();
        CmdHostGame(matchID, publicMatch, playerName, lordsWagered,selectedRealm);
    }



    [Command]
    void CmdHostGame(string _matchID, bool publicMatch,string _playerName, int lordsWagered,SelectedTeam SavedTroop) // this happens in the server, also Cmd is important 
    {

        matchID = _matchID;
        if (MatchMaker.instance.HostGame(_matchID, this,_playerName,publicMatch,lordsWagered, SavedTroop))  // if it manages to host the game then...
        {                                                  // server checks the match maker and depending on out come stuff happens
            networkMatch.matchId = _matchID.ToGuid();
            Debug.Log("<color=green> Game hosted succesfully</color>");
            TargetHostGame(true,_matchID, _playerName, lordsWagered);
        }
        else
        {
            Debug.Log("<color=red> Game hosted un-succesfully</color>");
            TargetHostGame(false,_matchID, _playerName, lordsWagered);
        }
    }


    [TargetRpc]
    void TargetHostGame(bool success, string _matchID,string _playerName, int wager) 
    {
        playerName = _playerName;
        matchID = _matchID;
        Debug.Log($"MathcId = {_matchID} = {matchID}");
        UILobby.instance.HostSuccess(success,matchID,   wager.ToString());
    }

    #endregion



    #region Get all matches for the Menu

    public void GetAllMatches(int minRange, int maxRange)
    {   
        CmdGetAllMatches(minRange,maxRange);
    }



    [Command]
    void CmdGetAllMatches(int minRange, int maxRange) // this happens in the server, also Cmd is important 
    {
        int freeMatches = 0;


        for (int i = 0; i < MatchMaker.instance.matches.Count; i++)
        {
            if (MatchMaker.instance.matches[i].publicMatch == true && !MatchMaker.instance.matches[i].matchFull) 
            {
                freeMatches++;
            }
        }




        List<Match> matches = new List<Match>();
        // if less then 50 do the same without the if


        if (freeMatches <= 10) 
        {
            for (int i = 0; i < MatchMaker.instance.matches.Count; i++)
            {

                Match currMatch = MatchMaker.instance.matches[i];

                
                if (currMatch.publicMatch == true && !MatchMaker.instance.matches[i].matchFull)
                {
                    if (minRange == 0 && maxRange == 0) // look for any match
                    {
                        matches.Add(MatchMaker.instance.matches[i]);
                    }
                    else if (0 == minRange && maxRange >= currMatch.lordsWagered)
                    {
                        matches.Add(MatchMaker.instance.matches[i]);
                    }
                    else if (0 == maxRange && minRange <= currMatch.lordsWagered)
                    {
                        matches.Add(MatchMaker.instance.matches[i]);
                    }
                    else if (minRange <= currMatch.lordsWagered && maxRange >= currMatch.lordsWagered) // between two ranges
                    {
                        matches.Add(MatchMaker.instance.matches[i]);
                    }
                    else
                    {

                    }
                }
            }
           
        }
        else
        {
            Debug.Log($"this is in the other");
            List<int> indexesTaken = new List<int>();

            int maxNum = MatchMaker.instance.matches.Count;

            for (int i = 0; i < 3; i++)
            {
                while (true)
                {

                    int index = Random.Range(0, maxNum);


                    if (indexesTaken.Contains(index)) 
                    {
                        Debug.Log($"does contain {index}");
                    }
                    else 
                    {

                        if (MatchMaker.instance.matches[index].publicMatch && !MatchMaker.instance.matches[index].matchFull) 
                        {
                            Debug.Log($"douesnt contian {index}");
                            indexesTaken.Add(index);
                            Match currMatch = MatchMaker.instance.matches[index];
                            
                                if (minRange == 0 && maxRange == 0) // look for any match
                                {
                                    matches.Add(MatchMaker.instance.matches[index]);
                                }
                                else if (0 == minRange && maxRange >= currMatch.lordsWagered)
                                {
                                    matches.Add(MatchMaker.instance.matches[index]);
                                }
                                else if (0 == maxRange && minRange <= currMatch.lordsWagered)
                                {
                                    matches.Add(MatchMaker.instance.matches[index]);
                                }
                                else if (minRange <= currMatch.lordsWagered && maxRange >= currMatch.lordsWagered) // between two ranges
                                {
                                    matches.Add(MatchMaker.instance.matches[index]);
                                }
                                else
                                {

                                }


                            break;

                        }



                       
                    }
                }
            }
            





        }





        // if more then



















        TargetGetAllMatches(matches);
       
    }


    [TargetRpc]
    void TargetGetAllMatches(List<Match> matches)
    {
        UILobby.instance.ReceiveAllMatches(matches);
    }



    #endregion



    #region Join game


    public void JoinGame(string _inputId)
    {
        CmdJoinGame(_inputId, playerName, selectedRealm);
    }




    [Command]
    void CmdJoinGame(string _matchID, string _playerName, SelectedTeam savedRealm) // this happens in the server, also Cmd is important 
    {

        matchID = _matchID;
        if (MatchMaker.instance.JoinGame(_matchID, this,_playerName, out int wager,savedRealm))  // if it manages to host the game then...
        {                                                  // server checks the match maker and depending on out come stuff happens
            networkMatch.matchId = _matchID.ToGuid();
            Debug.Log("<color=green> Game joined succesfully</color>");
            TargetJoinGame(true, _matchID, _playerName, wager);
        }
        else
        {
            Debug.Log("<color=red> Game joined un-succesfully</color>");
            TargetJoinGame(false, _matchID, _playerName, wager);
        }
    }


    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID, string _playerName, int wager)
    {
        playerName = _playerName;
        matchID=_matchID;
        UILobby.instance.JoinSuccess(success,matchID,wager.ToString());
    }

    #endregion



    #region Start game

    public void BeginGame()
    {
        CmdBeginGame();
    }

    [Command]
    void CmdBeginGame() // this happens in the server, also Cmd is important 
    {
        MatchMaker.instance.BeginGame(matchID);
        Debug.Log($"Game beginning");
    }


    public void StartGame() 
    {
        TargetBeginGame();
    }


    [TargetRpc]
    void TargetBeginGame()
    {
        Debug.Log($"MacthId game {matchID} is starting ");

        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }

    #endregion



    #region Disconnect from server

    public void DisconnectGame()
    {
        CmdDisconnectGame();
    }



    [Command]
    void CmdDisconnectGame() 
    {
        ServerDisconnect();

    }


    void ServerDisconnect() 
    {
        MatchMaker.instance.playerDisconnect(this, matchID);

        networkMatch.matchId = string.Empty.ToGuid();
        RpcDisconnectGame();
    }




    [ClientRpc]
    void RpcDisconnectGame()
    {
        ClientDisconnect();
    }


    void ClientDisconnect() 
    {
        if (playerLobbyUI != null)
        {
            Destroy(playerLobbyUI);
        }

        CmdClientDisconnect();
    }



    [Command]
    void CmdClientDisconnect() // this happens in the server, also Cmd is important 
    {
        
        ServerDisconnect();

    }


    #endregion








    // the first player is always the host



    public void DealCards() 
    {
        CmdDealCards(matchID);
    }

    [Command]
    void CmdDealCards(string _matchID)
    {
        // this runs for both independently

        List<int> cardsHost = new List<int>();
        List<int> cardsjoin = new List<int>();

        string hostName = string.Empty;

        for (int i = 0; i < MatchMaker.instance.matches.Count; i++)
        {
            if (MatchMaker.instance.matches[i].matchID == _matchID)
            {
                Match matchData = MatchMaker.instance.matches[i];

                if (matchData.matchHostName == playerName)   // if this is the host
                {
                    cardsHost = matchData.gameManager.transform.GetComponent<TurnManager>().ChoosenCardsHost;
                    hostName = playerName;
                    cardsjoin = matchData.gameManager.transform.GetComponent<TurnManager>().ChoosenCardsJoiner;
                }
                else
                {
                    cardsjoin = matchData.gameManager.transform.GetComponent<TurnManager>().ChoosenCardsJoiner;

                    cardsHost = matchData.gameManager.transform.GetComponent<TurnManager>().ChoosenCardsHost;
                }
            }
        }

        RpcDealCards(cardsjoin, cardsHost, hostName);
    }



    [TargetRpc]
    void RpcDealCards(List<int> JoinerCards, List<int> HosterCards, string HostName)
    {
        if (HostName == playerName)   
        {
            GameMedium.Instance.PopSelfZone(HosterCards);
            GameMedium.Instance.PopEnemyZone(JoinerCards.Count);
        }
        else
        {
            GameMedium.Instance.PopSelfZone(JoinerCards);
            GameMedium.Instance.PopEnemyZone(HosterCards.Count);
        }
    }











    public void PlayCard(int type, int vit,GameObject opponent)
    {
        CmdPlayCard(matchID,opponent,type, vit,playerName);
    }

    [Command]
    void CmdPlayCard(string _matchID,GameObject opponent,int _type,int _vit,string _playername)
    {
        // this runs for both independently

        Debug.Log($"called by how many times");
        List<CompressTroopData> data = new List<CompressTroopData>();

        for (int i = 0; i < MatchMaker.instance.matches.Count; i++)
        {
            if (MatchMaker.instance.matches[i].matchID == _matchID)
            {
                Match matchData = MatchMaker.instance.matches[i];

                if (matchData.matchHostName == _playername) 
                {
                    CompressTroopData savedRef = new CompressTroopData(_type, _vit);
                    matchData.gameManager.transform.GetComponent<TurnManager>().HostCardsOnTheField.Add(savedRef);

                    data = matchData.gameManager.transform.GetComponent<TurnManager>().HostCardsOnTheField;
                    NetworkIdentity opponentIdentity = opponent.GetComponent<NetworkIdentity>();
                    RpcPlayCard(opponentIdentity.connectionToClient, data);
                }
                else {

                    CompressTroopData savedRef = new CompressTroopData(_type, _vit);
                    matchData.gameManager.transform.GetComponent<TurnManager>().JoinerCardsOnTheField.Add(savedRef);

                    data = matchData.gameManager.transform.GetComponent<TurnManager>().JoinerCardsOnTheField;
                    NetworkIdentity opponentIdentity = opponent.GetComponent<NetworkIdentity>();
                    RpcPlayCard(opponentIdentity.connectionToClient, data);
                }


              


                
            }
        }
        

    }



    [TargetRpc]
    void RpcPlayCard(NetworkConnection target,List<CompressTroopData> data)
    {

        
            Debug.Log($"addddddddddddddddddddddddddddddddddddd");
            GameMedium.Instance.PopEnemyField(data);
        
       
    }




































    public void CallStartGame()
    {
        CmdCallStartGame(matchID);
    }


    [Command]
    void CmdCallStartGame(string _matchID)
    {
        for (int i = 0; i < MatchMaker.instance.matches.Count; i++)
        {
            if (MatchMaker.instance.matches[i].matchID == _matchID)
            {
                Match matchData = MatchMaker.instance.matches[i];

                Debug.Log($"this is  in the command cmaclallstartgame");


                if (matchData.matchHostName == playerName)   // if this is the host
                {
                    matchData.gameManager.transform.GetComponent<TurnManager>().StartTurnsGame();
                }
          
            }
        }
    }











    // this is the one thats gets called from the courite
    public void SetTurn(bool _myTurn)
    {
        myTurn = _myTurn; //Set myTurn on server
        RpcSetTurn(_myTurn);
    }

    [ClientRpc]
    void RpcSetTurn(bool _myTurn)
    {
        //Debug.Log($"{(isLocalPlayer ? "localPlayer" : "other player")}'s turn.");

        myTurn = _myTurn; //Set myTurn on clients
    }




    // this is the one that gets called from the scene
    public void DoSomething()
    {
        if (myTurn)
        { //Check that it is your turn
            CmdDoSomething();
            Debug.Log($"it is your turn");
        }
        else
        {
            Debug.Log($"It is not your turn!");
        }
    }

    [Command]
    void CmdDoSomething()
    {
        RpcDoSomething();
    }

    [ClientRpc]
    void RpcDoSomething()
    {
       // Debug.Log($"{(isLocalPlayer ? "localPlayer" : "other player")} is doing something {playerName}.");
    }








}







[System.Serializable]
public class SelectedTeam 
{

    public int RealmID;
    public int Order;

    public List<int> TroopsIdList = new List<int>();

    public SelectedTeam() { }


    public void setID(int _ID) { this.RealmID = _ID; }

    public void setOrder(int _Order) { this.Order = _Order; }

    public void setTroops(List<int> list) { this.TroopsIdList = list; }




    public void Shuffle()
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = this.TroopsIdList.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            int value = this.TroopsIdList[k];
            this.TroopsIdList[k] = this.TroopsIdList[n];
            this.TroopsIdList[n] = value;
        }
    }
}






