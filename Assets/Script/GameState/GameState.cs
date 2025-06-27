using UnityEngine;

public abstract class GameState {
    public virtual void OnEnter() { } // ��ʼ��״̬��Ϊ
    public virtual void OnUpdate() { } // ����״̬�߼�
    public virtual void OnExit() { }  // ����״̬��Դ
}
