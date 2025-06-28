using UnityEngine;
// --- ����״̬ ---
public class PieceIdleState : PieceState
{
    public PieceIdleState(Piece piece) : base(piece) { }
    public override void OnEnter()
    {
        // ���ӽ������״̬
        Debug.Log($"{_piece.name} �������״̬.");
        // TODO:���һЩ�����������Ӿ���ʾ
    }
    public override void OnUpdate()
    {
        // ����״̬�¼���������룬���������ӽ���ѡ��֮���
        // TODO:�����ҵ���˴����ӣ��л���PieceSelectedState
        // if (Input.GetMouseButtonDown(0) && IsClicked(_piece))
        // {
        //     _piece.StateMachine.ChangeState(new PieceSelectedState(_piece));
        // }
        PlayIdleAnimation();
    }
    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳�����״̬.");
    }

    // �������������ڼ���Ƿ����˸����ӣ���Ҫ���߼��֮���
    private bool IsClicked(Piece piece) { return false; }

    // ���ӵĴ�������
    private void PlayIdleAnimation()
    {
        Mood mood = _piece.CurrentMood; // ��ȡ���ӵĵ�ǰ����
        // �������ӵ����鲥�Ų�ͬ�Ĵ�������
        if (mood.CurrentMoodLevel <= 1)
        {
            // TODO: ������
        }
        else if (mood.CurrentMoodLevel <= 10)
        {
            // ���¸��� Ч���о����Ǻܺã���������ɾ��
            //float offsetY = Mathf.Sin(Time.time * 3) * 0.001f;
            //_piece.transform.position += new Vector3(0, offsetY, 0);
            
        }
        else
        {
            // TODO: ������Ծ
        }
        
    }
}