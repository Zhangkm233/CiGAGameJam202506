using UnityEngine;
using System.Collections; 

public class PieceMovingState : PieceState
{
    private Vector2Int _targetPosition;
    private float _moveSpeed = 5f; // 目前这里不直接使用，目前BoardManager将处理视觉移动，后续效果或许可以插值或者使用动画的播放实现

    public PieceMovingState(Piece piece, Vector2Int targetPosition) : base(piece)
    {
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} 进入移动状态，目标：{_targetPosition}");
        // BoardManager处理实际的棋子移动和位置更新
        // 此状态主要用于指示棋子正在移动过程中
        // 移动完成后的状态改变逻辑应在BoardManager或其回调中
        // 目前，BoardManager会在移动后回调以改变状态
    }

    public override void OnUpdate()
    {
        // 可能需要插值以实现平滑移动？
        // 此处实现为简单的利用BoardManager的MovePiece将处理直接的位置改变
        // 此处的状态改变反映了移动的“启动”，然后根据剩余移动次数转换回空闲/选中状态

        // 更新内部棋盘位置
        _piece.MoveTo(_targetPosition);

        _piece.CurrentMovementCount--; // 消耗一个移动点

        // 转换到下一个状态
        if (_piece.CurrentMovementCount > 0 && _piece.Type != Piece.PieceType.Enemy)
        {
            _piece.StateMachine.ChangeState(new PieceSelectedState(_piece)); // 如果仍然可移动，则保持选中状态
        }
        else
        {
            _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // 没有更多移动次数，进入空闲状态
        }
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出移动状态.");
    }
}