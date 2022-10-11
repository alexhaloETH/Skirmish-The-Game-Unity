using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDel : MonoBehaviour
{


    private void Start()
    {
        this.GetComponent<Animator>().SetTrigger("Call");
    }

    public void Del() { Destroy(this.gameObject); }

    
}
