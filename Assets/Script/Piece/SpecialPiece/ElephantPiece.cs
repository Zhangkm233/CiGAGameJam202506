using UnityEngine;
using System.Collections.Generic;

public class ElephantPiece : Piece
{

    public override List<Vector2Int> GetPossibleMoves(/*TODO:��ҪBoardManager���Ͳ���*/ object boardManager)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int currentPos = BoardPosition;

        // ����ĸ�б����� (2��Զ)
        Vector2Int[] possibleOffsets = new Vector2Int[]
        {
            new Vector2Int(2, 2), new Vector2Int(2, -2),
            new Vector2Int(-2, 2), new Vector2Int(-2, -2)
        };

        foreach (Vector2Int offset in possibleOffsets)
        {
            Vector2Int targetPos = currentPos + offset;
            Vector2Int eyePos = currentPos + offset / 2; // ����λ��

            // if (actualBoardManager.IsValidBoardPosition(targetPos) && !actualBoardManager.IsAcrossRiver(targetPos, Type)) // ����Ƿ����������Ҳ�����
            // {
            //     // ����Ƿ�������
            //     if (actualBoardManager.GetPieceAtPosition(eyePos) == null)
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