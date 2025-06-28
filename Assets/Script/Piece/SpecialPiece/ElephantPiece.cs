using System.Collections.Generic;
using UnityEngine;

public class ElephantPiece : Piece
{
    public override List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int[] offsets =
        {
            new Vector2Int(2, 2), new Vector2Int(2, -2),
            new Vector2Int(-2, 2), new Vector2Int(-2, -2)
        };

        foreach (Vector2Int offset in offsets)
        {
            Vector2Int targetPos = BoardPosition + offset;
            // ����λ��
            Vector2Int eyePos = BoardPosition + new Vector2Int(offset.x / 2, offset.y / 2);

            if (BoardManager.Instance.IsValidBoardPosition(targetPos))
            {
                // ����Ƿ�������
                if (BoardManager.Instance.GetPieceAtPosition(eyePos) == null)
                {
                    Piece targetPiece = BoardManager.Instance.GetPieceAtPosition(targetPos);
                    if (targetPiece == null || targetPiece.Type != Type) // ���Ŀ��Ϊ�ջ�з�����
                    {
                        possibleMoves.Add(targetPos);
                    }
                }
            }
        }
        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition)
    {
        return GetPossibleMoves().Contains(targetPosition);
    }
}