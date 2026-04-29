using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnExit : StateMachineBehaviour
{
    //动画状态机退出消息脚本
    //功能：离开动画状态时，向上发送自定义消息，调用角色控制器的方法
    //作用：动画结束后自动执行收尾逻辑

    //离开动画状态时，要发送的消息数组
    public string[] OnExitMessages;

    //离开动画状态时自动执行一次
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //循环发送所有消息
        foreach (var msg in OnExitMessages)
        {
            //向上级发送消息，调用对应的方法
            animator.gameObject.SendMessageUpwards(msg);
        }
    }
}
