using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class PieceInfoUI : MonoBehaviour
{
    // --- ����ģʽ ---
    public static PieceInfoUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject infoPanel; // ������Ϣ���Ķ���
    [SerializeField] private TextMeshProUGUI pieceNameText; // ��������/����
    [SerializeField] private Image pieceIconImage; // ������ͼ
    [SerializeField] private TextMeshProUGUI movementText; // ���ƶ�����
    [SerializeField] private TextMeshProUGUI moodText; // ����
    [SerializeField] private TextMeshProUGUI personalityNameText; // �Ը�����
    [SerializeField] private TextMeshProUGUI positiveEffectText; // ����/����Ч��
    [SerializeField] private TextMeshProUGUI negativeEffectText; // ʹ��/����Ч��
    [SerializeField] private TextMeshProUGUI newPiece; // ʹ��/����Ч��
    [SerializeField] private Sprite[] personalityIcons; // �Ը�ͼ�����飨����ж���Ը�ͼ�꣩

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

        // ��Ϸ��ʼʱĬ������
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // ��������״̬�ı��¼�
        Piece.OnPieceStateChangedEvent += HandlePieceStateChanged;
    }

    private void OnDisable()
    {
        // ȡ�����ģ���ֹ�ڴ�й©
        Piece.OnPieceStateChangedEvent -= HandlePieceStateChanged;
    }


    // �¼������������κ����ӵ�״̬�ı�ʱ����
    private void HandlePieceStateChanged(Piece piece, PieceState newState)
    {
        // ������ӽ��롰ѡ�С�״̬���������ѷ����ӣ�����ʾ��Ϣ
        if (newState is PieceSelectedState && piece.Type != Piece.PieceType.Enemy)
        {
            ShowInfo(piece);
        }
        // ����������Ϣ���
        else
        {
            HideInfo();
        }
    }


    // ��ʾ�����������Ϣ
    public void ShowInfo(Piece piece)
    {
        if (infoPanel == null || piece == null || piece.PiecePersonality == null)
        {
            HideInfo();
            return;
        }

        // --- ��������Ϣ ---
        pieceNameText.text = GetPieceTypeName(piece.Type); // ��ʾ��������
        pieceIconImage.sprite = personalityIcons[(int)piece.CurrentMood.CurrentMoodLevel]; // ��ʾ����ͼ��
        pieceIconImage.SetNativeSize(); // ����ͼ���СΪԭʼ��С
        movementText.text = $"���ƶ�����: {piece.CurrentMovementCount}/{piece.OriginMovementCount}";
        moodText.text = $"����: {ConvertMoodLevelToString(piece.CurrentMood.CurrentMoodLevel)}";
        personalityNameText.text = $"�Ը�: {piece.PiecePersonality.PersonalityName}";

        // --- ����Ը�Ч������ ---
        // �����Ҫ��̬��ʾ����Ч������Ҫ�޸�Personality���԰���Ч�������ַ�����
        switch (piece.PiecePersonality.PersonalityName)
        {
            case "�ֹ�����":
                positiveEffectText.text = "����Ч�����ڻغϿ�ʼʱ������4���ڵ����ӵĿ��ƶ�����+1";
                negativeEffectText.text = "ʹ��Ч�����޷��Ե��������ӡ�";
                break;
            case "��������":
                positiveEffectText.text = "����Ч�����޷��Ե���������";
                negativeEffectText.text = "ʹ��Ч������";
                break;
            case "����":
                positiveEffectText.text = "����Ч�����ڻغϿ�ʼʱ��������ƶ�����+1";
                negativeEffectText.text = "ʹ��Ч�����޷��ƶ�";
                break;
            case "����":
                positiveEffectText.text = "����Ч�����ڻغϿ�ʼʱ������4���ڵ����ӵ�����-1";
                negativeEffectText.text = "ʹ��Ч�����ڻغϿ�ʼʱ������4���ڵ����ӵĿ��ƶ�����+1";
                break;
            case "��������":
                positiveEffectText.text = "����Ч�����ڻغϿ�ʼʱ������-3��ȫ�������ӿ��ƶ�����+1";
                negativeEffectText.text = "ʹ��Ч�����ڻغϿ�ʼʱ�����������";
                break;
            case "��ʵ":
                positiveEffectText.text = "����Ч�����ڻغϿ�ʼʱ��������ƶ�����+2";
                negativeEffectText.text = "ʹ��Ч�����ڻغϿ�ʼʱ������4���ڵ����ӿ��ƶ�����+1";
                break;
            case "��������":
                positiveEffectText.text = "����Ч�����ڻغϿ�ʼʱ��ʹ3x3��������������+1";
                negativeEffectText.text = "ʹ��Ч�����޷��ƶ�";
                break;
            case "��Ȼ":
                positiveEffectText.text = "����Ч������";
                negativeEffectText.text = "ʹ��Ч�����ڻغϿ�ʼʱ��������ƶ�����-1";
                break;
            default:
                positiveEffectText.text = " ";
                negativeEffectText.text = " ";
                break;
                // ... �ڴ���������Ը������

        }

        infoPanel.SetActive(true);
    }


    // ������Ϣ���
    public void HideInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    // ����������ת��Ϊ�����ַ���
    private string GetPieceTypeName(Piece.PieceType type)
    {
        return type switch
        {
            Piece.PieceType.Pawn => "��",
            Piece.PieceType.Cannon => "��",
            Piece.PieceType.Elephant => "��",
            Piece.PieceType.Horse => "��",
            Piece.PieceType.Rook => "��",
            Piece.PieceType.General => "��",
            _ => "δ֪"
        };
    }


    // ��������ֵת��Ϊ��������
    private string ConvertMoodLevelToString(int moodLevel)
    {
        if (moodLevel < 1) return "ʹ��";       // ����ȼ�0-5��ֵ��Χ����ӳ��
        if (moodLevel < 2) return "����";
        if (moodLevel < 3) return "ƽ��";
        if (moodLevel < 4) return "��ϲ";
        return "����";
    }
}