using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public GameObject tilePrefab; // 预制体引用

    public void Start() {
        GenTiles(); // 调用生成格子的方法
    }

    public void GenTiles() {
        for (int i = 0;i < GameData.mapHeight;i++) {
            for (int j = 0;j < GameData.mapWidth;j++) {
                GameObject tile = Instantiate(tilePrefab,new Vector3(j - 5,i - 5,0),Quaternion.identity);
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
