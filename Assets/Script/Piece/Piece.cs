using UnityEngine;
using System.Collections.Generic;

public abstract class Piece : MonoBehaviour
{
    // --- 棋子核心属性 ---
    public enum PieceType { Pawn, Cannon, Elephant, Horse, Rook, Enemy } // 枚举棋子类型
    public PieceType Type; // 当前棋子的具体类型

    [SerializeField] private int _currentMovementCount; // 当前回合剩余移动次数
    public int CurrentMovementCount
    {
        get { return _currentMovementCount; }
        set { _currentMovementCount = Mathf.Max(0, value); } // 确保移动次数不为负
    }

    // --- 心情和性格 ---
    public Mood CurrentMood { get; private set; } // 棋子特有的心情实例
    public Personality PiecePersonality { get; private set; }

    // --- 状态管理 ---
    public PieceStateMachine StateMachine { get; private set; } // 棋子的状态机

    // --- 棋盘位置 ---
    public Vector2Int BoardPosition { get; set; } // 棋子在棋盘上的网格坐标

    // --- 事件 (UI更新、日志...) ---
    public delegate void OnPieceStateChanged(Piece piece, PieceState newState);
    public static event OnPieceStateChanged OnPieceStateChangedEvent; // 棋子状态改变时触发

    public delegate void OnMoodChanged(Piece piece, Mood oldMood, Mood newMood);
    public static event OnMoodChanged OnMoodChangedEvent; // 棋子心情改变时触发

    public virtual void Awake()
    {
        StateMachine = new PieceStateMachine(this); // 初始化棋子状态机
        CurrentMood = new Mood(this); // 初始化棋子的心情实例
    }

    public virtual void Start()
    {
        // 棋子初始状态为待机
        StateMachine.ChangeState(new PieceIdleState(this));
    }

    public virtual void Update()
    {
        // 每帧更新当前状态的逻辑
        StateMachine.CurrentState?.OnUpdate();
    }


    // 初始化棋子的性格和在棋盘上的初始位置
    public void InitializePiece(Personality personality, Vector2Int initialPosition)
    {
        PiecePersonality = personality;
        BoardPosition = initialPosition;
        // 根据性格设置初始移动次数和初始心情
        CurrentMovementCount = personality.BaseMovementCount;
        CurrentMood.SetInitialMood(personality.InitialMoodLevel);
    }

    // --- 抽象方法：具体棋子实现其特有的移动和攻击规则 ---

    // 计算该棋子所有可能的合法移动位置
    public abstract List<Vector2Int> GetPossibleMoves(); 


    // 检查一个目标位置对于当前棋子是否是有效的移动或攻击
    public abstract bool IsValidMove(Vector2Int targetPosition); 

    // 执行棋子的移动操作。
    public virtual void MoveTo(Vector2Int targetPosition)
    {
        BoardPosition = targetPosition; // 更新棋子的内部坐标
        Debug.Log($"{Type} 内部位置更新到 {BoardPosition}");
    }

    // 处理棋子攻击另一个目标棋子的逻辑
    public virtual void Attack(Piece targetPiece)
    {
        Debug.Log($"{Type} 在 {BoardPosition} 攻击了 {targetPiece.Type} 在 {targetPiece.BoardPosition}");
        targetPiece.StateMachine.ChangeState(new PieceDeadState(targetPiece)); // 将目标棋子设置为死亡状态
        // 可以添加攻击动画、音效
    }


    // 回合开始时调用的方法

    public virtual void OnTurnStart()
    {
        CurrentMovementCount = PiecePersonality.BaseMovementCount; // 重置本回合的移动次数
        CurrentMood.ApplyPersonalityEffectOnTurnStart(this); // 应用基于心情的性格效果
        Debug.Log($"{Type} 在 {BoardPosition} 回合开始。心情: {CurrentMood.CurrentMoodLevel}, 移动次数: {CurrentMovementCount}");
    }

    // 回合结束时调用的方法
    public virtual void OnTurnEnd()
    {
        CurrentMood.UpdateMoodBasedOnSurroundings(BoardPosition); 
        CurrentMood.ApplyPersonalityEffectOnTurnEnd(this); // 应用基于心情的性格效果
        Debug.Log($"{Type} 在 {BoardPosition} 回合结束。新心情: {CurrentMood.CurrentMoodLevel}");
    }

    // --- 辅助方法,触发事件和回调 ---

    // 通知外部监听者棋子状态已改变。
    public void SetState(PieceState newState)
    {
        OnPieceStateChangedEvent?.Invoke(this, newState);
        // 可以添加一些通用的视觉or音效？抖一抖？
    }


    // 通知外部监听者棋子心情已改变
    public void OnMoodUpdated(Mood oldMood, Mood newMood)
    {
        OnMoodChangedEvent?.Invoke(this, oldMood, newMood);
    }
}