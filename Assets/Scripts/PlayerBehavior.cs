using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameBehavior _gameManager;
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float rotateSpeed = 75f;
    public float jumpVelocity = 5f;
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;

    //输入变量
    private float vInput;
    private float hInput;
    private Rigidbody _rb;
    private CapsuleCollider _col;

    

    [Header("Camera Settings")]
    public Transform camTransform; // 摄像机Transform引用，用于移动方向计算
    public Camera mainCamera;      // 摄像机组件（用于射线检测计算）

    [Header("Shooting Settings")]
    public GameObject bullet;
    public float bulletSpeed = 100f;
    public Transform muzzle;      // 枪口
    
    public LayerMask aimLayerMask;// 射击检测层

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();

        //获取游戏管理器
        _gameManager = GameObject.Find("GameManager").GetComponent<GameBehavior>();

        //自动获取摄像机引用
        if (mainCamera == null) mainCamera = Camera.main;
        if (camTransform == null && mainCamera != null) camTransform = mainCamera.transform;

        //锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //获取输入
        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");

        //跳跃逻辑
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        }

        // 发射子弹
        if (Input.GetMouseButtonDown(0))
        {
            ShootAtCrosshair();
        }
    }

    void ShootAtCrosshair()
    {
        // 找到屏幕正中心（准星位置）
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);

        Vector3 targetPoint;

        // 发射射线，看看准星指到了哪里
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimLayerMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            // 如果打向天空，目标点就是射线远处的一个虚拟点
            targetPoint = ray.GetPoint(1000f);
        }

        // 计算从“枪口”到“目标点”的方向
        // 注意：这里必须用 normalized 归一化向量
        Vector3 shootDirection = (targetPoint - muzzle.position).normalized;

        // 生成子弹并让它朝向目标方向飞行
        GameObject newBullet = Instantiate(bullet, muzzle.position, Quaternion.LookRotation(shootDirection));

        Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
        if (bulletRB != null)
        {
            bulletRB.useGravity = false; // 不启用重力
            bulletRB.velocity = shootDirection * bulletSpeed;
        }
    }

    void FixedUpdate()
    {
        // 摄像机方向上的移动
        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * vInput + camRight * hInput).normalized;

        _rb.MovePosition(this.transform.position + moveDir * moveSpeed * Time.fixedDeltaTime);


        // 让玩家面朝摄像机的水平 forward 方向
        Vector3 lookDir = camTransform.forward;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            transform.forward = lookDir.normalized;
        }
    }

    private bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(_col.bounds.center.x, _col.bounds.min.y, _col.bounds.center.z);
        bool grounded = Physics.CheckCapsule(_col.bounds.center, capsuleBottom, distanceToGround, groundLayer, QueryTriggerInteraction.Ignore);
        return grounded;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Enemy")// 敌人碰撞伤害
        {
            _gameManager.HP -= 25;
        }
    }

    
}
