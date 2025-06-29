using UnityEngine;

public class GlobalInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel; // 整个信息面板的对象
    [SerializeField] private TMPro.TextMeshProUGUI turnText; // 当前回合数文本
    [SerializeField] private TMPro.TextMeshProUGUI newPieceCountDown; // 新棋子倒计时文本
    [SerializeField] private TMPro.TextMeshProUGUI enemySpawnPerTurn; // 每回合生成敌人数量文本

    public void UpdateInfoText(int turn,int countDown,int enemies) {
        turnText.text = $"当前回合: {turn}"; // 更新回合数文本
        newPieceCountDown.text = $"新棋子倒计时: {countDown}"; // 更新新棋子倒计时文本
        enemySpawnPerTurn.text = $"生成敌人数量: {enemies}"; // 更新敌人生成数量文本
    }
}
