using UnityEngine;
using System.Collections.Generic;

public class PieceSelectedState : PieceState
{
    // TODO:存储当前棋子所有可能的合法移动位置和可攻击目标
    private List<Vector2Int> _possibleMoves;

    public PieceSelectedState(Piece piece) : base(piece) { }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} 进入选中状态.");
        // TODO:突出显示选中棋子，例如改变颜色或添加选中特效
        // 需要BoardManager来获取和显示合法的移动和攻击位置
        // _possibleMoves = _piece.GetPossibleMoves(BoardManager.Instance); // Assuming BoardManager.Instance is available
        // (BoardManager.Instance as BoardManager)?.HighlightMoves(_possibleMoves);
    }

    public override void OnUpdate()
    {
        // 监听玩家输入
        // TODO:如果玩家点击了棋盘上的某个位置
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Vector2Int clickedBoardPosition = GetClickedBoardPosition(); // 需要一个方法来获取鼠标点击的棋盘坐标
        //
        //     var boardManager = BoardManager.Instance as BoardManager; // 获取 BoardManager 实例
        //     if (boardManager != null && _piece.IsValidMove(clickedBoardPosition, boardManager))
        //     {
        //         Piece targetPiece = boardManager.GetPieceAtPosition(clickedBoardPosition);
        //    
        //         if (targetPiece != null && targetPiece.Type != _piece.Type) // 如果目标位置有棋子且是敌人
        //         {
        //             _piece.StateMachine.ChangeState(new PieceAttackingState(_piece, targetPiece, clickedBoardPosition)); // 传入目标位置以更新棋子的实际位置
        //         }
        //         else if (targetPiece == null) // 如果目标位置为空，直接移动
        //         {
        //             _piece.StateMachine.ChangeState(new PieceMovingState(_piece, clickedBoardPosition));
        //         }
        //         else // 目标是友方棋子，不能移动或攻击
        //         {
        //             Debug.Log("无法移动到友方棋子所在的位置。");
        //         }
        //     }
        //     else
        //     {
        //         // 点击非法位置可以取消选择或保持选中状态
        //         Debug.Log("无效的移动或攻击目标。");
        //          // _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // 取消选中
        //     }
        // }
        // TODO: 如果玩家按下Esc键或右键取消选择
        // if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        // {
        //     _piece.StateMachine.ChangeState(new PieceIdleState(_piece));
        // }
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出选中状态.");
        // TODO:取消突出显示
        // (BoardManager.Instance as BoardManager)?.ClearHighlights();
    }

    // TODO:辅助函数，将鼠标点击位置转换为棋盘坐标，需要相机和棋盘的引用
    // private Vector2Int GetClickedBoardPosition() { return Vector2Int.zero; }
}