using UnityEngine;

// --- 待机状态 ---
public class PieceIdleState : PieceState
{
    public PieceIdleState(Piece piece) : base(piece) { }

    public override void OnEnter()
    {
        // 棋子进入待机状态

        Debug.Log($"{_piece.name} 进入待机状态.");
    }

    public override void OnUpdate()
    {
        // 待机状态下监听玩家输入

    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出待机状态.");
    }
}