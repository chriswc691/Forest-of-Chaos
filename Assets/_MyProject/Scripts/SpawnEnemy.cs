using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject[] enemies;
    GameObject player;
    public GameObject samurai;
    public GameObject knight;

    private float xPos;
    private float yPos;
    private float zPos;
    int sCount = 0;
    int kCount = 0;
    public static int dropSword = 0;
    public static int dropKartana = 0;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating("spawnSamurai", 150f, 25f);
        InvokeRepeating("spawnKnight", 270f, 40f);
    }

    void Update()
    {
        StartCoroutine(spawnEnemies());
    }


    IEnumerator spawnEnemies()
    {
        while (GameObject.FindGameObjectsWithTag("Enemy").Length < (40 + sCount + kCount))
        {
            int random_objects = Random.Range(0, enemies.Length);
            xPos = Random.Range(player.transform.position.x - 60, player.transform.position.x + 60);
            yPos = player.transform.position.y + 10;
            zPos = Random.Range(player.transform.position.z - 60, player.transform.position.z + 60);

            if (xPos > 10 && zPos > 10 && xPos < 990 && zPos < 990)
            {
                Instantiate(enemies[random_objects], new Vector3(xPos, yPos , zPos), transform.rotation * Quaternion.Euler(0f, 180f, 0f));
                yield return new WaitForSeconds(1f);
                
            }                        
        }               
    }

    void spawnSamurai()
    {
        xPos = Random.Range(player.transform.position.x - 15, player.transform.position.x + 15);
        yPos = player.transform.position.y + 20;
        zPos = Random.Range(player.transform.position.z - 15, player.transform.position.z + 15);

        if (xPos > 10 && zPos > 10 && xPos < 990 && zPos < 990)
        {
            Instantiate(samurai, new Vector3(xPos, yPos, zPos), transform.rotation * Quaternion.Euler(0f, 180f, 0f));
            sCount += 1;            
        }

    }

    void spawnKnight()
    {
        xPos = Random.Range(player.transform.position.x - 10, player.transform.position.x + 10);
        yPos = player.transform.position.y + 20;
        zPos = Random.Range(player.transform.position.z - 10, player.transform.position.z + 10);

        if (xPos > 10 && zPos > 10 && xPos < 990 && zPos < 990)
        {
            Instantiate(knight, new Vector3(xPos, yPos, zPos), transform.rotation * Quaternion.Euler(0f, 180f, 0f));
            kCount += 1;
        }

    }
}
