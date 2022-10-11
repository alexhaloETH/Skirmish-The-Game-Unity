using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TXViewHandler : MonoBehaviour
{

    //has ana array of size ten
    // the arrays are of the prefab button tx 


    // this will be called from js once called populate one button and deploy it


    [SerializeField] GameObject TXPrefabUI;
    [SerializeField] Transform contentView;

    public void AddTX(string Txconc) 
    {
        string[] tx = Txconc.Split(' ');

        if (contentView.childCount >= 10) 
        {
            Destroy(contentView.transform.GetChild(contentView.childCount - 1).gameObject);
        }

        GameObject newRef = Instantiate(TXPrefabUI, contentView);
        newRef.GetComponentInChildren<OpenVoyager>().SetTx(tx[0]);
        newRef.GetComponentInChildren<OpenVoyager>().SetActionTx(tx[1]);
        newRef.GetComponentInChildren<OpenVoyager>().SetStatusTx("Approved...");
        
        newRef.transform.SetSiblingIndex(0);
    }




    public void UpdateTXStatus(string tx_conc) 
    {
        string[] tx = tx_conc.Split(' ');
       
        foreach (Transform child in contentView)
        {
            if (child.GetComponent<OpenVoyager>().TxText.text == tx[0])
            {
                child.GetComponent<OpenVoyager>().SetStatusTx(tx[1]);

                if (tx[1] == "Success") { GeneralUtilScript.instance.SpawnMessagePrefab("Your tx " + tx[0] + " has succeded", true); }
                else if (tx[1] == "Failed") { GeneralUtilScript.instance.SpawnMessagePrefab("Your tx " + tx[0] + " has failed", false); }

                break;
            }
        }
    }
}
