using UnityEngine;
using System.Collections.Generic;

public class CannonPiece : Piece
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
            bool hasScreen = false; // �Ƿ������ˡ��ڼܡ�
            for (int i = 1; ; i++)
            {
                Vector2Int nextPos = currentPos + dir * i;

                // if (!actualBoardManager.IsValidBoardPosition(nextPos)) break; // ��������

                // Piece targetPiece = actualBoardManager.GetPieceAtPosition(nextPos);

                // if (!hasScreen) // û�������ڼ�
                // {
                //     if (targetPiece == null) // �ո񣬿����ƶ�
                //     {
                //         possibleMoves.Add(nextPos);
                //     }
                //     else // �������ӣ���Ϊ�ڼ�
                //     {
                //         hasScreen = true;
                //     }
                // }
                // else // �Ѿ������ڼ�
                // {
                //     if (targetPiece != null) // �����ڶ������ӣ����Թ���
                //     {
                //         if (targetPiece.Type != Type) // ����ǵз�����
                //         {
                //             possibleMoves.Add(nextPos); // ��Ϊ�ɹ���λ��
                //         }
                //         break; // �����������ѷ����Ӻ󣬴˷���ֹͣ
                //     }
                //     // ��������ո񣬼���������ֱ�������ڶ������ӻ����?orû�м�⵽�з����ӾͲ�����÷�����ƶ���
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