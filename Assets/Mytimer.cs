using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Mytimer
{
    public enum STATE//状态
    { 
        IDLE,
        RUN,
        FINISHED

    }
    public STATE state;//对应枚举变量

    public float duration = 1.0f;//计算时间

    private float elapsedTime = 0;//流失的时间

    public void Tick()//推进这套代码
    {
        if (state == STATE.IDLE)//闲置状态，啥也不用做
        {

        }
        else if (state == STATE.RUN)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= duration)
            { 
                state = STATE.FINISHED;
            }
        }
        else if (state == STATE.FINISHED)
        {

        }
        else 
        {
            Debug.Log("Mytimer error");
        }
    }

    public void Go() 
    {
        elapsedTime = 0;
        state = STATE.RUN;
    }
    public void Stop()//重置时间，以及切换为闲置状态
    {
        elapsedTime = 0;
        state = STATE.IDLE;
    }

}
