using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // ����DoTween�����ռ�

public class GameManager : MonoBehaviour
{
    // --- ����ģʽ ---
    public static GameManager Instance { get; private set; }

    // --- ģ������ ---
    public BoardManager boardManager; // ��Inspector������BoardManager����
    public GameStateMachine StateMachine { get; private set; }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;    // ���˵�UI���
    public GameObject teamInfoPanel;    // �Ŷӽ���UI���
    public GameObject gamePanel;        // ��Ϸ��UI��壨e.g.�غ�����
    public GameObject gameOverPanel;    // ��Ϸ����UI���

    [Header("UI Animators")]
    public Animator mainMenuAnimator;   // ���˵�����Animator
    public Animator gamePanelAnimator;      // ��Ϸ����Animator
    public Animator gameOverAnimator;   // ��Ϸ��������Animator

    private void Awake()
    {
        // ���õ���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ��ѡ�������Ҫ�ڳ����л�ʱ����GameManager
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ��ʼ��״̬��
        StateMachine = new GameStateMachine();
        // �����Զ���ȡBoardManager
        if (boardManager == null)
        {
            boardManager = GetComponent<BoardManager>();
        }
    }

    private void Start()
    {
        // ��Ϸ��ʼʱ���������˵�״̬
        if (StateMachine.CurrentState == null)
        {
            StateMachine.ChangeState(new MainMenuState(this));
        }
    }

    // UI��ť���ã���ʼ����Ϸ
    public void StartGame()
    {
        StateMachine.ChangeState(new GamePlayState(this));
    }

    // UI��ť���ã���ʾ�Ŷӽ���
    public void ShowTeamInfoState()
    {
        StateMachine.ChangeState(new TeamInfoState(this));
    }

    // ��������������ʱ������Ϸ����
    public void TriggerGameOver()
    {
        StateMachine.ChangeState(new GameOverState(this));
    }

    // UI��ť���ã��������˵�
    public void ReturnToMainMenu()
    {
        StateMachine.ChangeState(new MainMenuState(this));
    }

    // UI��ť���ã���ʾ�Ŷӽ��ܽ���
    public void ShowTeamInfo()
    {
        if (teamInfoPanel != null)
        {
            teamInfoPanel.SetActive(true);
        }
    }

    // UI��ť���ã������Ŷӽ��ܽ���
    public void HideTeamInfo()
    {
        if (teamInfoPanel != null)
        {
            teamInfoPanel.SetActive(false);
        }
    }

    #region ��Ϸ״̬��

    // --- MainMenuState:���˵�״̬ ---
    public class MainMenuState : GameState
    {
        private GameManager _manager;
        public MainMenuState(GameManager manager) { _manager = manager; }

        public override void OnEnter()
        {
            Debug.Log("����״̬�����˵�");
            // �������˵�UI���ر�����UI
            if (_manager.mainMenuPanel != null) _manager.mainMenuPanel.SetActive(true);
            if (_manager.gamePanel != null) _manager.gamePanel.SetActive(false);
            if (_manager.gameOverPanel != null) _manager.gameOverPanel.SetActive(false);
            if (_manager.teamInfoPanel != null) _manager.teamInfoPanel.SetActive(false);
        }
    }

    // --- TeamInfoState: �Ŷӽ���״̬ ---
    public class TeamInfoState : GameState
    {
        private GameManager _manager;
        public TeamInfoState(GameManager manager) { _manager = manager; }
        public override void OnEnter()
        {
            Debug.Log("����״̬���Ŷӽ���");
            if (_manager.mainMenuPanel != null) _manager.mainMenuPanel.SetActive(false);
            if (_manager.gamePanel != null) _manager.gamePanel.SetActive(false);
            if (_manager.gameOverPanel != null) _manager.gameOverPanel.SetActive(false);
            if (_manager.teamInfoPanel != null) _manager.teamInfoPanel.SetActive(true);
        }
        public override void OnExit()
        {
            if (_manager.teamInfoPanel != null) _manager.teamInfoPanel.SetActive(false);
        }
    }

