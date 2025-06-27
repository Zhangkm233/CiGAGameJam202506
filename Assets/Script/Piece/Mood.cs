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

    public void UpdateMoodBasedOnSurroundings(Vector2Int piecePosition)
    {
        // �������̱������ѷ����

    }


    // �ڻغϿ�ʼʱ�����ݵ�ǰ����ȼ�Ӧ�������Ը��Ч��
    public void ApplyPersonalityEffectOnTurnStart(Piece piece)
    {
        if (piece.PiecePersonality != null)
        {
            piece.PiecePersonality.ApplyEffectOnTurnStart(piece, CurrentMoodLevel);
        }
    }


    // �ڻغϽ���ʱ�����ݵ�ǰ����ȼ�Ӧ�������Ը��Ч��
    public void ApplyPersonalityEffectOnTurnEnd(Piece piece)
    {
        if (piece.PiecePersonality != null)
        {
            piece.PiecePersonality.ApplyEffectOnTurnEnd(piece, CurrentMoodLevel);
        }
    }
}