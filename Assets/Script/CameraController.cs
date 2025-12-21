using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerInput pi;
    public float horizontalSpeed = 200.0f;//摄像机水平的速度
    public float verticalSpeed = 80.0f;//摄像机垂直的速度

    public GameObject PlayerHandle;
    public GameObject CameraHandle;

    private float tempEulerX;

    private GameObject model;
    public GameObject camera;

    void Awake()
    {
        CameraHandle = transform.parent.gameObject;
        PlayerHandle = CameraHandle.transform.parent.gameObject;
        tempEulerX = 20.0f;
        model = PlayerHandle.GetComponent<ActorController>().model;
        camera = Camera.main.gameObject;
    }

    public void FixUpdate()
    {
        Vector3 tempModelEuler = model.transform.eulerAngles;
        PlayerHandle.transform.Rotate(Vector3.up,pi.Jright * horizontalSpeed * Time.fixedDeltaTime);//水平

        tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
        tempEulerX = Mathf.Clamp(tempEulerX,-40,30);
        CameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);//垂直

        model.transform.eulerAngles = tempModelEuler;//赋值现在的相机的位置以及欧拉角

        camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position, 0.1f);//让主摄像机平滑地移动到相机位置以及角度
        camera.transform.eulerAngles = transform.eulerAngles;//让主相机的欧拉角与相机的欧拉角朝向一致
    }

}
