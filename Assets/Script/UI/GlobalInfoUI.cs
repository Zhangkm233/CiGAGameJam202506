using UnityEngine;

public class GlobalInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel; // ������Ϣ���Ķ���
    [SerializeField] private TMPro.TextMeshProUGUI turnText; // ��ǰ�غ����ı�
    [SerializeField] private TMPro.TextMeshProUGUI newPieceCountDown; // �����ӵ���ʱ�ı�
    [SerializeField] private TMPro.TextMeshProUGUI enemySpawnPerTurn; // ÿ�غ����ɵ��������ı�
    [SerializeField] private TMPro.TextMeshProUGUI RightSideGameruleInfo; // �Ҳ���Ϸ������Ϣ�ı�
    [SerializeField] private TMPro.TextMeshProUGUI LeftSideGameruleInfo; // �����Ϸ������Ϣ�ı�

    void Start()
    {
        PieceSelectedState.PieceSelectedEvent += HideGameRules;
        PieceSelectedState.PieceDeselectedEvent += ShowGameRules;
        ShowGameRules(null);
    }

    public void UpdateInfoText(int turn, int countDown, int enemies)
    {
        turnText.text = $"��ǰ�غ�: {turn}"; // ���»غ����ı�
        newPieceCountDown.text = $"�����ӵ���ʱ: {countDown}"; // ���������ӵ���ʱ�ı�
        enemySpawnPerTurn.text = $"���ɵ�������: {enemies}"; // ���µ������������ı�
    }

    public void ShowGameRules(Piece piece)
    {
        RightSideGameruleInfo.gameObject.SetActive(true);
        LeftSideGameruleInfo.gameObject.SetActive(true);
        infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-5, -437);
    }

    public void HideGameRules(Piece piece)
    {
        RightSideGameruleInfo.gameObject.SetActive(false);
        LeftSideGameruleInfo.gameObject.SetActive(false);
        infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-6, 62);
    }

    
}
