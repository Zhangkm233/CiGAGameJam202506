using DG.Tweening;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 订阅棋子攻击完成事件
        Piece.OnPieceAttackFinishedEvent += OnPieceAttackFinishedShaking;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OnPieceAttackFinishedShaking(Piece attacker, Piece target)
    {
        // 触发摄像机震动效果
        Camera.main.transform.DOShakePosition(0.4f, 0.2f, 20, 90);
    }
}
