using UnityEngine;
using System;

[System.Serializable]
public class Mood
{
    private Piece _ownerPiece; // 拥有此心情的棋子实例

    [Range(0, 100)] // 心情等级范围限制在 0 到 100
    [SerializeField] private int _currentMoodLevel; // 当前心情等级
    public int CurrentMoodLevel
    {
        get { return _currentMoodLevel; }
        private set
        {
            int oldMood = _currentMoodLevel; // 记录旧的心情值
            _currentMoodLevel = Mathf.Clamp(value, 0, 100); // 限制心情在有效范围内
            if (oldMood != _currentMoodLevel) // 如果心情值实际发生了改变
            {
                // 触发心情更新事件，通知外部监听者（UI等）
                _ownerPiece.OnMoodUpdated(new Mood(_ownerPiece, oldMood), this);
                Debug.Log($"{_ownerPiece.Type} 的心情从 {oldMood} 变为 {_currentMoodLevel}");
            }
        }
    }


    // 构造函数，初始化心情
    public Mood(Piece owner)
    {
        _ownerPiece = owner;
        _currentMoodLevel = 50; // 默认初始心情值
    }


    // 构造函数重载，在事件中传递旧的心情值实例
    public Mood(Piece owner, int initialMoodLevel) : this(owner)
    {
        _currentMoodLevel = initialMoodLevel;
    }


    // 设置棋子的初始心情等级
    public void SetInitialMood(int initialLevel)
    {
        CurrentMoodLevel = initialLevel;
    }


    // 增加心情值
    public void IncreaseMood(int amount)
    {
        CurrentMoodLevel += amount;
    }


    // 减少心情值
    public void DecreaseMood(int amount)
    {
        CurrentMoodLevel -= amount;
    }

    // 根据棋子周围 3x3 范围内的友方棋子数量更新心情

    public void UpdateMoodBasedOnSurroundings(Vector2Int piecePosition)
    {
        // 进行棋盘遍历和友方检查

    }


    // 在回合开始时，根据当前心情等级应用所属性格的效果
    public void ApplyPersonalityEffectOnTurnStart(Piece piece)
    {
        if (piece.PiecePersonality != null)
        {
            piece.PiecePersonality.ApplyEffectOnTurnStart(piece, CurrentMoodLevel);
        }
    }


    // 在回合结束时，根据当前心情等级应用所属性格的效果
    public void ApplyPersonalityEffectOnTurnEnd(Piece piece)
    {
        if (piece.PiecePersonality != null)
        {
            piece.PiecePersonality.ApplyEffectOnTurnEnd(piece, CurrentMoodLevel);
        }
    }
}