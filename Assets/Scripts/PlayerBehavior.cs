using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameBehavior _gameManager;
    public float moveSpeed = 10f;
    public float rotateSpeed = 75f;
    private float vInput;
    private float hInput;

    // ① 添加一个私有的 Rigidbody 变量，用来存储胶囊的 Rigidbody 组件信息。
    private Rigidbody _rb;
    public float jumpVelocity = 5f;
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;
    private CapsuleCollider _col;
    public GameObject bullet;
    public float bulletSpeed = 100f;
    public Transform muzzle;//枪口


    void Start()
    {
        // ② Start 方法会在初始化脚本时触发，也就是单击 Play 按钮时。在初始化过程中，设置变量时都应该使用 Start 方法。
        // ③ 使用 GetComponent 方法检查脚本上附加的对象是否包含指定的组件类型，在本例中也就是 Rigidbody 组件。如果找到了，就返回。如果没有找到，那么返回 null。但在这里，我们已经知道 Player 对象上附有 Rigidbody 组件。
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameBehavior>();
    }

    void Update()
    {
        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * rotateSpeed;

        /* ④ 注释掉 Update 方法中对 Transform 和 Rotate 方法的调用，从而避免同时使用两种不同的控制方式。这里依然保留获取玩家输入的方式，以便后续继续使用。
        this.transform.Translate(Vector3.forward * vInput * Time.deltaTime);
        this.transform.Rotate(Vector3.up * hInput * Time.deltaTime);
        */
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        }
        //跳跃
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
        // 创建一个新的 Vector3 变量以存储左右旋转值。Vector3.up * hInput 与我们之前在 Rotate 方法中使用的旋转向量是相同的。
        Vector3 rotation = Vector3.up * hInput;

        // Quaternion.Euler 接收一个 Vector3 变量作为参数并使用欧拉角的格式返回旋转值。
        // 在 MoveRotation 方法中，我们需要使用 Quaternion 值而不是 Vector3 变量，这是 Unity 首选的旋转类型的转换。
        // 这里乘以 Time.fixedDeltaTime 的原因与在 Update 方法中乘以 Time.deltaTime 相同。
        Quaternion angleRot = Quaternion.Euler(rotation * Time.fixedDeltaTime);

        // 调用_rb 组件的 MovePosition 方法，该方法将接收一个 Vector3 变量作为参数并施加相应的力。
        // 使用的向量可以如下分解：胶囊的位置向量加上前向的方向向量与垂直输入和 Time.fixedDeltaTime 的乘积。
        // Rigidbody 组件负责调整施加的力以满足输入的向量参数。
        _rb.MovePosition(this.transform.position + this.transform.forward * vInput * Time.fixedDeltaTime);

        // 调用_rb 组件的 MoveRotation 方法，该方法也将接收一个 Vector3 变量作为参数并施加相应的力。angleRot 已经包含来自键盘的水平输入，所以只需要将当前 Rigidbody 组件的旋转值乘以 angleRot 就能得到同样的左右旋转值。
        _rb.MoveRotation(_rb.rotation * angleRot);
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation) as GameObject;
            Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
            bulletRB.velocity = muzzle.forward * bulletSpeed;
        }
    }
}
