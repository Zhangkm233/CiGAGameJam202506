using UnityEngine;
using System.Collections; 

public class PieceMovingState : PieceState
{
    private Vector2Int _targetPosition;
    private float _moveSpeed = 5f; // Ŀǰ���ﲻֱ��ʹ�ã�ĿǰBoardManager�������Ӿ��ƶ�������Ч��������Բ�ֵ����ʹ�ö����Ĳ���ʵ��

    public PieceMovingState(Piece piece, Vector2Int targetPosition) : base(piece)
    {
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} �����ƶ�״̬��Ŀ�꣺{_targetPosition}");
        // BoardManager����ʵ�ʵ������ƶ���λ�ø���
        // ��״̬��Ҫ����ָʾ���������ƶ�������
        // �ƶ���ɺ��״̬�ı��߼�Ӧ��BoardManager����ص���
        // Ŀǰ��BoardManager�����ƶ���ص��Ըı�״̬
    }

    public override void OnUpdate()
    {
        // ������Ҫ��ֵ��ʵ��ƽ���ƶ���
        // �˴�ʵ��Ϊ�򵥵�����BoardManager��MovePiece������ֱ�ӵ�λ�øı�
        // �˴���״̬�ı䷴ӳ���ƶ��ġ���������Ȼ�����ʣ���ƶ�����ת���ؿ���/ѡ��״̬

        // �����ڲ�����λ��
        _piece.MoveTo(_targetPosition);

        _piece.CurrentMovementCount--; // ����һ���ƶ���

        // ת������һ��״̬
        if (_piece.CurrentMovementCount > 0 && _piece.Type != Piece.PieceType.Enemy)
        {
            _piece.StateMachine.ChangeState(new PieceSelectedState(_piece)); // �����Ȼ���ƶ����򱣳�ѡ��״̬
        }
        else
        {
            _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // û�и����ƶ��������������״̬
        }
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳��ƶ�״̬.");
    }
}