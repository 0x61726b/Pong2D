using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Ball : MonoBehaviour 
{
    private Vector2 m_vBallSpeed;
    private Vector2 m_vScreenBoundary;
    private Vector2 m_vBallPosition;
    private Vector2 m_vInitialBallPosition;
    private Vector2 m_vInitialBallSpeed;
    private Vector2 m_vSpeedMultiplier;
    private Vector2 m_vPlayerPaddleSpeed;
    private Vector2 m_vAIPaddleSpeed;
    private bool m_bExpSpeed = false;
    private bool m_bDebug = false;
    private bool m_bPaused;
	// Use this for initialization
	void Start () 
    {
        

        m_vScreenBoundary = new Vector2(Screen.width, Screen.height);

        m_vInitialBallPosition = transform.position;

        m_bPaused = false;

        GetComponent<Renderer>().material.renderQueue = 1;
        GetComponent<Renderer>().sortingOrder= 0;

        
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        float dt = Time.fixedDeltaTime;

        

        if (!m_bPaused)
        {
            m_vBallPosition += (m_vSpeedMultiplier) * dt;
            CheckCollisionWithScreen();
            ApplyChanges();
            if (m_bExpSpeed)
            {
                m_vSpeedMultiplier.x += dt*0.5f;
            }
            else
            {

            }
        }

        
	}
    void ApplyChanges()
    {
        transform.position = m_vBallPosition;
    }
    void CheckCollisionWithScreen()
    {
        if (m_bDebug)
        {
            //Debug.Log(ToScreen(m_vBallPosition).ToString());
        }

        Vector3 HalfExtents = this.GetComponent<BoxCollider2D>().bounds.extents;
        Vector3 PositionMinusHExtents = new Vector3(m_vBallPosition.x - HalfExtents.x, m_vBallPosition.y - HalfExtents.y);
        Vector3 PositionPlusHExtents = new Vector3(m_vBallPosition.x + HalfExtents.x, m_vBallPosition.y + HalfExtents.y);

        if (m_vScreenBoundary.x <= ToScreen(m_vBallPosition).x || ToScreen(m_vBallPosition).x <= 0)
        {
            float winner = (ToScreen(m_vBallPosition).x / m_vScreenBoundary.x);
            winner = Mathf.Sign(winner);

            GameLogic.GameState gameStatus = new GameLogic.GameState(true,(int)winner,false);
            
            
            GameObject.Find("GameLogic").SendMessage("SetGameStatus", gameStatus);

            m_vSpeedMultiplier = m_vInitialBallSpeed;
            m_vBallPosition = new Vector2(0,0);
        }
        if (m_vScreenBoundary.y <= ToScreen(PositionPlusHExtents).y || ToScreen(PositionMinusHExtents).y <= 0)
        {
            m_vSpeedMultiplier.y *= -1;
        }
    }
    void BallPaddleCollision(Collision2D coll)
    {
        //****************This is called Collision Detection with Contact Points Averaging
        //****************
        //****************
        //Collision2D c = coll;

        //Vector2 Average = Vector2.zero;
        //Vector2 Normal = Vector2.zero;
        //foreach(ContactPoint2D cp in c.contacts)
        //{
        //    Average = new Vector2( Average.x + Mathf.Abs(cp.point.x), Average.y + Mathf.Abs(cp.point.y) );
        //    Normal = new Vector2( Normal.x + (cp.normal.x), Normal.y + (cp.normal.y) );
        //}
        //Average /= c.contacts.Length;
        //Average = Average.normalized;

        //Normal /= c.contacts.Length;
        //Normal = Normal.normalized;

        //float distance = Vector2.Dot(Average, Normal);

        //Average = new Vector2(Average.x * Normal.x, Average.y * Normal.y); //LMFAO NO VECTOR MULTIPLICATION 

        //****************
        //****************
        //****************Though I decided not to include this since it was unstable

        Vector2 Reflect = Vector2.zero;
        if( coll.collider.name == "AIPaddle")
        {
            Reflect = m_vAIPaddleSpeed * 0.4f;
        }
        else
        {
            Reflect = m_vPlayerPaddleSpeed * 0.4f;
        }

        m_vSpeedMultiplier.x *= -1;

        Debug.Log(-Reflect.y);
        m_vSpeedMultiplier.y = -Reflect.y;
    }
    void UpdatePaddleSpeed(Vector2 Speed)
    {
        m_vPlayerPaddleSpeed = Speed;
    }
    void UpdateAIPaddleSpeed(Vector2 Speed)
    {
        m_vAIPaddleSpeed = Speed;
    }
    void SetSpeedState(Ai.SpeedState b)
    {
        Ai.SpeedState state = b;

        switch(state)
        {
            case Ai.SpeedState.Constant:
                m_vSpeedMultiplier = new Vector2(5,0.5f);
                break;
            case Ai.SpeedState.Exponential:
                m_bExpSpeed = true;
                m_vSpeedMultiplier = new Vector2(12, 5);
                break;
            case Ai.SpeedState.Linear:
                m_vSpeedMultiplier = new Vector2(7, 0);
                break;
        }
        m_vInitialBallSpeed = m_vSpeedMultiplier;
    }
    Vector3 ToScreen(Vector3 V)
    {
        return Camera.main.WorldToScreenPoint(V);
    }
    void Restart()
    {
        m_vBallPosition = new Vector2(0, 0);
        m_vBallSpeed = m_vInitialBallSpeed;
    }
    void CheckGameStatus(object status)
    {

        GameLogic.GameState desiredState = (GameLogic.GameState)status;

        m_bPaused = desiredState.Paused;
    }
}
