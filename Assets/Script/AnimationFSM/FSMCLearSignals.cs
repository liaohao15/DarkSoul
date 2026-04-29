using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMCLearSignals : StateMachineBehaviour
{
    //进入状态时要清空的触发器
    public string[] ClearEnterSignals; 

    //离开状态时要清空的触发器
    public string[] ClearExitSignals;

    //==   进入状态执行一次     
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //循环清空所有进入时需要清理的触发器
        foreach (var signal in ClearEnterSignals)
        {
            animator.ResetTrigger(signal);//清空
        }
    }

    //== 离开状态执行一次   
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //循环清空所有离开时需要清理的触发器
        foreach (var signal in ClearExitSignals)//foreach循环把数组里的每一个信号，逐个拿出来进行清空
        {
            animator.ResetTrigger(signal);
        }
    }
}
