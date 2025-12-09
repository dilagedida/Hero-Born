using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehavior : MonoBehaviour
{
    private int _itemsCollected = 0;
    private int _playerHP = 100;
    public string labelText = "收集所有4个物品，然后赢得你的自由!";
    public int maxItems = 4;

    public bool showWinScreen = false;
    public bool showLossScreen = false;
    public int Items
    {
        get { return _itemsCollected; }
        set
        {
            _itemsCollected = value;
            if (_itemsCollected >= maxItems)
            {
                labelText = "你已经找到所有物品!";
                showWinScreen = true;
                Time.timeScale = 0f;
            }
            else
            {
                labelText = "找到物品，还有 " + (maxItems - _itemsCollected) + "个物品需要你去收集!";
            }
        }
    }
    public int HP
    {
        get { return _playerHP; }
        set
        {
            _playerHP = value;
            if (_playerHP <= 0)
            {
                labelText = "你想要再试一次？";
                showLossScreen = true;
                Time.timeScale = 0f;
            }
            else
            {
                labelText = "哦不，你受伤了！还剩 " + _playerHP + " 点生命值！";
            }
        }
    }
    void OnGUI()
    {
        GUI.Box(new Rect(20, 20, 150, 25), "玩家生命值: " + _playerHP);

        GUI.Box(new Rect(20, 50, 150, 25), "已收集的物品: " + _itemsCollected);

        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 50, 300, 50), labelText);
        if (showWinScreen)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), "你获胜了!"))
            {
                SceneManager.LoadScene(0);
                Time.timeScale = 1.0f;
            }
        }
        if (showLossScreen)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), "你失败了!"))
            {
                SceneManager.LoadScene(0);
                Time.timeScale = 1.0f;
            }
        }
    }

}
