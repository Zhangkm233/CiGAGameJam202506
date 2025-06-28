using System.Collections.Generic;
using UnityEngine;

public class RookPiece : Piece
{
    public override List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in directions)
        {
            for (int i = 1; ; i++)
            {
                Vector2Int targetPos = BoardPosition + dir * i;
                if (!BoardManager.Instance.IsValidBoardPosition(targetPos)) break;

                Piece pieceAtTarget = BoardManager.Instance.GetPieceAtPosition(targetPos);
                if (pieceAtTarget == null)
                {
                    possibleMoves.Add(targetPos);
                }
                else
                {
                    if (pieceAtTarget.Type != Type) // ����ǵ���
                    {
                        possibleMoves.Add(targetPos);
                    }
                    break; // ���ܴ�����һ�������ƶ��򹥻�
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