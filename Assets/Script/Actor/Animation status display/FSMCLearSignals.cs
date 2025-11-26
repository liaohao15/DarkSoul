using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMCLearSignals : StateMachineBehaviour
{
    //          =======================       定义两组数组      ==============================
    public string[] clearEnterSignals; 
    public string[] clearExitSignals;


    //          =======================       进入状态执行一次       ==============================
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var signal in clearEnterSignals)
        {
            animator.ResetTrigger("jump");
        }
    }

    //          =======================       在状态里执行60次/s       ==============================
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //          =======================       离开状态执行一次       ==============================
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var signal in clearExitSignals)//foreach循环把数组里的每一个信号，逐个拿出来进行清空
        {
            animator.ResetTrigger("jump");
        }
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
