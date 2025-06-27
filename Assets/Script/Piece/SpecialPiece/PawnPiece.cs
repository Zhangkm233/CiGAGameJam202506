using UnityEngine;
using System.Collections.Generic;

public class PawnPiece : Piece
{

    public override List<Vector2Int> GetPossibleMoves(/*TODO:��ҪBoardManager���Ͳ���*/ object boardManager)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int currentPos = BoardPosition;

        // ����4��������������ϡ��¡�����
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), new Vector2Int(0, -1), // ����
            new Vector2Int(1, 0), new Vector2Int(-1, 0)  // ����
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int nextPos = currentPos + dir; // ÿ��ֻ�ƶ�һ��

            // if (actualBoardManager.IsValidBoardPosition(nextPos)) // ����Ƿ���������
            // {
            //     Piece targetPiece = actualBoardManager.GetPieceAtPosition(nextPos);
            //     // Ŀ��λ��Ϊ�գ�����Ŀ���ǵз�����
            //     if (targetPiece == null || targetPiece.Type != Type) 
            //     {
            //         possibleMoves.Add(nextPos);
            //     }
            // }
        }

        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition, /*TODO:��ҪBoardManager���Ͳ���*/ object boardManager)
    {
        // ���Ŀ��λ���Ƿ���GetPossibleMoves���ص��б���
        List<Vector2Int> possibleMoves = GetPossibleMoves(boardManager);
        return possibleMoves.Contains(targetPosition);
    }
}