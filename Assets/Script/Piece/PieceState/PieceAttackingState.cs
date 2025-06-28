using UnityEngine;

public class PieceAttackingState : PieceState
{
    private Piece _targetPiece;
    private Vector2Int _targetPosition; // ������Ҫ�ƶ�����λ��

    public PieceAttackingState(Piece piece, Piece targetPiece, Vector2Int targetPosition) : base(piece)
    {
        _targetPiece = targetPiece;
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} ���빥��״̬��Ŀ�꣺{_targetPiece.name}");

        // �������ӵĹ����߼� (e.g.����)
        _piece.Attack(_targetPiece);

        // BoardManager����ʵ�ʵĹ������ƶ�������
        // ״̬ת���ڹ����߼�����������������
        _piece.CurrentMovementCount--; // ����һ���ƶ���

        // ������ת������һ��״̬
        if (_piece.CurrentMovementCount > 0 && _piece.Type != Piece.PieceType.Enemy)
        {
            _piece.StateMachine.ChangeState(new PieceSelectedState(_piece)); // �����Ȼ���ƶ����򱣳�ѡ��״̬
        }
        else
        {
            _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // û�и����ƶ��������������״̬
        }
    }

    public override void OnUpdate()
    {
        // �ȴ�����������ɣ�
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳�����״̬.");
    }
}