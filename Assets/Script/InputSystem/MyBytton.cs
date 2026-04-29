using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton 
{
    [Header("按键核心状态")]
    public bool IsPressing = false;//按键正在按住
    public bool OnPressed = false;//按键刚刚按下
    public bool OnReleased = false;//按键刚刚松开
    public bool IsExtending = false;//松开后拓展状态
    public bool IsDelaying = false;//长按延时完成状态

    [Header("计时器时长")]
    public float ExtendingDuration = 0.3f;//释放后持续时间
    public float DelayingDuration = 0.2f;//长按判定时间

    //按键状态缓存
    private bool Curstate = false;//当前帧状态
    private bool Laststate = false;//上一帧状态
    public bool IsLongPress =  false;//是否为长按长按

    //自定义及时器（区分长按/短按）
    private Mytimer ExtTimer = new Mytimer();
    private Mytimer DelayTimer = new Mytimer();
    

    public void Tick(bool input)
    {
        //1.仅推进计时器（两个）
        ExtTimer.Tick();
        DelayTimer.Tick();

        //2.更新当前按钮状态
        Curstate = input;
        IsPressing = Curstate;

        //3.重置瞬时的状态
        OnPressed = false;
        OnReleased = false;

        // 4.更新延时/拓展状态
        IsDelaying = (DelayTimer.state == Mytimer.STATE.FINISHED);
        IsExtending = (ExtTimer.state == Mytimer.STATE.RUN);

        //5.检测按键状态变化
        if (Curstate != Laststate)
        {
            if (Curstate == true)
            {
                //按钮按下：触发按下事件，启动长计时器
                OnPressed = true;
                StartTimer(DelayTimer, DelayingDuration);
                IsLongPress = false;
                ExtTimer.Stop();
            }
            else
            {
                //按钮松开：触发释放事件，启动拓展计时器
                OnReleased = true;
                StartTimer(ExtTimer, ExtendingDuration);
                DelayTimer.Stop();
            }
        }
        //保存当前状态，用于下一帧对比
        Laststate = Curstate;

        // 6. 长按判定：计时器完成则标记为长按
        if (DelayTimer.state == Mytimer.STATE.FINISHED)
        {
            IsLongPress = true;
        }
    }
    
    //启动计时器
    private void StartTimer(Mytimer timer,float duration)
    { 
        timer.duration = duration;
        timer.Go();
    }

}
