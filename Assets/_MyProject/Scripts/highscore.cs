using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Invector.vCharacterController;


public class highscore : MonoBehaviour
{
    
    public static int[] scores = new int[3];
    
    public static bool SaveFile = false;
    //public int hasLoad = 0;

    void Start()
    {
        
        LoadScore();
        Debug.Log("Load!  " + scores[0] + "/" + scores[1] + "/" + scores[2]);
        
        
        

    }
    void Update()
    {
             
        //scores = new int[highScoreArray.Length];
        //Array.Sort(scores);

               
        //printArray = new int[scores.Length];
        //Array.Copy(scores, printArray, 3);

    }

    void OnApplicationQuit()
    {

        PlayerPrefs.SetInt("index0", scores[0]);
        PlayerPrefs.SetInt("index1", scores[1]);
        PlayerPrefs.SetInt("index2", scores[2]);
        Debug.Log("Save!   " + scores[0] + " / " + scores[1] + " / " + scores[2]);
    }


    public static void SaveScore()
    {
        //SaveSystem.SaveScore(this);

        PlayerPrefs.SetInt("index0", scores[0]);
        PlayerPrefs.SetInt("index1", scores[1]);
        PlayerPrefs.SetInt("index2", scores[2]);

    }
    public void LoadScore()
    {
        //SaveData data = SaveSystem.LoadScore();
        //Array.Copy(data.highScoresArray, scores, scores.Length);

        scores[0] = PlayerPrefs.GetInt("index0");
        scores[1] = PlayerPrefs.GetInt("index1");
        scores[2] = PlayerPrefs.GetInt("index2");

    }


    public void Reset()
    {
        scores[0] = 0;
        scores[1] = 0;
        scores[2] = 0;
        SaveScore();
        Debug.Log("Save!   " + highscore.scores[0] + " / " + highscore.scores[1] + " / " + highscore.scores[2]);
    }

}
