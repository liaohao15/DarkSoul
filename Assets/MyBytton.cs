using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton 
{
    public bool IsPressing;//正在被按压
    public bool OnPressed;//刚刚被按住
    public bool OnReleased;//刚刚被释放

    private bool curstate = false;
    private bool laststate = false;

    public void Tick(bool input)
    {
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
            }
        }
        laststate = curstate;
    }
}
