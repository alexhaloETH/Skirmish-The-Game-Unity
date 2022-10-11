using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    private void Start()
    {
        //if (!Application.isBatchMode)  // headless build so just the server, server starts it self
        //{
        //    Debug.Log("Client starting");
        //    networkManager.StartClient();
        //}
        //else
        //{
        //    networkManager.StartHost();
        //    Debug.Log("Server starting");
        //}
    }

    public void JoinLocal()
    { 
        if (PlayerPrefs.GetString("Address").Contains("0x")) 
        {
            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
        }
    }
}
