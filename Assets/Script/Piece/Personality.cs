using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[ CreateAssetMenu(fileName = "NewPersonality", menuName = "Game/Personality")]
public class Personality : ScriptableObject
{
    public string PersonalityName; // �Ը�����
    [TextArea] public string Description; // �Ը�����

    [Tooltip("���ӵĻ����ƶ�����")]
    public int BaseMovementCount = 1;
    [Tooltip("���ӵĳ�ʼ����ȼ�")]
    [Range(0, 100)]
    public int InitialMoodLevel = 50;

    // �洢��ͬ���������Ӧ������Ч���б�
    public List<MoodEffect> MoodEffects = new List<MoodEffect>();

    // �������ض����������ڴ�����Ч��
    [System.Serializable]
    public class MoodEffect
    {
        public string EffectName; // Ч�����ƣ����ֹ����棿��
        [Tooltip("����ȼ������� (����)")]
        [Range(0, 100)] public int MinMood; // ������������
        [Tooltip("����ȼ������� (����)")]
        [Range(0, 100)] public int MaxMood; // ������������

        [Header("�غϿ�ʼʱ������Ч��")]
        public List<EffectData> OnTurnStartEffects; // �غϿ�ʼʱӦ�õ�Ч���б�
        [Header("�غϽ���ʱ������Ч��")]
        public List<EffectData> OnTurnEndEffects; // �غϽ���ʱӦ�õ�Ч���б�
    }

    // ����һ������ĵ�һЧ������
    [System.Serializable]
    public class EffectData
    {
        public EffectType Type; // Ч������
        public int Value; // Ч������ֵ���������ӵ��ƶ���������ɵ��˺�����
        public int Range; // Ч����Χ (������ HealAdjacentFriendlies ������Ч��)
        public Piece.PieceType TargetType; // Ŀ���������� (���������Ч��)
    }

    // �������п��ܵ�����Ч�����ͣ�����չ��
    public enum EffectType
    {
        IncreaseMovementCountOfSelf, // ���������ƶ�����
        IncreaseMovementCountOfAdjacentFriendlies, // ���������ѷ����ӵ��ƶ�����
    }

    // �����ӻغϿ�ʼʱ�������䵱ǰ����ȼ�Ӧ����Ӧ���Ը�Ч����
    public void ApplyEffectOnTurnStart(Piece piece, int currentMoodLevel)
    {
        foreach (var effect in MoodEffects)
        {
            // ��鵱ǰ�����Ƿ���Ч����ָ��������
            if (currentMoodLevel >= effect.MinMood && currentMoodLevel <= effect.MaxMood)
            {
                foreach (var data in effect.OnTurnStartEffects)
                {
                    ApplyEffect(piece, data); // Ӧ�ô�Ч��
                }
            }
        }
    }

    // �����ӻغϽ���ʱ�������䵱ǰ����ȼ�Ӧ����Ӧ���Ը�Ч����
    public void ApplyEffectOnTurnEnd(Piece piece, int currentMoodLevel)
    {
        foreach (var effect in MoodEffects)
        {
            if (currentMoodLevel >= effect.MinMood && currentMoodLevel <= effect.MaxMood)
            {
                foreach (var data in effect.OnTurnEndEffects)
                {
                    ApplyEffect(piece, data); // Ӧ�ô�Ч��
                }
            }
        }
    }

    // ����Ч�����ݾ���Ӧ��Ч��
    private void ApplyEffect(Piece sourcePiece, EffectData data)
    {
        if (BoardManager.Instance == null) return; 

        switch (data.Type)
        {
            case EffectType.IncreaseMovementCountOfSelf:
                sourcePiece.CurrentMovementCount += data.Value;
                Debug.Log($"{sourcePiece.Type} �� {PersonalityName} (����: {sourcePiece.CurrentMood.CurrentMoodLevel}) �������� {data.Value} �ƶ�������");
                break;
            case EffectType.IncreaseMovementCountOfAdjacentFriendlies:
                List<Piece> adjacentFriendlies = BoardManager.Instance.GetAdjacentPieces(sourcePiece.BoardPosition)
                                                                     .Where(p => p.Type != Piece.PieceType.Enemy)
                                                                     .ToList();
                foreach (var friendly in adjacentFriendlies)
                {
                    friendly.CurrentMovementCount += data.Value;
                    Debug.Log($"{friendly.Type} (�ѷ�) �� {PersonalityName} ������ {data.Value} �ƶ�������");
                }
                Debug.Log($"{sourcePiece.Type} (�Ը�: {PersonalityName}) ʹ�����ѷ����� {data.Value} �ƶ�������");
                break;

        }
    }
}
