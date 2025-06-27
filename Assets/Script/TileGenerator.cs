using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public GameObject tilePrefab; // Ԥ��������

    public void Start() {
        GenTiles(); // �������ɸ��ӵķ���
    }

    public void GenTiles() {
        for (int i = 0;i < GameData.mapHeight;i++) {
            for (int j = 0;j < GameData.mapWidth;j++) {
                GameObject tile = Instantiate(tilePrefab,new Vector3(j - 5,i - 5,0),Quaternion.identity);
                tile.transform.SetParent(transform); // ���ø�����ΪTileGenerator
                Tile tileComponent = tile.GetComponent<Tile>();
                if (tileComponent != null) {
                    tileComponent.x = j; // ���ø����ڵ�ͼ�е�x����
                    tileComponent.y = i; // ���ø����ڵ�ͼ�е�y����
                    tileComponent.RenameSelf(); // ����������
                }
            }
        }
    }
}
