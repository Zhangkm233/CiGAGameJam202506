using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    public GameState CurrentState { get; private set; } // ��ǰ�����״̬

    public void ChangeState(GameState newState) {
        if (CurrentState != null) {
            CurrentState.OnExit(); // �˳���״̬
        }
        CurrentState = newState; // ������״̬
        CurrentState.OnEnter(); // ������״̬


    }
}
