//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class JoystickInput : BaseUserInput
//{
//    [Header("======  (JoystickInput Setting)手柄输入设置 ======")]
//    //手柄移动摇杆名称
//    public string AxisX = "axisX";// 左摇杆左右
//    public string AxisY = "axisY";// 左摇杆上下
//    //手柄功能按键名称
//    public string BtnA = "btn0";// 红键：原地按=后撤 | 跑步按=翻滚
//    public string BtnB = "btn1";// 粉键：预留
//    public string BtnC = "btn2";// 蓝键：普通攻击
//    public string BtnD = "btn3";// 绿键：预留
//    //手柄视角摇杆轴名称
//    public string AxisJup = "axis3";// 右摇杆上下
//    public string AxisJright= "axis4";// 右摇杆左右
//    //手柄肩键/扳机键名称
//    public string BtnLB = "btn4";// L1左肩键：防御（举盾）
//    public string BtnRT = "btn6";// R1右肩键：预留
//    //手柄摇杆按下按键
//    public string BtnRstick = "btn11";//  右摇杆按下：锁定敌人

//    [Header("自定义按键实例,用于精准检测按键状态")]
//    public MyButton ButtonA = new MyButton();// 红键（翻滚/跑步）
//    public MyButton ButtonB = new MyButton();// 粉键（预留）
//    public MyButton ButtonC = new MyButton();// 蓝键（普攻）
//    public MyButton ButtonD = new MyButton();// 绿键（预留）
//    public MyButton ButtonLB = new MyButton();// L1肩键（防御）
//    public MyButton ButtonRT = new MyButton();//  R1肩键（预留)
//    public MyButton ButtonRstick = new MyButton();// 右摇杆按下（锁敌）

//    //记录上一帧的IsExtending状态
//    private bool LastAExtending;

//    //每帧更新：检测手柄输入、处理信号
//    protected override void Update()
//    {
//        base.Update();

//        //1.更新所有自定义按键的按压状态
//        ButtonA.Tick(Input.GetButton(BtnA));
//        ButtonB.Tick(Input.GetButton(BtnB));
//        ButtonC.Tick(Input.GetButton(BtnC));
//        ButtonD.Tick(Input.GetButton(BtnD));
//        ButtonLB.Tick(Input.GetButton(BtnLB));
//        ButtonRT.Tick(Input.GetButton(BtnRT));
//        ButtonRstick.Tick(Input.GetButton(BtnRstick));

//        //2. 缓存核心按键状态
//        if (ButtonA.IsExtending != LastAExtending)
//        {
//            print($"ButtonA.IsExtending: {ButtonA.IsExtending}");
//            LastAExtending = ButtonA.IsExtending; 
//        }

//        //3.右摇杆 -> 相机视角输入
//        Jup = (Input.GetAxis(AxisJup));// 右摇杆上下输入
//        Jright = (Input.GetAxis(AxisJright));// 右摇杆左右输入

//        //4. 左摇杆 -> 角色移动原始输入
//        TargetDup = (Input.GetAxis(AxisY));// 左摇杆前后
//        TargetDturn = (Input.GetAxis(AxisX));// 左摇杆左右

//        //5. 输入禁用开关：禁止输入时清空移动信号
//        if (InputEnable == false)//使用InputEnable开关来控制玩家的输入功能
//        {
//            TargetDturn = 0;
//            TargetDup = 0;

//        }

//        //平滑输入：让摇杆移动更加丝滑，不生硬
//        //第一个参数是当前值，第二个数是目标值，第三个数是速度引用（引用参数而不是实数）,第四个数是平滑时间
//        Dup = Mathf.SmoothDamp(Dup, TargetDup, ref VelocityDup, 0.35f);
//        Dturn = Mathf.SmoothDamp(Dturn, TargetDturn, ref VelocityDturn, 0.35f);//平滑输入是为了，更好的与动作动画搭配

//        //计算移动方向和速度
//        Vector2 TempVc = SqureToCircle(new Vector2(Dturn, Dup));
//        float Dturn2 = TempVc.x;
//        float Dup2 = TempVc.y;

//        DL = Mathf.Sqrt((Dup2 * Dup2) + (Dturn2 * Dturn2));//角色的速度大小
//        DV = Dup2 * Vector3.forward + Dturn2 * Vector3.right;//角色要走的方向

//        //   ======  动作信号   ======
//        //跑步：长按/持续触发（左肩键LB）
//        Run = DL > 0.8f;
//        //按下触发，动画状态机根据速度自动判断是后撤步还是翻滚
//        Roll = ButtonA.OnPressed;

//        //防御：长按触发（左扳机LT）
//        Defense = ButtonLB.IsPressing;

//        //攻击：按下瞬间触发（C键（蓝）
//        Attack = ButtonC.OnPressed;

//        //索敌：摇杆按下触发（左摇杆）
//        LockOn = ButtonRstick.OnPressed;
//    }
//}