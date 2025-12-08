using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Vector3 camOffset = new Vector3(0, 1.2f, -2.6f);
    private Transform target;

    public float mouseSensitivity = 3.0f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float yaw = 0f;   // 水平旋转
    private float pitch = 10f; // 垂直旋转（初始略微俯视）

    void Start()
    {
        target = GameObject.Find("Player").transform;
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        Cursor.lockState = CursorLockMode.Locked; // 鼠标锁定在屏幕中央
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 计算旋转
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // 计算相机位置
        Vector3 desiredPosition = target.position + rotation * camOffset;
        transform.position = desiredPosition;

        // 始终看向玩家
        transform.LookAt(target.position + Vector3.up * 1.0f); // 视线略高于玩家中心
    }
}