    // --- GamePlayState: ��Ϸ����״̬ ---
    public class GamePlayState : GameState
    {
        private GameManager _manager;
        public GamePlayState(GameManager manager) { _manager = manager; }

        public override void OnEnter()
        {
            Debug.Log("����״̬����Ϸ������");
            TransitionToGame();
        }

        // ����Ϊ��Э�̷�����ʹ��DoTween Sequence������ȷ����Animator����
        private void TransitionToGame()
        {
            Sequence sequence = DOTween.Sequence();
            float animationDuration = 0.5f; // ����UI��������ʱ��Ϊ0.5�룬������Ķ���ʵ��ʱ������

            // 1. ��������������Ƴ�
            if (_manager.mainMenuAnimator != null)
            {
                _manager.mainMenuAnimator.SetTrigger("MoveOut"); // ����Animator��"MoveOut"����
                // ����ֱ�Ӳ��Ŷ���״̬��_manager.mainMenuAnimator.Play("MoveDown_Out");
                sequence.AppendInterval(animationDuration); // �ȴ�����������ϵ�ʱ��
            }
            else
            {
                // ���û��Animator��Ҳ��Ҫһ���ȴ�ʱ�䣬�����߼�������ִ��
                sequence.AppendInterval(animationDuration);
            }

            // 2. ���������棬��ʾ��Ϸ����
            sequence.AppendCallback(() =>
            {
                if (_manager.mainMenuPanel != null) _manager.mainMenuPanel.SetActive(false);
                if (_manager.gamePanel != null) _manager.gamePanel.SetActive(true);
            });

            // 3. ��Ϸ���������������
            if (_manager.gamePanelAnimator != null)
            {
                _manager.gamePanelAnimator.SetTrigger("MoveIn"); // ����Animator��"MoveIn"����
                // ����ֱ�Ӳ��Ŷ���״̬��_manager.gamePanelAnimator.Play("MoveUp_In");
                sequence.AppendInterval(animationDuration); // �ȴ�����ʱ��
            }
            else
            {
                sequence.AppendInterval(animationDuration);
            }

            sequence.OnComplete(() =>
            {
                // 4. ��ʼ������
                Debug.Log("��ʼ������...");
                _manager.boardManager.InitializeBoard();
            });
        }
    }

    // --- GameOverState: ��Ϸ����״̬ ---
    public class GameOverState : GameState
    {
        private GameManager _manager;
        public GameOverState(GameManager manager) { _manager = manager; }

        public override void OnEnter()
        {
            Debug.Log("����״̬����Ϸ����");
            TransitionToGameOver();
        }

        // ����Ϊ��Э�̷�����ʹ��DoTween Sequence������ȷ����Animator����
        private void TransitionToGameOver()
        {
            Sequence sequence = DOTween.Sequence();
            float animationDuration = 0.5f; // ����UI��������ʱ��Ϊ0.5�룬������Ķ���ʵ��ʱ������

            // 1. ��Ϸ��������½�
            if (_manager.gamePanelAnimator != null)
            {
                _manager.gamePanelAnimator.SetTrigger("MoveOut"); // ����Animator��"MoveOut"����
                sequence.AppendInterval(animationDuration);
            }
            else
            {
                sequence.AppendInterval(animationDuration);
            }

            sequence.AppendCallback(() => {
                // 2. ������Ϸ���棬��ʾ��������
                if (_manager.gamePanel != null) _manager.gamePanel.SetActive(false);
                if (_manager.gameOverPanel != null) _manager.gameOverPanel.SetActive(true);
            });

            // 3. ��Ϸ���������������
            if (_manager.gameOverAnimator != null)
            {
                _manager.gameOverAnimator.SetTrigger("MoveIn"); // ����Animator��"MoveIn"����
                // ����������һ��OnComplete�ص��������Ϸ��������Ҫִ�������߼�
            }
            // Ŀǰ����ҪAppendInterval����Ϊ�������е����һ������
            // sequence.AppendInterval(animationDuration); // ���滹�������߼���Ҫ�ȴ�
        }
    }

    #endregion
}