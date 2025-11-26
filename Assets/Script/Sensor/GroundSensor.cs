using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    //  *******************      这个脚本我们要实现物体碰撞的检测        **********************
    //  ***************************************************************************************
    //我们要是使用到Overlap的方法，需要知道四个东西，胶囊体的上下两个端点，胶囊体的半径，检测与谁的碰撞
    
    public CapsuleCollider cpC;//人物碰撞体通常都是CapsuleCollider（胶囊碰撞体）
    //下面定义碰撞体的上下两个圆心
    [SerializeField]
    private Vector3 poinT1;
    [SerializeField]
    private Vector3 poinT2;
    //下面是定义的半径
    private float radius;

    private void Awake()
    {
        radius = cpC.radius;//这个碰撞体半径是我们物体的胶囊半径
    }

    void FixedUpdate()
    {
        poinT1 = transform.position + transform.up * radius;
        poinT2 = transform.position + transform.up * cpC.height - transform.up * radius;
        LayerMask layerMask1 = LayerMask.GetMask("Ground");
        Collider[] outcolliders = Physics.OverlapCapsule(poinT1, poinT2, radius, layerMask1);
        if (outcolliders.Length != 0)
        {
            foreach (var col in outcolliders)
            {
                print("" + col.name);
            }
        }

    }

}
