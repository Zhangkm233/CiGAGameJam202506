using UnityEngine;
using System.Collections.Generic;

public class RookPiece : Piece
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
            for (int i = 1; ; i++) // �ص�ǰ���������
            {
                Vector2Int nextPos = currentPos + dir * i;

                // if (!actualBoardManager.IsValidBoardPosition(nextPos)) // �������̱߽�
                // {
                //     break;
                // }

                // Piece targetPiece = actualBoardManager.GetPieceAtPosition(nextPos);

                // if (targetPiece == null) // Ŀ��λ��Ϊ�գ������ƶ�
                // {
                //     possibleMoves.Add(nextPos);
                // }
                // else // Ŀ��λ��������
                // {
                //     if (targetPiece.Type != Type) // ����ǵз����ӣ����Թ�����Ȼ��ֹͣ�˷��������
                //     {
                //         possibleMoves.Add(nextPos);
                //     }
                //     break; // �����ѷ����ӻ򹥻��з����Ӻ�ֹͣ
                // }
                break; 
            }
        }
        return possibleMoves;
    }

    public override bool IsValidMove(Vector2Int targetPosition, /*TODO:��ҪBoardManager���Ͳ���*/ object boardManager)
    {
        List<Vector2Int> possibleMoves = GetPossibleMoves(boardManager);
        return possibleMoves.Contains(targetPosition);
    }
}