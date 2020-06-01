using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDropText : MonoBehaviour
{
    public static bool IsDropSword = false;
    public static bool IsDropKartana = false;

    public Animator dropAnimator;

    void Update()
    {
        if (IsDropSword == true)
        {
            StartCoroutine(swordDrop());
            IsDropSword = false;
        }
        

        if (IsDropKartana == true)
        {
            StartCoroutine(kartanaDrop());
            IsDropKartana = false;
        }
       
    }

    IEnumerator swordDrop()
    {
        dropAnimator.SetTrigger("IsDrop");
        yield return new WaitForSeconds(4);
        dropAnimator.SetTrigger("BackDefult");
        yield break;
    }

    IEnumerator kartanaDrop()
    {
        dropAnimator.SetTrigger("IsDrop");        
        yield return new WaitForSeconds(4);
        dropAnimator.SetTrigger("BackDefult");
        yield break;
    }
}
