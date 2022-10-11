using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thign : MonoBehaviour
{

    //make instance of the turnmanager
    // call the player for now

    public void blabla() 
    {
        Debug.Log($"call on the do something");
        Player.localPlayer.DoSomething();
    }
}
