using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public Transform player;
    public Transform patrolRoute;
    public List<Transform> locations;
    private int locationIndex = 0;
    private NavMeshAgent agent;
    private int _lives = 3;

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
        player = GameObject.Find("Player").transform;
        InitializePatrolRoute();
        MoveToNexPatrolLocation();
    }
    void Update()
    {
        if (agent.remainingDistance < 0.2f && !agent.pathPending)
        {
            MoveToNexPatrolLocation();
        }
    }
    void InitializePatrolRoute()
    {
        foreach(Transform child in patrolRoute)
        {
            locations.Add(child);
        }
    }
    void MoveToNexPatrolLocation()
    {
        if (locations.Count == 0)
        {
            return;
        }
        agent.destination = locations[locationIndex].position;
        locationIndex = (locationIndex + 1) % locations.Count;
    }
    // ① 任何时候，当一个对象进入 Enemy 游戏对象的球形触发器时，OnTriggerEnter 方法就会被触发。
    // 类似于 OnCollisionEnter 方法，OnTriggerEnter 方法的参数用于存储对象的 Collider 组件的引用。
    // 注意参数对象的类型是 Collider 而不是 Collision。
    void OnTriggerEnter(Collider other)
    {
        // ② 使用 other 获取碰撞体对象的名称并使用 if 语句检查是不是 Player 对象。如果是，就输出 Player 对象位于危险区域的提示信息。
        if (other.name == "Player")
        {
            agent.destination = player.position;
            Debug.Log("Player detected - attack!");
        }
    }

    // ③ 当对象离开 Enemy 游戏对象的球形触发器时，触发 OnTriggerExit 方法。
    void OnTriggerExit(Collider other)
    {
        // ④ 使用 if 语句按名称检查离开球形触发器的对象。如果是 Player 对象，就将另一条信息打印到控制台，指示玩家现在是安全的。
        if (other.name == "Player")
        {
            Debug.Log("Player out of range, resume patrol");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Bullet(Clone)")
        {
            EnemyLives -= 1;
            Debug.Log("Critical hit!");
        }
    }
}
