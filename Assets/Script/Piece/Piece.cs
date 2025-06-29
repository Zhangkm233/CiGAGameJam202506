using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // 引入DoTween命名空间
using System.Collections; 

public abstract class Piece : MonoBehaviour
{
    // --- 棋子核心属性 ---
    public enum PieceType { Pawn, Cannon, Elephant, Horse, Rook, Enemy, General } // 枚举棋子类型
    public PieceType Type; // 当前棋子的具体类型
    public bool hasBeenSpawned = false;//是否已经被生成过

    [SerializeField] private int _originMovementCount;
    [SerializeField] private int _currentMovementCount; // 当前回合剩余移动次数
    public int CurrentMovementCount
    {
        get { return _currentMovementCount; }
        set { _currentMovementCount = Mathf.Max(0, value); } // 确保移动次数不为负数
    }
    public int OriginMovementCount
    {
        get { return _originMovementCount; }
        set { _originMovementCount = Mathf.Max(0, value); } // 确保初始移动次数至少为1
    } // 初始移动次数

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

    // 新增事件：当棋子移动动画完成后触发
    // 这个事件应该在BoardManager中订阅和取消订阅，以避免内存泄漏
    public event System.Action OnMoveAnimationCompleted;

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
        if (Type != PieceType.Enemy)
        {
            OriginMovementCount = GameData.GetRandomMovementCount(Type);
        }
        else
        {
            OriginMovementCount = 1; // 敌人棋子的默认初始移动次数
        }
        CurrentMovementCount = OriginMovementCount;

        if (personality != null) // 仅当性格存在时设置（例如，对于非敌人棋子）
        {
            CurrentMood.SetInitialMood(personality.InitialMoodLevel);
        }
        else // 敌人或没有性格的棋子的默认值
        {
            CurrentMood.SetInitialMood(2); // 默认心情
        }
    }

    // --- 抽象方法：由具体棋子类型实现其特有的移动和攻击规则 ---
    // 计算该棋子所有可能的合法移动位置
    public abstract List<Vector2Int> GetPossibleMoves();

    // 检查一个目标位置对于当前棋子是否是有效的移动或攻击
    public abstract bool IsValidMove(Vector2Int targetPosition);

    // 执行棋子的内部位置更新。
    // 这个方法只负责更新逻辑位置，动画由MovingAnimation负责
    public virtual void MoveTo(Vector2Int targetPosition)
    {
        BoardPosition = targetPosition; // 更新棋子的内部坐标
        Debug.Log($"{Type} 内部位置更新到 {BoardPosition}");
    }

    // 棋子平滑移动的动画 (修正为先放大 -> 移动 -> 缩小，并在动画结束后触发事件)
    public void MovingAnimation(Vector2Int startPosition, Vector2Int targetPosition, Piece targetPiece, System.Action onComplete = null)
    {
        transform.DOKill(true);
        Sequence sequence = DOTween.Sequence();

        float scaleTransitionDuration = 0.2f;
        float totalMoveDuration = 0.6f;

        // 确保BoardManager.Instance是可访问的单例
        Vector3 targetWorldPosition = BoardManager.Instance.GetWorldPosition(targetPosition);

        Tweener scaleUpTweener = transform.DOScale(1.6f, scaleTransitionDuration)
                                         .SetEase(Ease.OutSine);
        Tweener moveTweener = transform.DOMove(targetWorldPosition, totalMoveDuration)
                                       .SetEase(Ease.InOutSine);
        Tweener scaleDownTweener = transform.DOScale(1.0f, scaleTransitionDuration)
                                            .SetEase(Ease.OutSine);

        sequence.Append(moveTweener);
        sequence.Insert(0, scaleUpTweener);
        sequence.Insert(totalMoveDuration - scaleTransitionDuration, scaleDownTweener);

        // 为动画序列添加 OnComplete 回调函数
        sequence.OnComplete(() =>
        {
            // 在动画完成时，先执行攻击逻辑
            if (targetPiece != null)
            {
                BoardManager.Instance.AttackPiece(this, targetPiece);
            }
            // 然后执行外部传入的 nComplete回调
            onComplete?.Invoke();
            // 最后，触发棋子移动动画完成事件
            OnMoveAnimationCompleted?.Invoke(); // 动画完成时触发事件
        });
    }

    // 处理棋子攻击另一个目标棋子的逻辑。此方法仅包含攻击动画/音效触发等，实际吃子和移除由BoardManager处理
    public virtual void Attack(Piece targetPiece)
    {
        Debug.Log($"{Type} 在 {BoardPosition} 攻击了 {targetPiece.Type} 在 {targetPiece.BoardPosition}");
        OnPieceAttackFinishedEvent?.Invoke(this, targetPiece); // 触发攻击完成事件
        // 播放攻击动画、音效等
    }

    // 回合开始时调用的方法
    public virtual void OnTurnStart()
    {
        // 对于友方棋子，重置移动次数并应用性格效果
        if (Type != PieceType.Enemy)
        {
            CurrentMovementCount = OriginMovementCount; // 重置本回合的移动次数
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