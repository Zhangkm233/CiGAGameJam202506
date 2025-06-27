using UnityEngine;

public class PieceSelectedState : PieceState
{
    public PieceSelectedState(Piece piece) : base(piece) { }

    public override void OnEnter()
    {
        // 棋子被选中时的逻辑：

        Debug.Log($"{_piece.name} 进入选中状态.");
    }

    public override void OnUpdate()
    {
        // 监听玩家点击目标位置进行移动

    }

    public override void OnExit()
    {
        // 退出选中状态

        Debug.Log($"{_piece.name} 退出选中状态.");
    }
}