using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isObstacle = false; //�Ƿ����ϰ��Ĭ�ϲ���
    public int x = 0; //�����ڵ�ͼ�е�x����
    public int y = 0; //�����ڵ�ͼ�е�y����
    public Unit unitOccupied; //��������ĵ�λ
    public void RenameSelf() {
        this.name = "Tile_" + x + "_" + y; //����������
    }
}
