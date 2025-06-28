using UnityEngine;
using System;
public class TileGenerator : MonoBehaviour
{
    public GameObject tilePrefab; // Ԥ��������

    [Obsolete("�Ѿ���tile��ӵ���Ϸ�У�����������")]
    public void GenTiles()
    {
        for (int i = 0; i < GameData.mapHeight; i++)
        {
            for (int j = 0; j < GameData.mapWidth; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(j * GameData.tileSize,i* GameData.tileSize, 0), Quaternion.identity);
                //�ҵ����������������������
                tile.transform.SetParent(transform.Find("TileParent")); // ���ø�����ΪGameManager��TileParent
                Tile tileComponent = tile.GetComponent<Tile>();
                if (tileComponent != null)
                {
                    tileComponent.x = j; // ���ø����ڵ�ͼ�е�x����
                    tileComponent.y = i; // ���ø����ڵ�ͼ�е�y����
                    tileComponent.RenameSelf(); // ����������
                }
            }
        }
    }
}
