using UnityEngine;
// --- ����״̬ ---
public class PieceIdleState : PieceState
{
    public PieceIdleState(Piece piece) : base(piece) { }
    public override void OnEnter()
    {
        // ���ӽ������״̬
        Debug.Log($"{_piece.name} �������״̬.");
        // TODO:���һЩ�����������Ӿ���ʾ
    }
    public override void OnUpdate()
    {
        // ����״̬�¼���������룬���������ӽ���ѡ��֮���
        // TODO:�����ҵ���˴����ӣ��л���PieceSelectedState
        // if (Input.GetMouseButtonDown(0) && IsClicked(_piece))
        // {
        //     _piece.StateMachine.ChangeState(new PieceSelectedState(_piece));
        // }
    }
    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳�����״̬.");
    }

    // �������������ڼ���Ƿ����˸����ӣ���Ҫ���߼��֮���
    private bool IsClicked(Piece piece) { return false; }
}