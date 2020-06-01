using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class addPoint_K : MonoBehaviour
{

    void Start()
    {
        vHUDController.count = vHUDController.count + 30000;
        showAddPoints.plus30000 = true;
    }

}

