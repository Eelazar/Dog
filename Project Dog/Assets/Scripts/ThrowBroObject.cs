using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBroObject : BaseObject
{
    public ThrowBroToss toss;

    public void OnCommandToss()
    {
        Debug.Log("TOSS NIGGA");
        //toss.Toss();
    }
}
