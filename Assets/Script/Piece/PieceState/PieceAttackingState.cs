using UnityEngine;

public class PieceAttackingState : PieceState
{
    private Piece _targetPiece;
    private Vector2Int _targetPosition; // 攻击后要移动到的位置

    public PieceAttackingState(Piece piece, Piece targetPiece, Vector2Int targetPosition) : base(piece)
    {
        _targetPiece = targetPiece;
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} 进入攻击状态，目标：{_targetPiece.name}");

        // 触发棋子的攻击逻辑 (e.g.动画)
        _piece.Attack(_targetPiece);

        // BoardManager处理实际的攻击和移动攻击者
        // 状态转换在攻击逻辑触发后立即发生。
        _piece.CurrentMovementCount--; // 消耗一个移动点

        // 攻击后，转换到下一个状态
        if (_piece.CurrentMovementCount > 0 && _piece.Type != Piece.PieceType.Enemy)
        {
            _piece.StateMachine.ChangeState(new PieceSelectedState(_piece)); // 如果仍然可移动，则保持选中状态
        }
        else
        {
            _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // 没有更多移动次数，进入空闲状态
        }
    }

    public override void OnUpdate()
    {
        // 等待攻击动画完成？
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出攻击状态.");
    }
}