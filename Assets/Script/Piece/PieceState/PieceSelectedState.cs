using UnityEngine;

public class PieceSelectedState : PieceState
{
    public PieceSelectedState(Piece piece) : base(piece) { }

    public override void OnEnter()
    {
        // ���ӱ�ѡ��ʱ���߼���

        Debug.Log($"{_piece.name} ����ѡ��״̬.");
    }

    public override void OnUpdate()
    {
        // ������ҵ��Ŀ��λ�ý����ƶ�

    }

    public override void OnExit()
    {
        // �˳�ѡ��״̬

        Debug.Log($"{_piece.name} �˳�ѡ��״̬.");
    }
}