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
        Debug.Log($"{_piece.name} ����ѡ��״̬.");
        // ͻ����ʾѡ�����ӣ�����ı���ɫ�����ѡ����Ч
        _possibleMoves = _piece.GetPossibleMoves();
        BoardManager.Instance?.HighlightMoves(_possibleMoves);
        // ����ѡ���¼�
        PieceSelectedEvent?.Invoke(_piece);
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
        // ����ȡ��ѡ���¼�
        PieceDeselectedEvent?.Invoke(_piece);   
        
    }
}