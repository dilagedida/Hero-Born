using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    public GameBehavior gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameBehavior>();
    }

    // ① 当把另一个对象移至 Pickup_Item 且 isTrigger 处于关闭状态时，Unity 会自动调用 OnCollisionEnter 方法。
    // OnCollisionEnter 方法有一个参数用于存储 Collider 引用。
    // 注意 collision 变量的类型是 Collision 而不是 Collider。
    void OnCollisionEnter(Collision collision)
    {
        // ② Collision 类的 gameObject 属性用于保存对 GameObject 碰撞体的引用。可以使用 gameObject 属性获取游戏对象的名称并使用 if 语句检查碰撞体是否是 Player 对象。
        if (collision.gameObject.name == "Player")
        {
            // ③ 如果碰撞体是 Player 对象，就调用 Destroy 方法，该方法接收一个游戏对象作为参数。
            // 我们必须使整个 Pickup_Item 对象被销毁，而不仅仅是销毁 Capsule 对象。
            // 因为 ItemBehavior 脚本被附加到了 Capsule 对象上，所以可以使用 this.transform.gameObject 将 Pickup_Item 对象销毁。
            Destroy(this.transform.gameObject);
            // ④ 向控制台打印一条日志，指明已经收集了道具。
            Debug.Log("Item collected!");
            gameManager.Items += 1;
        }
    }
}
