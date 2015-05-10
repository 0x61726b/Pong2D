using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour 
{
    private Vector2 m_vBallSpeed;
    private Vector2 m_vScreenBoundary;
    private Vector2 m_vBallPosition;
    private Vector2 m_vInitialBallPosition;
    private Vector2 m_vInitialBallSpeed;

    private Vector2 m_vPlayerPaddleSpeed;

    private bool m_bDebug = false;
    private bool m_bPaused;
	// Use this for initialization
	void Start () 
    {
        m_vInitialBallSpeed = new Vector2(10, 1);
        m_vBallSpeed = m_vInitialBallSpeed;

        m_vScreenBoundary = new Vector2(Screen.width, Screen.height);

        m_vInitialBallPosition = transform.position;

        m_bPaused = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float dt = Time.fixedDeltaTime;

        if (!m_bPaused)
        {
            m_vBallPosition += m_vBallSpeed * dt;
            CheckCollisionWithScreen();
            ApplyChanges();
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
            Debug.Log(ToScreen(m_vBallPosition).ToString());
        }


        if (m_vScreenBoundary.x <= ToScreen(m_vBallPosition).x || ToScreen(m_vBallPosition).x <= 0)
        {
            m_vBallSpeed.x *= -1;
            float winner = (ToScreen(m_vBallPosition).x / m_vScreenBoundary.x);
            winner = Mathf.Sign(winner);

            GameLogic.GameState gameStatus = new GameLogic.GameState(true,(int)winner,false);
            
            
            GameObject.Find("GameLogic").SendMessage("SetGameStatus", gameStatus);

            m_vBallSpeed = m_vInitialBallSpeed;
            m_vBallPosition = new Vector2(0,0);
        }
        if (m_vScreenBoundary.y <= ToScreen(m_vBallPosition).y || ToScreen(m_vBallPosition).y <= 0)
        {
            m_vBallSpeed.y *= -1;
        }
    }
    void BallPaddleCollision(Collision2D coll)
    {
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
        
        //Debug.Log(c.rigidbody.velocity.ToString());


        
        m_vBallSpeed.x = -1 * Random.Range(5, 20) * (m_vBallSpeed/m_vBallSpeed.magnitude).x;
        
        Vector2 Force = m_vPlayerPaddleSpeed * 0.3f;
        m_vBallSpeed.y = -Force.y;
    }
    void UpdatePaddleSpeed(Vector2 Speed)
    {
        m_vPlayerPaddleSpeed = Speed;
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
