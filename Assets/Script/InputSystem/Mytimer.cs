using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Mytimer
{
    //自定义计时器
    //作用：专门用来计时（长按、冷却、延迟、技能间隔）

    //计时器三种状态
    public enum STATE
    { 
        IDLE,//空闲：未开始
        RUN,//运行中：正在计时
        FINISHED//完成：时间到了
    }

    public STATE state;//当前状态
    public float duration = 1.0f;//计算总时间
    private float elapsedTime = 0;//已经流逝的时间

    //每帧调用，让时间往前走
    public void Tick()
    {
        if (state == STATE.IDLE)//闲置状态，啥也不用做
        {
            //空闲不做处理
        }
        else if (state == STATE.RUN)
        {
            //时间累加
            elapsedTime += Time.deltaTime;

            //时间到→状态变为完成
            if (elapsedTime >= duration)
            { 
                state = STATE.FINISHED;
            }
        }

        else if (state == STATE.FINISHED)
        {
            //完成后不做处理
        }
        else 
        {
            Debug.Log("Mytimer error");
        }
    }

    //开始计时
    public void Go() 
    {
        elapsedTime = 0;
        state = STATE.RUN;
    }

    //停止并重置计时器
    public void Stop()
    {
        elapsedTime = 0;
        state = STATE.IDLE;
    }
}