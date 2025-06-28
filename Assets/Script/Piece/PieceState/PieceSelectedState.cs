using UnityEngine;
using System.Collections.Generic;

public class PieceSelectedState : PieceState
{
    private List<Vector2Int> _possibleMoves;

    public PieceSelectedState(Piece piece) : base(piece) { }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} ����ѡ��״̬.");
        // ͻ����ʾѡ�����ӣ�����ı���ɫ�����ѡ����Ч
        _possibleMoves = _piece.GetPossibleMoves(); 
        BoardManager.Instance?.HighlightMoves(_possibleMoves);
    }

    public override void OnUpdate()
    {
        // ���������BoardManager������Чʱ����ExecuteMove
        
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳�ѡ��״̬.");
        // �˳�ѡ��״̬ʱ���������ʾ
        BoardManager.Instance?.ClearHighlights();
    }
}