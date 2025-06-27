using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isObstacle = false; //是否是障碍物，默认不是
    public int x = 0; //格子在地图中的x坐标
    public int y = 0; //格子在地图中的y坐标
    public Unit unitOccupied; //格子上面的单位
    public void RenameSelf() {
        this.name = "Tile_" + x + "_" + y; //重命名格子
    }
}
