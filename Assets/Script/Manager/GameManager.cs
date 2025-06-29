using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // 引入DoTween命名空间

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

    // UI按钮调用：显示团队介绍
    public void ShowTeamInfoState()
    {
        StateMachine.ChangeState(new TeamInfoState(this));
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

    // --- TeamInfoState: 团队介绍状态 ---
    public class TeamInfoState : GameState
    {
        private GameManager _manager;
        public TeamInfoState(GameManager manager) { _manager = manager; }
        public override void OnEnter()
        {
            Debug.Log("进入状态：团队介绍");
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

    // --- GamePlayState: 游戏进行状态 ---
    public class GamePlayState : GameState
    {
        private GameManager _manager;
        public GamePlayState(GameManager manager) { _manager = manager; }

        public override void OnEnter()
        {
            Debug.Log("进入状态：游戏进行中");
            TransitionToGame();
        }

        // 修正为非协程方法，使用DoTween Sequence，并正确处理Animator动画
        private void TransitionToGame()
        {
            Sequence sequence = DOTween.Sequence();
            float animationDuration = 0.5f; // 假设UI动画持续时间为0.5秒，根据你的动画实际时长调整

            // 1. 主界面快速向下移出
            if (_manager.mainMenuAnimator != null)
            {
                _manager.mainMenuAnimator.SetTrigger("MoveOut"); // 触发Animator的"MoveOut"动画
                // 或者直接播放动画状态：_manager.mainMenuAnimator.Play("MoveDown_Out");
                sequence.AppendInterval(animationDuration); // 等待动画播放完毕的时间
            }
            else
            {
                // 如果没有Animator，也需要一个等待时间，否则逻辑会立即执行
                sequence.AppendInterval(animationDuration);
            }

            // 2. 隐藏主界面，显示游戏界面
            sequence.AppendCallback(() =>
            {
                if (_manager.mainMenuPanel != null) _manager.mainMenuPanel.SetActive(false);
                if (_manager.gamePanel != null) _manager.gamePanel.SetActive(true);
            });

            // 3. 游戏界面从下往上移入
            if (_manager.gamePanelAnimator != null)
            {
                _manager.gamePanelAnimator.SetTrigger("MoveIn"); // 触发Animator的"MoveIn"动画
                // 或者直接播放动画状态：_manager.gamePanelAnimator.Play("MoveUp_In");
                sequence.AppendInterval(animationDuration); // 等待动画时间
            }
            else
            {
                sequence.AppendInterval(animationDuration);
            }

            sequence.OnComplete(() =>
            {
                // 4. 初始化棋盘
                Debug.Log("初始化棋盘...");
                _manager.boardManager.InitializeBoard();
            });
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
            TransitionToGameOver();
        }

        // 修正为非协程方法，使用DoTween Sequence，并正确处理Animator动画
        private void TransitionToGameOver()
        {
            Sequence sequence = DOTween.Sequence();
            float animationDuration = 0.5f; // 假设UI动画持续时间为0.5秒，根据你的动画实际时长调整

            // 1. 游戏界面快速下降
            if (_manager.gamePanelAnimator != null)
            {
                _manager.gamePanelAnimator.SetTrigger("MoveOut"); // 触发Animator的"MoveOut"动画
                sequence.AppendInterval(animationDuration);
            }
            else
            {
                sequence.AppendInterval(animationDuration);
            }

            sequence.AppendCallback(() => {
                // 2. 隐藏游戏界面，显示结束界面
                if (_manager.gamePanel != null) _manager.gamePanel.SetActive(false);
                if (_manager.gameOverPanel != null) _manager.gameOverPanel.SetActive(true);
            });

            // 3. 游戏结束界面快速上升
            if (_manager.gameOverAnimator != null)
            {
                _manager.gameOverAnimator.SetTrigger("MoveIn"); // 触发Animator的"MoveIn"动画
                // 这里可以添加一个OnComplete回调，如果游戏结束后还需要执行其他逻辑
            }
            // 目前不需要AppendInterval，因为这是序列的最后一个动画
            // sequence.AppendInterval(animationDuration); // 后面还有其他逻辑需要等待
        }
    }

    #endregion
}