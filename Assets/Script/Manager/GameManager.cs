using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Camera mainCamera;
    public GameStateMachine StateMachine { get; private set; } // ��Ϸ��״̬��

    public void Awake() {
        this.GetComponent<TileGenerator>().GenTiles(); // ���ɸ���
    }
    public void Start() {
        mainCamera = Camera.main; // ��ȡ�������
        this.GetComponent<BoardManager>().InitializeBoard(); // ��ʼ������
    }

    public void Update() {
        if (Input.GetMouseButtonDown(0)) { // ������������
            HandleMouseClick();
        }
    }

    // ����������¼�
    // TODO: ���ƴ��� Ӧ����Ҫ��״̬�� ���Ҽ��������� ʮ�ֱ�Ǹ
    public void HandleMouseClick() {
        GameObject target = TileTarget();
        if (target != null) {
            Debug.Log("Clicked on: " + target.name);
            if (target.GetComponent<Piece>() != null) {
                Piece piece = target.GetComponent<Piece>();
                // �������������ӣ��л���ѡ��״̬
                piece.StateMachine.ChangeState(new PieceSelectedState(piece));
            }
        } else {
            Debug.Log("Clicked on nothing");
        }
    }

    // ��ȡ�������Tile��Piece����
    public GameObject TileTarget() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
        if (hit.collider != null) {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null) {
                return tile.gameObject; // ���ر������Tile����
            }
            Piece piece = hit.collider.GetComponent<Piece>();
            if (piece != null) {
                return piece.gameObject; // ���ر������Piece����
            }
        }
        return null;
    }
}