using UnityEngine;
using System.Collections.Generic;

public class PawnPiece : Piece
{

    public override List<Vector2Int> GetPossibleMoves(/*TODO:需要BoardManager类型参数*/ object boardManager)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int currentPos = BoardPosition;

        // 定义4个方向的增量：上、下、左、右
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), new Vector2Int(0, -1), // 上下
            new Vector2Int(1, 0), new Vector2Int(-1, 0)  // 左右
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int nextPos = currentPos + dir; // 每次只移动一格

            // if (actualBoardManager.IsValidBoardPosition(nextPos)) // 检查是否在棋盘内
            // {
            //     Piece targetPiece = actualBoardManager.GetPieceAtPosition(nextPos);
            //     // 目标位置为空，或者目标是敌方棋子
            //     if (targetPiece == null || targetPiece.Type != Type) 
            //     {
            //         possibleMoves.Add(nextPos);
            //     }
            // }
        }

        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition, /*TODO:需要BoardManager类型参数*/ object boardManager)
    {
        // 检查目标位置是否在GetPossibleMoves返回的列表中
        List<Vector2Int> possibleMoves = GetPossibleMoves(boardManager);
        return possibleMoves.Contains(targetPosition);
    }
}