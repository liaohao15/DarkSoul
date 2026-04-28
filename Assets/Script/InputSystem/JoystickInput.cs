using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInput : BaseUserInput
{
    [Header("======  (JoystickInput Setting)手柄输入设置 ======")]
    //手柄移动摇杆名称
    public string AxisX = "AxisX";
    public string AxisY = "AxisY";
    //手柄功能按键名称
    public string BtnA = "btn0";
    public string BtnB = "btn1";
    public string BtnC = "btn2";
    public string BtnD = "btn3";
    //手柄视角摇杆轴名称
    public string AxisJup = "Axis3";
    public string AxisJright= "Axis4";
    //手柄肩键/扳机键名称
    public string BtnLB = "btn4";
    public string BtnLT = "btn6";
    //手柄摇杆按下按键
    public string BtnJstick = "btn11";

    [Header("自定义按键实例,用于精准检测按键状态")]
    public MyButton ButtonA = new MyButton();
    public MyButton ButtonB = new MyButton();
    public MyButton ButtonC = new MyButton();
    public MyButton ButtonD = new MyButton();
    public MyButton ButtonLB = new MyButton();
    public MyButton ButtonLT = new MyButton();
    public MyButton ButtonJstick = new MyButton();

    //记录上一帧的IsExtending状态
    private bool LastAExtending;

    //每帧更新：检测手柄输入、处理信号
    protected override void Update()
    {
        base.Update();

        //1.更新所有自定义按键的按压状态
        ButtonA.Tick(Input.GetButton(BtnA));
        ButtonB.Tick(Input.GetButton(BtnB));
        ButtonC.Tick(Input.GetButton(BtnC));
        ButtonD.Tick(Input.GetButton(BtnD));
        ButtonLB.Tick(Input.GetButton(BtnLB));
        ButtonLT.Tick(Input.GetButton(BtnLT));
        ButtonJstick.Tick(Input.GetButton(BtnJstick));
     
        //记录按键A的长按状态，用于逻辑判断
        if (ButtonA.IsExtending != LastAExtending)
        {
            print($"ButtonA.IsExtending: {ButtonA.IsExtending}");
            LastAExtending = ButtonA.IsExtending; // 更新上一帧状态
        }
       
        //控制摄像机：读取手柄右摇杆输入
        Jup = (Input.GetAxis(AxisJup));
        Jright = (Input.GetAxis(AxisJright));

        //移动控制：读取手柄左摇杆输入
        TargetDup = (Input.GetAxis(AxisY));
        TargetDturn = (Input.GetAxis(AxisX));

        //移动方向输入：禁止输入时，清空移动信号
        if (InputEnable == false)//使用InputEnable开关来控制玩家的输入功能
        {
            TargetDturn = 0;
            TargetDup = 0;

        }

        //平滑输入：让摇杆移动更加丝滑，不生硬
        //第一个参数是当前值，第二个数是目标值，第三个数是速度引用（引用参数而不是实数）,第四个数是平滑时间
        Dup = Mathf.SmoothDamp(Dup, TargetDup, ref VelocityDup, 0.1f);
        Dturn = Mathf.SmoothDamp(Dturn, TargetDturn, ref VelocityDturn, 0.1f);//平滑输入是为了，更好的与动作动画搭配

        //计算移动方向和速度
        Vector2 TempVc = SqureToCircle(new Vector2(Dturn, Dup));
        float Dturn2 = TempVc.x;
        float Dup2 = TempVc.y;

        DL = Mathf.Sqrt((Dup2 * Dup2) + (Dturn2 * Dturn2));//角色的速度大小
        DV = Dup2 * Vector3.forward + Dturn2 * Vector3.right;//角色要走的方向

        //   ======  动作信号   ======
        //跑步：长按/持续触发
        Run = (ButtonA.IsPressing && ButtonA.IsDelaying) || ButtonA.IsExtending;

        //翻滚：短按释放/长按中断触发
        Roll = (ButtonA.OnReleased && !ButtonA.isLongPress) || (ButtonA.OnPressed && ButtonA.IsExtending);

        //防御：长按触发
        Defense = ButtonLB.IsPressing;

        //攻击：按下瞬间触发
        Attack = ButtonC.OnPressed;

        //索敌：摇杆按下触发
        LockOn = ButtonJstick.OnPressed;
    }
}