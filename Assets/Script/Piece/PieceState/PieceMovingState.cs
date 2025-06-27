using UnityEngine;
using static Piece;

public class PieceMovingState : PieceState
{
    private Vector2Int _targetPosition; // 棋子要移动到的目标位置
    private float _moveSpeed = 5f; // 棋子移动速度

    public PieceMovingState(Piece piece, Vector2Int targetPosition) : base(piece)
    {
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} 进入移动状态，目标：{_targetPosition}");
        // 播放移动动画或音效

    }

    public override void OnUpdate()
    {

        _piece.MoveTo(_targetPosition); // 更新棋子内部位置

        // 检查是否有棋子被“吃掉”
    

        _piece.CurrentMovementCount--; // 消耗一次移动次数

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

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出移动状态.");
    }
}