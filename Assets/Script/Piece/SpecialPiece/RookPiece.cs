using UnityEngine;
using System.Collections.Generic;

public class RookPiece : Piece
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
            for (int i = 1; ; i++) // 沿当前方向逐格检测
            {
                Vector2Int nextPos = currentPos + dir * i;

                // if (!actualBoardManager.IsValidBoardPosition(nextPos)) // 超出棋盘边界
                // {
                //     break;
                // }

                // Piece targetPiece = actualBoardManager.GetPieceAtPosition(nextPos);

                // if (targetPiece == null) // 目标位置为空，可以移动
                // {
                //     possibleMoves.Add(nextPos);
                // }
                // else // 目标位置有棋子
                // {
                //     if (targetPiece.Type != Type) // 如果是敌方棋子，可以攻击，然后停止此方向的搜索
                //     {
                //         possibleMoves.Add(nextPos);
                //     }
                //     break; // 遇到友方棋子或攻击敌方棋子后停止
                // }
                break; 
            }
        }
        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition, /*TODO:需要BoardManager类型参数*/ object boardManager)
    {
        List<Vector2Int> possibleMoves = GetPossibleMoves(boardManager);
        return possibleMoves.Contains(targetPosition);
    }
}