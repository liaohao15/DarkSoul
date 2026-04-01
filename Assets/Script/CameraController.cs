using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public PlayerInput pi;修改成手柄输入
    public BaseUserInput pi;
    public float horizontalSpeed = 80.0f;//摄像机水平的速度
    public float verticalSpeed = 80.0f;//摄像机垂直的速度
    public float cameraDamp = 0.2f; // 摄像机平滑参数
    private Vector3 cameraDampVelocity; // SmoothDamp需要的速度参数


    public GameObject PlayerHandle;
    public GameObject CameraHandle;

    private float tempEulerX;//摄像机垂直旋转角度
    private float tempEulerY;//摄像机水平旋转角度

    private GameObject model;
    public Camera maincamera;
    
    [SerializeField]
    public GameObject lockTarget;

    void Awake()
    {
        maincamera = Camera.main;
        CameraHandle = transform.parent.gameObject;
        tempEulerX = 20.0f;
        //PlayerHandle = CameraHandle.transform.parent.gameObject;//PlayerHandle这里是我们的人物模型。但实际上应该是最顶层的PlayerHandle
        //model = PlayerHandle.GetComponent<ActorController>().model;// 我的这个是“危险代码”，必须拆分成两步并加空值判断。

        //初始化水平旋转角度
        tempEulerY = CameraHandle.transform.localEulerAngles.y;
        ActorController actor = PlayerHandle.GetComponentInChildren<ActorController>();
        if (actor != null && actor.model != null)
        {
            model = actor.model;
        }
        
    }

    void FixedUpdate()
    {
        if (lockTarget == null)
        {
            //摄像机水平旋转
            Vector3 tempModelEuler = model.transform.eulerAngles;
            //PlayerHandle.transform.Rotate(Vector3.up,pi.Jright * horizontalSpeed * Time.fixedDeltaTime);//水平。出现了控制人物位移的问题

            tempEulerY += pi.Jright * horizontalSpeed * Time.fixedDeltaTime;
            // 限制水平旋转范围
            //tempEulerY = Mathf.Clamp(tempEulerY, -180, 180);


            //摄像机垂直旋转
            tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);

            CameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, tempEulerY, 0);//垂直+水平

            model.transform.eulerAngles = tempModelEuler;//赋值现在的相机的位置以及欧拉角

        }
        else
        {
            // ================== 锁敌 ==================
            // 计算角色到敌人的方向，只保留水平方向
            Vector3 tempForward = lockTarget.transform.position - model.transform.position;
            tempForward.y = 0;
            // 让角色父物体自动朝向敌人，实现锁敌时角色始终面对敌人
            model.transform.forward = tempForward;

            // 锁敌时重置垂直角度，保持摄像机水平
            tempEulerX = Mathf.Lerp(tempEulerX, 0, Time.fixedDeltaTime * 5f);
            CameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);
            tempEulerY = 0;
        }
    

        maincamera.transform.position = Vector3.SmoothDamp(maincamera.transform.position, transform.position, ref cameraDampVelocity,cameraDamp);//让主摄像机平滑地移动到相机位置以及角度
        maincamera.transform.LookAt(CameraHandle.transform);

        ////    camera.transform.eulerAngles = transform.eulerAngles;//让主相机的欧拉角与相机的欧拉角朝向一致
        //Vector3 targetDir = CameraHandle.transform.position - maincamera.transform.position;
        //Quaternion targetRot = Quaternion.LookRotation(targetDir);
        //maincamera.transform.rotation = Quaternion.Lerp(
        //    maincamera.transform.rotation,
        //    targetRot,
        //    Time.deltaTime * 5f  // 旋转平滑速度，数值越大转向越快
        //);
    }

    public void LockUnlock()
    {
        //print("lockUnlock");
       
        //try to lock
        Vector3 modelOrigin1 = model.transform.position;
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);
        Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f),model.transform.rotation,LayerMask.GetMask("Enemy"));

        if (cols.Length == 0)
        {
            lockTarget = null;
        }
        else 
        {
            foreach (var col in cols)
            {
                //print(col.name);
                if (lockTarget == col.gameObject)
                {
                    lockTarget = null;
                    break;
                }
                lockTarget = col.gameObject;
                break;
            }
        }

       
    }

}
