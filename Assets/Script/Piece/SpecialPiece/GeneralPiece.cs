using System.Collections.Generic;
using UnityEngine;

public class GeneralPiece : Piece
{
    //如果后续需要加入将棋的移动？
    public override List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        // 将可以向任何方向（水平、垂直、对角线）移动一步
        Vector2Int[] directions =
        {
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(1, 1), new Vector2Int(1, -1),
            new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int targetPos = BoardPosition + dir;
            if (BoardManager.Instance.IsValidBoardPosition(targetPos))
            {
                Piece targetPiece = BoardManager.Instance.GetPieceAtPosition(targetPos);
                // 将可以移动到空位或攻击敌人
                if (targetPiece == null || targetPiece.Type == PieceType.Enemy)
                {
                    possibleMoves.Add(targetPos);
                }
            }
        }

        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition)
    {
        // 将棋应该是不可移动的
        // 如果后续有可移动需求：return GetPossibleMoves().Contains(targetPosition);
        // 不过目前BoardManager将GeneralPiece的CurrentMovementCount设置为0，此方法在移动时实际上不会被调用
        return GetPossibleMoves().Contains(targetPosition);
    }
}
