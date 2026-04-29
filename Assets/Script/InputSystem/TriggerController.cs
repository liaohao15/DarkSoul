using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    //动画触发器控制器
    //功能：专门用来重置、清理Animator中的Trigger触发器
    //解决：动画未触发时trugger残留，导致下一次误触发的bug

    //角色动画控制器
    private Animator anim;

    //初始化：获取动画组件
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //清理指定名称的trigger
    public void cleanTrigger(string triggername)
    {
        //重置动画触发器，取消未触发的信号
        anim.ResetTrigger(triggername);
    }
}
