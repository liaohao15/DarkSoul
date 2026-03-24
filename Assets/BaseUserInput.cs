using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUserInput : MonoBehaviour
{
    [Header("===  Joystick signal  ===")]
    public float Dup;//当前前后输入值
    public float Dturn;//当前左右输入值
    public float TargetDup;//目标前后输入值
    public float TargetDturn;//目标左右输入值
    public float VelocityDup;// 调用Mathf.SmoothDamp方法时的速度参数，不赋值
    public float VelocityDturn;

    //Pressing signal
    public bool run;//跑步状态
    public bool defense;//防御状态

    //Trigger signal
    
    public bool jump;//通过对jump的判断来触发触发器
    protected bool Lastjump;//在对jump判断之前，增加Lastjump与newJump的判断来控制跳跃次数
    public bool roll;//翻滚/后撤信号
    public bool attack;//通过对attack的判断来触发触发器
    protected bool Lastattack;//在对attack判断之前，增加Lastattack与newattack的判断来控制跳跃次数
    public bool lockon;


    public bool InputEnable = true;//通过判断InputEnable的值来控制玩家输入

    public float Jup;//当前摄像机上下的输入值
    public float Jright;//当前摄像机左右的输入值

    [Header("=== other  === ")]
    public float dL;//(Direction Magnitude)方向模长
    public Vector3 dV;//(Direction Vector)方向向量


    [Header("===   cameraConroller")]//这里我们是用来对Camera进行控制的
    public KeyCode KeyJup;
    public KeyCode KeyJdown;
    public KeyCode KeyJleft;
    public KeyCode KeyJright;

    [Header("===   Mouse setting   ===")]//用滑鼠来控制视角切换
    public bool mouseEnable = true;
    public float mouseSensitivityX = 1f;
    public float mouseSensitivityY = 1f;


    public Vector2 SqureToCircle(Vector2 input)//这个方法就是用来将平面的二维坐标转化为圆面的二维坐标
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - input.y * input.y / 2);
        output.y = input.y * Mathf.Sqrt(1 - input.x * input.x / 2);
        return output;
    }

    void Update()
    {

        // ==============         控制摄像机================
        if (mouseEnable == true)
        {
            //              修改成用滑鼠来控制
            Jup = Input.GetAxis("Mouse Y") * mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X") * mouseSensitivityX;
        }
        else
        {
            Jup = ((Input.GetKey(KeyJup) ? 1.0f : 0) - (Input.GetKey(KeyJdown) ? 1.0f : 0));
            print(Jup);
            Jright = ((Input.GetKey(KeyJright) ? 1.0f : 0) - (Input.GetKey(KeyJleft) ? 1.0f : 0));
            print(Jright);
        }
    } 
}
