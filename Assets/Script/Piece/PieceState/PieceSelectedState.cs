using UnityEngine;
using System.Collections.Generic;

public class PieceSelectedState : PieceState
{
    // TODO:�洢��ǰ�������п��ܵĺϷ��ƶ�λ�úͿɹ���Ŀ��
    private List<Vector2Int> _possibleMoves;

    public PieceSelectedState(Piece piece) : base(piece) { }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} ����ѡ��״̬.");
        // TODO:ͻ����ʾѡ�����ӣ�����ı���ɫ�����ѡ����Ч
        // ��ҪBoardManager����ȡ����ʾ�Ϸ����ƶ��͹���λ��
        // _possibleMoves = _piece.GetPossibleMoves(BoardManager.Instance); // Assuming BoardManager.Instance is available
        // (BoardManager.Instance as BoardManager)?.HighlightMoves(_possibleMoves);
    }

    public override void OnUpdate()
    {
        // �����������
        // TODO:�����ҵ���������ϵ�ĳ��λ��
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Vector2Int clickedBoardPosition = GetClickedBoardPosition(); // ��Ҫһ����������ȡ���������������
        //
        //     var boardManager = BoardManager.Instance as BoardManager; // ��ȡ BoardManager ʵ��
        //     if (boardManager != null && _piece.IsValidMove(clickedBoardPosition, boardManager))
        //     {
        //         Piece targetPiece = boardManager.GetPieceAtPosition(clickedBoardPosition);
        //    
        //         if (targetPiece != null && targetPiece.Type != _piece.Type) // ���Ŀ��λ�����������ǵ���
        //         {
        //             _piece.StateMachine.ChangeState(new PieceAttackingState(_piece, targetPiece, clickedBoardPosition)); // ����Ŀ��λ���Ը������ӵ�ʵ��λ��
        //         }
        //         else if (targetPiece == null) // ���Ŀ��λ��Ϊ�գ�ֱ���ƶ�
        //         {
        //             _piece.StateMachine.ChangeState(new PieceMovingState(_piece, clickedBoardPosition));
        //         }
        //         else // Ŀ�����ѷ����ӣ������ƶ��򹥻�
        //         {
        //             Debug.Log("�޷��ƶ����ѷ��������ڵ�λ�á�");
        //         }
        //     }
        //     else
        //     {
        //         // ����Ƿ�λ�ÿ���ȡ��ѡ��򱣳�ѡ��״̬
        //         Debug.Log("��Ч���ƶ��򹥻�Ŀ�ꡣ");
        //          // _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // ȡ��ѡ��
        //     }
        // }
        // TODO: �����Ұ���Esc�����Ҽ�ȡ��ѡ��
        // if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        // {
        //     _piece.StateMachine.ChangeState(new PieceIdleState(_piece));
        // }
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳�ѡ��״̬.");
        // TODO:ȡ��ͻ����ʾ
        // (BoardManager.Instance as BoardManager)?.ClearHighlights();
    }

    // TODO:�����������������λ��ת��Ϊ�������꣬��Ҫ��������̵�����
    // private Vector2Int GetClickedBoardPosition() { return Vector2Int.zero; }
}