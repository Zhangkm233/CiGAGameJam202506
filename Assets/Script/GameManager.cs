using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Camera mainCamera;

    public void Start() {
        mainCamera = Camera.main; // ��ȡ�������
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0)) { // ������������
            GameObject target = TileTarget();
            if (target != null) {
                Debug.Log("Clicked on: " + target.name);
                // �����������ӶԵ��Tile��Unit�Ĵ����߼�
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
                return tile.gameObject; // ���ر������Tile����
            }
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit != null) {
                return unit.gameObject; // ���ر������Unit����
            }
        }
        return null;
    }
}