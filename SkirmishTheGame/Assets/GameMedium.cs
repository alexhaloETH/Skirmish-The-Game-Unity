using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GameMedium : MonoBehaviour
{



    // for next time:  the issue is that for some reason the children are not dleteing on the black end
    // the ui is not really updateing 





    public static GameMedium Instance;

    [SerializeField] GameObject EnemyPlayerField;
    [SerializeField] GameObject EnemyPlayerZone;
    [SerializeField] GameObject SelfPlayerField;
    [SerializeField] GameObject SelfPlayerZone;

    [SerializeField] List<GameObject> AllUnitCardsPrefabsArray;  // this is  the already main array that holds the prefabs
    [SerializeField] GameObject BackCard;

    public List <GameObject> PlayerCardsInHand;
    public List <GameObject> PlayerCardsInField;

    public List <GameObject> EnemyCardsInField;

    [SerializeField] Slider PowerSlider;
    [SerializeField] TMP_Text SelfCurrentPowerText;
    [SerializeField] TMP_Text EnemyCurrentPowerText;

    [SerializeField] GameObject OpponentObject;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Player(Clone)");

        foreach (var item in objects)
        {
            Debug.Log($"in the loop");
            if (item.GetComponent<Player>().playerName == Player.localPlayer.playerName) { }
            else { OpponentObject = item; }
        }

    }

  
    public void CallStartGame()
    {
        Player.localPlayer.CallStartGame();
    }

    public void CallDealing()
    {
        Player.localPlayer.DealCards();
    }


    






    public void PopEnemyZone(int enemyCardsInHand) 
    {
        for (int i = 0; i < enemyCardsInHand; i++)
        {
            Instantiate(BackCard, EnemyPlayerZone.transform);
        }
    }



    public void DecreaseEnemyZone(int decreaseAmount)
    {
        for (int i = 0; i < decreaseAmount; i++)
        {
            Destroy(EnemyPlayerZone.transform.GetChild(0).transform);
        }
    }



    public void PopSelfZone(List<int> selectedTeam) 
    {
        foreach (int troopId in selectedTeam) 
        {
            PlayerCardsInHand.Add(Instantiate(AllUnitCardsPrefabsArray[troopId], SelfPlayerZone.transform));
        }
    }










    public void PopEnemyField(List<CompressTroopData> enemyData)
    {

        Debug.Log($"called in the enemyfield");
        EnemyCardsInField.Clear();
   
        foreach (Transform child in EnemyPlayerField.transform)
        {
            Debug.Log($"called in the enemy field first loop");
            GameObject.Destroy(child.gameObject);
        }


        foreach (var item in enemyData)
        {
            Debug.Log($"called in the enemy field seodn daddadadadaadd loop");
            GameObject savedRef = Instantiate(AllUnitCardsPrefabsArray[item.type], EnemyPlayerField.transform);
            EnemyCardsInField.Add(savedRef);
        }
        DecreaseEnemyZone(1);
        SetPowerUI();
    }











    public void SetPowerUI() 
    {
        // this will be called whenever the users place a new card or osmthing happens
      
        int enemyPower = 0;
        int selfPower = 0;

        foreach (GameObject card in EnemyCardsInField)
        {
           enemyPower =+ card.transform.GetComponent<GameTroopsBehaviour>().Vitality;
        }

        foreach (GameObject card in PlayerCardsInField)
        {
            selfPower =+ card.transform.GetComponent<GameTroopsBehaviour>().Vitality;
        }


        EnemyCurrentPowerText.text = $"curr power {enemyPower}";
        SelfCurrentPowerText.text = $"curr power {selfPower}";

        PowerSlider.maxValue = enemyPower + selfPower;
        PowerSlider.value = selfPower;


    }



    public void PlayCard(GameObject playedCard) 
    {
        playedCard.transform.SetParent(SelfPlayerField.transform);

        PlayerCardsInField.Add(playedCard);
        SetPowerUI();

        Player.localPlayer.PlayCard((int)playedCard.GetComponent<GameTroopsBehaviour>().TroopID, playedCard.GetComponent<GameTroopsBehaviour>().Vitality, OpponentObject) ;

    }


}
