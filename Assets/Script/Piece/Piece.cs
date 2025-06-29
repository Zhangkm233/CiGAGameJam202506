using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // ����DoTween�����ռ�
using System.Collections; 

public abstract class Piece : MonoBehaviour
{
    // --- ���Ӻ������� ---
    public enum PieceType { Pawn, Cannon, Elephant, Horse, Rook, Enemy, General } // ö����������
    public PieceType Type; // ��ǰ���ӵľ�������
    public bool hasBeenSpawned = false;//�Ƿ��Ѿ������ɹ�

    [SerializeField] private int _originMovementCount;
    [SerializeField] private int _currentMovementCount; // ��ǰ�غ�ʣ���ƶ�����
    public int CurrentMovementCount
    {
        get { return _currentMovementCount; }
        set { _currentMovementCount = Mathf.Max(0, value); } // ȷ���ƶ�������Ϊ����
    }
    public int OriginMovementCount
    {
        get { return _originMovementCount; }
        set { _originMovementCount = Mathf.Max(0, value); } // ȷ����ʼ�ƶ���������Ϊ1
    } // ��ʼ�ƶ�����

    // --- ������Ը� ---
    public Mood CurrentMood { get; private set; } // �������е�����ʵ��
    public Personality PiecePersonality { get; private set; }

    // --- ״̬���� ---
    public PieceStateMachine StateMachine { get; private set; } // ���ӵ�״̬��

    // --- ����λ�� ---
    public Vector2Int BoardPosition { get; set; } // �����������ϵ���������

    // --- �¼� (����UI���¡���־��) ---
    public delegate void OnPieceStateChanged(Piece piece, PieceState newState);
    public static event OnPieceStateChanged OnPieceStateChangedEvent; // ����״̬�ı�ʱ����

    public delegate void OnMoodChanged(Piece piece, Mood oldMood, Mood newMood);
    public static event OnMoodChanged OnMoodChangedEvent; // ��������ı�ʱ����

    public delegate void OnPieceAttackFinished(Piece attacker, Piece target);
    public static event OnPieceAttackFinished OnPieceAttackFinishedEvent; // ����ʱ�̴���

    // �����¼����������ƶ�������ɺ󴥷�
    // ����¼�Ӧ����BoardManager�ж��ĺ�ȡ�����ģ��Ա����ڴ�й©
    public event System.Action OnMoveAnimationCompleted;

    public virtual void Awake()
    {
        StateMachine = new PieceStateMachine(this); // ��ʼ������״̬��
        CurrentMood = new Mood(this); // ��ʼ�����ӵ�����ʵ��
    }

    public virtual void Start()
    {
        // ���ӳ�ʼ״̬Ϊ����
        StateMachine.ChangeState(new PieceIdleState(this));
    }

    public virtual void Update()
    {
        // ÿ֡���µ�ǰ״̬���߼�
        StateMachine.CurrentState?.OnUpdate();
    }

    // ��ʼ�����ӵ��Ը���������ϵĳ�ʼλ��
    public void InitializePiece(Personality personality, Vector2Int initialPosition)
    {
        PiecePersonality = personality;
        BoardPosition = initialPosition;
        // �����Ը����ó�ʼ�ƶ������ͳ�ʼ����
        if (Type != PieceType.Enemy)
        {
            OriginMovementCount = GameData.GetRandomMovementCount(Type);
        }
        else
        {
            OriginMovementCount = 1; // �������ӵ�Ĭ�ϳ�ʼ�ƶ�����
        }
        CurrentMovementCount = OriginMovementCount;

        if (personality != null) // �����Ը����ʱ���ã����磬���ڷǵ������ӣ�
        {
            CurrentMood.SetInitialMood(personality.InitialMoodLevel);
        }
        else // ���˻�û���Ը�����ӵ�Ĭ��ֵ
        {
            CurrentMood.SetInitialMood(2); // Ĭ������
        }
    }

