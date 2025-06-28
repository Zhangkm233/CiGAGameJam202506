using UnityEngine;

public class PieceDeadState : PieceState
{
    public PieceDeadState(Piece piece) : base(piece) { }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} ��������״̬.");
        // �������Ӿ�Ч�� (e.g.��������ը)
        _piece.gameObject.SetActive(false); // ��ʱ��������
        // BoardManager���ջ�����ֵ����Ƴ�����
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳�����״̬.");
    }
}