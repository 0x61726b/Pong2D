//--------------------------------------------------------------------------------
//This is a file from Pong2D.A hobby project for Gram Games evaluation
//
//Copyright (c) Alperen Gezer.All rights reserved.
//
//GameLogic.cs
//
//This class is attached to an empty game object and controls basically the game.
//It creates the Paddles and the Ball.Sets difficulty initializes UI elemets
//control the game state
//--------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//--------------------------------------------------------------------------------
public class GameLogic : MonoBehaviour
{
    //--------------------------------------------------------------------------------
    public void Start()
    {
        Application.targetFrameRate = 60;
        
        m_b2Player = false;
        if (PlayerPrefs.GetInt("Is2Player") == 1)
            m_b2Player = true;

        #region UI STUFF
        m_iPlayerScore = 0;
        m_iAIScore = 0;

        m_CanvasTextComps = GameObject.Find("ScoreCanvas").GetComponentsInChildren<Text>();
        m_CanvasButtons = GameObject.Find("ScoreCanvas").GetComponentsInChildren<Button>();
        
        //PauseMenu
        m_PauseMenu = Resources.Load("PauseMenuPrefab", typeof(GameObject)) as GameObject;

        m_PauseMenu = (GameObject)Instantiate(m_PauseMenu, new Vector3(0, 0, 0), new Quaternion());
        m_PauseMenu.name = "PauseMenuPrefab";
        m_PauseMenuButtons = GameObject.Find("PauseMenuPrefab").GetComponentsInChildren<Button>();

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
            if (m_PauseMenuButtons[i].name == "MainMenuButton")
            {
                m_MainMenuButton = m_PauseMenuButtons[i];
                m_MainMenuButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnMainMenuButtonClick));
            }
        }


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
        Text[] t = GameObject.Find("ScoreCanvas").GetComponentsInChildren<Text>();


        for (int i = 0; i < t.Length; ++i)
        {
            if (t[i].name == "CountDownText")
            {
                m_DelayText = t[i];
            }
        }
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
        m_Paddle.SendMessage("SetPlayer", true);

        if (m_b2Player)
        {
            m_Player2Paddle = Resources.Load("Player2Paddle", typeof(GameObject)) as GameObject;
            paddleOffsetFromEdge = m_Player2Paddle.GetComponent<BoxCollider2D>().bounds.extents.x / 2;
            m_Player2Paddle.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - paddleOffsetFromEdge, Screen.height / 2, 0));
            m_Player2Paddle.transform.position += new Vector3(0, 0, 1);
            m_Player2Paddle.transform.localScale = new Vector3(0.1f, 0.5f, 0);
            m_Player2Paddle = Instantiate(m_Player2Paddle);
            m_Player2Paddle.name = "Player2Paddle";
            m_vInitial2PlayerPaddlePosition = m_Player2Paddle.transform.position;
            m_Player2Paddle.SendMessage("SetPlayer", false);
        }
        else
        {
            m_AIPaddle = Resources.Load("AIPaddle", typeof(GameObject)) as GameObject;
            m_AIPaddle.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - paddleOffsetFromEdge, Screen.height / 2, 0));
            m_AIPaddle.transform.position += new Vector3(0, 0, 1);
            m_AIPaddle.transform.localScale = new Vector3(0.1f, 0.5f, 0);
            m_AIPaddle = Instantiate(m_AIPaddle);
            m_AIPaddle.name = "AIPaddle";
            m_vInitial2PlayerPaddlePosition = m_AIPaddle.transform.position;
            m_Player2Paddle = m_AIPaddle;
        }
        Ai.SpeedState s = Ai.SpeedState.Linear;
        
        m_Ball = Instantiate(m_Ball);
        m_Ball.name = "Ball";
        m_Ball.transform.localScale = new Vector3(0.2f, 0.2f, 0);
        m_Ball.SendMessage("SetSpeedState", s);
        m_vScreenBoundary = new Vector2(Screen.width, Screen.height);

        #endregion

        #region Game State Initialization
        GameState initialStatus = new GameState(false, -1, false);
        m_sStatus = initialStatus;

        m_PlayerWinnerEndScreen = Resources.Load("PlayerWinnerScreen", typeof(GameObject)) as GameObject;
        m_AIWinnerEndScreen = Resources.Load("AIWinnerScreen", typeof(GameObject)) as GameObject;

        m_PlayerWinnerEndScreen = (GameObject)Instantiate(m_PlayerWinnerEndScreen, new Vector3(0, 5, 0), new Quaternion());
        m_AIWinnerEndScreen = (GameObject)Instantiate(m_AIWinnerEndScreen, new Vector3(0, 5, 0), new Quaternion());

        m_PlayerWinnerEndScreen.SetActive(false);
        m_AIWinnerEndScreen.SetActive(false);

        m_bDelayAfterAScore = false;

        if (PlayerPrefs.GetFloat("Difficulty") != -1)
        {
            float difficulty = PlayerPrefs.GetFloat("Difficulty");
            //Debug.Log(difficulty.ToString());
        }
        #endregion
    }
    //--------------------------------------------------------------------------------
    public void Update()
    {
        float dt = Time.fixedDeltaTime;

        if( m_bDelayAfterAScore )
        {
            m_DelayText.enabled = true;
            if (m_fTimer >= m_iDelayBetweenScores)
            {
                RestartGame();
                m_fTimer = 0.0f;
                m_sStatus.Paused = false;
                m_bDelayAfterAScore = false;
                m_DelayText.enabled = true;
                m_DelayText.enabled = false;
            }
            m_fTimer += dt;

            m_DelayText.text = ((int)(m_iDelayBetweenScores - m_fTimer)).ToString();
        }
        if (!m_sStatus.Paused && !m_bDelayAfterAScore)
        {
            if (!m_bStopUpdate)
            {
                if (m_sStatus.Status)
                {
                    GameOver();
                    if ((m_iPlayerScore != m_iMaxScore && m_iAIScore != m_iMaxScore))
                    {
                        m_sStatus.Status = false;
                        m_bDelayAfterAScore = true;
                        m_fTimer = 0.0f;
                        m_sStatus.Paused = true;
                        InformBehaviours();
                    }
                    else
                    {
                        m_sStatus.Status = true;
                    }
                }
            }
            UpdateScoreUI();
        }
    }
    //--------------------------------------------------------------------------------
    public void Restart()
    {
        m_sStatus.Paused = false;
        m_sStatus.Status = false;
        m_bStopUpdate = false;

        m_iPlayerScore = 0;
        m_iAIScore = 0;
        m_PauseMenu.SetActive(false);
        if (m_PlayerWinnerEndScreen.activeInHierarchy)
        {
            (m_PlayerWinnerEndScreen).SetActive(false);
            //Destroy(m_PlayerWinnerEndScreen); //Somehow it destroys the resource loaded WTF
        }
        if (m_AIWinnerEndScreen.activeInHierarchy)
        {
            (m_AIWinnerEndScreen).SetActive(false);
            //Destroy(m_AIWinnerEndScreen); //Somehow it destroys the resource loaded WTF
        }
        GameObject.Find("Ball").SendMessage("Restart");

        InformBehaviours();
        m_Paddle.transform.position = m_vInitialPlayerPaddlePosition;
        m_Player2Paddle.transform.position = m_vInitial2PlayerPaddlePosition;
    }
    //--------------------------------------------------------------------------------
    public void RestartGame()
    {
        m_sStatus.Paused = false;
        m_sStatus.Status = false;
        m_bStopUpdate = false;
        if ((m_iPlayerScore >= m_iMaxScore || m_iAIScore >= m_iMaxScore))
        {
            m_iPlayerScore = 0;
            m_iAIScore = 0;
            m_PauseMenu.SetActive(false);
            if (m_PlayerWinnerEndScreen.activeInHierarchy)
            {
                (m_PlayerWinnerEndScreen).SetActive(false);
            }
            if (m_AIWinnerEndScreen.activeInHierarchy)
            {
                (m_AIWinnerEndScreen).SetActive(false);
            }
        }
        InformBehaviours();
        
        m_Paddle.transform.position = m_vInitialPlayerPaddlePosition;
        m_Player2Paddle.transform.position = m_vInitial2PlayerPaddlePosition;
    }
    //--------------------------------------------------------------------------------
    public Vector3 ToScreen(Vector3 V)
    {
        return Camera.main.WorldToScreenPoint(V);
    }
    //--------------------------------------------------------------------------------
    public void GameOver()
    {
        if (m_sStatus.Winner == 1) //Player scores
            m_iPlayerScore++;
        else
            m_iAIScore++;
    }
    //--------------------------------------------------------------------------------
    public void SetGameStatus(object c)
    {
        GameState desiredState = (GameState)c;

        m_sStatus = desiredState;

        InformBehaviours();

        m_PauseMenu.SetActive(false);
        m_MenuButton.enabled = true;
    }
    //--------------------------------------------------------------------------------
    public void InformBehaviours()
    {
        GameObject.Find("Ball").SendMessage("CheckGameStatus", m_sStatus);

    }
    //--------------------------------------------------------------------------------
    public void OnPauseMenuExitButtonClick()
    {
        m_MenuButton.enabled = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    //--------------------------------------------------------------------------------
    public void OnMainMenuButtonClick()
    {
        Application.LoadLevel("MainMenu");
    }
   //--------------------------------------------------------------------------------
    public void OnPauseMenuGoBackButtonClick()
    {
        m_MenuButton.enabled = true;
        m_sStatus.Paused = false;
        InformBehaviours();
        m_PauseMenu.SetActive(false);
    }
    //--------------------------------------------------------------------------------
    public void OnPauseMenuRestartButtonClick()
    {
        m_MenuButton.enabled = true;
        Restart();
    }
    //--------------------------------------------------------------------------------
    public void OnMenuButtonClick()
    {
        m_MenuButton.enabled = false;
        m_sStatus.Paused = true;
        GameObject.Find("Ball").SendMessage("CheckGameStatus", m_sStatus);

        m_PauseMenu.SetActive(true);
    }
    //--------------------------------------------------------------------------------
    public void UpdateScoreUI()
    {
        if (m_iPlayerScore >= m_iMaxScore && m_sStatus.Status)
        {
            m_PlayerWinnerEndScreen.SetActive(true);

            m_sStatus.Paused = true;
            m_sStatus.Winner = 1;
            m_sStatus.Status = false;

            m_bStopUpdate = true;
            InformBehaviours();
        }
        if (m_iAIScore >= m_iMaxScore && m_sStatus.Status)
        {
            m_AIWinnerEndScreen.SetActive(true);
            m_sStatus.Paused = true;
            m_sStatus.Winner = 0;
            m_sStatus.Status = false;
            m_bStopUpdate = true;
            InformBehaviours();
        }
        m_PlayerScoreText.text = "Player 1 Score:" + m_iPlayerScore.ToString();
        m_AIScoreText.text = "Player 2 Score:" + m_iAIScore.ToString();
    }
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    //-------------------------------Variables----------------------------------------
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    #region OBJECTS AND STUFF
    private GameObject m_Ball;
    private GameObject m_Paddle;
    private GameObject m_AIPaddle;
    private GameObject m_Player2Paddle;
    private Vector2 m_vScreenBoundary;
    private Vector2 m_vBallPosition;
    private bool m_bDebug = false;
    private int m_iSphereRadius;
    private GameObject m_PlayerWinnerEndScreen;
    private GameObject m_AIWinnerEndScreen;

    private Vector3 m_vInitialBallPosition;
    private Vector3 m_vInitialPlayerPaddlePosition;
    private Vector3 m_vInitial2PlayerPaddlePosition;
    private float m_fTimer;
    private bool m_bDelayAfterAScore;
    #endregion

    #region GAME STATE AND STUFF

    private bool m_b2Player;
    private const int m_iMaxScore = 5;
    private const int m_iDelayBetweenScores = 3;
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
    private int m_iPlayerScore;
    private int m_iAIScore;

    #endregion

    #region UI STUFF

    private Text[] m_CanvasTextComps;
    private Text m_PlayerScoreText;
    private Text m_AIScoreText;
    private Button[] m_CanvasButtons;
    private Button m_MenuButton;
    private Text m_DelayText;

    private GameObject m_PauseMenu;
    private Button[] m_PauseMenuButtons;
    private Button m_ExitButton;
    private Button m_GoBackButton;
    private Button m_RestartButton;
    private Button m_MainMenuButton;
   
    #endregion
}
//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------
// ~End of GameLogic.cs
//--------------------------------------------------------------------------------
