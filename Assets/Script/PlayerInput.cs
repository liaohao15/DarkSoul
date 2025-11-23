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
    public float Dup;
    public float Dturn;

    public float TargetDup;
    public float TargetDturn;

    public float VelocityDup;
    public float VelocityDturn;

    [Header("=== other  === ")]
    public bool InputEnable = true;
    public bool run;//这里我们设置的按压信号

    public float dL;//(Direction Magnitude)方向模长
    public Vector3 dV;//(Direction Vector)方向向量

    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        TargetDup = ((Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0));
        TargetDturn = ((Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0));

        if (InputEnable == false)//使用InputEnable开关来控制玩家的输入功能
        {
            TargetDturn = 0;
            TargetDup = 0;

        }

        //第一个参数是当前值，第二个数是目标值，第三个数是速度引用（引用参数而不是实数）,第四个数是平滑时间
        Dup = Mathf.SmoothDamp(Dup, TargetDup, ref VelocityDup, 0.1f);
        Dturn = Mathf.SmoothDamp(Dturn, TargetDturn, ref VelocityDturn, 0.1f);//平滑输入是为了，更好的与动作动画搭配

        Vector2 TempVc = SqureToCircle(new Vector2(Dturn, Dup));
        float Dturn2 = TempVc.x;
        float Dup2 = TempVc.y;

        dL = Mathf.Sqrt((Dup2 * Dup2) + (Dturn2 * Dturn2));//角色要走的模长
        dV = Dup2 * Vector3.forward + Dturn2 * Vector3.right;//角色要走的方向

        run = Input.GetKey(KeyA);
    }

    public Vector2 SqureToCircle(Vector2 input)//这个方法就是用来将平面的二维坐标转化为圆面的二维坐标
    { 
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - input.y * input.y / 2);
        output.y = input.y * Mathf.Sqrt(1 - input.x * input.x / 2);


        return output;
    }



}
