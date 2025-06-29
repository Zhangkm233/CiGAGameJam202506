using UnityEngine;
using System.Collections.Generic;

public class PieceSelectedState : PieceState
{
    private List<Vector2Int> _possibleMoves;

    public PieceSelectedState(Piece piece) : base(piece) { }
    
    public delegate void OnPieceSelected(Piece piece);
    public static event OnPieceSelected PieceSelectedEvent;

    public delegate void OnPieceDeselected(Piece piece);
    public static event OnPieceDeselected PieceDeselectedEvent;

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} 进入选中状态.");
        // 突出显示选中棋子，例如改变颜色或添加选中特效
        _possibleMoves = _piece.GetPossibleMoves();
        BoardManager.Instance?.HighlightMoves(_possibleMoves);
        // 触发选中事件
        PieceSelectedEvent?.Invoke(_piece);
    }

    public override void OnUpdate()
    {
        // 玩家输入由BoardManager处理，有效时调用ExecuteMove
        
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} 退出选中状态.");
        // 退出选中状态时清除高亮显示
        BoardManager.Instance?.ClearHighlights();
        // 触发取消选中事件
        PieceDeselectedEvent?.Invoke(_piece);   
        
    }
}