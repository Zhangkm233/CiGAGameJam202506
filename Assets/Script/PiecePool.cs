using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePool : MonoBehaviour
{
    //对象池
    private Queue<GameObject> _enemies = new();

    public GameObject enemyPrefab; // 敌人预制体引用
    public int initialPoolSize = 0; // 初始池大小
    public int maxPoolSize = 120;
    public int minPoolSize = 5; // 最小池大小

    private void Start() {
        // 初始化敌人对象池
        for (int i = 0;i < initialPoolSize;i++) {
            CreateEnemy();
        }
    }
    private GameObject CreateEnemy() {
        _enemies.Enqueue(Instantiate(enemyPrefab)); // 创建敌人并添加到池中
        GameObject enemy = _enemies.Peek();
        enemy.transform.SetParent(transform); // 设置父物体为当前对象池
        enemy.SetActive(false); // 初始时将敌人设置为不可见
        return enemy;
    }

    public GameObject GetEnemy() {
        if (_enemies.Count > 0) {
            GameObject enemy = _enemies.Dequeue(); // 从池中获取一个敌人
            enemy.SetActive(true); // 将敌人设置为可见
            return enemy;
        } else if (_enemies.Count < maxPoolSize) {
            return CreateEnemy(); // 如果池中没有敌人且未达到最大池大小，则创建新的敌人
        }
        return null; // 如果池已满，返回null
    }

    // 返回敌人到池中
    public void ReturnEnemy(GameObject enemy) {
        if (enemy != null) {
            enemy.SetActive(false); // 将敌人设置为不可见
            _enemies.Enqueue(enemy); // 将敌人返回到池中
        }
    }
}
