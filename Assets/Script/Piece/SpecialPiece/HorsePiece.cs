using System.Collections.Generic;
using UnityEngine;

public class HorsePiece : Piece
{
    public override List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int[] offsets =
        {
            new Vector2Int(1, 2), new Vector2Int(1, -2),
            new Vector2Int(-1, 2), new Vector2Int(-1, -2),
            new Vector2Int(2, 1), new Vector2Int(2, -1),
            new Vector2Int(-2, 1), new Vector2Int(-2, -1)
        };

        foreach (Vector2Int offset in offsets)
        {
            Vector2Int targetPos = BoardPosition + offset;
            // ��������λ��
            Vector2Int legPos = BoardPosition;
            if (Mathf.Abs(offset.x) == 2) // ������2�������ں���
            {
                legPos += new Vector2Int(offset.x / 2, 0);
            }
            else // ������2������������
            {
                legPos += new Vector2Int(0, offset.y / 2);
            }

            if (BoardManager.Instance.IsValidBoardPosition(targetPos)) // ���Ŀ��λ���Ƿ���������
            {
                // ����Ƿ�������
                if (BoardManager.Instance.GetPieceAtPosition(legPos) == null)
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