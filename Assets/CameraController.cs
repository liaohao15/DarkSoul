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

    public void Update()
    {
        Vector3 tempModelEuler = model.transform.eulerAngles;
        PlayerHandle.transform.Rotate(Vector3.up,pi.Jright * horizontalSpeed * Time.deltaTime);

        tempEulerX -= pi.Jup * verticalSpeed * Time.deltaTime;
        tempEulerX = Mathf.Clamp(tempEulerX,-40,30);
        CameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

        model.transform.eulerAngles = tempModelEuler;

        camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position, 0.1f);
        camera.transform.eulerAngles = transform.eulerAngles;
    }

}
