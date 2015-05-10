using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    #region OBJECTS AND STUFF
    private GameObject m_Ball;
    private GameObject m_Paddle;
    private GameObject m_AIPaddle;
    private Vector2 m_vScreenBoundary;
    private Vector2 m_vBallPosition;
    private bool m_bDebug = false;
    private int m_iSphereRadius;
    private GameObject m_PlayerWinnerEndScreen;
    private GameObject m_AIWinnerEndScreen;

    private Vector3 m_vInitialBallPosition;
    private Vector3 m_vInitialPlayerPaddlePosition;
    private Vector3 m_vInitialAIPaddlePosition;
    #endregion

    #region GAME STATE AND STUFF
    public struct GameState
    {
        private bool m_bStatus;

        public bool Status
        {
            get { return m_bStatus; }
            set { m_bStatus = value; }
        }

        private int m_iWinner;
        public int Winner
        {
            get { return m_iWinner; }
            set
            {
                m_iWinner = value;
            }
        }
        public bool Paused { get; set; }
        //public string WinnerName { get; set; }

        public GameState(bool bStatus, int iWinner, bool paused)
        {
            m_bStatus = bStatus;
            m_iWinner = iWinner;
            Paused = paused;
        }
    }
    private static GameState m_sStatus;
    public static GameState GameStatus
    {
        get { return m_sStatus; }
        set { m_sStatus = value; }
    }
    private bool m_bStopUpdate = false;
    void GameOver()
    {
        if (m_sStatus.Winner == 1) //Player scores
            m_iPlayerScore++;
        else
            m_iAIScore++;
    }
    void SetGameStatus(object c)
    {
        GameState desiredState = (GameState)c;

        m_sStatus = desiredState;
        
        InformBehaviours();

        m_PauseMenu.SetActive(false);
        m_MenuButton.enabled = true;
    }
    void InformBehaviours()
    {
        GameObject.Find("GameLogic").SendMessage("CheckGameStatus", m_sStatus);
        GameObject.Find("Ball").SendMessage("CheckGameStatus", m_sStatus);
    }
    private int m_iPlayerScore;
    private int m_iAIScore;
    #endregion

    #region UI STUFF

    private Text[] m_CanvasTextComps;
    private Text m_PlayerScoreText;
    private Text m_AIScoreText;
    private Button[] m_CanvasButtons;
    private Button m_MenuButton;

    private GameObject m_PauseMenu;
    private Button[] m_PauseMenuButtons;
    private Button m_ExitButton;
    private Button m_GoBackButton;
    private Button m_RestartButton;
    void OnPauseMenuExitButtonClick()
    {
        m_MenuButton.enabled = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    void OnPauseMenuGoBackButtonClick()
    {
        m_MenuButton.enabled = true;
    }
    void OnPauseMenuRestartButtonClick()
    {
        m_MenuButton.enabled = true;
    }
    void OnMenuButtonClick()
    {
        m_MenuButton.enabled = false;
        m_sStatus.Paused = true;
        GameObject.Find("GameLogic").SendMessage("CheckGameStatus", m_sStatus);
        GameObject.Find("Ball").SendMessage("CheckGameStatus", m_sStatus);

        m_PauseMenu.SetActive(true);
    }
    void UpdateScoreUI()
    {
        m_PlayerScoreText.text = "Player Score:" + m_iPlayerScore.ToString();
        m_AIScoreText.text = "AI Score:" + m_iAIScore.ToString();
    }
    #endregion


    public void Start()
    {
        m_iSphereRadius = 1;
        #region UI STUFF
        m_iPlayerScore = 0;
        m_iAIScore = 0;

        m_CanvasTextComps = GameObject.Find("ScoreCanvas").GetComponentsInChildren<Text>();
        m_CanvasButtons = GameObject.Find("ScoreCanvas").GetComponentsInChildren<Button>();

        //PauseMenu
        m_PauseMenu = Resources.Load("PauseMenuPrefab", typeof(GameObject)) as GameObject;
        m_PauseMenuButtons = m_PauseMenu.GetComponentsInChildren<Button>();
        
        for (int i = 0; i < m_PauseMenuButtons.Length; i++)
        {
            if (m_PauseMenuButtons[i].name == "ExitButton")
            {
                m_ExitButton = m_PauseMenuButtons[i];
                m_ExitButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnPauseMenuExitButtonClick));
            }
            if (m_PauseMenuButtons[i].name == "GoBackButton")
            {
                m_GoBackButton = m_PauseMenuButtons[i];
                m_GoBackButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnPauseMenuGoBackButtonClick));
            }
            if (m_PauseMenuButtons[i].name == "RestartButton")
            {
                m_RestartButton = m_PauseMenuButtons[i];
                m_RestartButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnPauseMenuRestartButtonClick));
            }
        }

        m_PauseMenu = (GameObject)Instantiate(m_PauseMenu, new Vector3(0, 0, 0), new Quaternion());
        m_PauseMenu.SetActive(false);
        //Global Canvas
        for (int i = 0; i < m_CanvasButtons.Length; i++)
        {
            if (m_CanvasButtons[i].name == "MenuButton")
                m_MenuButton = m_CanvasButtons[i];
        }
        m_MenuButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnMenuButtonClick));

        for (int i = 0; i < m_CanvasTextComps.Length; i++)
        {
            if (m_CanvasTextComps[i].name == "PlayerScoreText")
            {
                m_PlayerScoreText = m_CanvasTextComps[i];
            }
            if (m_CanvasTextComps[i].name == "AIScoreText")
            {
                m_AIScoreText = m_CanvasTextComps[i];
            }
        }
        m_PlayerScoreText.text = "Player Score:";
        m_AIScoreText.text = "AI Score:";
        #endregion

        #region RESOURCE INITIALIZATION
        m_Ball = Resources.Load("Ball", typeof(GameObject)) as GameObject;
        m_Paddle = Resources.Load("Paddle", typeof(GameObject)) as GameObject;
        m_vInitialBallPosition = m_Ball.transform.position;

        float paddleOffsetFromEdge = m_Paddle.GetComponent<BoxCollider2D>().bounds.extents.x / 2;
        m_Paddle.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(paddleOffsetFromEdge, Screen.height / 2, 0));
        m_Paddle.transform.position += new Vector3(0, 0, 1);
        m_Paddle.transform.localScale = new Vector3(0.1f, 0.5f, 0);
        m_Paddle = Instantiate(m_Paddle);
        m_Paddle.name = "LeftPaddle";
        m_vInitialPlayerPaddlePosition = m_Paddle.transform.position;

        m_AIPaddle = Resources.Load("AIPaddle", typeof(GameObject)) as GameObject;
        m_AIPaddle.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - paddleOffsetFromEdge, Screen.height / 2, 0));
        m_AIPaddle.transform.position += new Vector3(0, 0, 1);
        m_AIPaddle.transform.localScale = new Vector3(0.1f, 0.5f, 0);
        m_AIPaddle = Instantiate(m_AIPaddle);
        m_AIPaddle.name = "AIPaddle";
        m_vInitialAIPaddlePosition = m_AIPaddle.transform.position;

        m_Ball = Instantiate(m_Ball);
        m_Ball.name = "Ball";
        m_Ball.transform.localScale = new Vector3(0.2f, 0.2f, 0);
        m_vScreenBoundary = new Vector2(Screen.width, Screen.height);

        #endregion

        GameState initialStatus = new GameState(false, -1, false);
        m_sStatus = initialStatus;

        m_PlayerWinnerEndScreen = Resources.Load("PlayerWinnerScreen", typeof(GameObject)) as GameObject;
        m_AIWinnerEndScreen = Resources.Load("AIWinnerScreen", typeof(GameObject)) as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.fixedDeltaTime;

        if (!m_sStatus.Paused)
        {

            if (!m_bStopUpdate)
            {
                if (m_sStatus.Status)
                {
                    GameOver();
                    m_sStatus.Status = false;
                    RestartGame();
                }
            }
            UpdateScoreUI();
        }
    }
    void RestartGame()
    {
        //m_Ball.transform.position = m_vInitialBallPosition;
        m_Paddle.transform.position = m_vInitialPlayerPaddlePosition;
        m_AIPaddle.transform.position = m_vInitialAIPaddlePosition;
    }
   
    Vector3 ToScreen(Vector3 V)
    {
        return Camera.main.WorldToScreenPoint(V);
    }
}
