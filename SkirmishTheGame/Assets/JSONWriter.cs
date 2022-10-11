using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static JSONWriter;

public class JSONWriter : MonoBehaviour
{


    // when we save a new name we reload the thing

    public static JSONWriter instance;



    


    [System.Serializable]
    public class Player
    {
        public string address;
        public string key;
        public bool music;

        public Player(){}
    }

    [System.Serializable]
    public class PlayerList
    {
        public List<Player> list;
    }

    public PlayerList playerList = new PlayerList();

    [SerializeField]private TextAsset textJSON;






    private void Start()
    {
        instance = this;
    }


    public void OutputJson() 
    {
        Debug.Log($"called to write");
        string strOutput = JsonUtility.ToJson(playerList);

        File.WriteAllText(Application.dataPath + "/Resources/Database.txt", strOutput);
        Debug.Log($"called to write");

        textJSON = (TextAsset)Resources.Load("Database");

        playerList = new PlayerList();
    }


    public void GetKeyFromJson(string _name)
    {
        playerList = JsonUtility.FromJson<PlayerList>(textJSON.text);

        foreach (Player employee in playerList.list)
        {
            if (employee.address == _name) 
            {
                Debug.Log($"i found this name and the key is {employee.key}");
                break;
            }
            else { Debug.Log($"stopped working"); }
        }

        playerList = new PlayerList();
    }


    public bool CheckIfNameExistsJSON(string _address)
    {
        playerList = JsonUtility.FromJson<PlayerList>(textJSON.text);

        foreach (Player employee in playerList.list)
        {
            if (employee.address == _address)
            {
                playerList = new PlayerList();
                return true;
            }
        }
        playerList = new PlayerList();
        return false;
    }


    public void modifyMusicPref(bool _pref, string _address) 
    {


        playerList = JsonUtility.FromJson<PlayerList>(textJSON.text);
        foreach (Player employee in playerList.list)
        {

            if (employee.address == _address)
            {
                employee.music = _pref;
                break;
            }

            else { Debug.Log($"i didnt find this name"); }
        }

        OutputJson();
    }


    public void AddPlayer(string _address) 
    {
        
        if (!CheckIfNameExistsJSON(_address))
        {

            playerList = JsonUtility.FromJson<PlayerList>(textJSON.text);
            
            Player newRef = new Player();

            newRef.address = _address;
            newRef.key = GenKey();
            newRef.music = true;

            playerList.list.Add(newRef);

            OutputJson();
        }
    }



    


    public string GenKey() 
    {
        string Key = string.Empty;  // create an empty string

        for (int i = 0; i < 20; i++)       // we want it to be 20 big
        {
            int random = Random.Range(0, 36);    // random number from 0 to 36    that is all char and int possible


            if (random < 26)
            {
                int ran = Random.Range(0, 1);


                if (ran == 0) 
                { 
                    Key += (char)(random); 
                }
                else 
                { 
                    Key += (char)(random + 65); 
                }

                  
            }
            else
            {
                Key += (random - 26).ToString();      // if bigger then 26 then its a normal number
            }
        }

        return Key;

    }

    //public void modifyMusicKeyAndMusic(string _name, bool _music)
    //{

    //}

}
