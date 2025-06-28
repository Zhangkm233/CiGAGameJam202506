using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePool : MonoBehaviour
{
    //�����
    private Queue<GameObject> _enemies = new();

    public GameObject enemyPrefab; // ����Ԥ��������
    public int initialPoolSize = 0; // ��ʼ�ش�С
    public int maxPoolSize = 120;
    public int minPoolSize = 5; // ��С�ش�С

    private void Start() {
        // ��ʼ�����˶����
        for (int i = 0;i < initialPoolSize;i++) {
            CreateEnemy();
        }
    }
    private GameObject CreateEnemy() {
        _enemies.Enqueue(Instantiate(enemyPrefab)); // �������˲���ӵ�����
        GameObject enemy = _enemies.Peek();
        enemy.transform.SetParent(transform); // ���ø�����Ϊ��ǰ�����
        enemy.SetActive(false); // ��ʼʱ����������Ϊ���ɼ�
        return enemy;
    }

    public GameObject GetEnemy() {
        if (_enemies.Count > 0) {
            GameObject enemy = _enemies.Dequeue(); // �ӳ��л�ȡһ������
            enemy.SetActive(true); // ����������Ϊ�ɼ�
            return enemy;
        } else if (_enemies.Count < maxPoolSize) {
            return CreateEnemy(); // �������û�е�����δ�ﵽ���ش�С���򴴽��µĵ���
        }
        return null; // ���������������null
    }

    // ���ص��˵�����
    public void ReturnEnemy(GameObject enemy) {
        if (enemy != null) {
            enemy.SetActive(false); // ����������Ϊ���ɼ�
            _enemies.Enqueue(enemy); // �����˷��ص�����
        }
    }
}
