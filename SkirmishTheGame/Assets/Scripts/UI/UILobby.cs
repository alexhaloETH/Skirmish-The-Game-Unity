using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices;

public class UILobby : MonoBehaviour
{


    [DllImport("__Internal")]
    static extern void CheckSNS(string SNS);

    [DllImport("__Internal")]
    static extern void SetSNS(string SNS);


    public static UILobby instance;

    GameObject playerLobbyUI;

    [Header("Host Join")]


    [SerializeField] TMP_InputField joinMatchInput;

    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;

    [SerializeField] TMP_InputField LordsWageredInputField;
    

    [Header("Lobby")]

    [SerializeField] Transform UIPlayerParent;
    [SerializeField] GameObject UIPlayerPrefab;
    [SerializeField] TMP_Text matchIDText;
    [SerializeField] TMP_Text LordsWageredText;


    [Header("Get Matches")]

    [SerializeField] Transform UIMatchParent;
    [SerializeField] GameObject UIMatchPrefab;

    [SerializeField] TMP_InputField MinLordsWageredInputField;
    [SerializeField] TMP_InputField MaxLordsWageredInputField;

    [SerializeField] GameObject allGames;

    [SerializeField] Button GetMatchesButton;
    [SerializeField] Animator InputFieldsAnimator;


    [Header("Main Menu")]


    [SerializeField] TMP_Text AddressText;

    [Header("SNS")]


    [SerializeField] TMP_InputField SetSNSInputField;
    [SerializeField] TMP_InputField CheckSNSInputField;








    private void Start()
    {
        instance = this;
        AddressText.text = "Address: " + PlayerPrefs.GetString("TestName");
    }


    #region Host/Join button and success

    public void HostPrivate()
    {
        int wager;

        if (int.TryParse(LordsWageredInputField.text, out wager))
        {

            Menumanager.Instance.OpenMenu("LoadingMenu");

            Player.localPlayer.HostGame(false, wager);
        }
        else
        {

            LordsWageredInputField.GetComponent<Animator>().SetTrigger("errorTrigger");
        }
    }

    public void HostPublic()
    {
        int wager;

        if (int.TryParse(LordsWageredInputField.text, out wager))
        {
            Menumanager.Instance.OpenMenu("LoadingMenu");

            Player.localPlayer.HostGame(true, wager);
        }
        else
        {
            LordsWageredInputField.GetComponent<Animator>().SetTrigger("errorTrigger");
        }
    }

    public void HostSuccess(bool success, string _matchID, string wager)
    {
        if (success)
        {

            Debug.Log($"call on the host {Player.localPlayer.playerName}");
            Menumanager.Instance.OpenMenu("LobbyMenu");

            if (playerLobbyUI != null) Destroy(playerLobbyUI);

            playerLobbyUI = spawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = "Match Id is:\n" + _matchID;
            LordsWageredText.text = "Lords wagered:\n" + wager;



        }
        else
        {


            Menumanager.Instance.OpenMenu("HostMenu");
        }
    }

    public void JoinSuccess(bool success, string _matchID, string wager)
    {

        if (success)
        {

            if (playerLobbyUI != null) Destroy(playerLobbyUI);
            playerLobbyUI = spawnPlayerUIPrefab(Player.localPlayer);

            matchIDText.text = "Match Id is:\n" + _matchID.ToString();
            LordsWageredText.text = "Lords wagered:\n" + wager;

            Menumanager.Instance.OpenMenu("LobbyMenu");
        }
        else
        {
            if (allGames.activeSelf) 
            {
                Menumanager.Instance.OpenMenu("AllGamesMenu");
                GetAllMatches();
            }
            else
            {
                Menumanager.Instance.OpenMenu("JoinMenu");
            }
        }
    }

    public void Join()
    {
        if (joinMatchInput.text.Length != 5)
        {
            Debug.Log($"on the if");
            joinMatchInput.GetComponent<Animator>().SetTrigger("errorTrigger");
        }
        else
        {
            Menumanager.Instance.OpenMenu("LoadingMenu");
            Debug.Log($"on the else");
            Player.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
        }
    }

    public void Join(string _match)
    {
        Player.localPlayer.JoinGame(_match.ToUpper());
    }

    #endregion


    #region Get Matches region

