using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class addPoint_S : MonoBehaviour
{
    
    void Start()
    {
        vHUDController.count = vHUDController.count + 10000;
        showAddPoints.plus10000 = true;
    }

}
