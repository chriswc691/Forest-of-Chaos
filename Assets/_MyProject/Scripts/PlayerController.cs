using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform m_Transform;
    public float speed = 0.1f;
    public Animator anim;

    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame

    void Update()
    {

        MoveControl();

    }

    void MoveControl()

    {

        if (Input.GetKey(KeyCode.W))

        {

            m_Transform.Translate(Vector3.forward * speed, Space.Self);
            //anim.Play("RUN_F", -1, 0f);
        }

        if (Input.GetKey(KeyCode.S))

        {

            m_Transform.Translate(Vector3.back * speed * 0.4f, Space.Self);
            //anim.Play("WALK_B", -1, 0f);
        }

        if (Input.GetKey(KeyCode.A))

        {

            m_Transform.Translate(Vector3.left * speed, Space.Self);
            //anim.Play("RUN_L", -1, 0f);

        }

        if (Input.GetKey(KeyCode.D))

        {

            m_Transform.Translate(Vector3.right * speed, Space.Self);
            //anim.Play("RUN_R", 1-, 0f);

        }



        //m_Transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));

        //m_Transform.Rotate(Vector3.left, Input.GetAxis("Mouse Y"));

    }

}
