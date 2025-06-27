using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Camera mainCamera;
    public GameStateMachine StateMachine { get; private set; } // 游戏的状态机

    public void Awake() {
        this.GetComponent<TileGenerator>().GenTiles(); // 生成格子
    }
    public void Start() {
        mainCamera = Camera.main; // 获取主摄像机
        this.GetComponent<BoardManager>().InitializeBoard(); // 初始化棋盘
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0)) { // 检测鼠标左键点击
            HandleMouseClick();
        }
    }

    // 处理鼠标点击事件
    // TODO: 完善代码 应该需要用状态机 但我技术力不够 十分抱歉
    public void HandleMouseClick() {
        GameObject target = TileTarget();
        if (target != null) {
            Debug.Log("Clicked on: " + target.name);
            if (target.GetComponent<Piece>() != null) {
                Piece piece = target.GetComponent<Piece>();
                // 如果点击的是棋子，切换到选中状态
                piece.StateMachine.ChangeState(new PieceSelectedState(piece));
            }
        } else {
            Debug.Log("Clicked on nothing");
        }
    }

    // 获取鼠标点击的Tile或Piece对象
    public GameObject TileTarget() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
        if (hit.collider != null) {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null) {
                return tile.gameObject; // 返回被点击的Tile对象
            }
            Piece piece = hit.collider.GetComponent<Piece>();
            if (piece != null) {
                return piece.gameObject; // 返回被点击的Piece对象
            }
        }
        return null;
    }
}