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
            // 移动
            for (int i = 1; ; i++)
            {
                Vector2Int targetPos = BoardPosition + dir * i;
                if (!BoardManager.Instance.IsValidBoardPosition(targetPos)) break;

                Piece pieceAtTarget = BoardManager.Instance.GetPieceAtPosition(targetPos);
                if (pieceAtTarget == null)
                {
                    possibleMoves.Add(targetPos);
                }
                else // 找到一个棋子，不能再往这个方向移动
                {
                    break;
                }
            }

            // 攻击
            bool foundScreen = false; // 炮架
            for (int i = 1; ; i++)
            {
                Vector2Int currentCheckPos = BoardPosition + dir * i;
                if (!BoardManager.Instance.IsValidBoardPosition(currentCheckPos)) break;

                Piece pieceAtCheck = BoardManager.Instance.GetPieceAtPosition(currentCheckPos);
                if (pieceAtCheck != null)
                {
                    if (!foundScreen)
                    {
                        foundScreen = true; // 找到炮架
                    }
                    else // 在炮架之后找到第二个棋子
                    {
                        if (pieceAtCheck.Type != Type) // 如果是敌人，则是有效的攻击目标
                        {
                            possibleMoves.Add(currentCheckPos);
                        }
                        break; // 不能穿过两个棋子攻击
                    }
                }
                else if (foundScreen) // 如果找到炮架，现在没有棋子，不能再攻击
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