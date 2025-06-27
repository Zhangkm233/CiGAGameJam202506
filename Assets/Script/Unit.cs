using UnityEngine;

public class Unit : MonoBehaviour
{
    public Tile currentTile; //当前所在的格子
    public int x = 0; //单位在地图中的x坐标
    public int y = 0; //单位在地图中的y坐标
    public void RenameSelf() {
        this.name = "Unit_" + x + "_" + y; //重命名单位
    }
    public void MoveTo(int newX,int newY) {
        x = newX;
        y = newY;
        RenameSelf();
        currentTile = GameObject.Find("Tile_" + x + "_" + y).GetComponent<Tile>(); //找到新的格子
        if (currentTile != null) {
            currentTile.unitOccupied = this; //设置新的格子上面的单位
            transform.position = new Vector3(x - 5,y - 5,0); //更新单位的位置
        } else {
            Debug.LogError("Tile not found at position: " + x + ", " + y);
        }
    }
}
