using UnityEngine;

// --- ����״̬ ---
public class PieceIdleState : PieceState
{
    public PieceIdleState(Piece piece) : base(piece) { }

    public override void OnEnter()
    {
        // ���ӽ������״̬

        Debug.Log($"{_piece.name} �������״̬.");
    }

    public override void OnUpdate()
    {
        // ����״̬�¼����������

    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳�����״̬.");
    }
}