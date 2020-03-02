using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_behaviour : StateMachineBehaviour
{
    [SerializeField] private string attack; 
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.GetComponent<Character>().IsAttacking = true;
        //animator.GetComponent<Rigidbody>().velocity = Vector3.zero;
        if(animator.GetComponent<Character>())
        animator.GetComponent<Character>().SetDirection();

        //animator.ResetTrigger(attack);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.ResetTrigger(attack);
        //animator.GetComponent<Character>().IsAttacking = false;
        //animator.GetComponent<Character>().AttackCounter = 0;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
