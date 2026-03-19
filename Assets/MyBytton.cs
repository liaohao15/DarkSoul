using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton 
{
    public bool IsPressing = false;//正在被按压
    public bool OnPressed = false;//刚刚被按住
    public bool OnReleased = false;//刚刚被释放
    public bool IsExtending = false;

    private bool curstate = false;
    private bool laststate = false;

    private Mytimer extTimer = new Mytimer();

    public void Tick(bool input)
    {

        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    extTimer.duration = 1.0f;
        //    extTimer.Go();
        //}
        StartTimer(extTimer, 1.0f);
        extTimer.Tick();
        //Debug.Log(extTimer.state);

        curstate = input;
        IsPressing = curstate;

        OnPressed = false;
        OnReleased = false;
        if (curstate != laststate)
        {
            if (curstate == true)
            {
                OnPressed = true;
            }
            else
            {
                OnReleased = true;
                StartTimer(extTimer, 1.0f);
            }
        }
        laststate = curstate;

        if (extTimer.state == Mytimer.STATE.RUN)
        {
            IsExtending = true;
        }
        else 
        {
            IsExtending = false;
        }
    }

    private void StartTimer(Mytimer timer,float duration)
    { 
        timer.duration = duration;
        timer.Go();
    }

}
