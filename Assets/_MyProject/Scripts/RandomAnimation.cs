using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public string[] _AniName = new string[] { "WAIT01", "WAIT02", "WAIT03", "WAIT04" };
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RandomPlay();
        StartCoroutine(load());
        IEnumerator load()
        {
            yield return new WaitForSeconds(3);    //注意等待时间的写法
        }
    }

    void RandomPlay()
    {
        int index = Random.Range(0, _AniName.Length);
        Animator ani = gameObject.GetComponent<Animator>();
        ani.Play(_AniName[index], -1, 0f);
    }
}
