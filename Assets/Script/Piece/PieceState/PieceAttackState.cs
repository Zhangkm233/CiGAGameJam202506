using UnityEngine;

public class PieceAttackingState : PieceState
{
    private Piece _targetPiece;
    private Vector2Int _targetPosition; // �����������ƶ�����λ�ã����Ӻ�

    public PieceAttackingState(Piece piece, Piece targetPiece, Vector2Int targetPosition) : base(piece)
    {
        _targetPiece = targetPiece;
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} ���빥��״̬��Ŀ�꣺{_targetPiece.name}");
        // TODO:���Ź������������Ź�����Ч

        _piece.Attack(_targetPiece); // ִ�й����߼�����Ŀ����Ϊ����״̬
        _piece.MoveTo(_targetPosition); // �������ƶ���Ŀ��λ��

        _piece.CurrentMovementCount--; // ����һ���ƶ�����

        // ������ص�ѡ��״̬�����״̬��ȡ�����Ƿ����ƶ�����
        if (_piece.CurrentMovementCount > 0 && _piece.Type != Piece.PieceType.Enemy)
        {
            _piece.StateMachine.ChangeState(new PieceSelectedState(_piece)); // ��������������ƶ����������Լ�������
        }
        else
        {
            _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // �����������꣬�ص�����״̬
        }
    }

    public override void OnUpdate()
    {
        // ��������������Ϻ�or��ʱ���л���Idle��Selected״̬
        // TODO:���ݶ���or��ʱ��?��ɹ�������
  
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳�����״̬.");
    }
}