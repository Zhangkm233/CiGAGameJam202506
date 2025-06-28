using UnityEngine;

// 抽象基类，定义了所有棋子状态的通用接口
public abstract class PieceState
{
    protected Piece _piece; // 对拥有此状态的棋子实例的引用

    public PieceState(Piece piece)
    {
        _piece = piece;
    }

    public virtual void OnEnter() { } // 初始化状态行为
    public virtual void OnUpdate() { } // 处理状态逻辑
    public virtual void OnExit() { }  // 清理状态资源
}