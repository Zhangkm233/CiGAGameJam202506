using UnityEngine;
using static Piece;

public class PieceMovingState : PieceState
{
    private Vector2Int _targetPosition; // ����Ҫ�ƶ�����Ŀ��λ��
    private float _moveSpeed = 5f; // �����ƶ��ٶ�
    private Vector3 _startWorldPosition;
    private Vector3 _targetWorldPosition;
    private float _journeyLength;
    private float _startTime;

    public PieceMovingState(Piece piece, Vector2Int targetPosition) : base(piece)
    {
        _targetPosition = targetPosition;
    }

    public override void OnEnter()
    {
        Debug.Log($"{_piece.name} �����ƶ�״̬��Ŀ�꣺{_targetPosition}");
        // �����ƶ���������Ч
        _startWorldPosition = _piece.transform.position;
        // TODO:��ҪBoardManager�� BoardPosition ת��Ϊ��������
        // _targetWorldPosition = (boardManager as BoardManager)?.GetWorldPosition(_targetPosition); 
        _targetWorldPosition = new Vector3(_targetPosition.x, _targetPosition.y, _piece.transform.position.z); // ������XYƽ�棿
        _journeyLength = Vector3.Distance(_startWorldPosition, _targetWorldPosition);
        _startTime = Time.time;
    }

    public override void OnUpdate()
    {
        // ƽ���ƶ��߼�
        float distCovered = (Time.time - _startTime) * _moveSpeed;
        float fractionOfJourney = _journeyLength > 0 ? distCovered / _journeyLength : 1f;
        _piece.transform.position = Vector3.Lerp(_startWorldPosition, _targetWorldPosition, fractionOfJourney);

        // �����ӽӽ�Ŀ��λ��ʱ������ƶ����л�״̬
        if (Vector3.Distance(_piece.transform.position, _targetWorldPosition) < 0.05f || fractionOfJourney >= 1f)
        {
            _piece.transform.position = _targetWorldPosition; // ȷ����ȷ����Ŀ��λ��
            _piece.MoveTo(_targetPosition); // ���������ڲ�λ��

            _piece.CurrentMovementCount--; // ����һ���ƶ�����

            // ����Ƿ������ӱ����Ե���
            // TODO:������ҪBoardManager�����_targetPosition�Ƿ��ез�����
            // Piece eatenPiece = (BoardManager.Instance as BoardManager)?.GetPieceAtPosition(_targetPosition); // Assuming BoardManager.Instance is available
            // if (eatenPiece != null && eatenPiece.Type == PieceType.Enemy)
            // {
            //    _piece.Attack(eatenPiece); // ���������߼�����������Ϊ����״̬
            // }

            // ����ʣ���ƶ�����������һ��״̬
            if (_piece.CurrentMovementCount > 0 && _piece.Type != PieceType.Enemy)
            {
                _piece.StateMachine.ChangeState(new PieceSelectedState(_piece)); // ��������ƶ����������Լ�������
            }
            else
            {
                _piece.StateMachine.ChangeState(new PieceIdleState(_piece)); // �ƶ��������꣬�ص�����״̬
            }
        }
    }

    public override void OnExit()
    {
        Debug.Log($"{_piece.name} �˳��ƶ�״̬.");
    }
}