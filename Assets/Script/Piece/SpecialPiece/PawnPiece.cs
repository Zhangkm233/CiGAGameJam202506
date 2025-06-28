using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : Piece
{
    public override List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int forward = new Vector2Int(0, 1); // ����ǰ�ƶ�

        // �򵥵���ǰ�ƶ�
        Vector2Int targetPos = BoardPosition + forward;
        if (BoardManager.Instance.IsValidBoardPosition(targetPos) && BoardManager.Instance.GetPieceAtPosition(targetPos) == null)
        {
            possibleMoves.Add(targetPos);
        }

        // �򵥵�б�߹�������Ľ�����һ��ʵ���뷨������Ҫ����ɾȥ��
        Vector2Int attackLeft = new Vector2Int(-1, 1);
        Vector2Int attackRight = new Vector2Int(1, 1);

        Piece targetPieceLeft = BoardManager.Instance.GetPieceAtPosition(BoardPosition + attackLeft);
        if (BoardManager.Instance.IsValidBoardPosition(BoardPosition + attackLeft) && targetPieceLeft != null && targetPieceLeft.Type == PieceType.Enemy)
        {
            possibleMoves.Add(BoardPosition + attackLeft);
        }

        Piece targetPieceRight = BoardManager.Instance.GetPieceAtPosition(BoardPosition + attackRight);
        if (BoardManager.Instance.IsValidBoardPosition(BoardPosition + attackRight) && targetPieceRight != null && targetPieceRight.Type == PieceType.Enemy)
        {
            possibleMoves.Add(BoardPosition + attackRight);
        }

        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition)
    {
        // ���Ŀ���ڿ��ܵ��ƶ���Χ��
        return GetPossibleMoves().Contains(targetPosition);
    }
}
