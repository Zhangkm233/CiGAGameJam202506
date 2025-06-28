using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // 引入DoTween命名空间

public abstract class Piece : MonoBehaviour
{
    // --- 棋子核心属性 ---
    public enum PieceType { Pawn, Cannon, Elephant, Horse, Rook, Enemy, General } // 枚举棋子类型
    public PieceType Type; // 当前棋子的具体类型

    [SerializeField] private int _currentMovementCount; // 当前回合剩余移动次数
    public int CurrentMovementCount
    {
        get { return _currentMovementCount; }
        set { _currentMovementCount = Mathf.Max(0, value); } // 确保移动次数不为负数
    }

    // --- 心情和性格 ---
    public Mood CurrentMood { get; private set; } // 棋子特有的心情实例
    public Personality PiecePersonality { get; private set; }

    // --- 状态管理 ---
    public PieceStateMachine StateMachine { get; private set; } // 棋子的状态机

    // --- 棋盘位置 ---
    public Vector2Int BoardPosition { get; set; } // 棋子在棋盘上的网格坐标

    // --- 事件 (用于UI更新、日志等) ---
    public delegate void OnPieceStateChanged(Piece piece, PieceState newState);
    public static event OnPieceStateChanged OnPieceStateChangedEvent; // 棋子状态改变时触发

    public delegate void OnMoodChanged(Piece piece, Mood oldMood, Mood newMood);
    public static event OnMoodChanged OnMoodChangedEvent; // 棋子心情改变时触发

    public delegate void OnPieceAttackFinished(Piece attacker, Piece target);
    public static event OnPieceAttackFinished OnPieceAttackFinishedEvent; // 落子时刻触发

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
        if (personality != null) // 仅当性格存在时设置（例如，对于非敌人棋子）
        {
            CurrentMovementCount = personality.BaseMovementCount;
            CurrentMood.SetInitialMood(personality.InitialMoodLevel);
        }
        else // 敌人或没有性格的棋子的默认值
        {
            CurrentMovementCount = 1; // 敌人棋子的默认移动次数（如果没有性格）
            CurrentMood.SetInitialMood(50); // 默认心情
        }
    }

    // --- 抽象方法：由具体棋子类型实现其特有的移动和攻击规则 ---
    // 计算该棋子所有可能的合法移动位置
    public abstract List<Vector2Int> GetPossibleMoves();

    // 检查一个目标位置对于当前棋子是否是有效的移动或攻击
    public abstract bool IsValidMove(Vector2Int targetPosition);

    // 执行棋子的内部位置更新。
    public virtual void MoveTo(Vector2Int targetPosition)
    {
        BoardPosition = targetPosition; // 更新棋子的内部坐标
        Debug.Log($"{Type} 内部位置更新到 {BoardPosition}");
    }

    // 棋子平滑移动的动画 (修正为先放大 -> 移动 -> 缩小，并优化流畅度)
    public void MovingAnimation(Vector2Int startPosition, Vector2Int targetPosition)
    {
        // 防止旧的动画与新的动画发生冲突
        transform.DOKill(true); // true表示也杀死子物体的Tween，以防Canvas等也有Tween

        Sequence sequence = DOTween.Sequence();

        // 定义动画时长
        float scaleTransitionDuration = 0.2f; // 缩放动画的时长
        float totalMoveDuration = 0.6f;      // 整体移动动画的总时长 (可以根据需要调整，让缩放和移动融合得更好)

        // 1. 放大动画：在移动动画开始时并行播放，并快速完成
        // 使用Insert方法，让scaleUpTweener从序列的0秒开始播放
        Tweener scaleUpTweener = transform.DOScale(1.6f, scaleTransitionDuration)
                                          .SetEase(Ease.OutSine); // 使用更平滑的Sine缓动

        // 2. 移动动画：作为序列的主体，定义整体持续时间
        Tweener moveTweener = transform.DOMove(BoardManager.Instance.GetWorldPosition(targetPosition), totalMoveDuration)
                                       .SetEase(Ease.InOutSine); // 使用更平滑的Sine缓动

        // 3. 缩小动画：在移动动画即将结束时并行播放，并在移动结束时完成
        // 使用Insert方法，让scaleDownTweener在moveTweener即将结束时开始
        Tweener scaleDownTweener = transform.DOScale(1.0f, scaleTransitionDuration)
                                            .SetEase(Ease.OutSine); // 使用更平滑的Sine缓动

        // 构建序列
        sequence.Append(moveTweener); // 首先将移动动画添加到序列中，作为主线

        // 将放大动画插入到序列的开始（时间0），与移动动画并行
        sequence.Insert(0, scaleUpTweener);

        // 将缩小动画插入到移动动画结束前开始，与移动动画的尾部并行
        // 确保缩小动画在总移动时间结束时完成
        sequence.Insert(totalMoveDuration - scaleTransitionDuration, scaleDownTweener);

        // 可以在序列完成时添加一个回调，以确保在动画结束后执行任何清理或逻辑
        // 例如：sequence.OnComplete(() => Debug.Log("移动动画完成！"));
    }

    // 处理棋子攻击另一个目标棋子的逻辑。此方法仅包含攻击动画/音效触发等，实际吃子和移除由BoardManager处理。
    public virtual void Attack(Piece targetPiece)
    {
        Debug.Log($"{Type} 在 {BoardPosition} 攻击了 {targetPiece.Type} 在 {targetPiece.BoardPosition}");
        OnPieceAttackFinishedEvent?.Invoke(this, targetPiece); // 触发攻击完成事件
        // 播放攻击动画、音效等，实际的棋子移除和死亡状态设置由 BoardManager.AttackPiece 处理
    }

    // 回合开始时调用的方法
    public virtual void OnTurnStart()
    {
        // 对于友方棋子，重置移动次数并应用性格效果
        if (Type != PieceType.Enemy)
        {
            CurrentMovementCount = PiecePersonality.BaseMovementCount; // 重置本回合的移动次数
            CurrentMood.ApplyPersonalityEffectOnTurnStart(this); // 应用基于心情的性格效果
            Debug.Log($"{Type} 在 {BoardPosition} 回合开始。心情: {CurrentMood.CurrentMoodLevel}, 移动次数: {CurrentMovementCount}");
        }
    }

    // 回合结束时调用的方法
    public virtual void OnTurnEnd()
    {
        if (Type != PieceType.Enemy)
        {
            CurrentMood.UpdateMoodBasedOnSurroundings(BoardPosition); // 根据周围环境更新心情
            CurrentMood.ApplyPersonalityEffectOnTurnEnd(this); // 在回合结束时应用性格效果
            Debug.Log($"{Type} 在 {BoardPosition} 回合结束。新心情: {CurrentMood.CurrentMoodLevel}");
        }
    }

    // --- 辅助方法，用于触发事件和回调 ---
    // 通知外部监听者棋子状态已改变。
    public void SetState(PieceState newState)
    {
        OnPieceStateChangedEvent?.Invoke(this, newState);
    }

    // 通知外部监听者棋子心情已改变
    public void OnMoodUpdated(Mood oldMood, Mood newMood)
    {
        OnMoodChangedEvent?.Invoke(this, oldMood, newMood);
    }
}