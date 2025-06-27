using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPersonality", menuName = "Game/Personality")]
public class Personality : ScriptableObject
{
    public string PersonalityName; // 性格名称
    [TextArea] public string Description; // 性格描述

    [Tooltip("棋子的基础移动次数")]
    public int BaseMovementCount = 1;

    [Tooltip("棋子的初始心情等级")]
    [Range(0, 100)]
    public int InitialMoodLevel = 50;

    // 存储不同心情区间对应的特质效果列表
    public List<MoodEffect> MoodEffects = new List<MoodEffect>();


    // 定义在特定心情区间内触发的效果
    [System.Serializable]
    public class MoodEffect
    {
        public string EffectName; // 效果名称，“乐观增益？”
        [Tooltip("心情等级的下限 (包含)")]
        [Range(0, 100)] public int MinMood; // 心情区间下限
        [Tooltip("心情等级的上限 (包含)")]
        [Range(0, 100)] public int MaxMood; // 心情区间上限

        [Header("回合开始时触发的效果")]
        public List<EffectData> OnTurnStartEffects; // 回合开始时应用的效果列表

        [Header("回合结束时触发的效果")]
        public List<EffectData> OnTurnEndEffects; // 回合结束时应用的效果列表
    }


    // 定义一个具体的单一效果数据
    [System.Serializable]
    public class EffectData
    {
        public EffectType Type; // 效果类型
        public int Value; // 效果的数值，增加的移动次数、造成的伤害量等

        public int Range;// 效果范围
        public Piece.PieceType TargetType;// 目标棋子类型
    }

 
    // 定义所有可能的特质效果类型（可扩展）
    public enum EffectType
    {
        IncreaseMovementCountOfSelf, // 增加自身移动次数
        IncreaseMovementCountOfAdjacentFriendlies, // 增加相邻友方棋子的移动次数

    }


    // 在棋子回合开始时，根据其当前心情等级应用相应的性格效果。

    public void ApplyEffectOnTurnStart(Piece piece, int currentMoodLevel)
    {
        foreach (var effect in MoodEffects)
        {
            // 检查当前心情是否在效果的指定区间内
            if (currentMoodLevel >= effect.MinMood && currentMoodLevel <= effect.MaxMood)
            {
                foreach (var data in effect.OnTurnStartEffects)
                {
                    ApplyEffect(piece, data); // 应用此效果
                }
            }
        }
    }


    // 在棋子回合结束时，根据其当前心情等级应用相应的性格效果。

    public void ApplyEffectOnTurnEnd(Piece piece, int currentMoodLevel)
    {
        foreach (var effect in MoodEffects)
        {
            if (currentMoodLevel >= effect.MinMood && currentMoodLevel <= effect.MaxMood)
            {
                foreach (var data in effect.OnTurnEndEffects)
                {
                    ApplyEffect(piece, data); // 应用此效果
                }
            }
        }
    }


    // 根据效果数据具体应用效果。

    private void ApplyEffect(Piece sourcePiece, EffectData data)
    {

        switch (data.Type)
        {
            case EffectType.IncreaseMovementCountOfSelf:
                sourcePiece.CurrentMovementCount += data.Value;
                Debug.Log($"{sourcePiece.Type} 因 {PersonalityName} (心情: {sourcePiece.CurrentMood.CurrentMoodLevel}) 增加自身 {data.Value} 移动次数。");
                break;

            case EffectType.IncreaseMovementCountOfAdjacentFriendlies:

                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 试图使相邻友方增加 {data.Value} 移动次数 (需要BoardManager才能实现遍历)。");
                break;

        }
    }
}