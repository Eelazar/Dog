using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private AudioSource audio_Source;

    bool idle;

    private void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        audio_Source = transform.GetComponent<AudioSource>();
    }

    public void SetIdle(bool value)
    {
        if(audio_Source != null)
        {
            animator.SetBool("Idle", value);

            if (!value && value != idle)
                audio_Source.Play();
            else if (value && value != idle)
                audio_Source.Stop();

            idle = value;
        }
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
