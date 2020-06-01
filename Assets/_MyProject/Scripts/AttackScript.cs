using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public Animator anim;
    public AnimatorStateInfo currentBaseState;
    
    static int attack1State = Animator.StringToHash("Base Layer.attack.Attack1");
    static int attack2State = Animator.StringToHash("Base Layer.attack.Attack2");
    static int attack3State = Animator.StringToHash("Base Layer.attack.Attack3");
    static int idleState = Animator.StringToHash("Base Layer.attack.WAIT00");

    
    public void Attack()
    {
        anim = this.gameObject.GetComponent<Animator>();
        if (currentBaseState.fullPathHash == idleState)
        {
            if (!anim.IsInTransition(0))
            {
                anim.SetTrigger("attack1");
                anim.SetInteger("attack", 1);
            }
        }
        else if (currentBaseState.fullPathHash == attack1State && currentBaseState.normalizedTime > 0.5)
        {
            anim.SetInteger("attack", 2);
        }
        else if (currentBaseState.fullPathHash == attack2State && currentBaseState.normalizedTime > 0.5)
        {
            anim.SetInteger("attack", 3);
        }
        else if (currentBaseState.fullPathHash == attack3State)
        {
            print("PL**************************************PL");
        }

    }

}
