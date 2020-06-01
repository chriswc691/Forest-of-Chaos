using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class PrintScore : MonoBehaviour
{

    public TextMeshProUGUI first;
    public TextMeshProUGUI second;
    public TextMeshProUGUI third;


    void Update()
    {
        PrintScore1();
        PrintScore2();
        PrintScore3();
    }

    public void PrintScore1()
    {

        if (highscore.scores[2] == 0)
        {
            first.text = " ";
        }
        else
        {
            first.text = highscore.scores[2].ToString();
        }
    }

    public void PrintScore2()
    {

        if (highscore.scores[1] == 0)
        {
            second.text = " ";
        }
        else
        {
            second.text = highscore.scores[1].ToString();
        }
    }

    public void PrintScore3()
    {

        if (highscore.scores[0] == 0)
        {
            third.text = " ";
        }
        else
        {
            third.text = highscore.scores[0].ToString();
        }
    }
    
}
