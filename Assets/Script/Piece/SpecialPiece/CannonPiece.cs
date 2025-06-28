using System.Collections.Generic;
using UnityEngine;

public class CannonPiece : Piece
{
    public override List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in directions)
        {
            // �ƶ�
            for (int i = 1; ; i++)
            {
                Vector2Int targetPos = BoardPosition + dir * i;
                if (!BoardManager.Instance.IsValidBoardPosition(targetPos)) break;

                Piece pieceAtTarget = BoardManager.Instance.GetPieceAtPosition(targetPos);
                if (pieceAtTarget == null)
                {
                    possibleMoves.Add(targetPos);
                }
                else // �ҵ�һ�����ӣ�����������������ƶ�
                {
                    break;
                }
            }

            // ����
            bool foundScreen = false; // �ڼ�
            for (int i = 1; ; i++)
            {
                Vector2Int currentCheckPos = BoardPosition + dir * i;
                if (!BoardManager.Instance.IsValidBoardPosition(currentCheckPos)) break;

                Piece pieceAtCheck = BoardManager.Instance.GetPieceAtPosition(currentCheckPos);
                if (pieceAtCheck != null)
                {
                    if (!foundScreen)
                    {
                        foundScreen = true; // �ҵ��ڼ�
                    }
                    else // ���ڼ�֮���ҵ��ڶ�������
                    {
                        if (pieceAtCheck.Type != Type) // ����ǵ��ˣ�������Ч�Ĺ���Ŀ��
                        {
                            possibleMoves.Add(currentCheckPos);
                        }
                        break; // ���ܴ����������ӹ���
                    }
                }
                else if (foundScreen) // ����ҵ��ڼܣ�����û�����ӣ������ٹ���
                {
                    break;
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