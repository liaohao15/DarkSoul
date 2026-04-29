using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //输入组件
    public BaseUserInput Pi;

    [Header("相机设置")]
    public float HorizontalSpeed = 80.0f;//摄像机水平的速度
    public float VerticalSpeed = 80.0f;//摄像机垂直的速度
    public float CameraDamp = 0.2f; // 摄像机平滑参数
    private Vector3 CameraDampVelocity; // SmoothDamp需要的速度参数

    [Header("相机层级")]
    public GameObject PlayerHandle;
    public GameObject CameraHandle;

    //相机旋转角度缓存
    private float TempEulerX;//摄像机垂直旋转角度
    private float TempEulerY;//摄像机水平旋转角度

    //获取角色模型、主相机
    private GameObject Model;
    public Camera Maincamera;

    [Header("锁定的目标敌人")]
    [SerializeField]
    public GameObject LockTarget;

    //初始化组件和默认参数
    void Awake()
    {
        //获取主相机
        Maincamera = Camera.main;
        //获取父类物体手柄
        CameraHandle = transform.parent.gameObject;
        //默认垂直角度
        TempEulerX = 20.0f;
        //初始化水平旋转角度
        TempEulerY = CameraHandle.transform.localEulerAngles.y;
        
        //安全获取角色模型
        ActorController actor = PlayerHandle.GetComponentInChildren<ActorController>();
        if (actor != null && actor.Model != null)
        {
            Model = actor.Model;
        }
        
    }

    //固定物理帧更新相机
    void FixedUpdate()
    {
        // 空值安全防护
        if (Model == null || Maincamera == null || CameraHandle == null || Pi == null) return;


        //无锁定目标：自由旋转相机 
        if (LockTarget == null)
        {
            //未锁定：正常相机控制

            //水平旋转：鼠标/手柄左右输入
            TempEulerY += Pi.Jright * HorizontalSpeed * Time.fixedDeltaTime;
            
            //垂直旋转：鼠标/手柄上下输入 + 限制俯仰角度
            TempEulerX -= Pi.Jup * VerticalSpeed * Time.fixedDeltaTime;
            TempEulerX = Mathf.Clamp(TempEulerX, -40, 30);

            //应用相机旋转
            CameraHandle.transform.localEulerAngles = new Vector3(TempEulerX, TempEulerY, 0);//垂直+水平
        }
        //有锁定目标：锁敌视角
        else
        { 
            //锁定：相机看向敌人，不修改角色朝向

            // 计算角色指向敌人的方向，只保留水平方向（忽略高度）
            Vector3 Dir = LockTarget.transform.position - CameraHandle.transform.position;
            Dir.y = 0;
            Quaternion TargetRot = Quaternion.LookRotation(Dir);

            //相机平滑看向敌人
            CameraHandle.transform.rotation = Quaternion.Lerp(
                CameraHandle.transform.rotation,
                TargetRot,
                0.1f);


            // 锁敌时保持摄像机水平
            TempEulerX = Mathf.Lerp(TempEulerX, 0, Time.fixedDeltaTime * 5f);
            TempEulerY = CameraHandle.transform.eulerAngles.y;
        }
    
        //主相机平滑跟随目标位置
        Maincamera.transform.position = Vector3.SmoothDamp(
            Maincamera.transform.position, 
            transform.position, 
            ref CameraDampVelocity,CameraDamp);//让主摄像机平滑地移动到相机位置以及角度
        //主相机始终看向相机手柄
        Maincamera.transform.LookAt(CameraHandle.transform);
    }

    public void LockUnlock()
    {
        //在角色前方生成重叠盒，检测敌人
        Vector3 ModelOrigin1 = Model.transform.position;
        Vector3 ModelOrigin2 = ModelOrigin1 + new Vector3(0, 1, 0);
        Vector3 boxCenter = ModelOrigin2 + Model.transform.forward * 5.0f;
        
        //检测Enemy层的碰撞体
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f),Model.transform.rotation,LayerMask.GetMask("Enemy"));

        //无敌人，清空锁定
        if (cols.Length == 0)
        {
            LockTarget = null;
        }
        //有敌人：锁定第一个敌人/重复按键取消锁定
        else 
        {
            foreach (var col in cols)
            {
                //print(col.name);
                if (LockTarget == col.gameObject)
                {
                    LockTarget = null;
                    break;
                }
                LockTarget = col.gameObject;
                break;
            }
        }
    }
}