using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnEnter : StateMachineBehaviour
{
    //动画状态机消息脚本
    //功能：进入动画状态时，向上发送自定义消息，调用角色控制器的方法
    //作用：让动画和游戏逻辑精准同步

    //进入动画状态时，要发送的消息数组
    public string[] OnEnterMessages;

    //进入动画状态时自动执行
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //循环发送所有的消息
        foreach (var msg in OnEnterMessages)
        {
            //向上级物体发送消息，调用对应名称的方法
            animator.gameObject.SendMessageUpwards(msg);
        }
    }
}
