using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class PieceInfoUI : MonoBehaviour
{
    // --- 单例模式 ---
    public static PieceInfoUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject infoPanel; // 整个信息面板的对象
    [SerializeField] private TextMeshProUGUI pieceNameText; // 棋子类型/名字
    [SerializeField] private Image pieceIconImage; // 棋子贴图
    [SerializeField] private TextMeshProUGUI movementText; // 可移动次数
    [SerializeField] private TextMeshProUGUI moodText; // 心情
    [SerializeField] private TextMeshProUGUI personalityNameText; // 性格名称
    [SerializeField] private TextMeshProUGUI positiveEffectText; // 满足/积极效果
    [SerializeField] private TextMeshProUGUI negativeEffectText; // 痛苦/悲伤效果
    [SerializeField] private TextMeshProUGUI newPiece; // 痛苦/悲伤效果
    [SerializeField] private Sprite[] personalityIcons; // 性格图标数组（如果有多个性格图标）

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 游戏开始时默认隐藏
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // 订阅棋子状态改变事件
        Piece.OnPieceStateChangedEvent += HandlePieceStateChanged;
    }

    private void OnDisable()
    {
        // 取消订阅，防止内存泄漏
        Piece.OnPieceStateChangedEvent -= HandlePieceStateChanged;
    }


    // 事件处理器：当任何棋子的状态改变时调用
    private void HandlePieceStateChanged(Piece piece, PieceState newState)
    {
        // 如果棋子进入“选中”状态，并且是友方棋子，则显示信息
        if (newState is PieceSelectedState && piece.Type != Piece.PieceType.Enemy)
        {
            ShowInfo(piece);
        }
        // 否则，隐藏信息面板
        else
        {
            HideInfo();
        }
    }


    // 显示并填充棋子信息
    public void ShowInfo(Piece piece)
    {
        if (infoPanel == null || piece == null || piece.PiecePersonality == null)
        {
            HideInfo();
            return;
        }

        // --- 填充基础信息 ---
        pieceNameText.text = GetPieceTypeName(piece.Type); // 显示棋子类型
        pieceIconImage.sprite = personalityIcons[(int)piece.CurrentMood.CurrentMoodLevel]; // 显示棋子图标
        pieceIconImage.SetNativeSize(); // 设置图标大小为原始大小
        movementText.text = $"可移动次数: {piece.CurrentMovementCount}/{piece.OriginMovementCount}";
        moodText.text = $"心情: {ConvertMoodLevelToString(piece.CurrentMood.CurrentMoodLevel)}";
        personalityNameText.text = $"性格: {piece.PiecePersonality.PersonalityName}";

        // --- 填充性格效果描述 ---
        // 如果需要动态显示代码效果，需要修改Personality类以包含效果描述字符串。
        switch (piece.PiecePersonality.PersonalityName)
        {
            case "乐观主义":
                positiveEffectText.text = "满足效果：在回合开始时，相邻4格内的棋子的可移动次数+1";
                negativeEffectText.text = "痛苦效果：无法吃掉敌人棋子。";
                break;
            case "悲观主义":
                positiveEffectText.text = "满足效果：无法吃掉敌人棋子";
                negativeEffectText.text = "痛苦效果：无";
                break;
            case "外向":
                positiveEffectText.text = "满足效果：在回合开始时，自身可移动次数+1";
                negativeEffectText.text = "痛苦效果：无法移动";
                break;
            case "内向":
                positiveEffectText.text = "满足效果：在回合开始时，相邻4格内的棋子的心情-1";
                negativeEffectText.text = "痛苦效果：在回合开始时，相邻4格内的棋子的可移动次数+1";
                break;
            case "完美主义":
                positiveEffectText.text = "满足效果：在回合开始时，心情-3，全场的棋子可移动次数+1";
                negativeEffectText.text = "痛苦效果：在回合开始时，消灭该棋子";
                break;
            case "务实":
                positiveEffectText.text = "满足效果：在回合开始时，自身可移动次数+2";
                negativeEffectText.text = "痛苦效果：在回合开始时，相邻4格内的棋子可移动次数+1";
                break;
            case "利他主义":
                positiveEffectText.text = "满足效果：在回合开始时，使3x3的所有棋子心情+1";
                negativeEffectText.text = "痛苦效果：无法移动";
                break;
            case "淡然":
                positiveEffectText.text = "满足效果：无";
                negativeEffectText.text = "痛苦效果：在回合开始时，自身可移动次数-1";
                break;
            default:
                positiveEffectText.text = " ";
                negativeEffectText.text = " ";
                break;
                // ... 在此添加其他性格的描述

        }

        infoPanel.SetActive(true);
    }


    // 隐藏信息面板
    public void HideInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    // 将棋子类型转换为中文字符串
    private string GetPieceTypeName(Piece.PieceType type)
    {
        return type switch
        {
            Piece.PieceType.Pawn => "卒",
            Piece.PieceType.Cannon => "炮",
            Piece.PieceType.Elephant => "象",
            Piece.PieceType.Horse => "马",
            Piece.PieceType.Rook => "车",
            Piece.PieceType.General => "将",
            _ => "未知"
        };
    }


    // 将心情数值转换为文字描述
    private string ConvertMoodLevelToString(int moodLevel)
    {
        if (moodLevel < 1) return "痛苦";       // 五个等级0-5数值范围，简单映射
        if (moodLevel < 2) return "悲伤";
        if (moodLevel < 3) return "平淡";
        if (moodLevel < 4) return "欣喜";
        return "满足";
    }
}