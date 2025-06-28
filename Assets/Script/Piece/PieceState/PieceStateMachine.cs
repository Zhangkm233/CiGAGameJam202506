using UnityEngine;

// ���ӵ�����״̬��������
public class PieceStateMachine
{
    private Piece _ownerPiece; // ״̬������������
    public PieceState CurrentState { get; private set; } // ��ǰ�����״̬

    public PieceStateMachine(Piece owner)
    {
        _ownerPiece = owner;
    }

    // �ı����ӵĵ�ǰ״̬
    public void ChangeState(PieceState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.OnExit(); // �˳���״̬
        }
        CurrentState = newState; // ������״̬
        CurrentState.OnEnter(); // ������״̬
        _ownerPiece.SetState(CurrentState); // ֪ͨ������״̬�Ѹı䣨���ڴ����¼���
        Debug.Log($"{_ownerPiece.Type} ״̬�л���: {CurrentState.GetType().Name}");
    }
}
