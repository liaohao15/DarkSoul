using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    //胶囊体重叠检测，判断角色是否落地
    //需要参数(胶囊体端点、半径、检测层级)

    //人物胶囊碰撞体（通常都是CapsuleCollider）
    public CapsuleCollider CPC;
    //碰撞体的上下两个圆心
    [SerializeField]
    private Vector3 Point1;
    [SerializeField]
    private Vector3 Point2;

    //检测半径
    public float Radius;
    public float Offset = 0.1f;

    private void Awake()
    {
        //初始化检测半径
        Radius = CPC.radius - 0.05f;//这个碰撞体半径是我们物体的胶囊半径
    }

    void FixedUpdate()
    {
        //计算胶囊体的两个端点
        Vector3 realCenter = transform.TransformPoint(CPC.center);
        Point1 = realCenter - transform.up * (CPC.height - Radius - Offset);
        Point2 = Point1 - transform.up * 0.2f;

        //胶囊体检测：检测与“Ground”层的碰撞
        Collider[] outcolliders = Physics.OverlapCapsule(Point1, Point2, Radius, LayerMask.GetMask("Ground"));

        //给父类发消息，通知是否在地面
        if (outcolliders.Length != 0)
        {
            SendMessageUpwards("Ingroud");//往父类发送信息
        }
        else
        {
            SendMessageUpwards("NotIngroud");
        }
        //绘制检测线
        Debug.DrawLine(Point1, Point2, Color.red); // 中轴线（红）
    }
}