using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TroopStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Position
    {
        FRONT = 1,
        MID = 2,
        BACK =3
    }


    public enum TroopType
    {
        INFANTRY = 1,
        CAVALRY,
        MAGE,
        GIANT
    }


    public TroopType TroopID;   
    public string Name = string.Empty;
    public int Vitality;
    public Position Pos;
    public int Ability;
    public RawImage TroopImage;


    [SerializeField] TMP_Text NameText;
    [SerializeField] TMP_Text VitalityText;
    [SerializeField] RawImage BackgroundImage;


    private void Start()
    {
        
        switch (TroopID)
        {
            case TroopType.INFANTRY:
                BackgroundImage.color = Color.yellow;
                break;

            case TroopType.CAVALRY:
                BackgroundImage.color = Color.red;
                break;

            case TroopType.MAGE:
                BackgroundImage.color = Color.blue;
                break;

            case TroopType.GIANT:
                BackgroundImage.color = Color.magenta;
                break;
            default:
                break;
        }

        NameText.text = "Name: "+  Name;

        VitalityText.text ="Vit: "  + Vitality.ToString();
    }



    public int GetTroopType() 
    {
        return (int)TroopID;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
     

        ToolTipSystem.instance.ShowToolTip(Name, "");

    }



    public void OnPointerExit(PointerEventData eventData)
    {
   

        ToolTipSystem.instance.HideToolTip();


    }


}



