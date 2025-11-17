using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Player : MonoBehaviour
{
    //设置玩家的移动键
    //public string KeyUp = "W";
    //public string KeyDown = "S";
    //public string KeyLift = "A";
    //public string KeyRight = "D";
    //这种写法是将按键变量名存为字符串，但是Input.GetKey()的参数，不能识别这类，得用KeyCode类型
    public KeyCode KeyUp = KeyCode.W;
    public KeyCode KeyDown = KeyCode.S;
    public KeyCode KeyLeft = KeyCode.A;
    public KeyCode KeyRight = KeyCode.D;
    //用正负值来决定上下左右键，其实就是将输入键转化为数值Image

    public float Dup;
    public float Dturn;

    public float TargetDup;
    public float TargetDturn;

    public float DupVelocity;
    public float DturnVelocity;

    public bool InputEnable = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float GetDupVelocity()
    {
        return DupVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        TargetDup = ((Input.GetKey(KeyCode.W)?1.0f:0) - (Input.GetKey(KeyCode.S) ? 1.0f : 0));
        TargetDturn = ((Input.GetKey(KeyCode.A) ? 1.0f : 0) - (Input.GetKey(KeyCode.D) ? 1.0f : 0));

        if (InputEnable == false)//使用InputEnable开关来控制玩家的输入功能
        {
            TargetDturn = 0;
            TargetDup = 0;
        
        }


        Dup = Mathf.SmoothDamp(Dup, TargetDup, ref DupVelocity, 0.1f);
        Dturn = Mathf.SmoothDamp(Dturn, TargetDturn, ref DturnVelocity, 0.1f);//平滑输入是为了，更好的与动作动画搭配
    }
}
