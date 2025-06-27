using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    public PieceState CurrentState { get; private set; } // ��ǰ�����״̬

    public void ChangeState(PieceState newState) {
        if (CurrentState != null) {
            CurrentState.OnExit(); // �˳���״̬
        }
        CurrentState = newState; // ������״̬
        CurrentState.OnEnter(); // ������״̬


    }
}
