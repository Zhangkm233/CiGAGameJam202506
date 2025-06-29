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
    //[SerializeField] private float cellSize = 0.85f;
    [Header("���̸��Ӷ�λ")]
    public GameObject tile_0_0;
    public float offset = 0.85f;
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
    [Header("��Ϸ������")]
    public AudioController audioController; // ��Ƶ���������ã����ڲ�����Ч
    public GlobalInfoUI globalInfoUI; // ȫ����ϢUI���ã�������ʾ�غ�������Ϣ
    // --- �ϰ������� ---
    [Header("�ϰ�������")]
    [Tooltip("�ڻغϿ�ʼʱˢ���ϰ���ĸ��ʣ�0��1�������ϰ���������������ʱ���ض�����ˢ�¡�")]
    [Range(0f, 1f)]
    [SerializeField] private float obstacleRefreshChancePerTurn = 0.33f; // �޸�Ϊ 33%
    [Tooltip("��Ϸ��ʼʱ���ɵĳ�ʼ�ϰ���������")]
    [SerializeField] private int initialObstacleCount = 0; // �޸�Ϊ 0����ʼ�������ϰ�
    [Tooltip("ÿ��ˢ��ʱ�����Ƴ��������ϰ���İٷֱȡ�")]
    [Range(0f, 1f)]
    [SerializeField] private float obstacleRemovalRate = 0.2f;
    [Tooltip("ÿ��ˢ��ʱ���Խ��ո��ӱ�Ϊ�ϰ���İٷֱȡ�")]
    [Range(0f, 1f)]
    [SerializeField] private float obstacleAdditionRate = 0.1f;
    //[Header("�����")]
    //public PiecePool enemyPool; // ���˳أ����ڴ�����ɵĵ���Ԥ����
    private Dictionary<Vector2Int, Piece> _pieceDict = new();
    // ���������ӵ��ֵ�
    private List<Tile> _highlightedTiles = new(); // �������ӻ���
    private List<Piece> _friendlyPieces = new();
    // �ѷ������б�
    private List<Piece> _enemyPieces = new(); // ���������б�
    public List<Piece> _StunPieces = new();
    // ��ѣ�����б����ڴ����˸���Ĳ����ƶ�״̬
    public List<Piece> _PacifismPieces = new();
    // ��ƽ���������б����ڴ����˸���Ĳ��ܹ���״̬
    // --- ͼ�黺�� ---
    private Dictionary<Vector2Int, Tile> _tileDict = new();
    // ���ڿ��ٲ���ͼ��Ļ���
    // ��Ϸ״̬����
    private int _currentTurn = 1;
    // ��ǰ�غ���
    private int _newPieceCountdown = 3; // �����ӵ���ʱ
    private int _totalNewPiecesGained = 0;
    // �ѻ�õ�����������
    private int _enemiesPerTurn = 1;
    // ÿ�غϳ��ֵĵ�������
    // ����������
    private Piece _selectedPiece = null;
    private Camera _camera;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _camera = Camera.main;
            // --- Ϊ������ܣ���Awakeʱ��������ͼ�� ---
            CacheAllTiles();
        }
        else
        {
            Destroy(gameObject);
        }
        //GetComponent<TileGenerator>().GenTiles(); // �������̸���
        offset = GameData.tileSize;
        // ���ø��Ӵ�С
    }
    private void Update()
    {
        HandleMouseInput();
    }
    // --- ������ͼ�黺�浽һ���ֵ����Ա���ٷ��� ---
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
                    // ����������ת��Ϊ����������Ϊ�ֵ�ļ�
                    Vector2Int boardPos = GetBoardPosition(tile.transform.position);
                    _tileDict[boardPos] = tile;
                    tile.BoardPosition = boardPos;
                }
            }
        }
    }
    // --- ʹ��ͼ�黺����ʵ�ָ���Ĳ��� ---
    public Tile GetTileAtPosition(Vector2Int pos)
    {
        _tileDict.TryGetValue(pos, out var tile);
        return tile;
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
        // �������Tile��unitOccupied��isObstacle
        foreach (var tile in _tileDict.Values) // ʹ�û����_tileDict
        {
            if (tile != null)
            {
                tile.unitOccupied = null;
                tile.SetObstacle(false); // ȷ�������ϰ���״̬
            }
        }
        // ��ʼ�������ϰ���Ƴ� GenerateInitialObstacles() �ĵ���
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
        generalPiece.CurrentMovementCount = 1;
        // ���岻���ƶ�
        AddPiece(generalPiece, generalPos);
        // 2. ����������ʼ���壬�ڽ����5x5��Χ�����λ��
        List<Vector2Int> availablePositionsNearGeneral = GetAvailablePositionsNearGeneral();
        // ʹ�������еķ���
        // �����ѡ������һ�޶���λ����������
        List<Vector2Int> selectedPawnPositions = new List<Vector2Int>();
        while (selectedPawnPositions.Count < 2 && availablePositionsNearGeneral.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePositionsNearGeneral.Count);
            Vector2Int pos = availablePositionsNearGeneral[randomIndex];
            selectedPawnPositions.Add(pos);
            availablePositionsNearGeneral.RemoveAt(randomIndex); // �Ƴ���ѡλ�ã�ȷ��Ψһ��
        }
        foreach (Vector2Int pawnPos in selectedPawnPositions)
        {
            GameObject pawnObj = Instantiate(pawnPrefab, GetWorldPosition(pawnPos), Quaternion.identity, transform);
            Piece pawnPiece = pawnObj.GetComponent<Piece>();
            Personality pawnPersonality = GetRandomPersonality(); // �����ӿ���������Ը�
            pawnPiece.InitializePiece(pawnPersonality, pawnPos);
            AddPiece(pawnPiece, pawnPos); // ����������ӵ�_friendlyPieces�б����õ�������
        }
        UpdateEachPieceMove(); // ����ÿ�����ӵ��ƶ�����
        UpdateGlobalInfoText();
    }
    // --- ���ɳ�ʼ�ϰ��Ｏ�ϵķ��� --- (�˷����� InitializeBoard �в��ٱ����ã����ȱ���)
    private void GenerateInitialObstacles()
    {
        // ���������ǰ���ϰ��� (�ٴ�ȷ��)
        foreach (var tile in _tileDict.Values)
        {
            if (tile.isObstacle)
            {
                tile.SetObstacle(false);
            }
        }
        // ��ȡ���п��Գ�Ϊ�ϰ����ͼ���б�
        List<Tile> validTiles = new List<Tile>();
        Vector2Int generalPos = new Vector2Int(boardWidth / 2, boardHeight / 2);
        // ����������ʼλ��
        foreach (var kvp in _tileDict) // ʹ�û����_tileDict
        {
            // ȷ������������ʼͼ�鼰�丽���������ϰ���
            // ����3x3������⽫�屻��
            if (Vector2Int.Distance(kvp.Key, generalPos) > 1 && kvp.Value.unitOccupied == null) // ȷ��ͼ����û������
            {
                validTiles.Add(kvp.Value);
            }
        }
        // �������̴�С������������������ϰ�������
        // ��ʼ����ʱ��ֻ�г�ʼ�Ľ�����������壬��Ϊ���߼�ͳһ����ʹ����������
        int totalPieceCount = _friendlyPieces.Count + _enemyPieces.Count;
        int maxFromPieceConstraint = (boardWidth * boardHeight) - (totalPieceCount * 2);
        int maxFromBoardFractionConstraint = (boardWidth * boardHeight) / 3;
        int maxTotalObstacles = Mathf.Min(maxFromPieceConstraint, maxFromBoardFractionConstraint);
        int countToGenerate = Mathf.Min(initialObstacleCount, maxTotalObstacles);
        // ��ʼ��������Ҳ���ܳ����������
        for (int i = 0; i < countToGenerate; i++)
        {
            if (validTiles.Count == 0) break;
            int randIdx = Random.Range(0, validTiles.Count);
            Tile tileToSet = validTiles[randIdx];
            tileToSet.SetObstacle(true);
            validTiles.RemoveAt(randIdx);
            // ȷ�����ǲ����ظ�ѡ��ͬһ��ͼ��
        }
    }

    // --- ����������������ϰ���ķ��� ---
    private void UpdateObstacles()
    {
        // 1. ��ȡ����δ��ռ�ݵ�ͼ�飬�������Ƿ�Ϊ�ϰ������ͨͼ�顣
        List<Tile> unoccupiedNormalTiles = new List<Tile>();
        List<Tile> unoccupiedObstacleTiles = new List<Tile>();
        foreach (var tile in _tileDict.Values) // ʹ�û����_tileDict
        {
            if (tile.unitOccupied == null) // ͼ�����Ϊ�ղ��ܱ�����
            {
                if (tile.isObstacle)
                    unoccupiedObstacleTiles.Add(tile);
                else
                    unoccupiedNormalTiles.Add(tile);
            }
        }
        // 2. ����Լ�������������������ϰ�������
        int totalPieceCount = _friendlyPieces.Count + _enemyPieces.Count;
        int maxFromPieceConstraint = (boardWidth * boardHeight) - (totalPieceCount * 2);
        int maxFromBoardFractionConstraint = (boardWidth * boardHeight) / 3;
        int maxTotalObstacles = Mathf.Min(maxFromPieceConstraint, maxFromBoardFractionConstraint);
        // ���㵱ǰ�����������ϰ��������
        int actualTotalObstacleCount = 0;
        foreach (var tile in _tileDict.Values) // ʹ�û����_tileDict
        {
            if (tile.isObstacle)
            {
                actualTotalObstacleCount++;
            }
        }
        // �ж��Ƿ���Ҫǿ�Ƹ��£���ʵ���ϰ����������������������ʱ��
        bool forceUpdate = (actualTotalObstacleCount > maxTotalObstacles);
        // �������Ҫǿ�Ƹ��£���������¼�δ��������ֱ�ӷ���
        if (!forceUpdate && Random.Range(0f, 1f) > obstacleRefreshChancePerTurn)
        {
            return;
            // δͨ�����ʼ�飬���غ��ޱ仯��
        }
        // --- ʵ�ʵ��ϰ���ˢ���߼���ֻ����forceUpdateΪtrue������¼�����ʱ�Ż�ִ�� ---
        // 3. ����Ƴ�һЩ���е��ϰ���
        // ���������Ƴ����Ƴ����ϰ�������
        int obstaclesToRemoveByRate = Mathf.FloorToInt(unoccupiedObstacleTiles.Count * obstacleRemovalRate);
        // Ϊ������Լ��������Ҫ�Ƴ����ϰ�������
        int neededToRemoveToMeetConstraint = Mathf.Max(0, actualTotalObstacleCount - maxTotalObstacles);
        // ʵ����Ҫ�Ƴ����ϰ�������ȡ�����е����ֵ����ȷ������Լ��
        int obstaclesToReallyRemove = Mathf.Max(obstaclesToRemoveByRate, neededToRemoveToMeetConstraint);
        for (int i = 0; i < obstaclesToReallyRemove; i++)
        {
            if (unoccupiedObstacleTiles.Count == 0) break;
            // ���û�п��Ƴ����ϰ����ˣ���ֹͣ
            int randIdx = Random.Range(0, unoccupiedObstacleTiles.Count);
            Tile tileToClear = unoccupiedObstacleTiles[randIdx];
            tileToClear.SetObstacle(false); // ���ϰ�������������
            unoccupiedNormalTiles.Add(tileToClear);
            unoccupiedObstacleTiles.RemoveAt(randIdx);
            actualTotalObstacleCount--; // ����ʵ���ϰ�������
        }
        // 4. ����ѭ�������޵�ǰ���£��������µ��ϰ���
        int obstaclesToAdd = Mathf.FloorToInt(unoccupiedNormalTiles.Count * obstacleAdditionRate);
        for (int i = 0; i < obstaclesToAdd; i++)
        {
            // ����Ѵﵽ���������ϰ�����������ֹͣ���
            if (actualTotalObstacleCount >= maxTotalObstacles) break;
            if (unoccupiedNormalTiles.Count == 0) break; // ���û�п��е����������ˣ���ֹͣ

            // ����Ѱ��һ������ʹ���̶Ͽ��ĸ���
            List<Tile> shuffledNormalTiles = unoccupiedNormalTiles.OrderBy(x => Random.value).ToList();
            Tile tileToSet = null;

            foreach (Tile candidateTile in shuffledNormalTiles)
            {
                // �ڷ���֮ǰ��������Ƿ��ʹ���̶Ͽ�
                if (IsBoardConnectedAfterPlacingObstacle(candidateTile.BoardPosition))
                {
                    tileToSet = candidateTile;
                    break;
                }
            }

            if (tileToSet != null)
            {
                tileToSet.SetObstacle(true); // ���������ӱ�Ϊ�ϰ���
                unoccupiedObstacleTiles.Add(tileToSet);
                unoccupiedNormalTiles.Remove(tileToSet); // ���б����Ƴ����ض�ͼ��
                actualTotalObstacleCount++; // ����ʵ���ϰ�������
            }
            else
            {
                // ����ڱ��ε�����û���ҵ����ʵ�ͼ�飬��ֹͣ������Ӹ����ϰ���
                // �Է�ֹ�����̱�ù�������ʱ��������ѭ����
                break;
            }
        }
    }

    // ���������������ָ��λ�÷����ϰ���������Ƿ���Ȼ��ͨ
    private bool IsBoardConnectedAfterPlacingObstacle(Vector2Int proposedObstaclePos)
    {
        // ��ʱ�������λ�ñ��Ϊ�ϰ���
        Tile proposedTile = GetTileAtPosition(proposedObstaclePos);
        if (proposedTile == null) return true;

        bool originalObstacleState = proposedTile.isObstacle;
        
        proposedTile.SetObstacle(true);

        // Ѱ��BFS/DFS����ʼ�㣬�õ㲻���ǽ�����ϰ���λ���Ҳ��������ϰ���
        Vector2Int startPos = Vector2Int.zero;
        bool startPosFound = false;
        foreach (var kvp in _tileDict)
        {
            if (!kvp.Value.isObstacle) // �������ʱ���õ��ϰ���״̬
            {
                startPos = kvp.Key;
                startPosFound = true;
                break;
            }
        }

        // ���û���ҵ���Ч����ʼ������Ϊ��ͨ
        if (!startPosFound)
        {
            
            proposedTile.SetObstacle(originalObstacleState); // �ָ���ʱ����
            return true;
        }

        // ִ��BFS�Լ���ɴ�ķ��ϰ���ͼ������
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startPos);
        visited.Add(startPos);

        while (queue.Count > 0)
        {
            Vector2Int currentPos = queue.Dequeue();

            Vector2Int[] directions = {
                new Vector2Int(0, 1), new Vector2Int(0, -1), // ��, ��
                new Vector2Int(1, 0), new Vector2Int(-1, 0)  // ��, ��
            };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighborPos = currentPos + dir;
                Tile neighborTile = GetTileAtPosition(neighborPos);

                if (IsInBounds(neighborPos) && neighborTile != null && !neighborTile.isObstacle && !visited.Contains(neighborPos))
                {
                    queue.Enqueue(neighborPos);
                    visited.Add(neighborPos);
                }
            }
        }

        // ������ʱ�����ϰ�����ܵķ��ϰ���ͼ������
        int totalNonObstacleTiles = 0;
        foreach (var kvp in _tileDict)
        {
            if (!kvp.Value.isObstacle)
            {
                totalNonObstacleTiles++;
            }
        }

        proposedTile.SetObstacle(originalObstacleState); // �ָ���ʱ����

        // ������ʵ���ͼ�����������ܵķ��ϰ���ͼ������������������ͨ��
        return visited.Count == totalNonObstacleTiles;
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
    public void ExecuteMove(Piece piece, Vector2Int targetPos)
    {
        Piece targetPiece = GetPieceAtPosition(targetPos);
        // ���Ŀ��λ���Ƿ����ѷ����ӣ�����ǣ���Ϊ��Ч�ƶ�
        if (targetPiece != null && targetPiece.Type != Piece.PieceType.Enemy) return;


        // ���ٹ������PieceAttackingState��
        piece.StateMachine?.ChangeState(new PieceMovingState(piece, targetPos));

        // ֱ�ӵ���MovePiece���÷������ڸ��������ݸ��º���������
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
            Debug.Log($"��ӵз�����: {piece.name} at {pos}");
            _enemyPieces.Add(piece);
        }
        else
        {
            Debug.Log($"����ѷ�����: {piece.name} at {pos}");
            _friendlyPieces.Add(piece);
        }
        // ����Tile
        Tile tile = GetTileAtPosition(pos);
        if (tile != null) tile.unitOccupied = piece;
    }
    // �������Ƴ�����
    public void RemovePiece(Vector2Int pos, bool IsPuttingBackToPool)
    {
        if (_pieceDict.TryGetValue(pos, out var piece))
        {
            // �Ӷ�Ӧ�б��Ƴ�
            if (piece.Type == Piece.PieceType.Enemy)
            {
                Debug.Log($"�Ƴ��з�����: {piece.name} at {pos}");
                _enemyPieces.Remove(piece);
                /*
                if (IsPuttingBackToPool)
                {
                    enemyPool.ReturnEnemy(piece.gameObject);
                }
                */
            }
            else
            {
                Debug.Log($"�Ƴ��ѷ�����: {piece.name} at {pos}");
                _friendlyPieces.Remove(piece);
            }
            // ���Tile����
            Tile tile = GetTileAtPosition(pos);
            if (tile != null && tile.unitOccupied == piece)
                tile.unitOccupied = null;
            _pieceDict.Remove(pos);
        }
    }
    // ��������ת��������
    public Vector3 GetWorldPosition(Vector2Int boardPos)
    {
        return tile_0_0.transform.position + new Vector3(boardPos.x * offset, boardPos.y * offset, 0);
    }
    // ��������ת��������
    public Vector2Int GetBoardPosition(Vector3 worldPos)
    {
        Vector3 local = worldPos - tile_0_0.transform.position;
        int x = Mathf.RoundToInt(local.x / offset);
        int y = Mathf.RoundToInt(local.y / offset);
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
        if (tile == null || tile.isObstacle) // ���������ڿ����붯̬�ϰ���һ����
            return false;
        return true;
    }
    public bool IsObstacleBoardPosition(Vector2Int pos)
    {
        Tile tile = GetTileAtPosition(pos);
        return tile != null && tile.isObstacle;
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
                tile.SetHighlight();
                // ���ø������
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

        // �ڲ�����������ǰ���Ȼ�ȡĿ��λ���Ͽ��ܴ��ڵĵз�����
        Piece targetPiece = GetPieceAtPosition(targetPos);

        // �����������ݣ��Ӿ�λ�õ��������Ƴ����ӣ���������ӵ���λ��
        RemovePiece(oldPos, false);

        // �����Ĺ���(����)�߼������ڶ���������Ļص���ִ�С�
        piece.MovingAnimation(oldPos,targetPos,targetPiece,() => {
            AddPiece(piece,targetPos);
            audioController.PlayDropPiece();
            piece.CurrentMovementCount--;
            UpdatePieceMove(piece);
        });
    }
    // ���ӹ���������״̬�����ã�
    public void AttackPiece(Piece attacker, Piece target)
    {
        if (attacker == null || target == null) return;
        if (attacker == target) return;

        // ���������߼�
        target.StateMachine?.ChangeState(new PieceDeadState(target));
        Debug.Log($"��������: {attacker.name} ���� {target.name} ��λ�� {target.BoardPosition}");

        RemovePiece(target.BoardPosition, true);
        // ����Ƿ񹥻��˽���
        if (target.Type == Piece.PieceType.General)
        {
            GameOver();
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
            if (piece.Type == Piece.PieceType.General)
            {
                return piece.BoardPosition;
            }
        }
        return new Vector2Int(boardWidth / 2, boardHeight / 2);
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
        UpdateGlobalInfoText();
        // --- ��ÿ�غϿ�ʼʱ�����ϰ��� ---
        // �ӵ����غϿ�ʼ���п��ܳ����ϰ�
        if (_currentTurn >= 3)
        {
            UpdateObstacles();
        }
        // 1. ���˳���
        SpawnEnemies();
        // �����ƶ�����
        UpdateEachPieceMove();
        // 2. ��һغϣ�ͨ��UI��ť����������
        // 3. �����ƶ�
        // StartCoroutine(EnemyTurnCoroutine());
        _StunPieces.Clear(); // �����ѣ�����б�
        _PacifismPieces.Clear();
        // ��պ�ƽ���������б�
    }
    // ������һغ�
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
    // ���˻غ�Э��
    private IEnumerator EnemyTurnCoroutine()
    {
        // �����ƶ�
        foreach (var enemy in _enemyPieces.ToArray())
        {
            if (enemy != null)
            {
                if (MoveEnemyTowardsGeneral(enemy))
                {
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    yield return null;
                }
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
            /*GameObject enemyObj = enemyPool.GetEnemy();*/
            GameObject enemyObj = Instantiate(enemyPrefab, GetWorldPosition(pos), Quaternion.identity, transform);
            enemyObj.transform.position = GetWorldPosition(pos);
            // ���õ���λ��
            Piece enemyPiece = enemyObj.GetComponent<Piece>();
            enemyPiece.InitializePiece(null, pos);
            // ���˲���Ҫ�Ը�
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
    private bool MoveEnemyTowardsGeneral(Piece enemy)
    {
        Vector2Int generalPos = GetGeneralPosition();
        Vector2Int enemyPos = enemy.BoardPosition;
        
        Debug.Log($"Enemy {enemy.name} at {enemyPos} moving towards General at {generalPos} (Turn {_currentTurn})");
        
        List<(Vector2Int pos, float distance)> potentialMoves = new List<(Vector2Int pos, float distance)>();
        // ����������
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(1, 0),   // Right
            new Vector2Int(-1, 0)   // Left
            // �Ƴ���б���ƶ��ķ�������
        };
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int candidatePos = enemyPos + dir;
           
            if (IsValidBoardPosition(candidatePos))
            {
                Piece pieceAtCandidatePos = GetPieceAtPosition(candidatePos);
                
                if (pieceAtCandidatePos == null || pieceAtCandidatePos.Type != Piece.PieceType.Enemy)
                {
                    float dist = Vector2Int.Distance(candidatePos, generalPos);
                    potentialMoves.Add((candidatePos, dist));
                }
            }
        }
        
        potentialMoves.Sort((a, b) => a.distance.CompareTo(b.distance));
        Vector2Int chosenTargetPos = Vector2Int.zero;
        bool foundMove = false;
        
        if (potentialMoves.Count > 0)
        {
            chosenTargetPos = potentialMoves[0].pos;
            
            foundMove = true;
        }
        if (foundMove)
        {
            
            Debug.Log($"Enemy {enemy.name} from {enemyPos} calculated target position: {chosenTargetPos}. Distance to General: {Vector2Int.Distance(chosenTargetPos, generalPos)}");
            
            MovePiece(enemy, chosenTargetPos);
            return true; 
        }
        else
        {
            Debug.Log($"Enemy {enemy.name} at {enemyPos}: No valid move found towards General.");
            return false; 
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
                Vector2Int posToSpawn = nearPositions[idx];
                // ���ѡ���������ͺ��Ը�
                Piece.PieceType randomType = GetRandomPieceType();
                GameObject prefab = GetPrefabForType(randomType);
                Personality randomPersonality = GetRandomPersonality();
                GameObject pieceObj = Instantiate(prefab, GetWorldPosition(posToSpawn), Quaternion.identity, transform);
                Piece newPiece = pieceObj.GetComponent<Piece>();
                newPiece.InitializePiece(randomPersonality, posToSpawn);
                AddPiece(newPiece, posToSpawn);
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

    private void UpdateGlobalInfoText() {
        globalInfoUI.UpdateInfoText(_currentTurn,_newPieceCountdown,_enemiesPerTurn);
    }
}