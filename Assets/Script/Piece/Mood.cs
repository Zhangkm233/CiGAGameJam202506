using UnityEngine;
using System;

[System.Serializable]
public class Mood
{
    private Piece _ownerPiece; // ӵ�д����������ʵ��

    [Range(0, 100)] // ����ȼ���Χ������ 0 �� 100
    [SerializeField] private int _currentMoodLevel; // ��ǰ����ȼ�

    public int CurrentMoodLevel
    {
        get { return _currentMoodLevel; }
        private set
        {
            int oldMood = _currentMoodLevel; // ��¼�ɵ�����ֵ
            _currentMoodLevel = Mathf.Clamp(value, 0, 100); // ������������Ч��Χ��
            if (oldMood != _currentMoodLevel) // �������ֵʵ�ʷ����˸ı�
            {
                // ������������¼���֪ͨ�ⲿ�����ߣ�UI�ȣ�
                _ownerPiece.OnMoodUpdated(new Mood(_ownerPiece, oldMood), this);
                Debug.Log($"{_ownerPiece.Type} ������� {oldMood} ��Ϊ {_currentMoodLevel}");
            }
        }
    }

    // ���캯������ʼ������
    public Mood(Piece owner)
    {
        _ownerPiece = owner;
        _currentMoodLevel = 50; // Ĭ�ϳ�ʼ����ֵ
    }

    // ���캯�����أ����¼��д��ݾɵ�����ֵʵ��
    public Mood(Piece owner, int initialMoodLevel) : this(owner)
    {
        _currentMoodLevel = initialMoodLevel;
    }

    // �������ӵĳ�ʼ����ȼ�
    public void SetInitialMood(int initialLevel)
    {
        CurrentMoodLevel = initialLevel;
    }

    // ��������ֵ
    public void IncreaseMood(int amount)
    {
        CurrentMoodLevel += amount;
    }

    // ��������ֵ
    public void DecreaseMood(int amount)
    {
        CurrentMoodLevel -= amount;
    }

    // ����������Χ 3x3 ��Χ�ڵ��ѷ�����������������
    public void UpdateMoodBasedOnSurroundings(Vector2Int piecePosition, /* TODO: ��Ҫ BoardManager ���Ͳ��� */ object boardManager) // ���� BoardManager
    {
        // �������̱������ѷ����
        int friendlyCount = 0;
        // var actualBoardManager = boardManager as BoardManager;
        // if (actualBoardManager != null)
        // {
        //     // ���� 3x3 ��Χ
        //     for (int x = -1; x <= 1; x++)
        //     {
        //         for (int y = -1; y <= 1; y++)
        //         {
        //             if (x == 0 && y == 0) continue; // ��������

        //             Vector2Int adjacentPos = piecePosition + new Vector2Int(x, y);
        //             // TODO:��Ҫ BoardManager ����ȡ adjacentPos λ���ϵ�����
        //             Piece adjacentPiece = actualBoardManager.GetPieceAtPosition(adjacentPos);

        //             if (adjacentPiece != null && adjacentPiece.IsFriendly())
        //             {
        //                 friendlyCount++;
        //             }
        //         }
        //     }
        // }


        if (friendlyCount > 0)
        {
            IncreaseMood(5 * friendlyCount); // �����ѷ������������飬�ɵ�����ֵ
            Debug.Log($"{_ownerPiece.Type} ��Χ�� {friendlyCount} ���ѷ����������ӡ�");
        }
        else
        {
            DecreaseMood(10); // ��Χû���ѷ���������٣��ɵ�����ֵ
            Debug.Log($"{_ownerPiece.Type} ��Χû���ѷ���������١�");
        }
    }

    // �ڻغϿ�ʼʱ�����ݵ�ǰ����ȼ�Ӧ�������Ը��Ч��
    public void ApplyPersonalityEffectOnTurnStart(Piece piece, /* TODO: ��Ҫ BoardManager ���Ͳ��� */ object boardManager)
    {
        if (piece.PiecePersonality != null)
        {
            piece.PiecePersonality.ApplyEffectOnTurnStart(piece, CurrentMoodLevel, boardManager); // ���� BoardManager
        }
    }

    // �ڻغϽ���ʱ�����ݵ�ǰ����ȼ�Ӧ�������Ը��Ч����
    public void ApplyPersonalityEffectOnTurnEnd(Piece piece, /* TODO: ��Ҫ BoardManager ���Ͳ��� */ object boardManager)
    {
        if (piece.PiecePersonality != null)
        {
            piece.PiecePersonality.ApplyEffectOnTurnEnd(piece, CurrentMoodLevel, boardManager); // ���� BoardManager
        }
    }
}