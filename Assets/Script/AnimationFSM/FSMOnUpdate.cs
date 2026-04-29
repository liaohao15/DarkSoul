using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnUpdate : StateMachineBehaviour
{
    //动画状态机持续消息脚本
    //功能：在动画播放的每一秒，每一帧，持续向上发信息
    //作用：驱动动画持续期间的逻辑

    //动画播放期间，每帧要发送的消息
    public string[] OnUpdateMessages;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //循环发送所有消息
        foreach (var msg in OnUpdateMessages)
        {
            //向上级物体发送消息，调用对应名称的方法
            animator.SendMessageUpwards(msg);
        }
    }
}