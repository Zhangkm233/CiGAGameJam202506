using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public GameObject tilePrefab; // 预制体引用

    public void GenTiles() {
        for (int i = 0;i < GameData.mapHeight;i++) {
            for (int j = 0;j < GameData.mapWidth;j++) {
                GameObject tile = Instantiate(tilePrefab,new Vector3(j,i,0),Quaternion.identity);
                //我调整了摄像机的坐标来对齐
                tile.transform.SetParent(transform); // 设置父物体为TileGenerator
                Tile tileComponent = tile.GetComponent<Tile>();
                if (tileComponent != null) {
                    tileComponent.x = j; // 设置格子在地图中的x坐标
                    tileComponent.y = i; // 设置格子在地图中的y坐标
                    tileComponent.RenameSelf(); // 重命名格子
                }
            }
        }
    }
}
