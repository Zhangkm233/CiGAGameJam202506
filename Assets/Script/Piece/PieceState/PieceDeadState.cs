using UnityEngine;
public class PieceDeadState : PieceState
{
    public PieceDeadState(Piece piece) : base(piece) { }
    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} 死亡了！");
        // 播放死亡动画、音效
        _piece.gameObject.SetActive(false);
    }
    public override void OnExit()
    {
        // 死亡状态的清理，依赖对象池？
        // GameObject.Destroy(_piece.gameObject);
    }
}