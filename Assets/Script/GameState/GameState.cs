using UnityEngine;

public abstract class GameState {
    public virtual void OnEnter() { } // 初始化状态行为
    public virtual void OnUpdate() { } // 处理状态逻辑
    public virtual void OnExit() { }  // 清理状态资源
}
