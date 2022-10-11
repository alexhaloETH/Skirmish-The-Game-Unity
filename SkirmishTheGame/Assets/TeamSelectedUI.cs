using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TeamSelectedUI : MonoBehaviour
{
    [SerializeField] TMP_Text realmIdText;
    [SerializeField] TMP_Text TroopsList;

    public void PopRealmID(string _Id) 
    {
        realmIdText.text = "Realm Id:" + _Id;
    }


    public void PopTroops(List<int> troops)
    {
        string concString = string.Empty;

        concString += "Troops Selected: \n";


        foreach (var item in troops)
        {
            concString += "Id: " + GeneralUtilScript.instance.IdToName(item) + " \n";
        }


        TroopsList.text = concString;
    }



    // the issue is that noew only one spanws 


    
}
