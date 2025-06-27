using UnityEngine;
using System.Collections.Generic;

public class HorsePiece : Piece
{

    public override List<Vector2Int> GetPossibleMoves(/*TODO:��ҪBoardManager���Ͳ���*/ object boardManager)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int currentPos = BoardPosition;

        // ���8�����ܵ��������ڵ�ǰλ�õ�ƫ��
        // ��ʽΪ(��1, ��2)��(��2, ��1)
        Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int(1, 2), new Vector2Int(1, -2),
            new Vector2Int(-1, 2), new Vector2Int(-1, -2),
            new Vector2Int(2, 1), new Vector2Int(2, -1),
            new Vector2Int(-2, 1), new Vector2Int(-2, -1)
        };


        foreach (Vector2Int offset in offsets)
        {
            Vector2Int targetPos = currentPos + offset;

            // ��������λ��
            Vector2Int legPos = currentPos;
            if (Mathf.Abs(offset.x) == 2) // ������2�������ں���
            {
                legPos += new Vector2Int(offset.x / 2, 0);
            }
            else // ������2������������
            {
                legPos += new Vector2Int(0, offset.y / 2);
            }

            // if (actualBoardManager.IsValidBoardPosition(targetPos)) // ���Ŀ��λ���Ƿ���������
            // {
            //     // ����Ƿ�������
            //     if (actualBoardManager.GetPieceAtPosition(legPos) == null)
            //     {
            //         Piece targetPiece = actualBoardManager.GetPieceAtPosition(targetPos);
            //         if (targetPiece == null || targetPiece.Type != Type) // ���Ŀ��Ϊ�ջ�з�����
            //         {
            //             possibleMoves.Add(targetPos);
            //         }
            //     }
            // }
        }
        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition, /*TODO:��ҪBoardManager���Ͳ���*/ object boardManager)
    {
        List<Vector2Int> possibleMoves = GetPossibleMoves(boardManager);
        return possibleMoves.Contains(targetPosition);
    }
}