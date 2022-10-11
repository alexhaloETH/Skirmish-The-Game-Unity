using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;
using Mirror;

public class UIConnectScript : MonoBehaviour
{

    public TMP_Text addressText;
    public TMP_Text balanceText;

    public TMP_InputField testInputField;



    [SerializeField] NetworkManager networkManager;
    [SerializeField] string addressName;


    [DllImport("__Internal")]
    static extern void CheckBalanceERC20();

    [DllImport("__Internal")]
    static extern void ConnectWallet();

    [DllImport("__Internal")]
    static extern void CallAddressUpdate();






    public void Connect()
    {
        ConnectWallet();
    }


    public void SetAddressFelt(string addressFelt)
    {
        PlayerPrefs.SetString("addressFelt", addressFelt);
    }

    public void SetAddress(string address)
    {
        PlayerPrefs.SetString("address", address);
        addressName = GeneralUtilScript.instance.CutHexDown(address);
        addressText.text = "Wallet Address: " + addressName;
        InvokeRepeating("CallBalanceCheck", 0f, 15f);
    }

    public void CallBalanceCheck() 
    {
        CheckBalanceERC20();
        Debug.Log($"CALLING FOR CALL BALANCE");
    }


    public void SetBalance(string balance)
    {
        Debug.Log($"CALLING FOR SET BALANCE");
        if (addressText.text.Contains("0x")) 
        {
            balance = balance.Substring(0, balance.Length - 16);
            balance = balance.Insert(balance.Length - 2,".");

            Debug.Log(balance);
            balanceText.text = "Wallet Balance: " + balance;
        }
    }


    public void JoinLocal()
    {
        //if (PlayerPrefs.GetString("Address").Contains("0x"))
        //{

        //    networkManager.networkAddress = "localhost";
        //    networkManager.StartClient();
        //}



        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
        if (testInputField.text == "") 
        {
            PlayerPrefs.SetString("TestName", addressName);
        }
        else 
        {
            PlayerPrefs.SetString("TestName", testInputField.text);
        }
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


}





// to delete in the future not now
public class obj
{
    public List<string> val;

    public List<string> valee;




    public obj()
    {

    }


    public static obj CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<obj>(jsonString);
    }
}


