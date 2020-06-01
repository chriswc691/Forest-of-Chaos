using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropWeapon_k : MonoBehaviour
{
    public GameObject weapon2;
    int randomDrop;
    

    // Start is called before the first frame update
    void Start()
    {
        randomDrop = Random.Range(0, 4);
        if (SpawnEnemy.dropKartana == 0 && randomDrop == 1)
        {
            Instantiate(weapon2, transform.position, transform.rotation);
            SpawnEnemy.dropKartana = 1;
            ShowDropText.IsDropKartana = true;
        }

    }

}