using UnityEngine;
using System;
public class TileGenerator : MonoBehaviour
{
    public GameObject tilePrefab; // 预制体引用

    [Obsolete("已经将tile添加到游戏中，无须再生成")]
    public void GenTiles()
    {
        for (int i = 0; i < GameData.mapHeight; i++)
        {
            for (int j = 0; j < GameData.mapWidth; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(j * GameData.tileSize,i* GameData.tileSize, 0), Quaternion.identity);
                //我调整了摄像机的坐标来对齐
                tile.transform.SetParent(transform.Find("TileParent")); // 设置父物体为GameManager的TileParent
                Tile tileComponent = tile.GetComponent<Tile>();
                if (tileComponent != null)
                {
                    tileComponent.x = j; // 设置格子在地图中的x坐标
                    tileComponent.y = i; // 设置格子在地图中的y坐标
                    tileComponent.RenameSelf(); // 重命名格子
                }
            }
        }
    }
}
