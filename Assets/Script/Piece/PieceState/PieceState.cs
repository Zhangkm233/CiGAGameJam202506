using UnityEngine;

// ������࣬��������������״̬��ͨ�ýӿ�
public abstract class PieceState
{
    protected Piece _piece; // ��ӵ�д�״̬������ʵ��������

    public PieceState(Piece piece)
    {
        _piece = piece;
    }

    public virtual void OnEnter() { } // ��ʼ��״̬��Ϊ
    public virtual void OnUpdate() { } // ����״̬�߼�
    public virtual void OnExit() { }  // ����״̬��Դ
}