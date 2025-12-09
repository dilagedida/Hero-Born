using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Vector3 camOffset = new Vector3(1.5f, 1, -6f);
    private Transform target;

    public float mouseSensitivity = 3.0f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float yaw = 0f;   // 水平旋转
    private float pitch = 10f; // 垂直旋转（初始略微俯视）

    public Vector3 pivotOffset = new Vector3(0.7f, 1.2f, 0);//旋转中心

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
        Vector3 pivot = target.position + pivotOffset;// 旋转中心位置
        Vector3 desiredPosition = pivot + rotation * camOffset;
        transform.position = desiredPosition;

        // 始终看向玩家右上方
        transform.LookAt(pivot);
    }
}
