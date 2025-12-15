using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol, // 巡逻
    Alert,  // 警戒
    Chase,  // 追击
    Lost    // 丢失目标/搜寻
}

public class EnemyBehavior : MonoBehaviour
{
    [Header("Basic Settings")]
    public Transform player;
    private int _lives = 75;

    [Header("Patrol Settings")]
    public Transform patrolRoute;
    public List<Transform> locations;
    private int locationIndex = 0;

    [Header("AI Settings")]
    public float baseSpeed = 3.5f;       // 基础巡逻速度
    public float alertWaitTime = 3.0f;   // 警戒等待时间
    public float lostWaitTime = 5.0f;    // 丢失后寻找时间
    public GameObject alertUI;           // 头顶的红色感叹号UI
    public AudioClip alertSound;         // 发现玩家时的音效

    // 内部状态变量
    private NavMeshAgent agent;
    private EnemyState currentState = EnemyState.Patrol;
    private float stateTimer = 0f;       // 通用计时器
    private bool playerInTrigger = false; // 玩家是否在球形触发器内
    private AudioSource audioSource;
    private float defaultSpeed;

    public int EnemyLives
    {
        get { return _lives; }
        private set
        {
            _lives = value;
            if (_lives <= 0)
            {
                Destroy(this.gameObject);
                Debug.Log("Enemy down!");
            }
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        if (GameObject.Find("Player"))
            player = GameObject.Find("Player").transform;

        InitializePatrolRoute();

        // 初始化速度
        agent.speed = baseSpeed;
        defaultSpeed = baseSpeed;

        // 确保警戒UI开始是隐藏的
        if (alertUI != null) alertUI.SetActive(false);

        // 开始巡逻
        MoveToNextPatrolLocation();
    }

    void Update()
    {
        // 核心状态机逻辑
        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolLogic();
                break;
            case EnemyState.Alert:
                AlertLogic();
                break;
            case EnemyState.Chase:
                ChaseLogic();
                break;
            case EnemyState.Lost:
                LostLogic();
                break;
        }

        // 只要玩家在触发范围内，就持续检测视线（处理状态切换）
        if (playerInTrigger)
        {
            CheckLineOfSight();
        }
    }

    // --- 状态逻辑 ---

    // 巡逻逻辑
    void PatrolLogic()
    {
        // 如果到达目的地，去下一个点
        if (agent.remainingDistance < 0.2f && !agent.pathPending)
        {
            MoveToNextPatrolLocation();
        }
    }

    // 警戒逻辑
    void AlertLogic()
    {
        agent.isStopped = true; // 警戒时停止移动

        // 计时
        stateTimer += Time.deltaTime;

        // 如果警戒时间到了，进入追击
        if (stateTimer >= alertWaitTime)
        {
            EnterChaseState();
        }
    }

    // 追击逻辑
    void ChaseLogic()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position); // 实时追踪玩家
    }

    // 丢失/搜寻逻辑
    void LostLogic()
    {
        agent.isStopped = true; // 原地疑惑/等待

        stateTimer += Time.deltaTime;

        // 如果等待超过设定时间（5秒），放弃追击，回原来的路巡逻
        if (stateTimer >= lostWaitTime)
        {
            EnterPatrolState();
        }
    }

    // --- 状态切换方法 ---

    void EnterAlertState()
    {
        if (currentState == EnemyState.Chase || currentState == EnemyState.Alert) return;

        Debug.Log("进入警戒状态");
        currentState = EnemyState.Alert;
        stateTimer = 0;

        // 播放音效
        if (audioSource && alertSound) audioSource.PlayOneShot(alertSound);
        // 显示UI
        if (alertUI) alertUI.SetActive(true);
    }

    void EnterChaseState()
    {
        Debug.Log("进入追击状态");
        currentState = EnemyState.Chase;
        agent.speed = defaultSpeed * 1.3f; // 速度增加30%
        if (alertUI) alertUI.SetActive(true); // 追击开始后继续显示感叹号
    }

    void EnterLostState()
    {
        if (currentState == EnemyState.Patrol) return; // 如果本来就在巡逻，不需要进入丢失状态
        
        Debug.Log("丢失目标，开始搜寻");
        currentState = EnemyState.Lost;
        stateTimer = 0;
        agent.speed = defaultSpeed; // 恢复正常速度
        //if (alertUI) alertUI.SetActive(false);//丢失目标后隐藏感叹号
    }

    void EnterPatrolState()
    {
        Debug.Log("回归巡逻");
        currentState = EnemyState.Patrol;
        agent.isStopped = false;
        if (alertUI) alertUI.SetActive(false); // 隐藏UI
        MoveToNextPatrolLocation(); // 重新寻找最近的巡逻点
    }

    // 核心视线检测：结合触发器和射线
    void CheckLineOfSight()
    {
        // 发射射线检测是否有墙遮挡
        Vector3 direction = player.position - transform.position;
        RaycastHit hit;

        // 向玩家方向发射射线，如果打中玩家且没有障碍物
        if (Physics.Raycast(transform.position + Vector3.up, direction.normalized, out hit, GetComponent<SphereCollider>().radius))
        {
            if (hit.collider.gameObject.name == "Player")
            {
                // 看见玩家了！

                // 如果当前是巡逻，进入警戒
                if (currentState == EnemyState.Patrol)
                {
                    EnterAlertState();
                }
                // 如果当前是丢失状态（比如玩家躲起来又出来了），直接追击
                else if (currentState == EnemyState.Lost)
                {
                    EnterChaseState();
                }
                return;
            }
        }

        // 视线被遮挡（虽然在触发器内，但隔着墙）
        // 如果之前在追击或警戒，现在看不到了，进入丢失逻辑
        if (currentState == EnemyState.Chase || currentState == EnemyState.Alert)
        {
            EnterLostState();
        }
    }

    void InitializePatrolRoute()
    {
        foreach (Transform child in patrolRoute)
        {
            locations.Add(child);
        }
    }

    void MoveToNextPatrolLocation()
    {
        if (locations.Count == 0) return;
        agent.destination = locations[locationIndex].position;
        locationIndex = (locationIndex + 1) % locations.Count;
    }

    // 触发器进入：只负责标记“玩家在范围内”
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            playerInTrigger = true;
        }
    }

    // 触发器退出：彻底丢失，直接进入Lost状态
    void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            playerInTrigger = false;
            // 只有当还在追击或警戒时才转入丢失状态
            if (currentState == EnemyState.Chase || currentState == EnemyState.Alert)
            {
                EnterLostState();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Bullet"))
        {
            EnemyLives -= 25;
            Destroy(collision.gameObject); // 销毁子弹

            // 被攻击后直接进入追击状态（所谓的仇恨机制）
            EnterChaseState();
        }
    }
}