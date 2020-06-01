using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showAddPoints : MonoBehaviour
{
    public GameObject addText1000;
    public GameObject addText10000;
    public GameObject addText30000;
    public static bool plus1000 = false;
    public static bool plus10000 = false;
    public static bool plus30000 = false;

    void Update()
    {
        if (plus1000 == true)
        {
            StartCoroutine(add1000());
        }
        if (plus10000 == true)
        {
            StartCoroutine(add10000());
        }
        if (plus30000 == true)
        {
            StartCoroutine(add30000());
        }
    }

    IEnumerator add1000()
    {
        addText1000.SetActive(true);
        yield return new WaitForSeconds(2);
        addText1000.SetActive(false);
        plus1000 = false;
        yield break;
    }

    IEnumerator add10000()
    {
        addText10000.SetActive(true);
        yield return new WaitForSeconds(2);
        addText10000.SetActive(false);
        plus10000 = false;
        yield break;
    }

    IEnumerator add30000()
    {
        addText30000.SetActive(true);
        yield return new WaitForSeconds(2);
        addText30000.SetActive(false);
        plus30000 = false;
        yield break;
    }
}
