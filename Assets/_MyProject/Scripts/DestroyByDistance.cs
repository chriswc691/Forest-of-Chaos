using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByDistance : MonoBehaviour
{
    float enemyDistance;
    private GameObject player;
    


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Destroy(this.gameObject, 240);
    }

    
    void Update()
    {
        enemyDistance = Vector3.Distance(player.transform.position, this.gameObject.transform.position);

        if (enemyDistance > 80)
        {
            
            Destroy(this.gameObject);            

        }
    }

}
