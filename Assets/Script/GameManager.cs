using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Camera mainCamera;

    public void Start() {
        mainCamera = Camera.main; // 获取主摄像机
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0)) { // 检测鼠标左键点击
            GameObject target = TileTarget();
            if (target != null) {
                Debug.Log("Clicked on: " + target.name);
                // 在这里可以添加对点击Tile或Unit的处理逻辑
            } else {
                Debug.Log("Clicked on nothing");
            }
        }
    }
    public GameObject TileTarget() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
        if (hit.collider != null) {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null) {
                return tile.gameObject; // 返回被点击的Tile对象
            }
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit != null) {
                return unit.gameObject; // 返回被点击的Unit对象
            }
        }
        return null;
    }
}