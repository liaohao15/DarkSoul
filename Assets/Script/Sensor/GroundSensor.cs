using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    //  *******************      这个脚本我们要实现胶囊体重叠的检测        **********************
    //  ***************************************************************************************
    //我们要是使用到Overlap的方法，需要知道四个东西，胶囊体的上下两个端点，胶囊体的半径，检测与谁的碰撞
    
    public CapsuleCollider cpC;//人物碰撞体通常都是CapsuleCollider（胶囊碰撞体）
    //下面定义碰撞体的上下两个圆心
    [SerializeField]
    private Vector3 poinT1;
    [SerializeField]
    private Vector3 poinT2;
    
    public float radius;//定义的半径
    public float offest = 0.1f;

    private void Awake()
    {
        radius = cpC.radius - 0.05f;//这个碰撞体半径是我们物体的胶囊半径
       
    }

    void FixedUpdate()
    {
        //poinT1 = transform.position + transform.up * radius;
        //poinT2 = transform.position + transform.up * cpC.height - transform.up * radius;
        //Vector3 realCenter = cpC.center;这个是胶囊相对于物体的中心位置
        //计算胶囊体的两个端点
        Vector3 realCenter = transform.TransformPoint(cpC.center);
        poinT1 = realCenter - transform.up * (cpC.height - radius - offest);
        poinT2 = poinT1 - transform.up *  + 0.2f;

        //胶囊体检测：检测与“Ground”层的碰撞
        //LayerMask layerMask1 = LayerMask.GetMask("Ground");
        Collider[] outcolliders = Physics.OverlapCapsule(poinT1, poinT2, radius, LayerMask.GetMask("Ground"));
        
        //给父类发消息，通知是否在地面
        if (outcolliders.Length != 0)
        {
           
               // SendMessage("Inground");只往子类或本身发
           
            SendMessageUpwards("Ingroud");//往父类发送信息
        }
        else 
        {
            SendMessageUpwards("NotIngroud");
        }
        Debug.DrawLine(poinT1, poinT2, Color.red); // 中轴线（红）
     
    }

}

/*
 * 知识点回顾
 * 1.这里的坐标转换:由本地→世界（transform.TramPoint）
 * 2.数组和层掩码（Physics.OverlapCapsule）
 * 3.消息通信（SendMessageUpwards）
 * 
 */
