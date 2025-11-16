using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Player : MonoBehaviour
{
    //设置玩家的移动键
    public string KeyUp = "W";
    public string KeyDown = "S";
    public string KeyLift = "A";
    public string KeyRight = "D";
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
        Dup = ((Input.GetKey(KeyUp)?1.0f:0) - (Input.GetKey(KeyDown) ? 1.0f : 0));
        Dturn = ((Input.GetKey(KeyLift) ? 1.0f : 0) - (Input.GetKey(KeyRight) ? 1.0f : 0));


    }
}
