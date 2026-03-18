using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class KeyboardInput : BaseUserInput
{

    //设置玩家的移动键
    //public string KeyUp = "W";
    //public string KeyDown = "S";
    //public string KeyLift = "A";
    //public string KeyRight = "D";
    //这种写法是将按键变量名存为字符串，但是Input.GetKey()的参数，不能识别这类，得用KeyCode类型
    [Header("===   key based    ===")]
    public KeyCode KeyUp = KeyCode.W;
    public KeyCode KeyDown = KeyCode.S;
    public KeyCode KeyLeft = KeyCode.A;
    public KeyCode KeyRight = KeyCode.D;

    public KeyCode KeyA;
    public KeyCode KeyB;
    public KeyCode KeyC;
    public KeyCode KeyD;

    //[Header("===   cameraConroller")]//这里我们是用来对Camera进行控制的
    //public KeyCode KeyJup;
    //public KeyCode KeyJdown;
    //public KeyCode KeyJleft;
    //public KeyCode KeyJright;

    //[Header("===   Mouse setting   ===")]//用滑鼠来控制视角切换
    //public bool mouseEnable = false;
    //public float mouseSensitivityX = 1f;
    //public float mouseSensitivityY = 1f;


    ////用正负值来决定上下左右键，其实就是将输入键转化为数值Image

    //[Header("===  key signal  ===")]
    //public float Dup;//当前前后输入值
    //public float Dturn;//当前左右输入值

    //public float TargetDup;//目标前后输入值
    //public float TargetDturn;//目标左右输入值

    //public float VelocityDup;// 调用Mathf.SmoothDamp方法时的速度参数，不赋值
    //public float VelocityDturn;

    //public bool InputEnable = true;//通过判断InputEnable的值来控制玩家输入

    //public float Jup;//当前摄像机上下的输入值
    //public float Jright;//当前摄像机左右的输入值


    ////Pressing signal
    //public bool run;
    ////Trigger signal
    //public bool jump;//通过对jump的判断来触发触发器
    //private  bool Lastjump;//在对jump判断之前，增加Lastjump与newJump的判断来控制跳跃次数
    //public bool attack;//通过对attack的判断来触发触发器
    //private bool Lastattack;//在对attack判断之前，增加Lastattack与newattack的判断来控制跳跃次数


    //[Header("=== other  === ")]
    //public float dL;//(Direction Magnitude)方向模长
    //public Vector3 dV;//(Direction Vector)方向向量

    // Start is called before the first frame update
    void Start()
    {

    }



    // 同理，修改update
    void Update()
    {


        // ==============         控制摄像机================
        if (mouseEnable == true)
        {
            //              修改成用滑鼠来控制
            Jup = Input.GetAxis("Mouse Y")*mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X")*mouseSensitivityX;
        }
        else
        {
            Jup = ((Input.GetKey(KeyJup) ? 1.0f : 0) - (Input.GetKey(KeyJdown) ? 1.0f : 0));
            print(Jup);
            Jright = ((Input.GetKey(KeyJright) ? 1.0f : 0) - (Input.GetKey(KeyJleft) ? 1.0f : 0));
            print(Jright);
        }
      




        // =============            控制方向向量     ===============
        //把按键转化为目标值
        TargetDup = ((Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0));
        TargetDturn = ((Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0));



        if (InputEnable == false)//使用InputEnable开关来控制玩家的输入功能
        {
            TargetDturn = 0;
            TargetDup = 0;

        }

        // ==============         使用平滑输入              ================

        //(平滑输入)第一个参数是当前值，第二个数是目标值，第三个数是速度引用（引用参数而不是实数）,第四个数是平滑时间
        Dup = Mathf.SmoothDamp(Dup, TargetDup, ref VelocityDup, 0.1f);
        Dturn = Mathf.SmoothDamp(Dturn, TargetDturn, ref VelocityDturn, 0.1f);//平滑输入是为了，更好的与动作动画搭配

        //===================      将正方形输入化为圆形输入    ================
        Vector2 TempVc = SqureToCircle(new Vector2(Dturn, Dup));
        float Dturn2 = TempVc.x;
        float Dup2 = TempVc.y;

        dL = Mathf.Sqrt((Dup2 * Dup2) + (Dturn2 * Dturn2));//角色的速度大小
        dV = Dup2 * Vector3.forward + Dturn2 * Vector3.right;//角色要走的方向
        run = Input.GetKey(KeyA);

        //      ======   设置跳跃信号和控制跳跃次数   ======
        bool newJump = Input.GetKey(KeyB);
        if (newJump != Lastjump && newJump == true)
        {
            jump = true;
            //print("Jump is Pressing");
        }
        else
        { 
            jump = false;
        }
        Lastjump = newJump;

        //      ======   设置攻击信号和控制攻击次数   ======
        bool newattack = Input.GetKey(KeyC);
        if (newattack != Lastattack && newattack == true)
        {
            attack = true;
            //print("Jump is Pressing");
        }
        else
        {
            attack = false;
        }
        Lastattack = newattack;
    }

    ////   ============     将方形范围改为圆形范围的方法    ==============
    //public Vector2 SqureToCircle(Vector2 input)//这个方法就是用来将平面的二维坐标转化为圆面的二维坐标
    //{ 
    //    Vector2 output = Vector2.zero;
    //    output.x = input.x * Mathf.Sqrt(1 - input.y * input.y / 2);
    //    output.y = input.y * Mathf.Sqrt(1 - input.x * input.x / 2);
    //    return output;
    //}




}
/*知识点回顾
 * 1.三元运算符（ TargetDup = ((Input.GetKey(KeyUp) ? 1 : 0) - (Input.GetKey(KeyDown) ? 1 : 0))   ）
 * 2.引用参数（Mathf.SmoothDamp）
 * 3.布尔值、条件判断（InputEnable）
 */