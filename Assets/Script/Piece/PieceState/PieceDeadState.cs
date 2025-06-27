using UnityEngine;
public class PieceDeadState : PieceState
{
    public PieceDeadState(Piece piece) : base(piece) { }
    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} �����ˣ�");
        // ����������������Ч
        _piece.gameObject.SetActive(false);
    }
    public override void OnExit()
    {
        // ����״̬��������������أ�
        // GameObject.Destroy(_piece.gameObject);
    }
}