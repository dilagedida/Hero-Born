using System.Collections;
using System.Collections.Generic;
using UnityEngine;

void Start()
{
    _rb = GetComponent<Rigidbody>();
    _col = GetComponent<CapsuleCollider>();
    _gameManager = GameObject.Find("GameManager").GetComponent<GameBehavior>();
    if (camTransform == null)
    {
        camTransform = Camera.main.transform; // 默认主摄像机
    }
}

void Update()
{
    vInput = Input.GetAxis("Vertical");
    hInput = Input.GetAxis("Horizontal");

    if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
    {
        _rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
    }
}

void FixedUpdate()
{
    // 计算摄像机方向上的移动
    Vector3 camForward = camTransform.forward;
    Vector3 camRight = camTransform.right;
    camForward.y = 0;
    camRight.y = 0;
    camForward.Normalize();
    camRight.Normalize();

    Vector3 moveDir = (camForward * vInput + camRight * hInput).normalized;

    _rb.MovePosition(this.transform.position + moveDir * moveSpeed * Time.fixedDeltaTime);

    // 保持原有旋转逻辑（可由摄像机脚本控制玩家朝向）
    // _rb.MoveRotation(_rb.rotation * angleRot);

    if (Input.GetMouseButtonDown(0))
    {
        GameObject newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation) as GameObject;
        Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
        bulletRB.velocity = muzzle.forward * bulletSpeed;
    }
}
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameBehavior _gameManager;
    public float moveSpeed = 10f;
    public float rotateSpeed = 75f;
    private float vInput;
    private float hInput;

    private Rigidbody _rb;
    public float jumpVelocity = 5f;
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;
    private CapsuleCollider _col;
    public GameObject bullet;
    public float bulletSpeed = 100f;
    public Transform muzzle; //枪口

    public Transform camTransform; // 摄像机Transform引用

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameBehavior>();
        if (camTransform == null)
        {
            camTransform = Camera.main.transform; // 默认主摄像机
        }
    }

    void Update()
    {
        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");

        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
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
        if (collision.gameObject.name == "Enemy")
        {
            _gameManager.HP -= 1;
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

        // 发射子弹
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation) as GameObject;
            Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
            bulletRB.velocity = muzzle.forward * bulletSpeed;
        }
    }
}
