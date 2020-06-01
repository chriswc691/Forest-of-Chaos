using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class dropWeapon : MonoBehaviour
{
    public GameObject weapon;
    
    int randomDrop;

    // Start is called before the first frame update
    void Start()
    {
        randomDrop = Random.Range(0, 4);

        if (SpawnEnemy.dropSword == 0 && randomDrop == 1)
        {
            Instantiate(weapon, transform.position, transform.rotation);
            SpawnEnemy.dropSword = 1;
            ShowDropText.IsDropSword = true;
        }
        
    }

}
