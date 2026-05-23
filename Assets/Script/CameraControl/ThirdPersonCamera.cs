using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Cinemachine")]
    [Tooltip("跟随的目标")]
    public GameObject CameraTarget;

    [Header("旋转参数")]
    [Tooltip("上移动的最大角度")]
    public float TopClamp = 70.0f;
    [Tooltip("下移动的最大角度")]
    public float BottomClamp = -30.0f;
    [Tooltip("鼠标灵敏度")]
    public float mouseSensitivity = 3.0f;

    [Header("距离控制（滚轮）")]
    [Tooltip("滚轮灵敏度")]
    public float scrollSensitivity = 2.0f;
    [Tooltip("最近距离")]
    public float minDistance = 2.0f;
    [Tooltip("最远距离")]
    public float maxDistance = 10.0f;
    [Tooltip("默认距离")]
    public float defaultDistance = 5.0f;

    private Camera _mainCamera;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private float _currentDistance;           // 当前相机与目标的距离

    private void Start()
    {
        if (CameraTarget == null)
        {
            Debug.LogError("ThirdPersonCamera: CameraTarget 未设置！");
            return;
        }

        // 获取主相机
        _mainCamera = GetComponent<Camera>();
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        // 初始化旋转角度
        _cinemachineTargetYaw = CameraTarget.transform.rotation.eulerAngles.y;
        _cinemachineTargetPitch = 0.0f;

        // 初始化距离
        _currentDistance = defaultDistance;

        // 锁定光标
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (CameraTarget == null) return;

        // 1. 鼠标控制旋转
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _cinemachineTargetYaw += mouseX;
        _cinemachineTargetPitch -= mouseY;
        //_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw,_cinemachineTargetYaw, 0f);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // 2. 滚轮控制距离
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        _currentDistance -= scroll;   // 向上滚轮减小距离（拉近）
        _currentDistance = Mathf.Clamp(_currentDistance, minDistance, maxDistance);

        // 3. 更新 CameraTarget 的旋转（相机环绕目标）
        CameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,
            _cinemachineTargetYaw, 0.0f);

        // 计算相机应该在的位置：目标位置 + 旋转后的偏移向量 (0, 高度偏移, -距离)
        Vector3 targetPosition = CameraTarget.transform.position;
        Vector3 offset = new Vector3(0, 0, -_currentDistance);
        Vector3 desiredPosition = targetPosition + CameraTarget.transform.rotation * offset;

        // 应用相机位置
        if (_mainCamera != null)
            _mainCamera.transform.position = desiredPosition;
        else
            transform.position = desiredPosition;   // 如果脚本挂在相机上

        // 让相机始终看向目标（可选，保持镜头对准）
        if (_mainCamera != null)
            _mainCamera.transform.LookAt(targetPosition);
        else
            transform.LookAt(targetPosition);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

}
