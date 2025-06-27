using UnityEngine;
using static Piece;

public class PieceMovingState : PieceState
{
    private Vector2Int _targetPosition; // ����Ҫ�ƶ�����Ŀ��λ��
    private float _moveSpeed = 5f; // �����ƶ��ٶ�

    public PieceMovingState(Piece piece, Vector2Int targetPosition) : base(piece)
    {
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} �����ƶ�״̬��Ŀ�꣺{_targetPosition}");
        // �����ƶ���������Ч

    }

    public override void OnUpdate()
    {

        _piece.MoveTo(_targetPosition); // ���������ڲ�λ��

        // ����Ƿ������ӱ����Ե���
    

        _piece.CurrentMovementCount--; // ����һ���ƶ�����

        // ����ʣ���ƶ�����������һ��״̬
        if (_piece.CurrentMovementCount > 0 && _piece.Type != PieceType.Enemy)
        {
            _piece.StateMachine.ChangeState(new PieceSelectedState(_piece)); // ��������ƶ����������Լ�������
        }
        else
        {
            _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // �ƶ��������꣬�ص�����״̬

        }
        
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳��ƶ�״̬.");
    }
}