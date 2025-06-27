using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isObstacle { get; set; } //�Ƿ����ϰ��Ĭ�ϲ���
    public int x { get; set; } //�����ڵ�ͼ�е�x����
    public int y { get; set; } //�����ڵ�ͼ�е�y����
    public Piece unitOccupied { get; set; } //��������ĵ�λ

    public Sprite highLightSprite;
    public Sprite defaultSprite;
    public void RenameSelf() {
        this.name = "Tile_" + x + "_" + y; //����������
    }
    public void SetHighlight() {
        this.GetComponent<SpriteRenderer>().sprite = highLightSprite;
    }
    public void SetDefault() {
        this.GetComponent<SpriteRenderer>().sprite = defaultSprite; //�ָ�Ĭ�����
    }
}
