using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    private GameObject m_LeftPaddle;
    private Vector2 m_vLeftPaddlePosition;
    private Vector2 m_vLeftPaddleSpeed;
    private Vector2 m_vInitialPosition;

    private GameObject m_RightPaddle;
    private Vector2 m_vRightPaddlePosition;


    private Vector2 m_vScreenBoundary;

    private Vector2 m_vPreviousInputPosition;
    private Vector2 m_vCurrentInputPosition;

    private Vector2 m_vLastInputPosition;
    private struct PaddlePositionHandler
    {
        public Vector3 Previous { get; set; }
        public Vector3 Next { get; set; }
    }

    private PaddlePositionHandler m_vLeftPaddlePositionHandler;
    private bool m_bDragging;
    private bool m_bPaused;
    void Start()
    {
        m_vScreenBoundary = new Vector2(Screen.width, Screen.height);

        m_LeftPaddle = GameObject.Find("LeftPaddle") as GameObject;

        m_vPreviousInputPosition = Vector3.zero;
        m_vCurrentInputPosition = Vector3.zero;

        if (m_LeftPaddle != null)
        {
            m_vLeftPaddlePosition = m_LeftPaddle.transform.position;
            m_vInitialPosition = m_vLeftPaddlePosition;
        }

        m_vRightPaddlePosition = Vector3.zero;
        m_vLeftPaddleSpeed = Vector2.zero;

        m_bDragging = false;

        m_vLeftPaddlePositionHandler = new PaddlePositionHandler();
        m_vLeftPaddlePositionHandler.Previous = m_LeftPaddle.transform.position;
        m_vLeftPaddlePositionHandler.Next = m_LeftPaddle.transform.position;

        m_bPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bPaused)
        {
            #region Boring Input stuff
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                m_bDragging = true;
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
                m_bDragging = true;
            else
                m_bDragging = false;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                m_bDragging = false;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (Input.GetMouseButton(0))
                {
                    m_bDragging = true;
                }
                if (Input.GetMouseButtonUp(0))
                    m_bDragging = false;
            }
            #endregion

            CheckLefthandSide();
            ApplyPaddleTranslation();
            UpdatePaddleLinearVelocity();
        }
        #region More Boring Input
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButton(0))
        {

            if (m_bPaused)
            {
                Vector2 inputPos;
#if UNITY_EDITOR
                inputPos = Input.mousePosition;
#else
                inputPos = Input.GetTouch(0).position;
#endif

                float halfX = m_vScreenBoundary.x * 0.5f;
                float halfY = m_vScreenBoundary.y * 0.5f;

                Debug.Log(halfY.ToString());
                if (inputPos.x <= halfY)
                {
                    GameLogic.GameState gameStatus = new GameLogic.GameState(false, (int)-1, false);


                    GameObject.Find("GameLogic").SendMessage("SetGameStatus", gameStatus);
                }
            }
        }
        #endregion
        
        
        
        m_vLastInputPosition = Vector3.zero;
    }
    void CheckLefthandSide()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            MovePaddle(Input.mousePosition, true);
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            MovePaddle(Input.GetTouch(0).position, false);
        }
    }
    void MovePaddle(Vector2 InputPosition, bool editor)
    {
        float halfX = m_vScreenBoundary.x * 0.5f;
        float halfY = m_vScreenBoundary.y * 0.5f;

        m_vCurrentInputPosition = InputPosition + m_vLastInputPosition; //m_vLastInputPosition is for awaking from pause screen so paddle wont jump somewhere ridiciluous

        if (m_vCurrentInputPosition.x <= halfX && m_bDragging)
        {
            Vector2 dv;
            if (!editor)
                dv = Input.GetTouch(0).deltaPosition;
            else
                dv = m_vCurrentInputPosition - m_vPreviousInputPosition;
            float dx = dv.x;
            float dy = dv.y;
            Vector3 dyInScreenPos = (dv) * 0.020f;
            m_vLeftPaddleSpeed = m_vCurrentInputPosition - m_vPreviousInputPosition;
            m_vLeftPaddlePosition += new Vector2(0, dyInScreenPos.y);
            m_vLeftPaddlePositionHandler.Next = m_vLeftPaddlePosition;
            m_vLeftPaddlePositionHandler.Previous = m_vLeftPaddlePositionHandler.Next;
        }
        m_vPreviousInputPosition = m_vCurrentInputPosition;
    }
    void ApplyPaddleTranslation()
    {
        m_LeftPaddle.transform.position = m_vLeftPaddlePositionHandler.Next;
        ClampPaddleMovementAgainstScreen();
    }
    void ClampPaddleMovementAgainstScreen()
    {
        float screenOffset = m_LeftPaddle.GetComponent<Collider2D>().bounds.size.y;
        Vector3 offsetVector = Camera.main.ScreenToWorldPoint(new Vector3(0, screenOffset / 2)); //This is constant so no need to calculate this every frame
        if ((m_LeftPaddle.transform.position).y <= offsetVector.y + 1)
        {
            Vector3 Temp = m_LeftPaddle.transform.position;
            m_LeftPaddle.transform.position = new Vector3(0, offsetVector.y + 1);
            m_LeftPaddle.transform.position = new Vector3(Temp.x, m_LeftPaddle.transform.position.y, Temp.z);
        }
        if ((m_LeftPaddle.transform.position).y >= -(offsetVector.y + 1))
        {
            Vector3 Temp = m_LeftPaddle.transform.position;
            m_LeftPaddle.transform.position = new Vector3(0, -(offsetVector.y + 1));
            m_LeftPaddle.transform.position = new Vector3(Temp.x, m_LeftPaddle.transform.position.y, Temp.z);
        }
    }
    void CheckGameStatus(object status)
    {
        GameLogic.GameState desiredState = (GameLogic.GameState)status;

        m_bPaused = desiredState.Paused;
        m_vPreviousInputPosition = m_vCurrentInputPosition;
        Debug.Log(m_bPaused.ToString());
    }
    void UpdatePaddleLinearVelocity()
    {
        GameObject.Find("Ball").SendMessage("UpdatePaddleSpeed", m_vLeftPaddleSpeed);
    }
}
