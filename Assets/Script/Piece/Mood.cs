using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq; 

[System.Serializable]
public class Mood
{
    private Piece _ownerPiece; // 拥有此心情的棋子实例
    [Range(0, 4)] // 心情等级范围限制在0到5
    [SerializeField] private int _currentMoodLevel; // 当前心情等级

    public int CurrentMoodLevel
    {
        get { return _currentMoodLevel; }
        private set
        {
            int oldMood = _currentMoodLevel; // 记录旧的心情值
            _currentMoodLevel = Mathf.Clamp(value, 0, 4); // 限制心情在有效范围内
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
        _currentMoodLevel = 2; // 默认初始心情值
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

    // 根据棋子周围3x3和5x5范围内的友方棋子数量更新心情
    public void UpdateMoodBasedOnSurroundings(Vector2Int piecePosition)
    {
        if (BoardManager.Instance == null) return;

        List<Piece> surroundingPieces = BoardManager.Instance.GetPiecesInRange(piecePosition, 1); // 1格范围表示3x3

        // 计算友方棋子数量(不包括自身)
        int friendlyCount = surroundingPieces.Count(p => p.Type != Piece.PieceType.Enemy && p != _ownerPiece);

        if (friendlyCount != 0)
        {
            CurrentMoodLevel += 1;
        }

        surroundingPieces = BoardManager.Instance.GetPiecesInRange(piecePosition, 2);
        friendlyCount = surroundingPieces.Count(p => p.Type != Piece.PieceType.Enemy && p != _ownerPiece);
        if (friendlyCount == 0)
        {
            CurrentMoodLevel -= 1;
        }

        Debug.Log($"{_ownerPiece.Type} ({_ownerPiece.BoardPosition}) 周围有 {friendlyCount} 个友方棋子。心情更新为: {CurrentMoodLevel}");
    }


    // 在回合开始时，根据当前心情等级应用所属性格的效果
    public void ApplyPersonalityEffectOnTurnStart(Piece piece)
    {
        if (piece.PiecePersonality != null)
        {
            piece.PiecePersonality.ApplyEffectOnTurnStart(piece, CurrentMoodLevel);
        }
    }

    // 在回合结束时，根据当前心情等级应用所属性格的效果。
    public void ApplyPersonalityEffectOnTurnEnd(Piece piece)
    {
        if (piece.PiecePersonality != null)
        {
            piece.PiecePersonality.ApplyEffectOnTurnEnd(piece, CurrentMoodLevel);
        }
    }
}