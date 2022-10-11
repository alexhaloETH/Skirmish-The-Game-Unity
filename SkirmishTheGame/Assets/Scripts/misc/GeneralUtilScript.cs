using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GeneralUtilScript : MonoBehaviour
{

    [DllImport("__Internal")]
    static extern void RefreshWebsite();



    public static GeneralUtilScript instance;


    [Header("webhooks for the discord database")]

    [SerializeField] string[] webhookLinks = { 
        "https://discordapp.com/api/webhooks/1022916465520820336/egVXIgZO5FlUDCeUSwG_m-drg6BaHWAVxCZ-WNunrAaHuZf_FJgBf5WGDtiR0VIpq47h", 
        
        "https://discordapp.com/api/webhooks/1022916340463443988/6WuflTI7C4TKGoqEDn7kJtZwgdGDFU-Ab5iYdhY3ZP7Wtmqe_ZWjWayv3ynHA4ttbUsH",
        
        "https://discordapp.com/api/webhooks/1022916100918349967/k14UtTjoqwMZqbMPKPc2TEHym6-7wJsqUE8xBIXX46TQa7ActWQ89E5bxtHIHuYEwhKE" };


    // 0 whitelist
    // 1 helpdesk
    // 2 match



    [Header("Alert message spawn vars")]

    [SerializeField] GameObject UIErrorPrefab;
    [SerializeField] Canvas SceneAlertCanvas;

    private GameObject previousErrorPrefabRef;







    void Start() { instance = this; }

    public void HardRstartGame() { RefreshWebsite(); }




    #region spawn alert message reagion

    public void SpawnMessagePrefab(string MessDesc, bool type)
    {

        if (previousErrorPrefabRef != null)     // its still there
        {
            Destroy(previousErrorPrefabRef);
        }


        GameObject newRef = Instantiate(UIErrorPrefab, SceneAlertCanvas.transform);

        if (type)      // positve is green
        {
            newRef.GetComponent<RawImage>().color = new Color32(0, 200, 0, 100);
            Debug.Log($"calling for green");
        }
        else    // negative is red
        {
            newRef.GetComponent<RawImage>().color = new Color32(200, 0, 0, 100);
        }


        newRef.GetComponentInChildren<TMP_Text>().text = MessDesc;


        previousErrorPrefabRef = newRef;
    }

    public void DeleteError()
    {
        Destroy(previousErrorPrefabRef);
    }

    #endregion



    #region message to discord region


    public void SendMessageToDis(string builtString,int channel)
    {
        StartCoroutine(SendWebhook(webhookLinks[channel], builtString, (success) =>
        {
            if (success)
                SpawnMessagePrefab("Message has been sent succesfully, thank you", true);
            else
                SpawnMessagePrefab("There was an issue sending the weebhook pleas try again later", false);
        }));
    }

    IEnumerator SendWebhook(string link, string message, System.Action<bool> action)
    {
        WWWForm form = new WWWForm();
        form.AddField("content", message);
        using (UnityWebRequest www = UnityWebRequest.Post(link, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);

                action(false);
            }
            else
                action(true);
        }
    }

    #endregion



    public string CutHexDown(string _hex) 
    {
        string concString = string.Empty;

        concString += _hex.Substring(0, 6);
        concString += "...";
        concString += _hex.Substring(_hex.Length - 4, 4);

        return concString;
    }



    public string IdToName(int id) 
    {

        switch (id)
        {

            case 1:

                return "Pikeman";

            case 2:

                return "Knight";

            case 3:

                return "Mage";

            case 4:

                return "Paladin";

            default:
                return "Non-applicable";
        }
    }















}
