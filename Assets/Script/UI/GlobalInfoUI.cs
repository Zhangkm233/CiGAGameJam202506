using UnityEngine;

public class GlobalInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel; // ������Ϣ���Ķ���
    [SerializeField] private TMPro.TextMeshProUGUI turnText; // ��ǰ�غ����ı�
    [SerializeField] private TMPro.TextMeshProUGUI newPieceCountDown; // �����ӵ���ʱ�ı�
    [SerializeField] private TMPro.TextMeshProUGUI enemySpawnPerTurn; // ÿ�غ����ɵ��������ı�

    public void UpdateInfoText(int turn,int countDown,int enemies) {
        turnText.text = $"��ǰ�غ�: {turn}"; // ���»غ����ı�
        newPieceCountDown.text = $"�����ӵ���ʱ: {countDown}"; // ���������ӵ���ʱ�ı�
        enemySpawnPerTurn.text = $"���ɵ�������: {enemies}"; // ���µ������������ı�
    }
}
