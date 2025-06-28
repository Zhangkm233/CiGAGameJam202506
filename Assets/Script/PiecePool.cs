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

        // �ؼ������õ���״̬
        enemy.SetActive(true);
        enemy.transform.SetParent(null); // ȡ��ʱ��������壬������Ϊ���̸�����
                                         // ��������������ö�����״̬��
        return enemy;
    }

    public void ReturnEnemy(GameObject enemy) {
        if (enemy != null) {
            // �ؼ����������õ���״̬
            enemy.SetActive(false);
            enemy.transform.SetParent(transform);
            // ��������λ�õ��������ĳ����ȫ��
            enemy.transform.position = new Vector3(-9999,-9999,0);
            // ��������������ö�����״̬��
            _enemies.Enqueue(enemy);
        }
    }
}
