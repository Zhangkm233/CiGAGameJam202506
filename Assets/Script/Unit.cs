using UnityEngine;

public class Unit : MonoBehaviour
{
    public Tile currentTile; //��ǰ���ڵĸ���
    public int x = 0; //��λ�ڵ�ͼ�е�x����
    public int y = 0; //��λ�ڵ�ͼ�е�y����
    public void RenameSelf() {
        this.name = "Unit_" + x + "_" + y; //��������λ
    }
    public void MoveTo(int newX,int newY) {
        x = newX;
        y = newY;
        RenameSelf();
        currentTile = GameObject.Find("Tile_" + x + "_" + y).GetComponent<Tile>(); //�ҵ��µĸ���
        if (currentTile != null) {
            currentTile.unitOccupied = this; //�����µĸ�������ĵ�λ
            transform.position = new Vector3(x - 5,y - 5,0); //���µ�λ��λ��
        } else {
            Debug.LogError("Tile not found at position: " + x + ", " + y);
        }
    }
}
