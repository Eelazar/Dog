using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;

    public AudioSource audio;

    bool idle;

    public void SetIdle(bool value)
    {
        animator.SetBool("Idle", value);

        if (!value && value != idle)
            audio.Play();
        else if (value && value != idle)
            audio.Stop();

        idle = value;
    }

    public void SetLeft(bool value)
    {
        animator.SetBool("Left", value);
    }

    public void SetRight(bool value)
    {
        animator.SetBool("Right", value);
    }

    public void Reset()
    {
        animator.SetBool("Left", false);
        animator.SetBool("Right", false);
    }
}
