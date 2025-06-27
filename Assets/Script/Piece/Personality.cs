using UnityEngine;
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
        public int Range; // 效果范围
        public Piece.PieceType TargetType; // 目标棋子类型
    }

    // 定义所有可能的特质效果类型（可扩展）
    public enum EffectType
    {
        IncreaseMovementCountOfSelf, // 增加自身移动次数
        IncreaseMovementCountOfAdjacentFriendlies, // 增加相邻友方棋子的移动次数
        HealAdjacentFriendlies, // 治疗相邻友方棋子（有生命值？）
        DealDamageToAdjacentEnemies, // 对相邻敌人棋子造成伤害（有生命值？）
    }

    // 在棋子回合开始时，根据其当前心情等级应用相应的性格效果。
    public void ApplyEffectOnTurnStart(Piece piece, int currentMoodLevel, /* TODO: 需要 BoardManager 类型参数 */ object boardManager) // 传入 BoardManager
    {
        foreach (var effect in MoodEffects)
        {
            // 检查当前心情是否在效果的指定区间内
            if (currentMoodLevel >= effect.MinMood && currentMoodLevel <= effect.MaxMood)
            {
                foreach (var data in effect.OnTurnStartEffects)
                {
                    ApplyEffect(piece, data, boardManager); // 应用此效果，传入 BoardManager
                }
            }
        }
    }

    // 在棋子回合结束时，根据其当前心情等级应用相应的性格效果。
    public void ApplyEffectOnTurnEnd(Piece piece, int currentMoodLevel, /* TODO:需要BoardManager类型参数 */ object boardManager) // 传入BoardManager
    {
        foreach (var effect in MoodEffects)
        {
            if (currentMoodLevel >= effect.MinMood && currentMoodLevel <= effect.MaxMood)
            {
                foreach (var data in effect.OnTurnEndEffects)
                {
                    ApplyEffect(piece, data, boardManager); // 应用此效果，传入BoardManager
                }
            }
        }
    }

    // 根据效果数据具体应用效果。
    private void ApplyEffect(Piece sourcePiece, EffectData data, /* TODO:需要BoardManager类型参数 */ object boardManager)
    {
        switch (data.Type)
        {
            case EffectType.IncreaseMovementCountOfSelf:
                sourcePiece.CurrentMovementCount += data.Value;
                Debug.Log($"{sourcePiece.Type} 因 {PersonalityName} (心情: {sourcePiece.CurrentMood.CurrentMoodLevel}) 增加自身 {data.Value} 移动次数。");
                break;
            case EffectType.IncreaseMovementCountOfAdjacentFriendlies:
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 试图使相邻友方增加 {data.Value} 移动次数。");
                // TODO:需要 BoardManager 来获取相邻的友方棋子，并增加其移动次数
                // var actualBoardManager = boardManager as BoardManager;
                // if (actualBoardManager != null)
                // {
                //     // 遍历sourcePiece周围3x3范围or （data.Range定义的范围？)
                //     for (int x = -data.Range; x <= data.Range; x++)
                //     {
                //         for (int y = -data.Range; y <= data.Range; y++)
                //         {
                //             Vector2Int adjacentPos = sourcePiece.BoardPosition + new Vector2Int(x, y);
                //             Piece adjacentPiece = actualBoardManager.GetPieceAtPosition(adjacentPos);
                //             if (adjacentPiece != null && adjacentPiece.IsFriendly() && adjacentPiece != sourcePiece)
                //             {
                //                 adjacentPiece.CurrentMovementCount += data.Value;
                //                 Debug.Log($"  - {adjacentPiece.Type} 增加了 {data.Value} 移动次数。");
                //             }
                //         }
                //     }
                // }
                break;

        }
    }
}