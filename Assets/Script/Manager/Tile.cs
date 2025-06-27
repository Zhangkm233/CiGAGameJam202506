using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isObstacle { get; set; } //是否是障碍物，默认不是
    public int x { get; set; } //格子在地图中的x坐标
    public int y { get; set; } //格子在地图中的y坐标
    public Piece unitOccupied { get; set; } //格子上面的单位

    public Sprite highLightSprite;
    public Sprite defaultSprite;
    public void RenameSelf() {
        this.name = "Tile_" + x + "_" + y; //重命名格子
    }
    public void SetHighlight() {
        this.GetComponent<SpriteRenderer>().sprite = highLightSprite;
    }
    public void SetDefault() {
        this.GetComponent<SpriteRenderer>().sprite = defaultSprite; //恢复默认外观
    }
}
