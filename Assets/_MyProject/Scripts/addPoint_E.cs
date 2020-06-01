using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class addPoint_E : MonoBehaviour
{

    void Start()
    {
        vHUDController.count = vHUDController.count + 1000;
        showAddPoints.plus1000 = true;
    }

}

