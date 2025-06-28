using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
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
    public GameObject cannonPrefab;// 炮棋预制体
    public GameObject elephantPrefab;// 象棋预制体
    public GameObject horsePrefab;// 马棋预制体
    public GameObject rookPrefab;// 车棋预制体
    public GameObject enemyPrefab;// 敌人棋预制体

    [Header("性格配置")]
    public Personality PersonalityGeneral;
    public List<Personality> pawnPersonalities;

    // --- 新增：障碍物设置 ---
    [Header("障碍物设置")]
    [Tooltip("在回合开始时刷新障碍物的概率（0到1）。")]
    [Range(0f, 1f)]
    [SerializeField] private float obstacleRefreshChancePerTurn = 0.5f;
    [Tooltip("游戏开始时生成的障碍物数量。")]
    [SerializeField] private int initialObstacleCount = 12;
    [Tooltip("每次刷新时尝试移除的现有障碍物的百分比。")]
    [Range(0f, 1f)]
    [SerializeField] private float obstacleRemovalRate = 0.2f;
    [Tooltip("每次刷新时尝试将空格子变为障碍物的百分比。")]
    [Range(0f, 1f)]
    [SerializeField] private float obstacleAdditionRate = 0.1f;
    // --- 新增结束 ---

    private Dictionary<Vector2Int, Piece> _pieceDict = new();
    // 棋盘上棋子的字典
    private List<Tile> _highlightedTiles = new(); // 高亮格子缓存
    private List<Piece> _friendlyPieces = new();
    // 友方棋子列表
    private List<Piece> _enemyPieces = new();
    // 敌人棋子列表

    // --- 图块缓存 ---
    private Dictionary<Vector2Int, Tile> _tileDict = new();
    // 用于快速查找图块的缓存

    // 游戏状态管理
    private int _currentTurn = 1; // 当前回合数
    private int _newPieceCountdown = 3; // 新棋子倒计时
    private int _totalNewPiecesGained = 0;
    // 已获得的新棋子总数
    private int _enemiesPerTurn = 1;
    // 每回合出现的敌人数量

    // 鼠标输入相关
    private Piece _selectedPiece = null;
    private Camera _camera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _camera = Camera.main;
            // --- 为提高性能，在Awake时缓存所有图块 ---
            CacheAllTiles();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandleMouseInput();
    }

    // --- 将所有图块缓存到一个字典中以便快速访问 ---
    private void CacheAllTiles()
    {
        _tileDict.Clear();
        GameObject[] tileObjects = GameObject.FindGameObjectsWithTag("TileGameObject");
        if (tileObjects != null)
        {
            foreach (GameObject tileObj in tileObjects)
            {
                Tile tile = tileObj.GetComponent<Tile>();
                if (tile != null)
                {
                    // 将世界坐标转换为棋盘坐标作为字典的键
                    Vector2Int boardPos = GetBoardPosition(tile.transform.position);
                    _tileDict[boardPos] = tile;
                }
            }
        }
    }

    // --- 使用图块缓存以实现更快的查找 ---
    public Tile GetTileAtPosition(Vector2Int pos)
    {
        _tileDict.TryGetValue(pos, out var tile);
        return tile;
    }

    // 初始化棋盘
    public void InitializeBoard()
    {
        // 清空棋子数据
        foreach (var kv in _pieceDict)
        {
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);
        }
        _pieceDict.Clear();
        _friendlyPieces.Clear();
        _enemyPieces.Clear();

        // 清空所有Tile的unitOccupied
        foreach (var tile in _tileDict.Values)
        {
            if (tile != null) tile.unitOccupied = null;
        }

        // --- 为新游戏生成初始障碍物 ---
        GenerateInitialObstacles();

        // 重置游戏状态
        _currentTurn = 1;
        _newPieceCountdown = 3;
        _totalNewPiecesGained = 0;
        _enemiesPerTurn = 1;

        // 1. 生成"将"棋，放在棋盘中心
        Vector2Int generalPos = new Vector2Int(boardWidth / 2, boardHeight / 2);
        GameObject generalObj = Instantiate(generalPrefab, GetWorldPosition(generalPos), Quaternion.identity, transform);
        Piece generalPiece = generalObj.GetComponent<Piece>();
        generalPiece.InitializePiece(PersonalityGeneral, generalPos);
        generalPiece.CurrentMovementCount = 0;
        AddPiece(generalPiece, generalPos);

        // 2. 生成两个初始卒棋
        List<Vector2Int> availablePositionsNearGeneral = GetAvailablePositionsNearGeneral();
        List<Vector2Int> selectedPawnPositions = new List<Vector2Int>();
        while (selectedPawnPositions.Count < 2 && availablePositionsNearGeneral.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePositionsNearGeneral.Count);
            Vector2Int pos = availablePositionsNearGeneral[randomIndex];
            selectedPawnPositions.Add(pos);
            availablePositionsNearGeneral.RemoveAt(randomIndex);
        }

        foreach (Vector2Int pawnPos in selectedPawnPositions)
        {
            GameObject pawnObj = Instantiate(pawnPrefab, GetWorldPosition(pawnPos), Quaternion.identity, transform);
            Piece pawnPiece = pawnObj.GetComponent<Piece>();
            Personality pawnPersonality = GetRandomPersonality();
            pawnPiece.InitializePiece(pawnPersonality, pawnPos);
            AddPiece(pawnPiece, pawnPos);
        }
    }

    // --- 生成初始障碍物集合的方法 ---
    private void GenerateInitialObstacles()
    {
        // 清除所有先前的障碍物
        foreach (var tile in _tileDict.Values)
        {
            if (tile.isObstacle)
            {
                tile.SetObstacle(false);
            }
        }

        // 获取所有可以成为障碍物的图块列表
        List<Tile> validTiles = new List<Tile>();
        Vector2Int generalPos = new Vector2Int(boardWidth / 2, boardHeight / 2);
        // “将”的起始位置

        foreach (var kvp in _tileDict)
        {
            // 确保“将”的起始图块及其附近区域不是障碍物
            if (Vector2Int.Distance(kvp.Key, generalPos) > 2)
            {
                validTiles.Add(kvp.Value);
            }
        }

        // 根据棋盘大小计算最大障碍物数量
        int maxFromBoardFractionConstraint = (boardWidth * boardHeight) / 3;
        int countToGenerate = Mathf.Min(initialObstacleCount, maxFromBoardFractionConstraint);

        for (int i = 0; i < countToGenerate; i++)
        {
            if (validTiles.Count == 0) break;
            int randIdx = Random.Range(0, validTiles.Count);
            Tile tileToSet = validTiles[randIdx];
            tileToSet.SetObstacle(true);
            validTiles.RemoveAt(randIdx);
            // 确保我们不会重复选择同一个图块
        }
    }

    // --- 在棋盘上随机更新障碍物的方法 ---
    private void UpdateObstacles()
    {
        // 1. 获取所有未被占据的图块，并将它们分为障碍物和普通图块。
        List<Tile> unoccupiedNormalTiles = new List<Tile>();
        List<Tile> unoccupiedObstacleTiles = new List<Tile>();

        foreach (var tile in _tileDict.Values)
        {
            if (tile.unitOccupied == null) // 图块必须为空才能被更改
            {
                if (tile.isObstacle)
                    unoccupiedObstacleTiles.Add(tile);
                else
                    unoccupiedNormalTiles.Add(tile);
            }
        }

        // 2. 根据约束条件计算允许的最大障碍物数量
        int totalPieceCount = _friendlyPieces.Count + _enemyPieces.Count;
        int maxFromPieceConstraint = (boardWidth * boardHeight) - (totalPieceCount * 2);
        int maxFromBoardFractionConstraint = (boardWidth * boardHeight) / 3;
        int maxTotalObstacles = Mathf.Min(maxFromPieceConstraint, maxFromBoardFractionConstraint);

        // 计算当前棋盘上所有障碍物的总数
        int actualTotalObstacleCount = 0;
        foreach (var tile in _tileDict.Values)
        {
            if (tile.isObstacle)
            {
                actualTotalObstacleCount++;
            }
        }

        // 判断是否需要强制更新（当实际障碍物数量超过最大允许数量时）
        bool forceUpdate = (actualTotalObstacleCount > maxTotalObstacles);

        // 如果不需要强制更新，并且随机事件未触发，则直接返回
        if (!forceUpdate && Random.Range(0f, 1f) > obstacleRefreshChancePerTurn)
        {
            return; // 未通过概率检查，本回合无变化。
        }

        // --- 实际的障碍物刷新逻辑，只有在forceUpdate为true或随机事件触发时才会执行 ---

        // 3. 随机移除一些现有的障碍物
        // 期望根据移除率移除的障碍物数量
        int obstaclesToRemoveByRate = Mathf.FloorToInt(unoccupiedObstacleTiles.Count * obstacleRemovalRate);
        // 为了满足约束至少需要移除的障碍物数量
        int neededToRemoveToMeetConstraint = Mathf.Max(0, actualTotalObstacleCount - maxTotalObstacles);

        // 实际需要移除的障碍物数量取两者中的最大值，以确保满足约束
        int obstaclesToReallyRemove = Mathf.Max(obstaclesToRemoveByRate, neededToRemoveToMeetConstraint);

        for (int i = 0; i < obstaclesToReallyRemove; i++)
        {
            if (unoccupiedObstacleTiles.Count == 0) break; // 如果没有可移除的障碍物则停止
            int randIdx = Random.Range(0, unoccupiedObstacleTiles.Count);
            Tile tileToClear = unoccupiedObstacleTiles[randIdx];
            tileToClear.SetObstacle(false); // 将障碍物变回正常格子
            unoccupiedNormalTiles.Add(tileToClear);
            unoccupiedObstacleTiles.RemoveAt(randIdx);
            actualTotalObstacleCount--; // 更新实际障碍物总数
        }

        // 4. 在遵循总数上限的前提下，随机添加新的障碍物
        int obstaclesToAdd = Mathf.FloorToInt(unoccupiedNormalTiles.Count * obstacleAdditionRate);
        for (int i = 0; i < obstaclesToAdd; i++)
        {
            // 如果已达到允许的最大障碍物数量，则停止添加
            if (actualTotalObstacleCount >= maxTotalObstacles) break;
            if (unoccupiedNormalTiles.Count == 0) break; // 如果没有空闲的正常格子则停止
            int randIdx = Random.Range(0, unoccupiedNormalTiles.Count);
            Tile tileToSet = unoccupiedNormalTiles[randIdx];
            tileToSet.SetObstacle(true); // 将正常格子变为障碍物
            unoccupiedObstacleTiles.Add(tileToSet);
            unoccupiedNormalTiles.RemoveAt(randIdx);
            actualTotalObstacleCount++; // 更新实际障碍物总数
        }
    }

    // 开始新回合
    public void StartNewTurn()
    {
        _currentTurn++;
        // --- 在每回合开始时更新障碍物 ---
        UpdateObstacles();

        // 1. 敌人出现
        SpawnEnemies();

        // 更新移动次数
        UpdateEachPieceMove();
    }

    #region 
    // 处理鼠标输入
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector2Int boardPos = GetBoardPosition(mouseWorldPos);
            HandleBoardClick(boardPos);
        }
        // 右键或ESC取消选择
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelSelection();
        }
    }

    // 处理棋盘点击
    private void HandleBoardClick(Vector2Int boardPos)
    {
        if (!IsValidBoardPosition(boardPos))
        {
            CancelSelection();
            return;
        }

        Piece clickedPiece = GetPieceAtPosition(boardPos);
        if (_selectedPiece == null)
        {
            // 没有选中棋子，尝试选择点击的棋子
            if (clickedPiece != null && clickedPiece.Type != Piece.PieceType.Enemy && clickedPiece.CurrentMovementCount > 0)
            {
                SelectPiece(clickedPiece);
            }
        }
        else
        {
            // 已有选中棋子，尝试移动或攻击
            if (clickedPiece == _selectedPiece)
            {
                // 点击自己，取消选择
                CancelSelection();
            }
            else if (IsValidMoveForSelectedPiece(boardPos))
            {
                // 执行移动或攻击
                ExecuteMove(_selectedPiece, boardPos);
                CancelSelection();
            }
            else
            {
                // 无效移动，重新选择
                CancelSelection();
                if (clickedPiece != null && clickedPiece.Type != Piece.PieceType.Enemy && clickedPiece.CurrentMovementCount > 0)
                {
                    SelectPiece(clickedPiece);
                }
            }
        }
    }

    // 选择棋子
    private void SelectPiece(Piece piece)
    {
        _selectedPiece = piece;
        piece.StateMachine?.ChangeState(new PieceSelectedState(piece));
        List<Vector2Int> possibleMoves = piece.GetPossibleMoves();
        HighlightMoves(possibleMoves);
    }

    // 取消选择
    private void CancelSelection()
    {
        if (_selectedPiece != null)
        {
            _selectedPiece.StateMachine?.ChangeState(new PieceIdleState(_selectedPiece));
            _selectedPiece = null;
        }
        ClearHighlights();
    }

    // 检查是否为选中棋子的有效移动
    private bool IsValidMoveForSelectedPiece(Vector2Int targetPos)
    {
        if (_selectedPiece == null) return false;
        return _selectedPiece.IsValidMove(targetPos);
    }

    // 执行移动
    private void ExecuteMove(Piece piece, Vector2Int targetPos)
    {
        Piece targetPiece = GetPieceAtPosition(targetPos);
        if (targetPiece != null && targetPiece.Type != Piece.PieceType.Enemy) return;

        if (targetPiece != null)
        {
            // 攻击敌人
            piece.StateMachine?.ChangeState(new PieceAttackingState(piece, targetPiece, targetPos));
        }
        else if (targetPiece == null)
        {
            // 移动到空格
            piece.StateMachine?.ChangeState(new PieceMovingState(piece, targetPos));
        }
        // 更新棋子位置
        MovePiece(piece, targetPos);
    }

    // 获取指定棋盘坐标上的棋子
    public Piece GetPieceAtPosition(Vector2Int pos)
    {
        _pieceDict.TryGetValue(pos, out var piece);
        return piece;
    }

    // 添加棋子到棋盘
    public void AddPiece(Piece piece, Vector2Int pos)
    {
        if (piece == null) return;
        _pieceDict[pos] = piece;
        piece.BoardPosition = pos;

        // 添加到对应列表
        if (piece.Type == Piece.PieceType.Enemy)
        {
            _enemyPieces.Add(piece);
        }
        else
        {
            _friendlyPieces.Add(piece);
        }

        // 关联Tile
        Tile tile = GetTileAtPosition(pos);
        if (tile != null) tile.unitOccupied = piece;
    }

    // 从棋盘移除棋子
    public void RemovePiece(Vector2Int pos)
    {
        if (_pieceDict.TryGetValue(pos, out var piece))
        {
            // 从对应列表移除
            if (piece.Type == Piece.PieceType.Enemy)
            {
                _enemyPieces.Remove(piece);
            }
            else
            {
                _friendlyPieces.Remove(piece);
            }

            // 解除Tile关联
            Tile tile = GetTileAtPosition(pos);
            if (tile != null && tile.unitOccupied == piece)
                tile.unitOccupied = null;
            _pieceDict.Remove(pos);
        }
    }

    // 棋盘坐标转世界坐标
    public Vector3 GetWorldPosition(Vector2Int boardPos)
    {
        return boardOrigin + new Vector3(boardPos.x * cellSize, boardPos.y * cellSize, 0);
    }

    // 世界坐标转棋盘坐标
    public Vector2Int GetBoardPosition(Vector3 worldPos)
    {
        Vector3 local = worldPos - boardOrigin;
        int x = Mathf.RoundToInt(local.x / cellSize);
        int y = Mathf.RoundToInt(local.y / cellSize);
        return new Vector2Int(x, y);
    }

    // 检查棋盘坐标是否在有效范围内
    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < boardWidth && pos.y >= 0 && pos.y < boardHeight;
    }

    // 检查棋盘坐标是否合法（不越界且不是障碍格）
    public bool IsValidBoardPosition(Vector2Int pos)
    {
        if (!IsInBounds(pos))
            return false;
        Tile tile = GetTileAtPosition(pos);
        if (tile == null || tile.isObstacle) // 这个检查现在可以与动态障碍物一起工作
            return false;
        return true;
    }

    // 高亮显示可移动/攻击的位置
    public void HighlightMoves(List<Vector2Int> moves)
    {
        ClearHighlights();
        foreach (var pos in moves)
        {
            Tile tile = GetTileAtPosition(pos);
            if (tile != null)
            {
                tile.SetHighlight();
                _highlightedTiles.Add(tile);
            }
        }
    }

    // 清除所有高亮
    public void ClearHighlights()
    {
        foreach (var tile in _highlightedTiles)
        {
            if (tile != null)
            {
                tile.SetDefault();
            }
        }
        _highlightedTiles.Clear();
    }

    // 移动棋子（用于状态机调用）
    public void MovePiece(Piece piece, Vector2Int targetPos)
    {
        if (piece == null) return;
        Vector2Int oldPos = piece.BoardPosition;
        RemovePiece(oldPos);

        // 如果目标格有敌方棋子，先处理攻击
        Piece targetPiece = GetPieceAtPosition(targetPos);
        if (targetPiece != null && targetPiece.Type != piece.Type)
        {
            AttackPiece(piece, targetPiece);
        }

        // 移动棋子
        AddPiece(piece, targetPos);
        piece.MovingAnimation(oldPos, targetPos); // 使用平滑移动动画
        piece.CurrentMovementCount--;
        piece.transform.Find("PieceCanvas").Find("PieceMove").GetComponent<TMP_Text>().text = (piece.CurrentMovementCount).ToString();
    }

    // 棋子攻击（用于状态机调用）
    public void AttackPiece(Piece attacker, Piece target)
    {
        if (attacker == null || target == null) return;
        target.StateMachine?.ChangeState(new PieceDeadState(target));
        RemovePiece(target.BoardPosition);
        if (target.Type == Piece.PieceType.Pawn && IsGeneral(target))
        {
            GameOver();
        }
    }

    // 生成随机棋子
    private void SpawnRandomPiece()
    {
        List<Vector2Int> availablePositions = GetAvailablePositions();
        if (availablePositions.Count == 0) return;

        int idx = Random.Range(0, availablePositions.Count);
        Vector2Int pos = availablePositions[idx];
        Piece.PieceType randomType = GetRandomPieceType();
        GameObject prefab = GetPrefabForType(randomType);
        Personality randomPersonality = GetRandomPersonality();
        GameObject pieceObj = Instantiate(prefab, GetWorldPosition(pos), Quaternion.identity, transform);
        Piece piece = pieceObj.GetComponent<Piece>();
        piece.InitializePiece(randomPersonality, pos);
        AddPiece(piece, pos);
    }

    // 获取可用位置列表
    private List<Vector2Int> GetAvailablePositions()
    {
        List<Vector2Int> availablePositions = new();
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (GetPieceAtPosition(pos) == null)
                {
                    Tile tile = GetTileAtPosition(pos);
                    if (tile != null && !tile.isObstacle)
                        availablePositions.Add(pos);
                }
            }
        }
        return availablePositions;
    }

    // 获取将棋附近5x5的可用位置
    private List<Vector2Int> GetAvailablePositionsNearGeneral()
    {
        List<Vector2Int> availablePositions = new();
        Vector2Int generalPos = GetGeneralPosition();
        for (int x = generalPos.x - 2; x <= generalPos.x + 2; x++)
        {
            for (int y = generalPos.y - 2; y <= generalPos.y + 2; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (IsValidBoardPosition(pos) && GetPieceAtPosition(pos) == null)
                {
                    availablePositions.Add(pos);
                }
            }
        }
        return availablePositions;
    }

    // 获取将棋位置
    private Vector2Int GetGeneralPosition()
    {
        foreach (var piece in _friendlyPieces)
        {
            if (IsGeneral(piece))
            {
                return piece.BoardPosition;
            }
        }
        return new Vector2Int(boardWidth / 2, boardHeight / 2);
        // 默认中心位置
    }

    // 检查是否为将棋
    private bool IsGeneral(Piece piece)
    {
        return piece.CurrentMovementCount == 0 && piece.Type != Piece.PieceType.Enemy;
    }

    // 获取随机棋子类型
    private Piece.PieceType GetRandomPieceType()
    {
        Piece.PieceType[] types = {
            Piece.PieceType.Pawn,
            Piece.PieceType.Cannon,
            Piece.PieceType.Elephant,
            Piece.PieceType.Horse,
            Piece.PieceType.Rook
        };
        return types[Random.Range(0, types.Length)];
    }

    // 根据类型获取预制体
    private GameObject GetPrefabForType(Piece.PieceType type)
    {
        return type switch
        {
            Piece.PieceType.Pawn => pawnPrefab,
            Piece.PieceType.Cannon => cannonPrefab,
            Piece.PieceType.Elephant => elephantPrefab,
            Piece.PieceType.Horse => horsePrefab,
            Piece.PieceType.Rook => rookPrefab,
            _ => pawnPrefab
        };
    }

    // 随机获取一个性格
    private Personality GetRandomPersonality()
    {
        if (pawnPersonalities != null && pawnPersonalities.Count > 0)
        {
            int idx = Random.Range(0, pawnPersonalities.Count);
            return pawnPersonalities[idx];
        }
        return null;
    }

    // 结束玩家回合
    public void EndPlayerTurn()
    {
        StartCoroutine(EnemyTurnCoroutine());
    }

    public void UpdateEachPieceMove()
    {
        foreach (var piece in _friendlyPieces)
        {
            if (piece != null)
            {
                UpdatePieceMove(piece);
            }
        }
        foreach (var piece in _enemyPieces)
        {
            if (piece != null)
            {
                UpdatePieceMove(piece);
            }
        }
    }

    public void UpdatePieceMove(Piece piece)
    {
        piece.transform.Find("PieceCanvas").Find("PieceMove").GetComponent<TMP_Text>().text = (piece.CurrentMovementCount).ToString();
    }

    // 敌人回合协程
    private IEnumerator EnemyTurnCoroutine()
    {
        // 敌人移动
        foreach (var enemy in _enemyPieces.ToArray())
        {
            if (enemy != null)
            {
                MoveEnemyTowardsGeneral(enemy);
                yield return new WaitForSeconds(0.2f);
            }
        }

        // 检查新棋子
        ProcessNewPieceCountdown();

        // 回合结算
        ProcessTurnEnd();

        // 准备下一回合
        StartNewTurn();
    }

    // 生成敌人
    private void SpawnEnemies()
    {
        // 计算本回合应该出现的敌人数量
        int enemiesToSpawn = _enemiesPerTurn;
        if (_currentTurn % 2 == 0 && _currentTurn <= 20)
        {
            _enemiesPerTurn++;
        }
        else if (_currentTurn > 20)
        {
            _enemiesPerTurn++;
        }

        // 在边缘位置生成敌人
        List<Vector2Int> edgePositions = GetEdgePositions();
        for (int i = 0; i < enemiesToSpawn && edgePositions.Count > 0; i++)
        {
            int idx = Random.Range(0, edgePositions.Count);
            Vector2Int pos = edgePositions[idx];
            edgePositions.RemoveAt(idx);
            GameObject enemyObj = Instantiate(enemyPrefab, GetWorldPosition(pos), Quaternion.identity, transform);
            Piece enemyPiece = enemyObj.GetComponent<Piece>();
            enemyPiece.InitializePiece(null, pos);
            AddPiece(enemyPiece, pos);
        }
    }

    // 获取边缘位置
    private List<Vector2Int> GetEdgePositions()
    {
        List<Vector2Int> edgePositions = new();
        for (int x = 0; x < boardWidth; x++)
        {
            Vector2Int topPos = new Vector2Int(x, boardHeight - 1);
            Vector2Int bottomPos = new Vector2Int(x, 0);
            if (IsValidBoardPosition(topPos) && GetPieceAtPosition(topPos) == null)
                edgePositions.Add(topPos);
            if (IsValidBoardPosition(bottomPos) && GetPieceAtPosition(bottomPos) == null)
                edgePositions.Add(bottomPos);
        }

        for (int y = 1; y < boardHeight - 1; y++)
        {
            Vector2Int leftPos = new Vector2Int(0, y);
            Vector2Int rightPos = new Vector2Int(boardWidth - 1, y);
            if (IsValidBoardPosition(leftPos) && GetPieceAtPosition(leftPos) == null)
                edgePositions.Add(leftPos);
            if (IsValidBoardPosition(rightPos) && GetPieceAtPosition(rightPos) == null)
                edgePositions.Add(rightPos);
        }
        return edgePositions;
    }

    // 移动敌人朝向将棋
    private void MoveEnemyTowardsGeneral(Piece enemy)
    {
        Vector2Int generalPos = GetGeneralPosition();
        Vector2Int enemyPos = enemy.BoardPosition;
        Vector2Int direction = Vector2Int.zero;

        if (generalPos.x > enemyPos.x) direction.x = 1;
        else if (generalPos.x < enemyPos.x) direction.x = -1;
        if (generalPos.y > enemyPos.y) direction.y = 1;
        else if (generalPos.y < enemyPos.y) direction.y = -1;

        Vector2Int targetPos = enemyPos + direction;
        if (IsValidBoardPosition(targetPos))
        {
            Piece targetPiece = GetPieceAtPosition(targetPos);
            if (targetPiece == null)
            {
                MovePiece(enemy, targetPos);
            }
            else if (targetPiece.Type != Piece.PieceType.Enemy)
            {
                AttackPiece(enemy, targetPiece);
                MovePiece(enemy, targetPos);
            }
        }
    }

    // 处理新棋子倒计时
    private void ProcessNewPieceCountdown()
    {
        _newPieceCountdown--;
        if (_newPieceCountdown <= 0)
        {
            List<Vector2Int> nearPositions = GetAvailablePositionsNearGeneral();
            if (nearPositions.Count > 0)
            {
                int idx = Random.Range(0, nearPositions.Count);
                Vector2Int posToSpawn = nearPositions[idx];
                Piece.PieceType randomType = GetRandomPieceType();
                GameObject prefab = GetPrefabForType(randomType);
                Personality randomPersonality = GetRandomPersonality();
                GameObject pieceObj = Instantiate(prefab, GetWorldPosition(posToSpawn), Quaternion.identity, transform);
                Piece newPiece = pieceObj.GetComponent<Piece>();
                newPiece.InitializePiece(randomPersonality, posToSpawn);
                AddPiece(newPiece, posToSpawn);
            }
            _totalNewPiecesGained++;
            _newPieceCountdown = _totalNewPiecesGained + 3;
        }
    }

    // 处理回合结束
    private void ProcessTurnEnd()
    {
        foreach (var piece in _friendlyPieces)
        {
            if (piece != null)
            {
                piece.OnTurnStart();
            }
        }
        foreach (var piece in _friendlyPieces)
        {
            if (piece != null)
            {
                piece.OnTurnEnd();
            }
        }
    }

    // 游戏结束
    private void GameOver()
    {
        Debug.Log("游戏结束！将棋被击败！");
        // GameManager.Instance.ShowGameOverScreen();
    }

    public int GetCurrentTurn() { return _currentTurn; }
    public int GetNewPieceCountdown() { return _newPieceCountdown; }
    public List<Piece> GetFriendlyPieces() { return new List<Piece>(_friendlyPieces); }
    public List<Piece> GetEnemyPieces() { return new List<Piece>(_enemyPieces); }

    public List<Piece> GetPiecesInRange(Vector2Int center, int range)
    {
        List<Piece> piecesInRange = new();
        for (int x = center.x - range; x <= center.x + range; x++)
        {
            for (int y = center.y - range; y <= center.y + range; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (pos != center && IsValidBoardPosition(pos))
                {
                    Piece piece = GetPieceAtPosition(pos);
                    if (piece != null)
                    {
                        piecesInRange.Add(piece);
                    }
                }
            }
        }
        return piecesInRange;
    }

    public List<Piece> GetAdjacentPieces(Vector2Int center)
    {
        List<Piece> adjacentPieces = new();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in directions)
        {
            Vector2Int pos = center + dir;
            if (IsValidBoardPosition(pos))
            {
                Piece piece = GetPieceAtPosition(pos);
                if (piece != null)
                {
                    adjacentPieces.Add(piece);
                }
            }
        }
        return adjacentPieces;
    }
    #endregion
}