    // --- ���󷽷����ɾ�����������ʵ�������е��ƶ��͹������� ---
    // ������������п��ܵĺϷ��ƶ�λ��
    public abstract List<Vector2Int> GetPossibleMoves();

    // ���һ��Ŀ��λ�ö��ڵ�ǰ�����Ƿ�����Ч���ƶ��򹥻�
    public abstract bool IsValidMove(Vector2Int targetPosition);

    // ִ�����ӵ��ڲ�λ�ø��¡�
    // �������ֻ��������߼�λ�ã�������MovingAnimation����
    public virtual void MoveTo(Vector2Int targetPosition)
    {
        BoardPosition = targetPosition; // �������ӵ��ڲ�����
        Debug.Log($"{Type} �ڲ�λ�ø��µ� {BoardPosition}");
    }

    // ����ƽ���ƶ��Ķ��� (����Ϊ�ȷŴ� -> �ƶ� -> ��С�����ڶ��������󴥷��¼�)
    public void MovingAnimation(Vector2Int startPosition, Vector2Int targetPosition, Piece targetPiece, System.Action onComplete = null)
    {
        transform.DOKill(true);
        Sequence sequence = DOTween.Sequence();

        float scaleTransitionDuration = 0.2f;
        float totalMoveDuration = 0.6f;

        // ȷ��BoardManager.Instance�ǿɷ��ʵĵ���
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

        // Ϊ����������� OnComplete �ص�����
        sequence.OnComplete(() =>
        {
            // �ڶ������ʱ����ִ�й����߼�
            if (targetPiece != null)
            {
                BoardManager.Instance.AttackPiece(this, targetPiece);
            }
            // Ȼ��ִ���ⲿ����� nComplete�ص�
            onComplete?.Invoke();
            // ��󣬴��������ƶ���������¼�
            OnMoveAnimationCompleted?.Invoke(); // �������ʱ�����¼�
        });
    }

    // �������ӹ�����һ��Ŀ�����ӵ��߼����˷�����������������/��Ч�����ȣ�ʵ�ʳ��Ӻ��Ƴ���BoardManager����
    public virtual void Attack(Piece targetPiece)
    {
        Debug.Log($"{Type} �� {BoardPosition} ������ {targetPiece.Type} �� {targetPiece.BoardPosition}");
        OnPieceAttackFinishedEvent?.Invoke(this, targetPiece); // ������������¼�
        // ���Ź�����������Ч��
    }

    // �غϿ�ʼʱ���õķ���
    public virtual void OnTurnStart()
    {
        // �����ѷ����ӣ������ƶ�������Ӧ���Ը�Ч��
        if (Type != PieceType.Enemy)
        {
            CurrentMovementCount = OriginMovementCount; // ���ñ��غϵ��ƶ�����
            CurrentMood.ApplyPersonalityEffectOnTurnStart(this); // Ӧ�û���������Ը�Ч��
            Debug.Log($"{Type} �� {BoardPosition} �غϿ�ʼ������: {CurrentMood.CurrentMoodLevel}, �ƶ�����: {CurrentMovementCount}");
        }
    }

    // �غϽ���ʱ���õķ���
    public virtual void OnTurnEnd()
    {
        if (Type != PieceType.Enemy)
        {
            CurrentMood.UpdateMoodBasedOnSurroundings(BoardPosition); // ������Χ������������
            CurrentMood.ApplyPersonalityEffectOnTurnEnd(this); // �ڻغϽ���ʱӦ���Ը�Ч��
            Debug.Log($"{Type} �� {BoardPosition} �غϽ�����������: {CurrentMood.CurrentMoodLevel}");
        }
    }

    // --- �������������ڴ����¼��ͻص� ---
    // ֪ͨ�ⲿ����������״̬�Ѹı䡣
    public void SetState(PieceState newState)
    {
        OnPieceStateChangedEvent?.Invoke(this, newState);
    }

    // ֪ͨ�ⲿ���������������Ѹı�
    public void OnMoodUpdated(Mood oldMood, Mood newMood)
    {
        OnMoodChangedEvent?.Invoke(this, oldMood, newMood);
    }
}