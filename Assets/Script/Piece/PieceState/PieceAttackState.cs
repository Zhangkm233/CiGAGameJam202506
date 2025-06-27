using UnityEngine;

public class PieceAttackingState : PieceState
{
    private Piece _targetPiece;
    private Vector2Int _targetPosition; // 攻击后棋子移动到的位置（吃子后）

    public PieceAttackingState(Piece piece, Piece targetPiece, Vector2Int targetPosition) : base(piece)
    {
        _targetPiece = targetPiece;
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} 进入攻击状态，目标：{_targetPiece.name}");
        // TODO:播放攻击动画，播放攻击音效

        _piece.Attack(_targetPiece); // 执行攻击逻辑，将目标置为死亡状态
        _piece.MoveTo(_targetPosition); // 攻击并移动到目标位置

        _piece.CurrentMovementCount--; // 消耗一次移动次数

        // 攻击后回到选中状态或待机状态，取决于是否还有移动次数
        if (_piece.CurrentMovementCount > 0 && _piece.Type != Piece.PieceType.Enemy)
        {
            _piece.StateMachine.ChangeState(new PieceSelectedState(_piece)); // 攻击后如果还有移动次数，可以继续操作
        }
        else
        {
            _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // 攻击次数用完，回到待机状态
        }
    }

    public override void OnUpdate()
    {
        // 攻击动画播放完毕后or延时后，切换回Idle或Selected状态
        // TODO:根据动画or计时器?完成攻击流程
  
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出攻击状态.");
    }
}