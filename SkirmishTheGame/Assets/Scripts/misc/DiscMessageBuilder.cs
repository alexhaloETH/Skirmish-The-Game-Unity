using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiscMessageBuilder : MonoBehaviour
{

    [SerializeField] TMP_InputField discordIDInputField;
    [SerializeField] TMP_InputField BrowserInputField;
    [SerializeField] TMP_InputField issueInputField;


   public void BuildAndSendMessage() 
   {
        if (discordIDInputField.text.Contains("#") && BrowserInputField.text != "" && issueInputField.text != "")   // something is valid at least
        {
            string buildString = string.Empty;


            buildString = buildString + "Time: " + System.DateTime.Now.ToString() + "\n\n";

            buildString = buildString + "Address: " + PlayerPrefs.GetString("TestName") + "\n\n";

            buildString = buildString + "DiscordID: " + discordIDInputField.text + "\n\n";

            buildString = buildString + "Browser used: " + BrowserInputField.text + "\n\n";

            buildString = buildString + "Issue Desc: " + issueInputField.text + "\n\n";

            GeneralUtilScript.instance.SendMessageToDis(buildString,1);

            discordIDInputField.text = "";
            issueInputField.text = "";
            BrowserInputField.text = "";

            GeneralUtilScript.instance.SpawnMessagePrefab("Message has been sent", true);
        }
        else 
        {
            GeneralUtilScript.instance.SpawnMessagePrefab("There seems to be some data missing", false);
        }

   }
}
