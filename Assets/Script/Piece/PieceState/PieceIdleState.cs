using UnityEngine;
// --- 待机状态 ---
public class PieceIdleState : PieceState
{
    public PieceIdleState(Piece piece) : base(piece) { }
    public override void OnEnter()
    {
        // 棋子进入待机状态
        Debug.Log($"{_piece.name} 进入待机状态.");
        // TODO:添加一些待机动画或视觉提示
    }
    public override void OnUpdate()
    {
        // 待机状态下监听玩家输入，比如点击棋子进行选中之类的
        // TODO:如果玩家点击了此棋子，切换到PieceSelectedState
        // if (Input.GetMouseButtonDown(0) && IsClicked(_piece))
        // {
        //     _piece.StateMachine.ChangeState(new PieceSelectedState(_piece));
        // }
    }
    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出待机状态.");
    }

    // 辅助函数，用于检测是否点击了该棋子，需要射线检测之类的
    private bool IsClicked(Piece piece) { return false; }
}