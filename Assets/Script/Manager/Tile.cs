using UnityEngine;


public class Tile : MonoBehaviour
{
    // --- 状态属性 ---
    // 此图块是否为不可通行的障碍物
    public bool isObstacle { get; private set; }


    // 格子在地图中的x坐标
    public int x { get; set; }


    // 格子在地图中的y坐标
    public int y { get; set; }


    // 格子上面占用的单位
    public Piece unitOccupied { get; set; }

    // --- 外观属性 ---
    [Header("外观精灵 (Sprite)")]
    public Sprite highLightSprite; // 高亮状态
    public Sprite defaultSprite;   // 默认状态
    public Sprite obstacleSprite;  // 障碍物状态

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // 在开始时获取并缓存SpriteRenderer组件，提高性能
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // 根据坐标重命名游戏对象，方便在编辑器中识别
    public void RenameSelf()
    {
        this.name = "Tile_" + x + "_" + y; // 重命名格子
    }


    // 设置高亮外观
    public void SetHighlight()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = highLightSprite;
        }
    }


    // 恢复默认外观
    public void SetDefault()
    {
        if (spriteRenderer != null)
        {
            // 如果是障碍物，则恢复为障碍物，否则恢复为普通默认精灵
            spriteRenderer.sprite = isObstacle ? obstacleSprite : defaultSprite;
        }
    }


    // 设置格子的障碍物状态，并立即更新其视觉外观
    public void SetObstacle(bool status)
    {
        isObstacle = status;
        // 调用SetDefault()来根据新的状态更新为正确的外观
        SetDefault();
    }
}