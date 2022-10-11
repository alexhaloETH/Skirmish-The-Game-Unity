using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class DeckViewLogic : MonoBehaviour, IPointerClickHandler
{

    public class ChangedRealms 
    {
        public string RealmId;
        public string data;
        public List<int> dataIds;
    }


    public static DeckViewLogic instance;

    /*
     * in the actual game there will be a function that checks if for example realm 2 had a new set of troops in its deck therefore needs a new transaction sent
     * if not then no need to block the game
     * 
     * 
     *   
     */
    [Header("Multicall stuff")]
    public List<ChangedRealms> StoredChangesRealms = new List<ChangedRealms>();
    public bool loading = false;
    public GameObject selectedTeamsPrefab;
    public Transform scrollViewSelectedTeams;


    [Header("For the realms/top view")]
    public GameObject UIRealms;
    public Transform RealmsViewTransform;
    public TMP_InputField RealmIdInputField;

    [SerializeField] GameObject savedRealmObject;

    public List<GameObject> listOfRealms = new List<GameObject>();





    [Header("For the troops/middle view")]

    public GameObject UIAdventurer;

    public GameObject UITroopInfantry;
    public GameObject UITroopMage;
    public GameObject UITroopCavalry;
    public GameObject UITroopGiant;

    public Transform TroopsViewTransform;
    [SerializeField] bool view = false;     // false is show cards      true is showadve  to turn private

    public List<GameObject> listOfTroops = new List<GameObject>();
    public List<GameObject> listOfAdventurers = new List<GameObject>();

    [SerializeField] TMP_Dropdown DropDownOrder;
    [SerializeField] TMP_Dropdown DropDownSort;

    [Header("For the deck/ bottom view")]
    public Transform DeckViewTransform;

    public List<GameObject> listOfDeck = new List<GameObject>();

    private bool toggle = false;


    //[SerializeField] string[] IdsToBits = { "100", "010", "110", "001" };
    // on click on a realm waht i can do is call the view function

    [DllImport("__Internal")]
    private static extern void RequestSquadData(string squadData);

    [DllImport("__Internal")]
    private static extern void SendSquadData(string squadData);





    void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        // this is here only till the data from cairo is received
            TestRando();
        
    }

 

    // also this
    public void TestRando()
    {
        GameObject newRefd = Instantiate(UIAdventurer, TroopsViewTransform);
        newRefd.SetActive(false);
        listOfAdventurers.Add(newRefd);





        GameObject newRef = Instantiate(UIRealms, RealmsViewTransform);

        listOfRealms.Add(newRef);


        List<int> test = new List<int>();



        test.Add(1);
        test.Add(1);
        test.Add(1);
        test.Add(1);
        test.Add(2);
        test.Add(2);
        test.Add(2);
        test.Add(2);
        test.Add(2);
        test.Add(3);
        test.Add(3);
        test.Add(3);
        test.Add(3);




        newRef.GetComponent<RealmUI>().TakeTroopData(test);
        newRef.GetComponent<RealmUI>().RealmID = 10;









        GameObject newRefdc = Instantiate(UIRealms, RealmsViewTransform);

        listOfRealms.Add(newRefdc);


        List<int> testdc = new List<int>();



        testdc.Add(1);
        testdc.Add(1);
        testdc.Add(3);
        testdc.Add(3);
        testdc.Add(3);
        testdc.Add(3);
        testdc.Add(3);
        testdc.Add(2);
        testdc.Add(1);
        testdc.Add(3);
        testdc.Add(2);




        newRefdc.GetComponent<RealmUI>().TakeTroopData(testdc);
        newRefdc.GetComponent<RealmUI>().RealmID = 20;









        GameObject newRefdbc = Instantiate(UIRealms, RealmsViewTransform);

        listOfRealms.Add(newRefdc);


        List<int> testdbc = new List<int>();



        testdbc.Add(1);
        testdbc.Add(1);
        testdbc.Add(1);
        testdbc.Add(1);
        testdbc.Add(3);
        testdbc.Add(3);
        testdbc.Add(3);
        testdbc.Add(3);
        testdbc.Add(3);
        testdbc.Add(3);
        testdbc.Add(3);
        testdbc.Add(3);
        testdbc.Add(3);
        testdbc.Add(3);
        testdbc.Add(2);
        testdbc.Add(2);
        testdbc.Add(1);
        testdbc.Add(3);
        testdbc.Add(2);
        testdbc.Add(2);




        newRefdbc.GetComponent<RealmUI>().TakeTroopData(testdbc);
        newRefdbc.GetComponent<RealmUI>().RealmID = 30;

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return)) 
        {
            int ID;

            if (int.TryParse(RealmIdInputField.text, out ID))
            {
                if (ID > 0 && ID <= 8000)
                SearchRealmID(ID);
            }
            else
            {
                GeneralUtilScript.instance.SpawnMessagePrefab("this is not a valid number", false);
            }
        }
    }

    public void SearchRealmID(int ID) 
    {
        foreach (GameObject realms in listOfRealms)
        {
            if (realms.GetComponent<RealmUI>().RealmID == ID) 
            {

                ClearCurrentCards();
                savedRealmObject = realms;
                SoftResetSelections();

                LoadRealmTroops(savedRealmObject.GetComponentInParent<RealmUI>().TroopTypesList);
                LoadRealmTeam(savedRealmObject.GetComponentInParent<RealmUI>().TroopTypesListCurrentTeam);

                break;
            }
        }

    }



    public void RequestDataJs(string key) 
    {
        string concString = savedRealmObject.GetComponentInParent<RealmUI>().RealmID.ToString() + "/" + key;
        Debug.Log($"in the request data and asking for this {concString}");
        RequestSquadData(concString);
    }

    public void SaveDataForTransfer() 
    {


        ChangedRealms changedDataRealm = new ChangedRealms();

        string concString = string.Empty;

        List<int> troopData =  savedRealmObject.GetComponentInParent<RealmUI>().TroopTypesListCurrentTeam;

        for (int i = 0; i < troopData.Count; i++)
        {
            //changedDataRealm.dataIds.Add(troopData[i]);
            Debug.Log($"this is an ID added {troopData[i]}");
            concString += troopData[i].ToString();

            if (troopData.Count - 1 == i) 
            {
                concString += "#";
            }
            else 
            {
                concString += "-";
            }
        }


        changedDataRealm.RealmId = savedRealmObject.GetComponentInParent<RealmUI>().RealmID.ToString();
        changedDataRealm.data = concString;
        changedDataRealm.dataIds = troopData;
        Debug.Log($"i saved this for now {concString}");

        StoredChangesRealms.Add(changedDataRealm);



    }




    public void BuildConfirmMenu() 
    {
        foreach (var item in StoredChangesRealms)
        { 
            GameObject newRef =  Instantiate(selectedTeamsPrefab, scrollViewSelectedTeams);

            Debug.Log($"{item.RealmId}");
            Debug.Log($"{item.dataIds}");
            Debug.Log($"{item.data}");
            newRef.transform.GetComponent<TeamSelectedUI>().PopRealmID(item.RealmId);
            newRef.transform.GetComponent<TeamSelectedUI>().PopTroops(item.dataIds);
        
        }
    }


    public void CallForSendData()
    {
        Player.localPlayer.CallForDatac();
    }




    private void OnGUI()
    {
        if (loading)
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "loading assets");
    }



    public void CallForRecieve(string troopData) 
    {
        string[] splitArray = troopData.Split(char.Parse("_"));
        string realmID = splitArray[1];
        string[] data = splitArray[0].Split(char.Parse(","));
        List<int> troopDB = new List<int>();
        List<int> AvailableTroops = savedRealmObject.GetComponentInParent<RealmUI>().TroopTypesList;

        bool fullSquad = true;

        Debug.Log($"does it get here {troopData}");

        if (realmID == savedRealmObject.GetComponentInParent<RealmUI>().RealmID.ToString()) 
        {//the troopdb not getting filled
            foreach (var item in data)
            {
                int ID;
                int.TryParse(item, out ID);

                if (ID == 0) { break; }
             

                if (AvailableTroops.Contains(ID)) 
                {
                    AvailableTroops.RemoveAt(AvailableTroops.IndexOf(ID));
                    Debug.Log($"called in the loop");
                    
                }
                else 
                {
                    fullSquad = false;
                }
                troopDB.Add(ID);


            }


           
        }
        Debug.Log($"{AvailableTroops.Count} and {troopDB.Count}");
        LoadRealmTroops(AvailableTroops);
        LoadRealmTeam(troopDB);

        loading = false;

    }


    public void BuildSendData(string key) 
    {

        if (StoredChangesRealms.Count <= 0) { return; }
        string concString = string.Empty;

        foreach (var item in StoredChangesRealms)
        {
            concString += item.data;
            concString += item.RealmId;
            concString += "/";
        }

        concString += key;

        Debug.Log($"{concString} this is from unity and this is for the buildsend data all" );

        SendSquadData(concString);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject e = eventData.pointerCurrentRaycast.gameObject;

        switch (e.name)
        {
            case "ForClickRealm":
                if (savedRealmObject == e.transform.parent.gameObject)
                {
                    Debug.Log($"same");
                }
                else 
                {
                    Debug.Log($"different");
                    //when this is clicked, call the view function of this realm
                    ClearCurrentCards();
                    savedRealmObject = e.transform.parent.gameObject;
                    SoftResetSelections();



                    Player.localPlayer.CallForData();
                    //RequestDataJs(savedRealmObject.GetComponentInParent<RealmUI>().RealmID.ToString());
                    loading = true;
                    //LoadRealmTroops(savedRealmObject.GetComponentInParent<RealmUI>().TroopTypesList);
                    //LoadRealmTeam(savedRealmObject.GetComponentInParent<RealmUI>().TroopTypesListCurrentTeam);
                    
                }
               

                break;

            case "ForClickTroop":
                //Debug.Log($"clickded on a troop");
                //chck the parent to see ifin the deck or in the select

                GameObject newRef = e.transform.parent.gameObject;

                newRef.transform.SetParent(DeckViewTransform);

                listOfTroops.RemoveAt(listOfTroops.IndexOf(newRef));

                listOfDeck.Add(newRef);


                int typeTroop = e.transform.parent.gameObject.GetComponent<TroopStats>().GetTroopType();
                //Debug.Log($"{typeTroop}");
                for (int i = 0; i < savedRealmObject.GetComponent<RealmUI>().TroopTypesList.Count; i++)
                {
                    if (savedRealmObject.GetComponent<RealmUI>().TroopTypesList[i] == typeTroop) 
                    {
                        savedRealmObject.GetComponent<RealmUI>().TroopTypesList.RemoveAt(i);
                        break;
                    }
                }

                savedRealmObject.GetComponent<RealmUI>().TroopTypesListCurrentTeam.Add(typeTroop);

                

                break;

            case "ForClickAdventurer":

                break;

            case "ForClickSomething":

                break;
        }
    }


    public void HardResetSelections() 
    {
        DropDownOrder.value = 0;
        DropDownSort.value = 3;

        savedRealmObject = null;
        listOfAdventurers.Clear();
        listOfTroops.Clear();
        listOfDeck.Clear();
        listOfRealms.Clear();
    }


    public void SoftResetSelections()
    {
        DropDownOrder.value = (int)savedRealmObject.GetComponent<RealmUI>().RealmOrder;
        DropDownSort.value = 0;

        listOfTroops.Clear();
        listOfDeck.Clear();
    }


    public List<Transform> GetAllChildren()
    {

        List<Transform> children = new List<Transform>();

        foreach (Transform child in TroopsViewTransform)
        {
            children.Add(child);
        }

        return children;
    }

    
    public void LoadRealmTroops(List<int> troopData)
    {
        if (toggle)
        {
            toggle = true;

            foreach (GameObject card in listOfAdventurers) { card.SetActive(false); }
        }


        foreach (int type in troopData)
        {
            GameObject newRef;

            switch (type)
            {
                case 1:
                    newRef = Instantiate(UITroopInfantry, TroopsViewTransform);
                    listOfTroops.Add(newRef);
                    break;

                case 2:

                    newRef = Instantiate(UITroopCavalry, TroopsViewTransform);
                    listOfTroops.Add(newRef);
                    break;

                case 3:

                    newRef = Instantiate(UITroopMage, TroopsViewTransform);
                    listOfTroops.Add(newRef);
                    break;

                case 4:

                    newRef = Instantiate(UITroopGiant, TroopsViewTransform);
                    listOfTroops.Add(newRef);
                    break;

                default:
                    GeneralUtilScript.instance.SpawnMessagePrefab("There seems to be an issues with the LoadRealmTroop function on DeckViewLogic", false);
                    break;
            }

        }
    }


    public void LoadRealmTeam(List<int> troopData)
    {

        foreach (int type in troopData)
        {

            GameObject newRef;

            switch (type)
            {
                case 1:
                    newRef = Instantiate(UITroopInfantry, DeckViewTransform);
                    listOfDeck.Add(newRef);
                    break;

                case 2:

                    newRef = Instantiate(UITroopCavalry, DeckViewTransform);
                    listOfDeck.Add(newRef);
                    break;

                case 3:

                    newRef = Instantiate(UITroopMage, DeckViewTransform);
                    listOfDeck.Add(newRef);
                    break;

                case 4:

                    newRef = Instantiate(UITroopGiant, DeckViewTransform);
                    listOfDeck.Add(newRef);
                    break;

                default:
                    GeneralUtilScript.instance.SpawnMessagePrefab("There seems to be an issues with the LoadRealmDeck function on DeckViewLogic", false);
                    break;
            }

        }
    }

    


    // this shouldnt be clear all acard or maybe just have a func that is different for the slection 
    public void ClearCurrentCards()
    {
        foreach (Transform child in TroopsViewTransform)
        {
            if (child.GetComponent<TroopStats>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Transform child in DeckViewTransform)
        {
            Destroy(child.gameObject);
        }
    }


    public void Switch()
    {
        if (!toggle)
        {
            foreach (GameObject card in listOfTroops) { card.SetActive(false); }

            foreach (GameObject card in listOfAdventurers) { card.SetActive(true); }
        }
        else
        {
            foreach (GameObject card in listOfTroops) { card.SetActive(true); }

            foreach (GameObject card in listOfAdventurers) { card.SetActive(false); }
        }

        toggle = !toggle;
    }



    public void OnValueChangeOrder(int index) 
    {
        switch (index)
        {
            case 0:
                savedRealmObject.GetComponent<RealmUI>().UpdateOrder(index);
                break;

            case 1:

                savedRealmObject.GetComponent<RealmUI>().UpdateOrder(index);
                break;

            case 2:

                savedRealmObject.GetComponent<RealmUI>().UpdateOrder(index);
                break;

            case 3:

                savedRealmObject.GetComponent<RealmUI>().UpdateOrder(index);
                break;

            default:
                break;
        }

    }


    public void OnValueChangeSorting(int index)
    {
        switch (index)
        {
            case 0:
                LoadAll();
                break;

            case 1:

                LoadPerPos(1);

                break;

            case 2:


                LoadPerPos(2);
                break;

            case 3:


                LoadPerPos(3);
                break;

            default:
                break;
        }
    }


    #region Sorting functions

    public void LoadPerPos(int pos)
    {
        foreach (Transform troop in GetAllChildren())
        {
            TroopStats stat = troop.GetComponent<TroopStats>();
            if (((int)stat.Pos) == pos)
            {
                troop.gameObject.SetActive(true);
            }
            else
            {
                troop.gameObject.SetActive(false);
            }
        }
    }

    public void LoadAll()
    {
        foreach (Transform troop in GetAllChildren())
        {
            troop.gameObject.SetActive(true);
        }
    }

    public void sortBestToWorst()
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in TroopsViewTransform)
        {
            if (child.gameObject.activeSelf)
            {
                children.Add(child);
            }
        }

        children = children.OrderBy(w => w.GetComponent<TroopStats>().Vitality).ToList();

        children.Reverse();
        for (int i = 0; i < children.Count; i++)
        {

            children[i].transform.SetSiblingIndex(i);
        }
    }

    public void sortWorstToBest()
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in TroopsViewTransform)
        {
            if (child.gameObject.activeSelf)
            {
                children.Add(child);
            }
        }

        children = children.OrderBy(w => w.GetComponent<TroopStats>().Vitality).ToList();

        for (int i = 0; i < children.Count; i++)
        {

            children[i].transform.SetSiblingIndex(i);
        }

    }

    #endregion




    public void ConfirmSelectedTeam() 
    {

        Player.localPlayer.ConfirmTeam(savedRealmObject.gameObject.GetComponent<RealmUI>());
       

    }


    // this si what we use to ask for data
    public void AskForNFTData() { }





}
