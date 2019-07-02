using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBroObject : BaseObject
{
    public Animator animator;

    public Transform platform;

    public void OnCommandActivate()
    {
        Debug.Log("DO SHIT !");

        animator.SetBool("PickUp", true);

        GameManager.current.player.transform.SetParent(platform, true);
    }
}
