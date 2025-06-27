using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPersonality", menuName = "Game/Personality")]
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
        public int Value; // Ч������ֵ�����ӵ��ƶ���������ɵ��˺�����
        public int Range; // Ч����Χ
        public Piece.PieceType TargetType; // Ŀ����������
    }

    // �������п��ܵ�����Ч�����ͣ�����չ��
    public enum EffectType
    {
        IncreaseMovementCountOfSelf, // ���������ƶ�����
        IncreaseMovementCountOfAdjacentFriendlies, // ���������ѷ����ӵ��ƶ�����
        HealAdjacentFriendlies, // ���������ѷ����ӣ�������ֵ����
        DealDamageToAdjacentEnemies, // �����ڵ�����������˺���������ֵ����
    }

    // �����ӻغϿ�ʼʱ�������䵱ǰ����ȼ�Ӧ����Ӧ���Ը�Ч����
    public void ApplyEffectOnTurnStart(Piece piece, int currentMoodLevel, /* TODO: ��Ҫ BoardManager ���Ͳ��� */ object boardManager) // ���� BoardManager
    {
        foreach (var effect in MoodEffects)
        {
            // ��鵱ǰ�����Ƿ���Ч����ָ��������
            if (currentMoodLevel >= effect.MinMood && currentMoodLevel <= effect.MaxMood)
            {
                foreach (var data in effect.OnTurnStartEffects)
                {
                    ApplyEffect(piece, data, boardManager); // Ӧ�ô�Ч�������� BoardManager
                }
            }
        }
    }

    // �����ӻغϽ���ʱ�������䵱ǰ����ȼ�Ӧ����Ӧ���Ը�Ч����
    public void ApplyEffectOnTurnEnd(Piece piece, int currentMoodLevel, /* TODO:��ҪBoardManager���Ͳ��� */ object boardManager) // ����BoardManager
    {
        foreach (var effect in MoodEffects)
        {
            if (currentMoodLevel >= effect.MinMood && currentMoodLevel <= effect.MaxMood)
            {
                foreach (var data in effect.OnTurnEndEffects)
                {
                    ApplyEffect(piece, data, boardManager); // Ӧ�ô�Ч��������BoardManager
                }
            }
        }
    }

    // ����Ч�����ݾ���Ӧ��Ч����
    private void ApplyEffect(Piece sourcePiece, EffectData data, /* TODO:��ҪBoardManager���Ͳ��� */ object boardManager)
    {
        switch (data.Type)
        {
            case EffectType.IncreaseMovementCountOfSelf:
                sourcePiece.CurrentMovementCount += data.Value;
                Debug.Log($"{sourcePiece.Type} �� {PersonalityName} (����: {sourcePiece.CurrentMood.CurrentMoodLevel}) �������� {data.Value} �ƶ�������");
                break;
            case EffectType.IncreaseMovementCountOfAdjacentFriendlies:
                Debug.Log($"{sourcePiece.Type} (�Ը�: {PersonalityName}) ��ͼʹ�����ѷ����� {data.Value} �ƶ�������");
                // TODO:��Ҫ BoardManager ����ȡ���ڵ��ѷ����ӣ����������ƶ�����
                // var actualBoardManager = boardManager as BoardManager;
                // if (actualBoardManager != null)
                // {
                //     // ����sourcePiece��Χ3x3��Χor ��data.Range����ķ�Χ��)
                //     for (int x = -data.Range; x <= data.Range; x++)
                //     {
                //         for (int y = -data.Range; y <= data.Range; y++)
                //         {
                //             Vector2Int adjacentPos = sourcePiece.BoardPosition + new Vector2Int(x, y);
                //             Piece adjacentPiece = actualBoardManager.GetPieceAtPosition(adjacentPos);
                //             if (adjacentPiece != null && adjacentPiece.IsFriendly() && adjacentPiece != sourcePiece)
                //             {
                //                 adjacentPiece.CurrentMovementCount += data.Value;
                //                 Debug.Log($"  - {adjacentPiece.Type} ������ {data.Value} �ƶ�������");
                //             }
                //         }
                //     }
                // }
                break;

        }
    }
}