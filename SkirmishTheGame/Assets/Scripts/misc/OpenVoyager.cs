using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpenVoyager : MonoBehaviour
{

    public TMP_Text TxText;
    public TMP_Text TxActionText;
    [SerializeField] TMP_Text StatusText;
    [SerializeField] RawImage BackgroundImage;

    

    public void SetTx(string tx) 
    {
        TxText.text = tx;
    }

    public void SetActionTx(string tx)
    {
        TxActionText.text = tx;
    }

    public void SetStatusTx(string txStatus)
    {
        Debug.Log($"is this gettgin called in set status");
        StatusText.text = txStatus;

        if (txStatus == "Approved...") 
        {
            BackgroundImage.color = Color.blue;
        }
        else if (txStatus == "Success") 
        {
            BackgroundImage.color = Color.green;
        }
        else if (txStatus == "Failed") 
        { 
            BackgroundImage.color = Color.red;
        }

    }


    public void OnCLick()
    {
        string firstPart = "https://goerli.voyager.online/tx/";

        string finalURL = firstPart + TxText.text;

        Application.OpenURL(finalURL);


        GeneralUtilScript.instance.SpawnMessagePrefab("opening URL right now ", true);
    }
}
