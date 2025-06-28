using DG.Tweening;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // �������ӹ�������¼�
        Piece.OnPieceAttackFinishedEvent += OnPieceAttackFinishedShaking;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OnPieceAttackFinishedShaking(Piece attacker, Piece target)
    {
        // �����������Ч��
        Camera.main.transform.DOShakePosition(0.4f, 0.2f, 20, 90);
    }
}