    public void GetAllMatches()
    {

        int minRange = 0;
        int maxRange = 0;

        if (string.IsNullOrEmpty(MinLordsWageredInputField.text) && string.IsNullOrEmpty(MaxLordsWageredInputField.text)) // if they are both empty     look for any
        {
            Player.localPlayer.GetAllMatches(minRange, maxRange);
        }
        else if (string.IsNullOrEmpty(MinLordsWageredInputField.text) && int.TryParse(MaxLordsWageredInputField.text, out maxRange)) // the min is empty but the top isnt        from 0  to max given
        {
            Player.localPlayer.GetAllMatches(0, maxRange);
        }
        else if (string.IsNullOrEmpty(MaxLordsWageredInputField.text) && int.TryParse(MinLordsWageredInputField.text, out minRange))  // the max is 0 the min isnt        from min to infinite
        {
            Player.localPlayer.GetAllMatches(minRange, 0);
        }
        else if (int.TryParse(MinLordsWageredInputField.text, out minRange) && int.TryParse(MaxLordsWageredInputField.text, out maxRange))  // from min to max normalcase scenario
        {
            Player.localPlayer.GetAllMatches(minRange, maxRange);
        }
        else 
        {
            InputFieldsAnimator.SetTrigger("errorTrigger");
            Debug.Log($"What given is invalid");
        }

    }

    public void ReceiveAllMatches(List<Match> matches)
    {

        DelGamesArr();

        for (int i = 0; i < matches.Count; i++)
        {
            GameObject newUIMatch = Instantiate(UIMatchPrefab, UIMatchParent);

            newUIMatch.GetComponent<UIMatch>().setData(matches[i]);
        }
    }

    public void DelGamesArr()
    {
        foreach (Transform child in UIMatchParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void sortBigToSmall() 
    {
        List<UIMatch> uiMatch = new List<UIMatch>();


        foreach (Transform child in UIMatchParent)
        {

            uiMatch.Add(child.GetComponent<UIMatch>());

            Destroy(child.gameObject);
        }

        uiMatch = uiMatch.OrderBy(w => w.LordsWagered).ToList();

        uiMatch.Reverse();

        for (int i = 0; i < uiMatch.Count; i++)
        {
            GameObject newUIMatch = Instantiate(UIMatchPrefab, UIMatchParent);

            newUIMatch.GetComponent<UIMatch>().setData(uiMatch[i]);
        }

    }

    public void sortSmallToBig()
    {
        List<UIMatch> uiMatch = new List<UIMatch>();

        foreach (Transform child in UIMatchParent)
        {
            uiMatch.Add(child.GetComponent<UIMatch>());

            Destroy(child.gameObject);
        }

        uiMatch = uiMatch.OrderBy(w => w.LordsWagered).ToList();

        for (int i = 0; i < uiMatch.Count; i++)
        {
            GameObject newUIMatch = Instantiate(UIMatchPrefab, UIMatchParent);

            newUIMatch.GetComponent<UIMatch>().setData(uiMatch[i]);
        }
    }


    #endregion








    public GameObject spawnPlayerUIPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);

        return newUIPlayer;
    }

    public void BeginGame()
    {
        Player.localPlayer.BeginGame();
        // begin game
    }

    public void DisconnectLobby()
    {
        if (playerLobbyUI != null) Destroy(playerLobbyUI);

        Player.localPlayer.DisconnectGame();

        Menumanager.Instance.OpenMenu("MainMenu");
    }





    public void CallTest(string JsonHopefully) 
    {
        obj somethig = obj.CreateFromJSON(JsonHopefully);

        Debug.Log($"daadadda");


        foreach (string text in somethig.valee) 
        {
            print(text);
        }
        foreach (string text in somethig.val)
        {
            print(text);
        }

    
    }





    public void callSetSNS() 
    {
        SetSNS(SetSNSInputField.text);
    }
    public void callCheckSNS() 
    {
        CheckSNS(CheckSNSInputField.text);
    }

    public void receiveCheckSNS(int exists) 
    {
        if (exists != 1) { GeneralUtilScript.instance.SpawnMessagePrefab("This SNS is not available", false); }
        else { GeneralUtilScript.instance.SpawnMessagePrefab("This SNS doesnt exist", true); }
    }


    public void Getkey() 
    {
        Player.localPlayer.CallForData();
    }



}

