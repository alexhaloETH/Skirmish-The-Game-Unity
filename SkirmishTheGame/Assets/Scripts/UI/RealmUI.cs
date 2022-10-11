using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using TMPro;



public class RealmUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int RealmID;
    public List<int> TroopTypesList = new List<int>();
    public List<int> TroopTypesListCurrentTeam = new List<int>();

    public bool changed = false;

    [SerializeField] RawImage OrderImage;



    public enum Order
    {
        LIGHT  = 0,
        DARK =1,
        CUNNING = 2,
        ANGER= 3
    }




    public Order RealmOrder;

    private void Start()
    {
        OrderImage = this.transform.GetComponent<RawImage>();
        UpdateOrder(0);
    }

    public void TakeTroopData(List<int> troopData) 
    {
        TroopTypesList = troopData;
    }


    public List<int> AskTroopData()
    {
        return TroopTypesList;
    }

    public List<int> AskTeamData()
    {
        Debug.Log($"{TroopTypesListCurrentTeam}");
        return TroopTypesListCurrentTeam;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
     
        ToolTipSystem.instance.ShowToolTip("Realm", RealmID.ToString());

    }



    public void OnPointerExit(PointerEventData eventData)
    {
      
        ToolTipSystem.instance.HideToolTip();


    }

    public void UpdateOrder(int order) 
    {

        switch (order)
        {

            case 0:
                RealmOrder = Order.LIGHT;
                OrderImage.color = Color.white;
                break;

            case 1:

                RealmOrder = Order.DARK;
                OrderImage.color = Color.black;
                break;

            case 2:

                RealmOrder = Order.CUNNING;
                OrderImage.color = Color.yellow;
                break;

            case 3:
                RealmOrder = Order.ANGER;
                OrderImage.color = Color.red;
                break;


            default:
                break;
        }
    }
}
