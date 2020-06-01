
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vAttackTriggerTest : MonoBehaviour {
    [Invector.vButton("Attack","Attack",typeof(vAttackTriggerTest))]
    public Animator animator;
    public AttackSequence[] attackSequences;
    public int minAttackCount;
    public int maxAttackCount;
    private int currentAttackCount;

    // Use this for initialization
    public void Start()
    {
        
       
    }
    public void Attack()
    {
        currentAttackCount = Random.Range(minAttackCount, maxAttackCount);
        StartCoroutine(Attack(attackSequences[0]));
    }
    public IEnumerator Attack(AttackSequence sequence,int index =0)
    {
        var speed = animator.speed;
        
        var time = 0f;

        if (index < sequence.sequence.Length && sequence.sequence.Length > 0)
        {
            currentAttackCount--;
            bool triggerAttack = false;
            animator.CrossFade(sequence.sequence[index].AnimationPlay, sequence.sequence[index].crossFade);
            while (time < sequence.sequence[index].timeToFinish)
            {
                time += Time.deltaTime;
                if (triggerAttack==false && time >= sequence.sequence[index].enableAttackTriggerTime && time < sequence.sequence[index].disableAttackTriggerTime)
                {
                    sequence.sequence[index].onEnableAttackTrigger.Invoke();
                    triggerAttack = true;
                }
                else if(triggerAttack && time >= sequence.sequence[index].disableAttackTriggerTime)
                {
                    sequence.sequence[index].onDisableAttackTrigger.Invoke();
                    triggerAttack = false;
                }
                animator.speed = sequence.sequence[index].animatorSpeed;
               
                yield return null;
            }
            animator.speed = speed;
            if(currentAttackCount>0)
            {
                if ((index + 1) < sequence.sequence.Length) StartCoroutine(Attack(sequence, index + 1));
                else StartCoroutine(Attack(attackSequences[0]));
            }
           
        }
        
    }
    
}
[System.Serializable]
public class AttackSequence
{
    public AttackHandle[] sequence;
}
[System.Serializable]
public class AttackHandle
{
    public string AnimationPlay;
    public float enableAttackTriggerTime;
    public float disableAttackTriggerTime;
    public float timeToFinish;
    public float crossFade = 0.1f;
    public float animatorSpeed;
    public UnityEngine.Events.UnityEvent onEnableAttackTrigger, onDisableAttackTrigger;
}
