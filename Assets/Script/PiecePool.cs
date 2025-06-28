using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePool : MonoBehaviour
{
    private Queue<GameObject> _enemies = new();

    public GameObject enemyPrefab;
    public int initialPoolSize = 0;
    public int maxPoolSize = 120;
    public int minPoolSize = 5;

    private void Start() {
        for (int i = 0;i < initialPoolSize;i++) {
            CreateEnemy();
        }
    }

    private GameObject CreateEnemy() {
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.transform.SetParent(transform);
        enemy.SetActive(false);
        _enemies.Enqueue(enemy);
        return enemy;
    }

    public GameObject GetEnemy() {
        GameObject enemy;
        if (_enemies.Count > 0) {
            enemy = _enemies.Dequeue();
        } else if (_enemies.Count + transform.childCount < maxPoolSize) {
            enemy = CreateEnemy();
        } else {
            return null;
        }

        // 关键：重置敌人状态
        enemy.SetActive(true);
        enemy.transform.SetParent(null); // 取出时解除父物体，或设置为棋盘父物体
                                         // 你可以在这里重置动画、状态等
        return enemy;
    }

    public void ReturnEnemy(GameObject enemy) {
        if (enemy != null) {
            // 关键：彻底重置敌人状态
            enemy.SetActive(false);
            enemy.transform.SetParent(transform);
            // 建议重置位置到池子外的某个安全点
            enemy.transform.position = new Vector3(-9999,-9999,0);
            // 你可以在这里重置动画、状态等
            _enemies.Enqueue(enemy);
        }
    }
}
