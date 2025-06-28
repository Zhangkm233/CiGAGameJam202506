using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : Piece
{
    public override List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // ��������������ƶ�
        Vector2Int[] directions =
        {
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 0), new Vector2Int(-1, 0)
        };

        foreach (Vector2Int dir in directions) {
            Vector2Int targetPos = BoardPosition + dir;
            if (BoardManager.Instance.IsValidBoardPosition(targetPos)) {
                Piece targetPiece = BoardManager.Instance.GetPieceAtPosition(targetPos);
                // �������ƶ�����λ�򹥻�����
                if (targetPiece == null || targetPiece.Type == PieceType.Enemy) {
                    possibleMoves.Add(targetPos);
                }
            }
        }

        // �򵥵�б�߹�������Ľ�����һ��ʵ���뷨������Ҫ����ɾȥ��
        Vector2Int attackLeft = new Vector2Int(-1, 1);
        Vector2Int attackRight = new Vector2Int(1, 1);
        Vector2Int attackRightDown = new Vector2Int(1, -1);
        Vector2Int attackLeftDown = new Vector2Int(-1, -1);
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
        Piece targetPieceRightDown = BoardManager.Instance.GetPieceAtPosition(BoardPosition + attackRightDown);
        if (BoardManager.Instance.IsValidBoardPosition(BoardPosition + attackRightDown) && targetPieceRightDown != null && targetPieceRightDown.Type == PieceType.Enemy) {
            possibleMoves.Add(BoardPosition + attackRightDown);
        }
        Piece targetPieceLeftDown = BoardManager.Instance.GetPieceAtPosition(BoardPosition + attackLeftDown);
        if (BoardManager.Instance.IsValidBoardPosition(BoardPosition + attackLeftDown) && targetPieceLeftDown != null && targetPieceLeftDown.Type == PieceType.Enemy) {
            possibleMoves.Add(BoardPosition + attackLeftDown);
        }

        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition)
    {
        // ���Ŀ���ڿ��ܵ��ƶ���Χ��
        return GetPossibleMoves().Contains(targetPosition);
    }
}
