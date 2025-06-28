using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    public GameObject cannonPrefab;// ����Ԥ����
    public GameObject elephantPrefab;// ����Ԥ����
    public GameObject horsePrefab;// ����Ԥ����
    public GameObject rookPrefab;// ����Ԥ����
    public GameObject enemyPrefab;// ������Ԥ����

    [Header("�Ը�����")]
    public Personality PersonalityGeneral;
    public List<Personality> pawnPersonalities;

    private Dictionary<Vector2Int, Piece> _pieceDict = new(); // ���������ӵ��ֵ�
    private List<Tile> _highlightedTiles = new(); // �������ӻ���
    private List<Piece> _friendlyPieces = new(); // �ѷ������б�
    private List<Piece> _enemyPieces = new(); // ���������б�

    // ��Ϸ״̬����
    private int _currentTurn = 1; // ��ǰ�غ���
    private int _newPieceCountdown = 3; // �����ӵ���ʱ
    private int _totalNewPiecesGained = 0; // �ѻ�õ�����������
    private int _enemiesPerTurn = 1; // ÿ�غϳ��ֵĵ�������

    // ����������
    private Piece _selectedPiece = null;
    private Camera _camera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _camera = Camera.main;
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

    // �����������
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector2Int boardPos = GetBoardPosition(mouseWorldPos);

            HandleBoardClick(boardPos);
        }

        // �Ҽ���ESCȡ��ѡ��
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelSelection();
        }
    }

    // �������̵��
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
            // û��ѡ�����ӣ�����ѡ����������
            if (clickedPiece != null && clickedPiece.Type != Piece.PieceType.Enemy && clickedPiece.CurrentMovementCount > 0)
            {
                SelectPiece(clickedPiece);
            }
        }
        else
        {
            // ����ѡ�����ӣ������ƶ��򹥻�
            if (clickedPiece == _selectedPiece)
            {
                // ����Լ���ȡ��ѡ��
                CancelSelection();
            }
            else if (IsValidMoveForSelectedPiece(boardPos))
            {
                // ִ���ƶ��򹥻�
                ExecuteMove(_selectedPiece, boardPos);
                CancelSelection();
            }
            else
            {
                // ��Ч�ƶ�������ѡ��
                CancelSelection();
                if (clickedPiece != null && clickedPiece.Type != Piece.PieceType.Enemy && clickedPiece.CurrentMovementCount > 0)
                {
                    SelectPiece(clickedPiece);
                }
            }
        }
    }

    // ѡ������
    private void SelectPiece(Piece piece)
    {
        _selectedPiece = piece;
        piece.StateMachine?.ChangeState(new PieceSelectedState(piece));
        List<Vector2Int> possibleMoves = piece.GetPossibleMoves();
        HighlightMoves(possibleMoves);
    }

    // ȡ��ѡ��
    private void CancelSelection()
    {
        if (_selectedPiece != null)
        {
            _selectedPiece.StateMachine?.ChangeState(new PieceIdleState(_selectedPiece));
            _selectedPiece = null;
        }
        ClearHighlights();
    }

    // ����Ƿ�Ϊѡ�����ӵ���Ч�ƶ�
    private bool IsValidMoveForSelectedPiece(Vector2Int targetPos)
    {
        if (_selectedPiece == null) return false;
        return _selectedPiece.IsValidMove(targetPos);
    }

    // ִ���ƶ�
    private void ExecuteMove(Piece piece, Vector2Int targetPos)
    {
        Piece targetPiece = GetPieceAtPosition(targetPos);
        if (targetPiece != null && targetPiece.Type != Piece.PieceType.Enemy) return;
        if (targetPiece != null)
        {
            // ��������
            piece.StateMachine?.ChangeState(new PieceAttackingState(piece, targetPiece, targetPos));
        }
        else if (targetPiece == null)
        {
            // �ƶ����ո�
            piece.StateMachine?.ChangeState(new PieceMovingState(piece, targetPos));
        }
        // TODO�� ��Ҫ�������ӵ��ƶ�����
        // ��������λ��
        MovePiece(piece, targetPos);
    }

    // ��ȡָ�����������ϵ�����
    public Piece GetPieceAtPosition(Vector2Int pos)
    {
        _pieceDict.TryGetValue(pos, out var piece);
        return piece;
    }

    // ������ӵ�����
    public void AddPiece(Piece piece, Vector2Int pos)
    {
        if (piece == null) return;

        _pieceDict[pos] = piece;
        piece.BoardPosition = pos;

        // ��ӵ���Ӧ�б�
        if (piece.Type == Piece.PieceType.Enemy)
        {
            _enemyPieces.Add(piece);
        }
        else
        {
            _friendlyPieces.Add(piece);
        }

        // ����Tile
        Tile tile = GetTileAtPosition(pos);
        if (tile != null) tile.unitOccupied = piece;
    }

    // �������Ƴ�����
    public void RemovePiece(Vector2Int pos)
    {
        if (_pieceDict.TryGetValue(pos, out var piece))
        {
            // �Ӷ�Ӧ�б��Ƴ�
            if (piece.Type == Piece.PieceType.Enemy)
            {
                _enemyPieces.Remove(piece);
            }
            else
            {
                _friendlyPieces.Remove(piece);
            }

            // ���Tile����
            Tile tile = GetTileAtPosition(pos);
            if (tile != null && tile.unitOccupied == piece)
                tile.unitOccupied = null;
            _pieceDict.Remove(pos);
        }
    }

    // ��ȡָ��λ�õ�Tile
    public Tile GetTileAtPosition(Vector2Int pos)
    {
        string tileName = $"Tile_{pos.x}_{pos.y}";
        GameObject tileObj = GameObject.Find(tileName);
        if (tileObj != null)
            return tileObj.GetComponent<Tile>();
        return null;
    }

    // ��������ת��������
    public Vector3 GetWorldPosition(Vector2Int boardPos)
    {
        return boardOrigin + new Vector3(boardPos.x * cellSize, boardPos.y * cellSize, 0);
    }

    // ��������ת��������
    public Vector2Int GetBoardPosition(Vector3 worldPos)
    {
        Vector3 local = worldPos - boardOrigin;
        int x = Mathf.RoundToInt(local.x / cellSize);
        int y = Mathf.RoundToInt(local.y / cellSize);
        return new Vector2Int(x, y);
    }

    // ������������Ƿ�����Ч��Χ��
    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < boardWidth && pos.y >= 0 && pos.y < boardHeight;
    }

    // ������������Ƿ�Ϸ�����Խ���Ҳ����ϰ���
    public bool IsValidBoardPosition(Vector2Int pos)
    {
        if (!IsInBounds(pos))
            return false;
        Tile tile = GetTileAtPosition(pos);
        if (tile == null || tile.isObstacle)
            return false;
        return true;
    }

    // ������ʾ���ƶ�/������λ��
    public void HighlightMoves(List<Vector2Int> moves)
    {
        ClearHighlights();
        foreach (var pos in moves)
        {
            Tile tile = GetTileAtPosition(pos);
            if (tile != null)
            {
                tile.SetHighlight(); // ���ø������
                _highlightedTiles.Add(tile);
            }
        }
    }

    // ������и���
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

    // �ƶ����ӣ�����״̬�����ã�
    public void MovePiece(Piece piece, Vector2Int targetPos)
    {
        if (piece == null) return;
        Vector2Int oldPos = piece.BoardPosition;
        RemovePiece(oldPos);

        // ���Ŀ����ез����ӣ��ȴ�����
        Piece targetPiece = GetPieceAtPosition(targetPos);
        if (targetPiece != null && targetPiece.Type != piece.Type)
        {
            AttackPiece(piece, targetPiece);
        }

        // �ƶ�����
        AddPiece(piece, targetPos);
        piece.MovingAnimation(oldPos, targetPos); // ʹ��ƽ���ƶ�����
        //piece.transform.position = GetWorldPosition(targetPos);
    }

    // ���ӹ���������״̬�����ã�
    public void AttackPiece(Piece attacker, Piece target)
    {
        if (attacker == null || target == null) return;
        // ���������߼�
        target.StateMachine?.ChangeState(new PieceDeadState(target));
        RemovePiece(target.BoardPosition);

        // ����Ƿ񹥻��˽���
        if (target.Type == Piece.PieceType.Pawn && IsGeneral(target)) // ���轫��Ҳ��Pawn���͵���������
        {
            GameOver();
        }
    }

    // ��ʼ������
    public void InitializeBoard()
    {
        // �����������
        foreach (var kv in _pieceDict)
        {
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);
        }
        _pieceDict.Clear();
        _friendlyPieces.Clear();
        _enemyPieces.Clear();

        // �������Tile��unitOccupied
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("TileGameObject");
        if (tiles != null)
        {
            foreach (GameObject t in tiles)
            {
                Tile tile = t.GetComponent<Tile>();
                if (tile != null) tile.unitOccupied = null;
            }
        }

        // ������Ϸ״̬
        _currentTurn = 1;
        _newPieceCountdown = 3;
        _totalNewPiecesGained = 0;
        _enemiesPerTurn = 1;

        // 1. ����"��"�壬������������
        Vector2Int generalPos = new Vector2Int(boardWidth / 2, boardHeight / 2);
        GameObject generalObj = Instantiate(generalPrefab, GetWorldPosition(generalPos), Quaternion.identity, transform);
        Piece generalPiece = generalObj.GetComponent<Piece>();
        generalPiece.InitializePiece(PersonalityGeneral, generalPos);
        generalPiece.CurrentMovementCount = 0; // ���岻���ƶ�
        AddPiece(generalPiece, generalPos);

        // 2. ����������ʼ���壬������ԣ������Tile
        for (int i = 0; i < 2; i++)
        {
            SpawnRandomPiece();
        }
    }

    // �����������
    private void SpawnRandomPiece()
    {
        List<Vector2Int> availablePositions = GetAvailablePositions();
        if (availablePositions.Count == 0) return;

        int idx = Random.Range(0, availablePositions.Count);
        Vector2Int pos = availablePositions[idx];

        // ���ѡ���������ͺ��Ը�
        Piece.PieceType randomType = GetRandomPieceType();
        GameObject prefab = GetPrefabForType(randomType);
        Personality randomPersonality = GetRandomPersonality();

        GameObject pieceObj = Instantiate(prefab, GetWorldPosition(pos), Quaternion.identity, transform);
        Piece piece = pieceObj.GetComponent<Piece>();
        piece.InitializePiece(randomPersonality, pos);
        AddPiece(piece, pos);
    }

    // ��ȡ����λ���б�
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

    // ��ȡ���帽��5x5�Ŀ���λ��
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

    // ��ȡ����λ��
    private Vector2Int GetGeneralPosition()
    {
        foreach (var piece in _friendlyPieces)
        {
            if (IsGeneral(piece))
            {
                return piece.BoardPosition;
            }
        }
        return new Vector2Int(boardWidth / 2, boardHeight / 2); // Ĭ������λ��
    }

    // ����Ƿ�Ϊ����
    private bool IsGeneral(Piece piece)
    {
        return piece.CurrentMovementCount == 0 && piece.Type != Piece.PieceType.Enemy;
    }

    // ��ȡ�����������
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

    // �������ͻ�ȡԤ����
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

    // �����ȡһ���Ը�
    private Personality GetRandomPersonality()
    {
        if (pawnPersonalities != null && pawnPersonalities.Count > 0)
        {
            int idx = Random.Range(0, pawnPersonalities.Count);
            return pawnPersonalities[idx];
        }
        return null;
    }

    // ��ʼ�»غ�
    public void StartNewTurn()
    {
        _currentTurn++;

        // 1. ���˳���
        SpawnEnemies();

        // 2. ��һغϣ�ͨ��UI��ť����������

        // 3. �����ƶ�
        // StartCoroutine(EnemyTurnCoroutine());
    }

    // ������һغ�
    public void EndPlayerTurn()
    {
        StartCoroutine(EnemyTurnCoroutine());
    }

    // ���˻غ�Э��
    private IEnumerator EnemyTurnCoroutine()
    {
        // �����ƶ�
        foreach (var enemy in _enemyPieces.ToArray()) // ʹ��ToArray�����޸ļ���ʱ������
        {
            if (enemy != null)
            {
                MoveEnemyTowardsGeneral(enemy);
                yield return new WaitForSeconds(0.5f); // �ӳ���ʾ�ƶ�����
            }
        }

        // ���������
        ProcessNewPieceCountdown();

        // �غϽ���
        ProcessTurnEnd();

        // ׼����һ�غ�
        StartNewTurn();
    }

    // ���ɵ���
    private void SpawnEnemies()
    {
        // ���㱾�غ�Ӧ�ó��ֵĵ�������
        int enemiesToSpawn = _enemiesPerTurn;
        if (_currentTurn % 2 == 0 && _currentTurn <= 20) // ǰ20�غ�ÿ���غ�����һ��
        {
            _enemiesPerTurn++;
        }
        else if (_currentTurn > 20) // 20�غϺ�ÿ�غ�����һ��
        {
            _enemiesPerTurn++;
        }

        // �ڱ�Եλ�����ɵ���
        List<Vector2Int> edgePositions = GetEdgePositions();

        for (int i = 0; i < enemiesToSpawn && edgePositions.Count > 0; i++)
        {
            int idx = Random.Range(0, edgePositions.Count);
            Vector2Int pos = edgePositions[idx];
            edgePositions.RemoveAt(idx);

            GameObject enemyObj = Instantiate(enemyPrefab, GetWorldPosition(pos), Quaternion.identity, transform);
            Piece enemyPiece = enemyObj.GetComponent<Piece>();
            enemyPiece.InitializePiece(null, pos); // ���˲���Ҫ�Ը�
            AddPiece(enemyPiece, pos);
        }
    }

    // ��ȡ��Եλ��
    private List<Vector2Int> GetEdgePositions()
    {
        List<Vector2Int> edgePositions = new();

        // ���±�Ե
        for (int x = 0; x < boardWidth; x++)
        {
            Vector2Int topPos = new Vector2Int(x, boardHeight - 1);
            Vector2Int bottomPos = new Vector2Int(x, 0);

            if (IsValidBoardPosition(topPos) && GetPieceAtPosition(topPos) == null)
                edgePositions.Add(topPos);
            if (IsValidBoardPosition(bottomPos) && GetPieceAtPosition(bottomPos) == null)
                edgePositions.Add(bottomPos);
        }

        // ���ұ�Ե
        for (int y = 1; y < boardHeight - 1; y++) // �����ظ���ӽ���
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

    // �ƶ����˳�����
    private void MoveEnemyTowardsGeneral(Piece enemy)
    {
        Vector2Int generalPos = GetGeneralPosition();
        Vector2Int enemyPos = enemy.BoardPosition;

        // ���㳯���������ƶ�����
        Vector2Int direction = Vector2Int.zero;
        if (generalPos.x > enemyPos.x) direction.x = 1;
        else if (generalPos.x < enemyPos.x) direction.x = -1;

        if (generalPos.y > enemyPos.y) direction.y = 1;
        else if (generalPos.y < enemyPos.y) direction.y = -1;

        Vector2Int targetPos = enemyPos + direction;

        // ���Ŀ��λ���Ƿ���Ч
        if (IsValidBoardPosition(targetPos))
        {
            Piece targetPiece = GetPieceAtPosition(targetPos);
            if (targetPiece == null)
            {
                // �ƶ����ո�
                MovePiece(enemy, targetPos);
            }
            else if (targetPiece.Type != Piece.PieceType.Enemy)
            {
                // �����ѷ�����
                AttackPiece(enemy, targetPiece);
                MovePiece(enemy, targetPos);
            }
        }
    }

    // ���������ӵ���ʱ
    private void ProcessNewPieceCountdown()
    {
        _newPieceCountdown--;
        if (_newPieceCountdown <= 0)
        {
            // �ڽ��帽������������
            List<Vector2Int> nearPositions = GetAvailablePositionsNearGeneral();
            if (nearPositions.Count > 0)
            {
                int idx = Random.Range(0, nearPositions.Count);
                Vector2Int pos = nearPositions[idx];
                SpawnRandomPiece();
            }

            // ���õ���ʱ
            _totalNewPiecesGained++;
            _newPieceCountdown = _totalNewPiecesGained + 3;
        }
    }

    // ����غϽ���
    private void ProcessTurnEnd()
    {
        // �������ѷ����ӵ��ûغϿ�ʼ
        foreach (var piece in _friendlyPieces)
        {
            if (piece != null)
            {
                piece.OnTurnStart();
            }
        }

        // �������ѷ����ӵ��ûغϽ���
        foreach (var piece in _friendlyPieces)
        {
            if (piece != null)
            {
                piece.OnTurnEnd();
            }
        }
    }

    // ��Ϸ����
    private void GameOver()
    {
        Debug.Log("��Ϸ���������屻���ܣ�");
        // ������Ϸ����UI
        // GameManager.Instance.ShowGameOverScreen();
    }

    // ��ȡ��ǰ�غ���
    public int GetCurrentTurn()
    {
        return _currentTurn;
    }

    // ��ȡ�����ӵ���ʱ
    public int GetNewPieceCountdown()
    {
        return _newPieceCountdown;
    }

    // ��ȡ�����ѷ�����
    public List<Piece> GetFriendlyPieces()
    {
        return new List<Piece>(_friendlyPieces);
    }

    // ��ȡ���ез�����
    public List<Piece> GetEnemyPieces()
    {
        return new List<Piece>(_enemyPieces);
    }

    // ��ȡָ��λ����Χ������
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

    // ��ȡ����4�������
    public List<Piece> GetAdjacentPieces(Vector2Int center)
    {
        List<Piece> adjacentPieces = new();
        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

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
}