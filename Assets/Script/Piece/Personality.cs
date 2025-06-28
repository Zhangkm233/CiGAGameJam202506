using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[ CreateAssetMenu(fileName = "NewPersonality", menuName = "Game/Personality")]
public class Personality : ScriptableObject
{
    public string PersonalityName; // 性格名称
    [TextArea] public string Description; // 性格描述

    [Tooltip("棋子的基础移动次数")]
    public int BaseMovementCount = 1;
    [Tooltip("棋子的初始心情等级")]
    [Range(0, 4)]
    public int InitialMoodLevel = 2;

    // 存储不同心情区间对应的特质效果列表
    public List<MoodEffect> MoodEffects = new List<MoodEffect>();

    // 定义在特定心情区间内触发的效果
    [System.Serializable]
    public class MoodEffect
    {
        public string EffectName; // 效果名称，“乐观增益？”
        [Tooltip("心情等级的下限 (包含)")]
        [Range(0, 4)] public int MinMood; // 心情区间下限
        [Tooltip("心情等级的上限 (包含)")]
        [Range(0, 4)] public int MaxMood; // 心情区间上限

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
        public int Value; // 效果的数值，例如增加的移动次数、造成的伤害量等
        public int Range; // 效果范围 (用于像 HealAdjacentFriendlies 这样的效果)
        public Piece.PieceType TargetType; // 目标棋子类型 (用于针对性效果)
    }

    // 定义所有可能的特质效果类型（可扩展）
    public enum EffectType
    {
        IncreaseMovementCountOfSelf, // 增加自身移动次数
        IncreaseMovementCountOfAdjacentFriendlies, // 增加相邻友方棋子的移动次数
        IncreaseMovementCountOfAllFriendlies, // 增加所有友方棋子的移动次数
        IncreaseMoodOfFriendliesInRange, // 增加范围内友方棋子的心情等级
        IncreaseSelectableRangeOf,   //自身的可选择的移动范围增加自身3x3范围
        DestorySelf, // 销毁自身棋子
        DecreaseMood, // 减少心情等级
        IncreaseMood, // 增加心情等级
        CantEatEnemy, // 无法吃掉敌人棋子
        CantMove, // 无法移动
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

    // 根据效果数据具体应用效果
    private void ApplyEffect(Piece sourcePiece, EffectData data)
    {
        if (BoardManager.Instance == null) return; 

        switch (data.Type)
        {
            case EffectType.IncreaseMovementCountOfSelf:
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 增加自身 {data.Value} 移动次数。");
                sourcePiece.CurrentMovementCount += data.Value;
                Debug.Log($"{sourcePiece.Type} 因 {PersonalityName} (心情: {sourcePiece.CurrentMood.CurrentMoodLevel}) 增加自身 {data.Value} 移动次数。");
                break;
            case EffectType.IncreaseMovementCountOfAdjacentFriendlies:
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 增加相邻友方棋子 {data.Value} 移动次数。");
                List<Piece> adjacentFriendlies = BoardManager.Instance.GetAdjacentPieces(sourcePiece.BoardPosition)
                                                                     .Where(p => p.Type != Piece.PieceType.Enemy)
                                                                     .ToList();
                foreach (var friendly in adjacentFriendlies)
                {
                    friendly.CurrentMovementCount += data.Value;
                    Debug.Log($"{friendly.Type} (友方) 因 {PersonalityName} 增加了 {data.Value} 移动次数。");
                }
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 使相邻友方增加 {data.Value} 移动次数。");
                break;
            case EffectType.IncreaseMovementCountOfAllFriendlies:
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 增加所有友方棋子 {data.Value} 移动次数。");
                List<Piece> allFriendlies = BoardManager.Instance.GetFriendlyPieces();
                foreach (var friendly in allFriendlies) {
                    friendly.CurrentMovementCount += data.Value;
                    Debug.Log($"{friendly.Type} (友方) 因 {PersonalityName} 增加了 {data.Value} 移动次数。");
                }
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 使所有友方增加 {data.Value} 移动次数。");
                break;
            case EffectType.IncreaseMoodOfFriendliesInRange:
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 增加范围内友方棋子 {data.Value} 心情。");
                List<Piece> friendliesInRange = BoardManager.Instance.GetPiecesInRange(sourcePiece.BoardPosition,data.Range)
                                                                     .Where(p => p.Type != Piece.PieceType.Enemy)
                                                                     .ToList();
                foreach (var friendly in friendliesInRange) {
                    friendly.CurrentMood.IncreaseMood(data.Value);
                    Debug.Log($"{friendly.Type} (友方) 因 {PersonalityName} 增加了 {data.Value} 心情。");
                }
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 使范围内友方增加 {data.Value} 心情。");
                break;
            case EffectType.DecreaseMood:
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 心情减少了 {data.Value}。");
                sourcePiece.CurrentMood.DecreaseMood(data.Value);
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 心情减少了 {data.Value}，当前心情等级: {sourcePiece.CurrentMood.CurrentMoodLevel}。");
                break;
            case EffectType.IncreaseMood:
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 心情增加了 {data.Value}。");
                sourcePiece.CurrentMood.IncreaseMood(data.Value);
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 心情增加了 {data.Value}，当前心情等级: {sourcePiece.CurrentMood.CurrentMoodLevel}。");
                break;
            case EffectType.DestorySelf:
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 销毁了自身棋子。");
                BoardManager.Instance.RemovePiece(sourcePiece.BoardPosition,false); // 调用 BoardManager 的攻击方法销毁自身
                break;
            case EffectType.CantEatEnemy:
                BoardManager.Instance._PacifismPieces.Add(sourcePiece);
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 无法吃掉敌人棋子。");
                break;
            case EffectType.IncreaseSelectableRangeOf:
                // TODO : 这里可以实现增加自身可选择范围的逻辑
                // 例如，增加3x3范围内的可选位置
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 增加了 {data.Range} 的可选择范围。");
                break;
            case EffectType.CantMove:
                BoardManager.Instance._StunPieces.Add(sourcePiece);
                Debug.Log($"{sourcePiece.Type} (性格: {PersonalityName}) 无法移动。");
                break;
        }
    }
}
