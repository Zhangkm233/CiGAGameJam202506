using UnityEngine;

public class PieceDeadState : PieceState
{
    public PieceDeadState(Piece piece) : base(piece) { }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} 进入死亡状态.");
        // 死亡的视觉效果 (e.g.淡出，爆炸)
        _piece.gameObject.SetActive(false); // 暂时隐藏棋子
        // BoardManager最终会从其字典中移除棋子
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出死亡状态.");
    }
}