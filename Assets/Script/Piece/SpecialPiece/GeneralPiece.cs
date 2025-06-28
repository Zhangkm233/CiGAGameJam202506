using System.Collections.Generic;
using UnityEngine;

public class GeneralPiece : Piece
{
    //���������Ҫ���뽫����ƶ���
    public override List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        // ���������κη���ˮƽ����ֱ���Խ��ߣ��ƶ�һ��
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
                // �������ƶ�����λ�򹥻�����
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
        // ����Ӧ���ǲ����ƶ���
        // ��������п��ƶ�����return GetPossibleMoves().Contains(targetPosition);
        // ����ĿǰBoardManager��GeneralPiece��CurrentMovementCount����Ϊ0���˷������ƶ�ʱʵ���ϲ��ᱻ����
        return GetPossibleMoves().Contains(targetPosition);
    }
}
