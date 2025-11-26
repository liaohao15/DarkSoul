using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PlayerInput : MonoBehaviour
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
 
    //用正负值来决定上下左右键，其实就是将输入键转化为数值Image

    [Header("===  key signal  ===")]
    public float Dup;//控制前后方向
    public float Dturn;//控制左右方向

    public float TargetDup;//想要转向的前后方向
    public float TargetDturn;//想要转向的左右方向

    public float VelocityDup;// 调用Mathf.SmoothDamp方法时的速度参数，不赋值
    public float VelocityDturn;

    public bool InputEnable = true;//通过判断InputEnable的值来控制玩家输入

    //Pressing signal
    public bool run;
    //Trigger signal
    public bool jump;//通过对jump的判断来触发触发器
    private  bool Lastjump;//在对jump判断之前，增加Lastjump与newJump的判断来控制跳跃次数

    [Header("=== other  === ")]
    public float dL;//(Direction Magnitude)方向模长
    public Vector3 dV;//(Direction Vector)方向向量

    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {

        // =============            控制方向向量     ===============
        TargetDup = ((Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0));
        TargetDturn = ((Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0));

        if (InputEnable == false)//使用InputEnable开关来控制玩家的输入功能
        {
            TargetDturn = 0;
            TargetDup = 0;

        }

        // ==============         使用平滑输入              ================

        //第一个参数是当前值，第二个数是目标值，第三个数是速度引用（引用参数而不是实数）,第四个数是平滑时间
        Dup = Mathf.SmoothDamp(Dup, TargetDup, ref VelocityDup, 0.1f);
        Dturn = Mathf.SmoothDamp(Dturn, TargetDturn, ref VelocityDturn, 0.1f);//平滑输入是为了，更好的与动作动画搭配


        //===================      将物体二维移动范围由正方形化为圆形    ================
        Vector2 TempVc = SqureToCircle(new Vector2(Dturn, Dup));
        float Dturn2 = TempVc.x;
        float Dup2 = TempVc.y;

        dL = Mathf.Sqrt((Dup2 * Dup2) + (Dturn2 * Dturn2));//角色要走的模长
        dV = Dup2 * Vector3.forward + Dturn2 * Vector3.right;//角色要走的方向
        run = Input.GetKey(KeyA);

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
    }

    //   ============     将方形范围改为圆形范围的方法    ==============
    public Vector2 SqureToCircle(Vector2 input)//这个方法就是用来将平面的二维坐标转化为圆面的二维坐标
    { 
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - input.y * input.y / 2);
        output.y = input.y * Mathf.Sqrt(1 - input.x * input.x / 2);
        return output;
    }



}
