using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    public PieceState CurrentState { get; private set; } // 当前激活的状态

    public void ChangeState(PieceState newState) {
        if (CurrentState != null) {
            CurrentState.OnExit(); // 退出旧状态
        }
        CurrentState = newState; // 设置新状态
        CurrentState.OnEnter(); // 进入新状态


    }
}
