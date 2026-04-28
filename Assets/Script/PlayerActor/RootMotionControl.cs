using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionControl : MonoBehaviour
{
    //动画根运动控制器
    //功能：提取动画自带的位移数据，发送给角色控制器
    //动画控制器
    private Animator anim;

    //初始化：获取动画组件
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //根运动专用方法：动画产生位移时自动执行
    void OnAnimatorMove()
    {
        // 把动画的位移数据，向上发送给角色控制器
        SendMessageUpwards("OnUpdateRM",(object)anim.deltaPosition);
    }
}
