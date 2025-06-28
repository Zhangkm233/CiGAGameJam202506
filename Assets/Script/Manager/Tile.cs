using UnityEngine;


public class Tile : MonoBehaviour
{
    // --- ״̬���� ---
    // ��ͼ���Ƿ�Ϊ����ͨ�е��ϰ���
    public bool isObstacle { get; private set; }


    // �����ڵ�ͼ�е�x����
    public int x { get; set; }


    // �����ڵ�ͼ�е�y����
    public int y { get; set; }


    // ��������ռ�õĵ�λ
    public Piece unitOccupied { get; set; }

    // --- ������� ---
    [Header("��۾��� (Sprite)")]
    public Sprite highLightSprite; // ����״̬
    public Sprite defaultSprite;   // Ĭ��״̬
    public Sprite obstacleSprite;  // �ϰ���״̬

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // �ڿ�ʼʱ��ȡ������SpriteRenderer������������
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // ����������������Ϸ���󣬷����ڱ༭����ʶ��
    public void RenameSelf()
    {
        this.name = "Tile_" + x + "_" + y; // ����������
    }


    // ���ø������
    public void SetHighlight()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = highLightSprite;
        }
    }


    // �ָ�Ĭ�����
    public void SetDefault()
    {
        if (spriteRenderer != null)
        {
            // ������ϰ����ָ�Ϊ�ϰ������ָ�Ϊ��ͨĬ�Ͼ���
            spriteRenderer.sprite = isObstacle ? obstacleSprite : defaultSprite;
        }
    }


    // ���ø��ӵ��ϰ���״̬���������������Ӿ����
    public void SetObstacle(bool status)
    {
        isObstacle = status;
        // ����SetDefault()�������µ�״̬����Ϊ��ȷ�����
        SetDefault();
    }
}