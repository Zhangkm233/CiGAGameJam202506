using UnityEngine;

// 棋子的有限状态机管理器
public class PieceStateMachine
{
    private Piece _ownerPiece; // 状态机所属的棋子
    public PieceState CurrentState { get; private set; } // 当前激活的状态

    public PieceStateMachine(Piece owner)
    {
        _ownerPiece = owner;
    }

    // 改变棋子的当前状态
    public void ChangeState(PieceState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.OnExit(); // 退出旧状态
        }
        CurrentState = newState; // 设置新状态
        CurrentState.OnEnter(); // 进入新状态
        _ownerPiece.SetState(CurrentState); // 通知棋子其状态已改变（用于触发事件）
        Debug.Log($"{_ownerPiece.Type} 状态切换到: {CurrentState.GetType().Name}");
    }
}
