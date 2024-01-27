using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct EyePose
{
    public float width;
    public float height;
}
public class FacialFeatures : MonoBehaviour
{
    public Animator animator;

    private bool _blinking;

    private void Update()
    {
        animator.SetBool("Blinking", _blinking);
    }

    public void Blinking()
    {
        _blinking = true;
        animator.SetBool("Blinking", _blinking);
        animator.SetTrigger("Blink");
    }

    public void Angry()
    {
        _blinking = false;
        animator.SetBool("Blinking", _blinking);

        animator.SetTrigger("Angry");
    }
    public void Shocked()
    {
        _blinking = false;
        animator.SetBool("Blinking", _blinking);

        animator.SetTrigger("Shocked");
    }
    public void Scared()
    {
        _blinking = false;
        animator.SetBool("Blinking", _blinking);

        animator.SetTrigger("Scared");
    }

    
}