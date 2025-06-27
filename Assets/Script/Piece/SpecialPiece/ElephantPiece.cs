using UnityEngine;
using System.Collections.Generic;

public class ElephantPiece : Piece
{

    public override List<Vector2Int> GetPossibleMoves(/*TODO:需要BoardManager类型参数*/ object boardManager)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int currentPos = BoardPosition;

        // 象的四个斜线落点 (2格远)
        Vector2Int[] possibleOffsets = new Vector2Int[]
        {
            new Vector2Int(2, 2), new Vector2Int(2, -2),
            new Vector2Int(-2, 2), new Vector2Int(-2, -2)
        };

        foreach (Vector2Int offset in possibleOffsets)
        {
            Vector2Int targetPos = currentPos + offset;
            Vector2Int eyePos = currentPos + offset / 2; // 象眼位置

            // if (actualBoardManager.IsValidBoardPosition(targetPos) && !actualBoardManager.IsAcrossRiver(targetPos, Type)) // 检查是否在棋盘内且不过河
            // {
            //     // 检查是否蹩象腿
            //     if (actualBoardManager.GetPieceAtPosition(eyePos) == null)
            //     {
            //         Piece targetPiece = actualBoardManager.GetPieceAtPosition(targetPos);
            //         if (targetPiece == null || targetPiece.Type != Type) // 如果目标为空或敌方棋子
            //         {
            //             possibleMoves.Add(targetPos);
            //         }
            //     }
            // }
        }
        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition, /*TODO:需要BoardManager类型参数*/ object boardManager)
    {
        List<Vector2Int> possibleMoves = GetPossibleMoves(boardManager);
        return possibleMoves.Contains(targetPosition);
    }
}