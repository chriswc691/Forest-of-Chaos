using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFallEmeny : MonoBehaviour
{
    public GameObject enemy;
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider sk)
    {
        if (sk.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }
}
