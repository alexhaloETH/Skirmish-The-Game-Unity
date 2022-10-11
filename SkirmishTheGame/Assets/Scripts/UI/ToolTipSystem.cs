using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTipSystem : MonoBehaviour
{
    [SerializeField] TMP_Text HeaderText;
    [SerializeField] TMP_Text DescText;
    [SerializeField] TMP_Text InfoText;

    [SerializeField] GameObject ToolTipObject;

    public bool ToolTipActive;

    private bool toggle  =false;
    public static ToolTipSystem instance;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;


        ToolTipObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (ToolTipActive) 
        {
            ToolTipObject.transform.position = Input.mousePosition;


            if (Input.GetKeyDown(KeyCode.K))
            {
                if (!toggle)
                {
                    toggle = true;
                    ToolTipObject.GetComponent<Image>().color = new Color(ToolTipObject.GetComponent<Image>().color.r, ToolTipObject.GetComponent<Image>().color.g, ToolTipObject.GetComponent<Image>().color.b, 0.35f);
                    HeaderText.color = new Color(HeaderText.color.r, HeaderText.color.g, HeaderText.color.b, 0.35f);
                    DescText.color = new Color(DescText.color.r, DescText.color.g, DescText.color.b, 0.35f);
                    InfoText.color = new Color(InfoText.color.r, InfoText.color.g, InfoText.color.b, 0.35f);
                }
                else 
                {
                    toggle = false;
                    ToolTipObject.GetComponent<Image>().color = new Color(ToolTipObject.GetComponent<Image>().color.r, ToolTipObject.GetComponent<Image>().color.g, ToolTipObject.GetComponent<Image>().color.b, 1);
                    HeaderText.color = new Color(HeaderText.color.r, HeaderText.color.g, HeaderText.color.b, 1);
                    DescText.color = new Color(DescText.color.r, DescText.color.g, DescText.color.b, 1);
                    InfoText.color = new Color(InfoText.color.r, InfoText.color.g, InfoText.color.b, 1);
                }
            }
        }
    }

    public void ShowToolTip(string _HeaderText, string _DescText) 
    {

        ToolTipActive = true;

        HeaderText.text = _HeaderText;  
        DescText.text = _DescText;  
        

        ToolTipObject.SetActive(true);
    }
    public void HideToolTip() 
    {
        toggle = false;
        ToolTipActive = false;

        ToolTipObject.GetComponent<Image>().color = new Color(ToolTipObject.GetComponent<Image>().color.r, ToolTipObject.GetComponent<Image>().color.g, ToolTipObject.GetComponent<Image>().color.b, 1);
        HeaderText.color = new Color(HeaderText.color.r, HeaderText.color.g, HeaderText.color.b, 1);
        DescText.color = new Color(DescText.color.r, DescText.color.g, DescText.color.b, 1);
        InfoText.color = new Color(InfoText.color.r, InfoText.color.g, InfoText.color.b, 1);

        ToolTipObject.SetActive(false);
    }


}
