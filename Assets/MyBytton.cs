using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton 
{
    public bool IsPressing = false;//正在被按压
    public bool OnPressed = false;//刚刚被按住
    public bool OnReleased = false;//刚刚被释放
    public bool IsExtending = false;//拓展信号

    private bool curstate = false;
    private bool laststate = false;

    private Mytimer extTimer = new Mytimer();

    // 新增：Double Trigger 状态机
    private enum TriggerState
    {
        IDLE,        // 闲置：等待第一次按下
        WAITING,     // 等待中：第一次按下后，计时窗口内
        TRIGGERED    // 触发成功：窗口内第二次按下
    }
    private TriggerState triggerState = TriggerState.IDLE;

    public void Tick(bool input)
    {

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    extTimer.duration = 1.0f;
        //    extTimer.Go();
        //}由startTimer方法代替
        //1.仅推进计时器
        extTimer.Tick();
        
        //2.更新当前按钮状态
        curstate = input;
        IsPressing = curstate;

        //3.重置单次触发的状态
        OnPressed = false;
        OnReleased = false;
        
        //4.判断按钮状态变化
        if (curstate != laststate)
        {
            if (curstate == true)
            {//按钮按下，标记OnPressed,停止计时器
                OnPressed = true;
                HandleDoubleTrigger(); // 按下时处理Double Trigger逻辑
            }
            else
            {//按钮松开，标记OnReleased,启动计时器
                OnReleased = true;
                
            }
        }
        laststate = curstate;


        if (extTimer.state == Mytimer.STATE.FINISHED)
        {
            triggerState = TriggerState.IDLE;
            IsExtending = false;
        }
      
    }

    private void HandleDoubleTrigger()
    {
        switch (triggerState)
        {
            case TriggerState.IDLE:
                // 第一次按下：启动1秒计时，进入等待状态，IsExtending=false
                StartTimer(extTimer, 1.0f);
                triggerState = TriggerState.WAITING;
                IsExtending = false;
                break;

            case TriggerState.WAITING:
                // 第二次按下：IsExtending=true
                triggerState = TriggerState.TRIGGERED;
                IsExtending = true;
                break;

            case TriggerState.TRIGGERED:
                // 已经触发成功
                break;
        }
    }

    private void StartTimer(Mytimer timer,float duration)
    { 
        timer.duration = duration;
        timer.Go();
    }

}
