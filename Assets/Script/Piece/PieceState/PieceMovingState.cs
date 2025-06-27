using UnityEngine;
using static Piece;

public class PieceMovingState : PieceState
{
    private Vector2Int _targetPosition; // 棋子要移动到的目标位置
    private float _moveSpeed = 5f; // 棋子移动速度
    private Vector3 _startWorldPosition;
    private Vector3 _targetWorldPosition;
    private float _journeyLength;
    private float _startTime;

    public PieceMovingState(Piece piece, Vector2Int targetPosition) : base(piece)
    {
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} 进入移动状态，目标：{_targetPosition}");
        // 播放移动动画或音效
        _startWorldPosition = _piece.transform.position;
        // TODO:需要BoardManager将 BoardPosition 转换为世界坐标
        // _targetWorldPosition = (boardManager as BoardManager)?.GetWorldPosition(_targetPosition); 
        _targetWorldPosition = new Vector3(_targetPosition.x, _targetPosition.y, _piece.transform.position.z); // 棋盘是XY平面？
        _journeyLength = Vector3.Distance(_startWorldPosition, _targetWorldPosition);
        _startTime = Time.time;
    }

    public override void OnUpdate()
    {
        // 平滑移动逻辑
        float distCovered = (Time.time - _startTime) * _moveSpeed;
        float fractionOfJourney = _journeyLength > 0 ? distCovered / _journeyLength : 1f;
        _piece.transform.position = Vector3.Lerp(_startWorldPosition, _targetWorldPosition, fractionOfJourney);

        // 当棋子接近目标位置时，完成移动并切换状态
        if (Vector3.Distance(_piece.transform.position, _targetWorldPosition) < 0.05f || fractionOfJourney >= 1f)
        {
            _piece.transform.position = _targetWorldPosition; // 确保精确到达目标位置
            _piece.MoveTo(_targetPosition); // 更新棋子内部位置

            _piece.CurrentMovementCount--; // 消耗一次移动次数

            // 检查是否有棋子被“吃掉”
            // TODO:这里需要BoardManager来检查_targetPosition是否有敌方棋子
            // Piece eatenPiece = (BoardManager.Instance as BoardManager)?.GetPieceAtPosition(_targetPosition); // Assuming BoardManager.Instance is available
            // if (eatenPiece != null && eatenPiece.Type == PieceType.Enemy)
            // {
            //    _piece.Attack(eatenPiece); // 触发攻击逻辑，将敌人置为死亡状态
            // }

            // 根据剩余移动次数决定下一个状态
            if (_piece.CurrentMovementCount > 0 && _piece.Type != PieceType.Enemy)
            {
                _piece.StateMachine.ChangeState(new PieceSelectedState(_piece)); // 如果还有移动次数，可以继续操作
            }
            else
            {
                _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // 移动次数用完，回到待机状态
            }
        }
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出移动状态.");
    }
}