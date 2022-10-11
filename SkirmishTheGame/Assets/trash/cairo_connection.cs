using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;




// for the contract i turned the hex into a int adn then into a string
// for the response i get an hex that in need to turn int an int and then string


// this script is pointless and only here as reference unity only support uint64 not uint256 

public class Call_Balance : MonoBehaviour
{
    public string contract_address = "0x0218f28664c5a5d896f97ab4acacfb7c1be8c8d7e272a54a6bc11ac9d859aa36";
    public string entry_point_selector = "0x0157235952bd1fa0ff167ec06a2903e97a71318b3375a39ab92a6b003b15d8cc";
    public string[] signature = new string[0];

    public string[] calldata = new string[2];

    public string CallBalance(string address)
    {   
        calldata[0] = "2318";
        calldata[1] = address;
        return JsonUtility.ToJson(this);
    }
}






[System.Serializable]
public class balanceResponse
{
    public string[] result;
}




public class cairo_connection : MonoBehaviour
{


    private void Start()
    {
        GetBalance();
    }



    public static cairo_connection instance;
    private string URL = "http://alpha4.starknet.io/feeder_gateway/call_contract";





    void GetBalance() 
    {
        Call_Balance to_json = new Call_Balance();
                                                            // player prefs get it from there
        string string_to_json = to_json.CallBalance("1363543499155882648611901807598860215298547236963503308056698514656129492843");

        StartCoroutine(PostData_Coroutine_forUint(URL, string_to_json));
    }

    // should be usefull to get the balance of the coin and then cost of the sns
    IEnumerator PostData_Coroutine_forUint(string url, string bodyJsonString)
    {
        print(bodyJsonString);
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log($"{request.downloadHandler.text}");
        balanceResponse BR = JsonUtility.FromJson<balanceResponse>(request.downloadHandler.text);

        Debug.Log($"{BR.result}");

        Debug.Log($"{request.downloadHandler.text}");
        //UInt32 intValue = Convert.ToUInt32(BR.result[0], 16);
        //Debug.Log("actual message " + BR.result[0]);
        //string to_send = intValue.ToString();


    }
}

