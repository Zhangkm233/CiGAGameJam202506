using UnityEngine;
using System.Collections; 

public class GameManager : MonoBehaviour
{
    // --- 单例模式 ---
    public static GameManager Instance { get; private set; }

    // --- 模块引用 ---
    public BoardManager boardManager; // 在Inspector中拖入BoardManager对象
    public GameStateMachine StateMachine { get; private set; }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;    // 主菜单UI面板
    public GameObject teamInfoPanel;    // 团队介绍UI面板
    public GameObject gamePanel;        // 游戏内UI面板（e.g.回合数）
    public GameObject gameOverPanel;    // 游戏结束UI面板

    [Header("UI Animators")]
    public Animator mainMenuAnimator;   // 主菜单面板的Animator
    public Animator gamePanelAnimator;      // 游戏面板的Animator
    public Animator gameOverAnimator;   // 游戏结束面板的Animator

    private void Awake()
    {
        // 设置单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 可选，如果需要在场景切换时保留GameManager
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化状态机
        StateMachine = new GameStateMachine();

        // 尝试自动获取BoardManager
        if (boardManager == null)
        {
            boardManager = GetComponent<BoardManager>();
        }
    }

    private void Start()
    {
        // 游戏开始时，进入主菜单状态
        if (StateMachine.CurrentState == null)
        {
            StateMachine.ChangeState(new MainMenuState(this));
        }
    }


    // UI按钮调用：开始新游戏
    public void StartGame()
    {
        StateMachine.ChangeState(new GamePlayState(this));
    }


    // 当“将”被击败时触发游戏结束
    public void TriggerGameOver()
    {
        StateMachine.ChangeState(new GameOverState(this));
    }


    // UI按钮调用：返回主菜单
    public void ReturnToMainMenu()
    {
        StateMachine.ChangeState(new MainMenuState(this));
    }


    // UI按钮调用：显示团队介绍界面

    public void ShowTeamInfo()
    {
        if (teamInfoPanel != null)
        {
            teamInfoPanel.SetActive(true);
        }
    }

    // UI按钮调用：隐藏团队介绍界面
    public void HideTeamInfo()
    {
        if (teamInfoPanel != null)
        {
            teamInfoPanel.SetActive(false);
        }
    }

    #region 游戏状态类

    // --- MainMenuState:主菜单状态 ---
    public class MainMenuState : GameState
    {
        private GameManager _manager;
        public MainMenuState(GameManager manager) { _manager = manager; }

        public override void OnEnter()
        {
            Debug.Log("进入状态：主菜单");
            // 激活主菜单UI，关闭其他UI
            if (_manager.mainMenuPanel != null) _manager.mainMenuPanel.SetActive(true);
            if (_manager.gamePanel != null) _manager.gamePanel.SetActive(false);
            if (_manager.gameOverPanel != null) _manager.gameOverPanel.SetActive(false);
            if (_manager.teamInfoPanel != null) _manager.teamInfoPanel.SetActive(false);
        }
    }

    // --- GamePlayState: 游戏进行状态 ---
    public class GamePlayState : GameState
    {
        private GameManager _manager;
        public GamePlayState(GameManager manager) { _manager = manager; }

        public override void OnEnter()
        {
            Debug.Log("进入状态：游戏进行中");
            // 启动切换动画的协程
            _manager.StartCoroutine(TransitionToGame());
            _manager.transform.GetComponent<TileGenerator>().GenTiles(); // 生成棋盘格子
        }

        private IEnumerator TransitionToGame()
        {
            // 根据策划案，执行UI切换动画
            // 1. 主界面快速向下移出
            if (_manager.mainMenuAnimator != null)
            {
                _manager.mainMenuAnimator.Play("MoveDown_Out"); // TODO:需要名为 "MoveDown_Out" 的动画片段
            }
            // 等待动画播放完毕
            yield return new WaitForSeconds(0.5f); // 等待动画时间，可调整

            // 2. 隐藏主界面，显示游戏界面
            if (_manager.mainMenuPanel != null) _manager.mainMenuPanel.SetActive(false);
            if (_manager.gamePanel != null) _manager.gamePanel.SetActive(true);

            // 3. 游戏界面从下往上移入
            if (_manager.gamePanelAnimator != null)
            {
                _manager.gamePanelAnimator.Play("MoveUp_In"); // TODO:需要名为 "MoveUp_In" 的动画片段
            }
            yield return new WaitForSeconds(0.5f); // 等待动画时间

            // 4. 初始化棋盘
            Debug.Log("初始化棋盘...");
            _manager.boardManager.InitializeBoard();
        }
    }

    // --- GameOverState: 游戏结束状态 ---
    public class GameOverState : GameState
    {
        private GameManager _manager;
        public GameOverState(GameManager manager) { _manager = manager; }

        public override void OnEnter()
        {
            Debug.Log("进入状态：游戏结束");
            _manager.StartCoroutine(TransitionToGameOver());
        }

        private IEnumerator TransitionToGameOver()
        {
            // UI切换动画
            // 1. 游戏界面快速下降
            if (_manager.gamePanelAnimator != null)
            {
                _manager.gamePanelAnimator.Play("MoveDown_Out");
            }
            yield return new WaitForSeconds(0.5f);

            // 2. 隐藏游戏界面，显示结束界面
            if (_manager.gamePanel != null) _manager.gamePanel.SetActive(false);
            if (_manager.gameOverPanel != null) _manager.gameOverPanel.SetActive(true);

            // 3. 游戏结束界面快速上升
            if (_manager.gameOverAnimator != null)
            {
                _manager.gameOverAnimator.Play("MoveUp_In");
            }
        }
    }

    #endregion
}