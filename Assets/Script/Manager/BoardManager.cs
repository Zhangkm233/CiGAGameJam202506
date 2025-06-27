using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [SerializeField] private int boardWidth = GameData.mapWidth;
    [SerializeField] private int boardHeight = GameData.mapHeight;
    [SerializeField] public Vector3 boardOrigin = Vector3.zero;
    [SerializeField] private float cellSize = 1f;

    [Header("����Prefab")]
    public GameObject generalPrefab;// ����Ԥ����
    public GameObject pawnPrefab;// ����Ԥ����

    [Header("�Ը�����")]
    public Personality PersonalityGeneral;
    public List<Personality> pawnPersonalities;

    private Dictionary<Vector2Int,Piece> _pieceDict = new(); // ���������ӵ��ֵ䣬��Ϊ�������ֵ꣬Ϊ���Ӷ���

    // ʮ�ֱ�Ǹ����ˮƽ��������copliot�����˴󲿷ִ���
    // ���޸ĺ���ܻ���һЩ����������ɾ����������д���

    // �������ӻ���
    private List<Tile> _highlightedTiles = new();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ��ȡָ�����������ϵ�����
    public Piece GetPieceAtPosition(Vector2Int pos) {
        _pieceDict.TryGetValue(pos,out var piece);
        return piece;
    }

    // ������ӵ�����
    public void AddPiece(Piece piece,Vector2Int pos) {
        if (piece == null) return;
        _pieceDict[pos] = piece;
        piece.BoardPosition = pos;
        // ����Tile
        Tile tile = GetTileAtPosition(pos);
        if (tile != null) tile.unitOccupied = piece;
    }

    // �������Ƴ�����
    public void RemovePiece(Vector2Int pos) {
        if (_pieceDict.TryGetValue(pos,out var piece)) {
            // ���Tile����
            Tile tile = GetTileAtPosition(pos);
            if (tile != null && tile.unitOccupied == piece)
                tile.unitOccupied = null;
            _pieceDict.Remove(pos);
        }
    }

    // ��ȡָ��λ�õ�Tile
    public Tile GetTileAtPosition(Vector2Int pos) {
        string tileName = $"Tile_{pos.x}_{pos.y}";
        GameObject tileObj = GameObject.Find(tileName);
        if (tileObj != null)
            return tileObj.GetComponent<Tile>();
        return null;
    }

    // ��������ת��������
    public Vector3 GetWorldPosition(Vector2Int boardPos) {
        return boardOrigin + new Vector3(boardPos.x * cellSize,boardPos.y * cellSize,0);
    }

    // ��������ת��������
    public Vector2Int GetBoardPosition(Vector3 worldPos) {
        Vector3 local = worldPos - boardOrigin;
        int x = Mathf.RoundToInt(local.x / cellSize);
        int y = Mathf.RoundToInt(local.y / cellSize);
        return new Vector2Int(x,y);
    }

    // ������������Ƿ�����Ч��Χ��
    public bool IsInBounds(Vector2Int pos) {
        return pos.x >= 0 && pos.x < boardWidth && pos.y >= 0 && pos.y < boardHeight;
    }

    // ������ʾ���ƶ�/������λ��
    public void HighlightMoves(List<Vector2Int> moves) {
        ClearHighlights();
        foreach (var pos in moves) {
            Tile tile = GetTileAtPosition(pos);
            if (tile != null) {
                tile.GetComponent<Tile>().SetHighlight(); // ���ø������
                _highlightedTiles.Add(tile);
            }
        }
    }

    // ������и���
    public void ClearHighlights() {
        foreach (var tile in _highlightedTiles) {
            if (tile != null) {
                tile.GetComponent<Tile>().SetDefault();
            }
        }
        _highlightedTiles.Clear();
    }

    // �ƶ����ӣ�����״̬�����ã�
    public void MovePiece(Piece piece,Vector2Int targetPos) {
        if (piece == null) return;
        Vector2Int oldPos = piece.BoardPosition;
        RemovePiece(oldPos);

        // ���Ŀ����ез����ӣ��ȴ�����
        Piece targetPiece = GetPieceAtPosition(targetPos);
        if (targetPiece != null && targetPiece.Type != piece.Type) {
            AttackPiece(piece,targetPiece);
        }

        // �ƶ�����
        AddPiece(piece,targetPos);
        piece.transform.position = GetWorldPosition(targetPos);
    }

    // ���ӹ���������״̬�����ã�
    public void AttackPiece(Piece attacker,Piece target) {
        if (attacker == null || target == null) return;
        // ���������߼�
        target.StateMachine?.ChangeState(new PieceDeadState(target));
        RemovePiece(target.BoardPosition);
    }

    // ��ʼ������
    public void InitializeBoard() {
        // �����������
        foreach (var kv in _pieceDict) {
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);
        }
        _pieceDict.Clear();

        // �������Tile��unitOccupied
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("TileGameObject");
        if (tiles != null) {
            foreach (GameObject t in tiles) {
                Tile tile = t.GetComponent<Tile>();
                if (tile != null) tile.unitOccupied = null;
            }
        }

        // 1. ���ɡ������壬������������
        Vector2Int generalPos = new Vector2Int(boardWidth / 2,boardHeight / 2);
        GameObject generalObj = Instantiate(generalPrefab,GetWorldPosition(generalPos),Quaternion.identity,transform);
        Piece generalPiece = generalObj.GetComponent<Piece>();
        generalPiece.InitializePiece(PersonalityGeneral,generalPos);
        generalPiece.CurrentMovementCount = 0; // ���岻���ƶ�
        AddPiece(generalPiece,generalPos);

        // 2. ����������ʼ���壬������ԣ������Tile
        List<Vector2Int> availablePositions = new();
        for (int x = 0;x < boardWidth;x++) {
            for (int y = 0;y < boardHeight;y++) {
                Vector2Int pos = new Vector2Int(x,y);
                if (pos == generalPos) continue;
                Tile tile = GetTileAtPosition(pos);
                if (tile != null && !tile.isObstacle && tile.unitOccupied == null)
                    availablePositions.Add(pos);
            }
        }

        for (int i = 0;i < 2;i++) {
            if (availablePositions.Count == 0) break;
            int idx = Random.Range(0,availablePositions.Count);
            Vector2Int pawnPos = availablePositions[idx];
            availablePositions.RemoveAt(idx);
            GameObject pawnObj = Instantiate(pawnPrefab,GetWorldPosition(pawnPos),Quaternion.identity,transform);
            Piece pawnPiece = pawnObj.GetComponent<Piece>();
            Personality randomPersonality = GetRandomPersonality();
            pawnPiece.InitializePiece(randomPersonality,pawnPos);
            AddPiece(pawnPiece,pawnPos);
        }
    }

    // �����ȡһ�������Ը�
    // �����������Ϲ���
    private Personality GetRandomPersonality() {
        if (pawnPersonalities != null && pawnPersonalities.Count > 0) {
            int idx = Random.Range(0,pawnPersonalities.Count);
            return pawnPersonalities[idx];
        }
        return null;
    }
    // ������������Ƿ�Ϸ�����Խ���Ҳ����ϰ���
    public bool IsValidBoardPosition(Vector2Int pos) {
        if (!IsInBounds(pos))
            return false;
        Tile tile = GetTileAtPosition(pos);
        if (tile == null || tile.isObstacle)
            return false;
        return true;
    }
}
