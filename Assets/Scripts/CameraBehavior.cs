using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    // ① 声明一个 Vector3 变量来存储想要的 Main Camera 对象与 Player 对象之间的偏移距离。
    // 可以在 Inspector 面板中手动设置相机的偏移位置，因为 camOffset 变量是公共的。
    // 现有的默认值比较合适，当然你也可以尝试修改。
    public Vector3 camOffset = new Vector3(0, 1.2f, -2.6f);

    // ② 创建一个 Transform 变量来保存 Player 对象的变换信息。
    // 这使得我们可以获取位置、旋转和缩放信息。
    // 这些信息不应该能够从 CameraBehavior 脚本之外进行访问，所以将 target 变量设置为私有的。
    private Transform target;

    void Start()
    {
        // ③ 使用 GameObject.Find 方法在场景中按名称查找 Player 对象并获取 Player 对象的 transform 属性，这意味着存储在 target 变量中的 Player 对象的位置会在每一帧进行更新。
        target = GameObject.Find("Player").transform;
    }

    // ④ LateUpdate 是 MonoBehaviour 脚本提供的方法，就像 Start 和 Update 方法一样，LateUpdate 方法也在 Update 方法之后执行。由于 PlayerBehavior 脚本在 Update 方法中移动了 Player 对象，因此我们希望 CameraBehavior 脚本在移动操作完成之后执行；这样可以确保 target 变量引用的是最新位置。
    void LateUpdate()
    {
        // ⑤ 每帧都把相机的位置设置为 target.TransformPoint(camOffset)以产生跟随效果。
        // TransformPoint 方法用于计算并返回世界空间的相对位置。
        this.transform.position = target.TransformPoint(camOffset);

        // ⑥ LookAt 方法会在每一帧更新胶囊的旋转值，使其朝向传入的 Transform 对象(本例中的 target 变量)所在的位置。
        this.transform.LookAt(target);
    }
}
