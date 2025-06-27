using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    [SerializeField] private int boardWidth = GameData.mapWidth;
    [SerializeField] private int boardHeight = GameData.mapHeight;
    [SerializeField] public Vector3 boardOrigin = Vector3.zero;
    [SerializeField] private float cellSize = 1f;

    [Header("棋子Prefab")]
    public GameObject generalPrefab;// 将棋预制体
    public GameObject pawnPrefab;// 卒棋预制体

    [Header("性格配置")]
    public Personality PersonalityGeneral;
    public List<Personality> pawnPersonalities;

    private Dictionary<Vector2Int,Piece> _pieceDict = new(); // 棋盘上棋子的字典，键为棋盘坐标，值为棋子对象

    // 十分抱歉，我水平不够，用copliot生成了大部分代码
    // 我修改后可能还有一些错误，请随意删减这里的所有代码

    // 高亮格子缓存
    private List<Tile> _highlightedTiles = new();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 获取指定棋盘坐标上的棋子
    public Piece GetPieceAtPosition(Vector2Int pos) {
        _pieceDict.TryGetValue(pos,out var piece);
        return piece;
    }

    // 添加棋子到棋盘
    public void AddPiece(Piece piece,Vector2Int pos) {
        if (piece == null) return;
        _pieceDict[pos] = piece;
        piece.BoardPosition = pos;
        // 关联Tile
        Tile tile = GetTileAtPosition(pos);
        if (tile != null) tile.unitOccupied = piece;
    }

    // 从棋盘移除棋子
    public void RemovePiece(Vector2Int pos) {
        if (_pieceDict.TryGetValue(pos,out var piece)) {
            // 解除Tile关联
            Tile tile = GetTileAtPosition(pos);
            if (tile != null && tile.unitOccupied == piece)
                tile.unitOccupied = null;
            _pieceDict.Remove(pos);
        }
    }

    // 获取指定位置的Tile
    public Tile GetTileAtPosition(Vector2Int pos) {
        string tileName = $"Tile_{pos.x}_{pos.y}";
        GameObject tileObj = GameObject.Find(tileName);
        if (tileObj != null)
            return tileObj.GetComponent<Tile>();
        return null;
    }

    // 棋盘坐标转世界坐标
    public Vector3 GetWorldPosition(Vector2Int boardPos) {
        return boardOrigin + new Vector3(boardPos.x * cellSize,boardPos.y * cellSize,0);
    }

    // 世界坐标转棋盘坐标
    public Vector2Int GetBoardPosition(Vector3 worldPos) {
        Vector3 local = worldPos - boardOrigin;
        int x = Mathf.RoundToInt(local.x / cellSize);
        int y = Mathf.RoundToInt(local.y / cellSize);
        return new Vector2Int(x,y);
    }

    // 检查棋盘坐标是否在有效范围内
    public bool IsInBounds(Vector2Int pos) {
        return pos.x >= 0 && pos.x < boardWidth && pos.y >= 0 && pos.y < boardHeight;
    }

    // 高亮显示可移动/攻击的位置
    public void HighlightMoves(List<Vector2Int> moves) {
        ClearHighlights();
        foreach (var pos in moves) {
            Tile tile = GetTileAtPosition(pos);
            if (tile != null) {
                tile.GetComponent<Tile>().SetHighlight(); // 设置高亮外观
                _highlightedTiles.Add(tile);
            }
        }
    }

    // 清除所有高亮
    public void ClearHighlights() {
        foreach (var tile in _highlightedTiles) {
            if (tile != null) {
                tile.GetComponent<Tile>().SetDefault();
            }
        }
        _highlightedTiles.Clear();
    }

    // 移动棋子（用于状态机调用）
    public void MovePiece(Piece piece,Vector2Int targetPos) {
        if (piece == null) return;
        Vector2Int oldPos = piece.BoardPosition;
        RemovePiece(oldPos);

        // 如果目标格有敌方棋子，先处理攻击
        Piece targetPiece = GetPieceAtPosition(targetPos);
        if (targetPiece != null && targetPiece.Type != piece.Type) {
            AttackPiece(piece,targetPiece);
        }

        // 移动棋子
        AddPiece(piece,targetPos);
        piece.transform.position = GetWorldPosition(targetPos);
    }

    // 棋子攻击（用于状态机调用）
    public void AttackPiece(Piece attacker,Piece target) {
        if (attacker == null || target == null) return;
        // 触发攻击逻辑
        target.StateMachine?.ChangeState(new PieceDeadState(target));
        RemovePiece(target.BoardPosition);
    }

    // 初始化棋盘
    public void InitializeBoard() {
        // 清空棋子数据
        foreach (var kv in _pieceDict) {
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);
        }
        _pieceDict.Clear();

        // 清空所有Tile的unitOccupied
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("TileGameObject");
        if (tiles != null) {
            foreach (GameObject t in tiles) {
                Tile tile = t.GetComponent<Tile>();
                if (tile != null) tile.unitOccupied = null;
            }
        }

        // 1. 生成“将”棋，放在棋盘中心
        Vector2Int generalPos = new Vector2Int(boardWidth / 2,boardHeight / 2);
        GameObject generalObj = Instantiate(generalPrefab,GetWorldPosition(generalPos),Quaternion.identity,transform);
        Piece generalPiece = generalObj.GetComponent<Piece>();
        generalPiece.InitializePiece(PersonalityGeneral,generalPos);
        generalPiece.CurrentMovementCount = 0; // 将棋不可移动
        AddPiece(generalPiece,generalPos);

        // 2. 生成两个初始卒棋，随机属性，随机空Tile
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

    // 随机获取一个卒棋性格
    // 这里的数据是瞎填的
    private Personality GetRandomPersonality() {
        if (pawnPersonalities != null && pawnPersonalities.Count > 0) {
            int idx = Random.Range(0,pawnPersonalities.Count);
            return pawnPersonalities[idx];
        }
        return null;
    }
    // 检查棋盘坐标是否合法（不越界且不是障碍格）
    public bool IsValidBoardPosition(Vector2Int pos) {
        if (!IsInBounds(pos))
            return false;
        Tile tile = GetTileAtPosition(pos);
        if (tile == null || tile.isObstacle)
            return false;
        return true;
    }
}
