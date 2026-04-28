using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUserInput : MonoBehaviour
{
    [Header("===  (Joystick signal)移动输入信号  ===")]
    //当前实际移动值
    public float Dup;//当前前后输入值
    public float Dturn;//当前左右输入值
    //目标移动值
    public float TargetDup;//目标前后输入值
    public float TargetDturn;//目标左右输入值
    //平滑过渡（Mathf.SmoothDamp）专用参数
    public float VelocityDup;
    public float VelocityDturn;

    [Header("===  (Pressing signal)持续按压信号  ===")]
    public bool Run;//跑步状态
    public bool Defense;//防御状态

    [Header("===  (Trigger signal)持续按压信号  ===")]//只在按下/松开的那一帧有效
    public bool Jump;//跳跃
    public bool Roll;//翻滚/后撤
    public bool Attack;//攻击
    public bool LockOn;//视角锁定

    [Header("===  上一帧按键状态  ===")]
    protected bool LastJump;//在对jump判断之前，增加Lastjump与newJump的判断来控制跳跃次数
    protected bool LastAttack;//在对attack判断之前，增加Lastattack与newattack的判断来控制跳跃次数

    [Header("===  全局设置  ===")]
    public bool InputEnable = true;//通过判断InputEnable的值来控制玩家输入

    [Header("===  相机控制输入  ===")]
    public float Jup;//当前摄像机上下的输入值
    public float Jright;//当前摄像机左右的输入值
    public KeyCode KeyJup;
    public KeyCode KeyJdown;
    public KeyCode KeyJleft;
    public KeyCode KeyJright;

    [Header("===  （Mouse setting）鼠标设置  ===")]
    public bool MouseEnable = true;
    public float MouseSensitivityX = 1f;
    public float MouseSensitivityY = 1f;

    [Header("=== 移动方向数据  === ")]
    public float DL;//(Direction Magnitude)方向模长
    public Vector3 DV;//(Direction Vector)方向向量

    //正方形输入转圆形（解决斜方向移动速度过快bug）
    public Vector2 SqureToCircle(Vector2 input)//这个方法就是用来将平面的二维坐标转化为圆面的二维坐标
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - input.y * input.y / 2);
        output.y = input.y * Mathf.Sqrt(1 - input.x * input.x / 2);
        return output;
    }
    protected virtual void Update()
    {
        // ==============         控制摄像机================
        //鼠标控制相机
        if (MouseEnable == true)
        {
            Jup = Input.GetAxis("Mouse Y") * MouseSensitivityY;
            Jright = Input.GetAxis("Mouse X") * MouseSensitivityX;
        }
        //键盘按键控制相机
        else
        {
            Jup = ((Input.GetKey(KeyJup) ? 1.0f : 0) - (Input.GetKey(KeyJdown) ? 1.0f : 0));
            print(Jup);
            Jright = ((Input.GetKey(KeyJright) ? 1.0f : 0) - (Input.GetKey(KeyJleft) ? 1.0f : 0));
            print(Jright);
        }
    }
}