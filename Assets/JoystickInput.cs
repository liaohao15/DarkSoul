using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInput : BaseUserInput
{
    [Header("======  JoystickInput Setting ======")]
    public string axisX = "axisX";
    public string axisY = "axisY";
    public string btnA = "btn0";
    public string btnB = "btn1";
    public string btnC = "btn2";
    public string btnD = "btn3";
    public string axisJup = "axis3";
    public string axisJright= "axis4";
    public string btnLB = "btn4";
    public string btnLT = "btn6";

    public MyButton btnx = new MyButton(); 

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonC = new MyButton();
    public MyButton buttonD = new MyButton();
    public MyButton buttonLB = new MyButton();
    public MyButton buttonLT = new MyButton();
    //记录上一帧的IsExtending状态
    private bool lastAExtending;

    //[Header("===  Joystick signal  ===")]
    //public float Dup;//当前前后输入值
    //public float Dturn;//当前左右输入值
    //public float TargetDup;//目标前后输入值
    //public float TargetDturn;//目标左右输入值
    //public float VelocityDup;// 调用Mathf.SmoothDamp方法时的速度参数，不赋值
    //public float VelocityDturn;

    ////Pressing signal
    //public bool run;
    ////Trigger signal
    //public bool jump;//通过对jump的判断来触发触发器
    //private bool Lastjump;//在对jump判断之前，增加Lastjump与newJump的判断来控制跳跃次数
    //public bool attack;//通过对attack的判断来触发触发器
    //private bool Lastattack;//在对attack判断之前，增加Lastattack与newattack的判断来控制跳跃次数


    //public bool InputEnable = true;//通过判断InputEnable的值来控制玩家输入

    //public float Jup;//当前摄像机上下的输入值
    //public float Jright;//当前摄像机左右的输入值

    //[Header("=== other  === ")]
    //public float dL;//(Direction Magnitude)方向模长
    //public Vector3 dV;//(Direction Vector)方向向量



    // 修改一下，将原来的update()里的逻辑修改成这样
    void Update()
    {
        buttonA.Tick(Input.GetButton(btnA));
        buttonB.Tick(Input.GetButton(btnB));
        buttonC.Tick(Input.GetButton(btnC));
        buttonD.Tick(Input.GetButton(btnD));
        buttonLB.Tick(Input.GetButton(btnLB));
        buttonLT.Tick(Input.GetButton(btnLT));

        if (buttonA.IsExtending != lastAExtending)
        {
            print($"buttonA.IsExtending: {buttonA.IsExtending}");
            lastAExtending = buttonA.IsExtending; // 更新上一帧状态
        }
        //print(buttonA.IsExtending);
        //print(btnx.IsPressing);
        //print(btnx.OnPressed);
        //print(btnx.OnReleased);

        // ==============         控制摄像机              ================
        Jup = (Input.GetAxis(axisJup));
        //print(Jup);
        Jright = (Input.GetAxis(axisJright));
        //print(Jright);

        // =============            控制方向向量     ===============
        //把按键转化为目标值
        TargetDup = (Input.GetAxis(axisY));
        TargetDturn = (Input.GetAxis(axisX));

        if (InputEnable == false)//使用InputEnable开关来控制玩家的输入功能
        {
            TargetDturn = 0;
            TargetDup = 0;

        }

        //(平滑输入)第一个参数是当前值，第二个数是目标值，第三个数是速度引用（引用参数而不是实数）,第四个数是平滑时间
        Dup = Mathf.SmoothDamp(Dup, TargetDup, ref VelocityDup, 0.1f);
        Dturn = Mathf.SmoothDamp(Dturn, TargetDturn, ref VelocityDturn, 0.1f);//平滑输入是为了，更好的与动作动画搭配

        //===================      将正方形输入化为圆形输入    ================
        Vector2 TempVc = SqureToCircle(new Vector2(Dturn, Dup));
        float Dturn2 = TempVc.x;
        float Dup2 = TempVc.y;

        dL = Mathf.Sqrt((Dup2 * Dup2) + (Dturn2 * Dturn2));//角色的速度大小
        dV = Dup2 * Vector3.forward + Dturn2 * Vector3.right;//角色要走的方向


        //run = Input.GetButton(btnA);//控制跑步按键
        //优化跑步按键
        run = buttonA.IsPressing;

        //defense = Input.GetButton(btnLB);//控制防御动作
        //优化防御动作
        defense = buttonLB.IsPressing;

        //      ======   设置跳跃信号和控制跳跃次数   ======
        //bool newJump = Input.GetButton(btnB);
        //if (newJump != Lastjump && newJump == true)
        //{
        //    jump = true;
        //    //print("Jump is Pressing");
        //}
        //else
        //{
        //    jump = false;
        //}
        //Lastjump = newJump;
        //优化跳跃信号
        jump = buttonB.OnPressed;


        //      ======   设置攻击信号和控制攻击次数   ======
        //    bool newattack = Input.GetButton(btnC);
        //    if (newattack != Lastattack && newattack == true)
        //    {
        //        attack = true;
        //        //print("Jump is Pressing");
        //    }
        //    else
        //    {
        //        attack = false;
        //    }
        //    Lastattack = newattack;
        //}
        //优化攻击信号
        attack = buttonC.OnPressed;
    }


    //   ============     将方形范围改为圆形范围的方法    ==============
    //public Vector2 SqureToCircle(Vector2 input)//这个方法就是用来将平面的二维坐标转化为圆面的二维坐标
    //{
    //    Vector2 output = Vector2.zero;
    //    output.x = input.x * Mathf.Sqrt(1 - input.y * input.y / 2);
    //    output.y = input.y * Mathf.Sqrt(1 - input.x * input.x / 2);
    //    return output;
    //}


}

