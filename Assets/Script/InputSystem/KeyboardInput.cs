using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : BaseUserInput
{
    [Header("===   (key based)键盘按键设置    ===")]

    [Header("移动按键")]
    public KeyCode KeyUp = KeyCode.W;
    public KeyCode KeyDown = KeyCode.S;
    public KeyCode KeyLeft = KeyCode.A;
    public KeyCode KeyRight = KeyCode.D;
    [Header("功能按键")]
    public KeyCode KeyA;
    public KeyCode KeyB;
    public KeyCode KeyC;
    public KeyCode KeyD;

    protected override void Update()
    {
        // 执行父类的相机逻辑
        base.Update();

        if (InputEnable)
        {
            GetInput();
        }
        else
        {
            ClearInput();
        }
    }

    void GetInput()
    {
        // =============控制方向向量     ===============
        //把按键转化为目标值
        TargetDup = ((Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0));
        TargetDturn = ((Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0));

        // ==============使用平滑输入              ================
        //(当前值、目标值、引用速度、平滑时间)
        Dup = Mathf.SmoothDamp(Dup, TargetDup, ref VelocityDup, 0.1f);
        Dturn = Mathf.SmoothDamp(Dturn, TargetDturn, ref VelocityDturn, 0.1f);
        //平滑输入是为了，更好的与动作动画搭配

        //===================将正方形转为圆形输入    ================
        Vector2 TempVc = SqureToCircle(new Vector2(Dturn, Dup));
        float Dturn2 = TempVc.x;
        float Dup2 = TempVc.y;

        //计算移动速度大小
        DL = Mathf.Sqrt((Dup2 * Dup2) + (Dturn2 * Dturn2));//角色的速度大小
        //计算移动方向向量
        DV = Dup2 * Vector3.forward + Dturn2 * Vector3.right;//角色要走的方向

        //跑步按键
        Run = Input.GetKey(KeyA);

        //      ======   跳跃：帧状态判断，控制跳跃次数   ======
        bool NewJump = Input.GetKey(KeyB);
        if (NewJump != LastJump && NewJump == true)
        {
            Jump = true;
        }
        else
        {
            Jump = false;
        }
        LastJump = NewJump;

        //      ======   攻击：帧状态判断，防止连打   ======
        bool NewAttack = Input.GetKey(KeyC);
        if (NewAttack != LastAttack && NewAttack == true)
        {
            Attack = true;
        }
        else
        {
            Attack = false;
        }
        LastAttack = NewAttack;
    }

    //清空输入
    void ClearInput()
    {
        TargetDup = 0;
        TargetDturn = 0;
        Dup = 0;
        Dturn = 0;
        Run = false;
        Jump = false;
        Attack = false;
    }


}