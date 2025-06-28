using UnityEngine;
using System.Collections; 

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

    // --- GamePlayState: ��Ϸ����״̬ ---
    public class GamePlayState : GameState
    {
        private GameManager _manager;
        public GamePlayState(GameManager manager) { _manager = manager; }

        public override void OnEnter()
        {
            Debug.Log("����״̬����Ϸ������");
            // �����л�������Э��
            _manager.StartCoroutine(TransitionToGame());
            _manager.transform.GetComponent<TileGenerator>().GenTiles(); // �������̸���
        }

        private IEnumerator TransitionToGame()
        {
            // ���ݲ߻�����ִ��UI�л�����
            // 1. ��������������Ƴ�
            if (_manager.mainMenuAnimator != null)
            {
                _manager.mainMenuAnimator.Play("MoveDown_Out"); // TODO:��Ҫ��Ϊ "MoveDown_Out" �Ķ���Ƭ��
            }
            // �ȴ������������
            yield return new WaitForSeconds(0.5f); // �ȴ�����ʱ�䣬�ɵ���

            // 2. ���������棬��ʾ��Ϸ����
            if (_manager.mainMenuPanel != null) _manager.mainMenuPanel.SetActive(false);
            if (_manager.gamePanel != null) _manager.gamePanel.SetActive(true);

            // 3. ��Ϸ���������������
            if (_manager.gamePanelAnimator != null)
            {
                _manager.gamePanelAnimator.Play("MoveUp_In"); // TODO:��Ҫ��Ϊ "MoveUp_In" �Ķ���Ƭ��
            }
            yield return new WaitForSeconds(0.5f); // �ȴ�����ʱ��

            // 4. ��ʼ������
            Debug.Log("��ʼ������...");
            _manager.boardManager.InitializeBoard();
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
            _manager.StartCoroutine(TransitionToGameOver());
        }

        private IEnumerator TransitionToGameOver()
        {
            // UI�л�����
            // 1. ��Ϸ��������½�
            if (_manager.gamePanelAnimator != null)
            {
                _manager.gamePanelAnimator.Play("MoveDown_Out");
            }
            yield return new WaitForSeconds(0.5f);

            // 2. ������Ϸ���棬��ʾ��������
            if (_manager.gamePanel != null) _manager.gamePanel.SetActive(false);
            if (_manager.gameOverPanel != null) _manager.gameOverPanel.SetActive(true);

            // 3. ��Ϸ���������������
            if (_manager.gameOverAnimator != null)
            {
                _manager.gameOverAnimator.Play("MoveUp_In");
            }
        }
    }

    #endregion
}