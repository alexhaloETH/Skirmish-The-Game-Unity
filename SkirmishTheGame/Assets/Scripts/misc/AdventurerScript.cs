using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdventurerScript : MonoBehaviour
{

    public string Name = "this is a name";
    public int Stamina;
    public int Defence;
    public int Speed;
    public int IdEquipHead;
    public int IdEquipTorso;
    public int IdEquipArm;
    public int IdEquipLeg;


    [SerializeField] TMP_Text name_text;



    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
        name_text.text = Name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
