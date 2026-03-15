using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public PlayerInput pi;修改成手柄输入
    public JoystickInput pi;
    public float horizontalSpeed = 80.0f;//摄像机水平的速度
    public float verticalSpeed = 80.0f;//摄像机垂直的速度

    public GameObject PlayerHandle;
    public GameObject CameraHandle;

    private float tempEulerX;//摄像机垂直旋转角度
    private float tempEulerY;//摄像机水平旋转角度

    private GameObject model;
    public GameObject camera;

    void Awake()
    {
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
        camera = Camera.main.gameObject;
    }

    void FixedUpdate()
    {
        //摄像机水平旋转
        Vector3 tempModelEuler = model.transform.eulerAngles;
        //PlayerHandle.transform.Rotate(Vector3.up,pi.Jright * horizontalSpeed * Time.fixedDeltaTime);//水平。出现了控制人物位移的问题

        tempEulerY += pi.Jright * horizontalSpeed * Time.fixedDeltaTime;
        // 限制水平旋转范围
        tempEulerY = Mathf.Clamp(tempEulerY, -180, 180);


        //摄像机垂直旋转
        tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
        tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
        CameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, tempEulerY, 0);//垂直+水平

        model.transform.eulerAngles = tempModelEuler;//赋值现在的相机的位置以及欧拉角

        camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position, 0.1f);//让主摄像机平滑地移动到相机位置以及角度
        //    camera.transform.eulerAngles = transform.eulerAngles;//让主相机的欧拉角与相机的欧拉角朝向一致
        camera.transform.LookAt(CameraHandle.transform);

    }
}
