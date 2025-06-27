using UnityEngine;
using System.Collections.Generic;

public class CannonPiece : Piece
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
            bool hasScreen = false; // 是否遇到了“炮架”
            for (int i = 1; ; i++)
            {
                Vector2Int nextPos = currentPos + dir * i;

                // if (!actualBoardManager.IsValidBoardPosition(nextPos)) break; // 超出棋盘

                // Piece targetPiece = actualBoardManager.GetPieceAtPosition(nextPos);

                // if (!hasScreen) // 没有遇到炮架
                // {
                //     if (targetPiece == null) // 空格，可以移动
                //     {
                //         possibleMoves.Add(nextPos);
                //     }
                //     else // 遇到棋子，作为炮架
                //     {
                //         hasScreen = true;
                //     }
                // }
                // else // 已经有了炮架
                // {
                //     if (targetPiece != null) // 遇到第二个棋子，可以攻击
                //     {
                //         if (targetPiece.Type != Type) // 如果是敌方棋子
                //         {
                //             possibleMoves.Add(nextPos); // 视为可攻击位置
                //         }
                //         break; // 攻击或遇到友方棋子后，此方向停止
                //     }
                //     // 如果遇到空格，继续跳过，直到遇到第二个棋子或出界?or没有检测到敌方棋子就不允许该方向的移动？
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