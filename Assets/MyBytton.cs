using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton 
{
    public bool IsPressing = false;//正在被按压
    public bool OnPressed = false;//刚刚被按住
    public bool OnReleased = false;//刚刚被释放
    public bool IsExtending = false;//拓展信号
    public bool IsDelaying = false;//长按信号

    public float extendingDuration = 0.3f;//拓展持续时间
    public float delayingDuration = 0.2f;//长按持续时间

    private bool curstate = false;
    private bool laststate = false;
    public bool isLongPress =  false;//用来记录按压是不是长按

    private Mytimer extTimer = new Mytimer();
    private Mytimer delayTimer = new Mytimer();

 
    public void Tick(bool input)
    {

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    extTimer.duration = 1.0f;
        //    extTimer.Go();
        //}由startTimer方法代替
        //1.仅推进计时器（两个）
        extTimer.Tick();
        delayTimer.Tick();

        //2.更新当前按钮状态
        curstate = input;
        IsPressing = curstate;

        //3.重置单次触发的状态
        OnPressed = false;
        OnReleased = false;

        // 4. 先更新状态（关键顺序）
        IsDelaying = (delayTimer.state == Mytimer.STATE.FINISHED);
        IsExtending = (extTimer.state == Mytimer.STATE.RUN);



        //5.判断按钮状态变化
        if (curstate != laststate)
        {
            if (curstate == true)
            {//按钮按下，标记OnPressed,停止计时器
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration);
                isLongPress = false;
                extTimer.Stop();
            }
            else
            {//按钮松开，标记OnReleased,启动计时器
                OnReleased = true;
                StartTimer(extTimer, extendingDuration);
                delayTimer.Stop();
            }
        }
        laststate = curstate;


        //if (extTimer.state == Mytimer.STATE.RUN)
        //{ 
        //    IsExtending = true;
        //}
        //if (delayTimer.state == Mytimer.STATE.FINISHED)
        //{ 
        //    IsDelaying = true;
        //}
        // 6. 最后记录长按状态
        if (delayTimer.state == Mytimer.STATE.FINISHED)
        {
            isLongPress = true;
        }


    }
    
    private void StartTimer(Mytimer timer,float duration)
    { 
        timer.duration = duration;
        timer.Go();
    }

}
