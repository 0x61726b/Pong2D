//--------------------------------------------------------------------------------
//This is a file from Pong2D.A hobby project for Gram Games evaluation
//
//Copyright (c) Alperen Gezer.All rights reserved.
//
//Paddle.cs
//
//This is the Paddle object used to reflect the ball.If playing against AI,
//there will only be one instance of this object and will be on the left side of the screen.
//If playing 2 player mode,there will be 2 instances of this object and will placed 
//across screen accordingly.
//--------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
//--------------------------------------------------------------------------------
public class Paddle : MonoBehaviour
{
    //--------------------------------------------------------------------------------
    public Paddle()
    {

    }
    //--------------------------------------------------------------------------------
    public void Start()
    {
        m_vScreenBoundary = new Vector2(Screen.width, Screen.height);
        m_vPreviousInputPosition = Vector3.zero;
        m_vCurrentInputPosition = Vector3.zero;

        m_vLeftPaddlePosition = transform.position;
        m_vInitialPosition = m_vLeftPaddlePosition;

        m_vLeftPaddlePositionHandler = new PaddlePositionHandler();
        m_vLeftPaddlePositionHandler.Previous = transform.position;
        m_vLeftPaddlePositionHandler.Next = transform.position;

        m_vLeftPaddleSpeed = Vector2.zero;
        m_vLastInputPosition = Vector2.zero;
        m_bPaused = false;
        m_bDragging = false;

    }
    //--------------------------------------------------------------------------------
    public void SetPlayer(bool b)
    {
        m_bPlayerNumber = b;
    }
    //--------------------------------------------------------------------------------
    public void Update()
    {
        if (!m_bPaused)
        {
            #region Boring Input stuff
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();


            if (Input.touchCount > 0)
            {
                int tapCount = Input.touchCount;
                for (int i = 0; i < tapCount; i++)
                {
                    m_CurrentTouch = Input.GetTouch(0);
                    if (m_CurrentTouch.phase == TouchPhase.Began)
                        m_bDragging = true;
                    if (m_CurrentTouch.phase == TouchPhase.Moved)
                        m_bDragging = true;
                    else
                        m_bDragging = false;

                    if (m_CurrentTouch.phase == TouchPhase.Ended)
                        m_bDragging = false;
                }
            }

            if (Input.GetMouseButton(0))
            {
                m_bDragging = true;
            }
            if (Input.GetMouseButtonUp(0))
                m_bDragging = false;

            #endregion

            CheckLefthandSide();
            ApplyPaddleTranslation();
            UpdatePaddleLinearVelocity();
        }
    }
    //--------------------------------------------------------------------------------
    public void CheckLefthandSide()
    {
#if UNITY_EDITOR
        MovePaddle(Input.mousePosition, true);
#else
        
        MovePaddle(m_CurrentTouch.position, false);
#endif
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// This basically takes the current input position
    /// the macro UNITY_EDITOR defines if we're playing in editor
    /// It will take mouse,if we're on Android it will take touch input
    /// And for instance on the Android,  MovePaddle(Input.mousePosition, true); this code wont compile so there is no problem
    /// This method compares the right or left half of the screen against the input position then
    /// moves the paddle according to the last input to achieve smooth movement instead of fixed up-down translation
    /// </summary>
    /// <param name="InputPosition"></param>
    /// <param name="editor"></param>
    public void MovePaddle(Vector2 InputPosition, bool editor)
    {
        float halfX = m_vScreenBoundary.x * 0.5f;
        float halfY = m_vScreenBoundary.y * 0.5f;


        m_vCurrentInputPosition = InputPosition + m_vLastInputPosition; //m_vLastInputPosition is for awaking from pause screen so paddle wont jump somewhere ridiciluous

        if (m_bPlayerNumber) //Player on the left
        {
            if (m_vCurrentInputPosition.x <= halfX && m_bDragging)
            {
                Vector2 dv = Vector2.zero;
                if (!editor)
                {

                    dv = m_CurrentTouch.deltaPosition;

                }
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
        else
        {
            if (m_vCurrentInputPosition.x >= halfX && m_bDragging)
            {
                Vector2 dv = Vector2.zero;
                if (!editor)
                {

                    dv = m_CurrentTouch.deltaPosition;

                }
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

    }
    //--------------------------------------------------------------------------------
    public void ApplyPaddleTranslation()
    {
        transform.position = m_vLeftPaddlePositionHandler.Next;
        ClampPaddleMovementAgainstScreen();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Used to prevent paddle getting off of the screen
    /// </summary>
    public void ClampPaddleMovementAgainstScreen()
    {
        float screenOffset = GetComponent<Collider2D>().bounds.size.y;
        Vector3 offsetVector = Camera.main.ScreenToWorldPoint(new Vector3(0, screenOffset / 2)); //This is constant so no need to calculate this every frame
        if ((transform.position).y <= offsetVector.y + 1)
        {
            Vector3 Temp = transform.position;
            transform.position = new Vector3(0, offsetVector.y + 1);
            transform.position = new Vector3(Temp.x, transform.position.y, Temp.z);
        }
        if ((transform.position).y >= -(offsetVector.y + 1))
        {
            Vector3 Temp = transform.position;
            transform.position = new Vector3(0, -(offsetVector.y + 1));
            transform.position = new Vector3(Temp.x, transform.position.y, Temp.z);
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// This is to update this instance of Paddle's speed 
    /// to ensure reflecting correct speed when ball and this Paddle collided
    /// </summary>
    public void UpdatePaddleLinearVelocity()
    {
        if (m_bPlayerNumber)
        {
            GameObject.Find("Ball").SendMessage("UpdatePaddleSpeed", m_vLeftPaddleSpeed);
        }
        else
        {
            GameObject.Find("Ball").SendMessage("UpdateAIPaddleSpeed", m_vLeftPaddleSpeed);
        }
    }
    //--------------------------------------------------------------------------------
    public void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.name == "Ball")
        {
            c.gameObject.SendMessage("BallPaddleCollision", c);
        }
    }
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    //-------------------------------Variables----------------------------------------
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    private Vector2 m_vLeftPaddlePosition;
    private Vector2 m_vLeftPaddleSpeed;
    private Vector2 m_vInitialPosition;
    private Vector2 m_vScreenBoundary;
    private Vector2 m_vPreviousInputPosition;
    private Vector2 m_vCurrentInputPosition;
    private Vector2 m_vLastInputPosition;
    private struct PaddlePositionHandler
    {
        public Vector3 Previous { get; set; }
        public Vector3 Next { get; set; }
    }
    private bool m_bPaused;
    private bool m_bDragging;
    private PaddlePositionHandler m_vLeftPaddlePositionHandler;
    private Touch m_CurrentTouch;
    private bool m_bPlayerNumber;
}
//--------------------------------------------------------------------------------
// ~End of Paddle.cs
//--------------------------------------------------------------------------------
