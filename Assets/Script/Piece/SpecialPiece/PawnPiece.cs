using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : Piece
{
    public override List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int forward = new Vector2Int(0, 1); // 卒向前移动

        // 简单的向前移动
        Vector2Int targetPos = BoardPosition + forward;
        if (BoardManager.Instance.IsValidBoardPosition(targetPos) && BoardManager.Instance.GetPieceAtPosition(targetPos) == null)
        {
            possibleMoves.Add(targetPos);
        }

        // 简单的斜线攻击（卒的进化的一种实现想法，不需要可以删去）
        Vector2Int attackLeft = new Vector2Int(-1, 1);
        Vector2Int attackRight = new Vector2Int(1, 1);

        Piece targetPieceLeft = BoardManager.Instance.GetPieceAtPosition(BoardPosition + attackLeft);
        if (BoardManager.Instance.IsValidBoardPosition(BoardPosition + attackLeft) && targetPieceLeft != null && targetPieceLeft.Type == PieceType.Enemy)
        {
            possibleMoves.Add(BoardPosition + attackLeft);
        }

        Piece targetPieceRight = BoardManager.Instance.GetPieceAtPosition(BoardPosition + attackRight);
        if (BoardManager.Instance.IsValidBoardPosition(BoardPosition + attackRight) && targetPieceRight != null && targetPieceRight.Type == PieceType.Enemy)
        {
            possibleMoves.Add(BoardPosition + attackRight);
        }

        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition)
    {
        // 检查目标在可能的移动范围内
        return GetPossibleMoves().Contains(targetPosition);
    }
}
