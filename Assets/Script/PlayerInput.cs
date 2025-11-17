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
    public KeyCode KeyLift = KeyCode.A;
    public KeyCode KeyRight = KeyCode.D;
    //用正负值来决定上下左右键，其实就是将输入键转化为数值Image
    public float Dup;
    public float Dturn;
    
    
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Dup = ((Input.GetKey(KeyCode.W)?1.0f:0) - (Input.GetKey(KeyCode.S) ? 1.0f : 0));
        Dturn = ((Input.GetKey(KeyCode.A) ? 1.0f : 0) - (Input.GetKey(KeyCode.D) ? 1.0f : 0));


    }
}
