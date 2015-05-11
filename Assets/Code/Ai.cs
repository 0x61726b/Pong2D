using UnityEngine;
using System.Collections;

public class Ai : MonoBehaviour
{
    private Vector3 m_vActorPosition;
    private Vector2 m_vSpring;
    private GameObject m_Ball;
    private Vector2 m_vSpeed = Vector2.zero;
    private enum Difficulty
    {
        Easy = 1,
        Normal = 2,
        Hard = 3,
        Unfair = 4,
        Impossible = 5
    }
    public enum SpeedState
    {
        Constant,
        Linear,
        Exponential
    }
    private float Range;
    private Difficulty m_Difficulty;
    private SpeedState m_SpeedState;
	// Use this for initialization
	void Start ()
    {
        m_vActorPosition = new Vector3(0, 0, 0);
        //Debug.Log(transform.position.y.ToString());
        m_vSpring = new Vector2(transform.position.y, 0);

        m_Ball = GameObject.Find("Ball") as GameObject;


        m_Difficulty = Difficulty.Impossible; //Default
        m_SpeedState = SpeedState.Exponential; //Default
        
        if (PlayerPrefs.GetFloat("Difficulty") != -1)
        {
            float difficulty = PlayerPrefs.GetFloat("Difficulty");

            if(difficulty == 1)
            {
                m_Difficulty = Difficulty.Easy;
                m_SpeedState = SpeedState.Constant;
            }
            if (difficulty == 2)
            {
                m_Difficulty = Difficulty.Normal;
                m_SpeedState = SpeedState.Constant;

                Range = 50;
            }
            if (difficulty == 3)
            {
                m_Difficulty = Difficulty.Hard;
                m_SpeedState = SpeedState.Linear;

                Range = 100;
            }
            if (difficulty == 4)
            {
                m_Difficulty = Difficulty.Unfair;
                m_SpeedState = SpeedState.Linear;

                Range = 200;
            }
            if(difficulty == 5)
            {
                m_Difficulty = Difficulty.Impossible;
                m_SpeedState = SpeedState.Exponential;
            }
        }
        m_Ball.SendMessage("SetSpeedState", m_SpeedState);
	}
	void Update () 
    {

        switch(m_Difficulty)
        {
            case Difficulty.Easy:
                EasyDifficulty();
                break;
            case Difficulty.Normal:
                NormalDifficulty();
                break;
            case Difficulty.Hard:
                HardDifficulty();
                break;
            case Difficulty.Unfair:
                UnfairDifficulty();
                break;
            case Difficulty.Impossible:
                ImpossibleDifficulty();
                break;
        }

        if (m_Difficulty != Difficulty.Impossible)
        {
            transform.position = new Vector2(transform.position.x, m_vSpring.x);
            m_vSpeed = new Vector2(0, m_vSpring.y);
            GameObject.Find("Ball").SendMessage("UpdateAIPaddleSpeed", m_vSpeed);
        }
        else
        {
            m_vSpeed = new Vector2(0, 5);
            GameObject.Find("Ball").SendMessage("UpdateAIPaddleSpeed", m_vSpeed);
        }

	}
    void EasyDifficulty()
    {
        //No range check
        //High Damping
        //High angular frequency
        m_vSpring = Spring(m_vSpring.x, m_vSpring.y, m_Ball.transform.position.y, 1.4f, 2, Time.deltaTime);
    }
    void NormalDifficulty()
    {
        Vector2 targetPos = m_Ball.transform.position;
        Vector2 Distance = new Vector2(transform.position.x, transform.position.y) - targetPos;
        float sqLen = Distance.SqrMagnitude();
        //Range finding
        //Normal Damping
        //High angular frequency
        if (sqLen < Range)
        {
            m_vSpring = Spring(m_vSpring.x, m_vSpring.y, m_Ball.transform.position.y, 1.2f, 6, Time.deltaTime);
        }
        else
        {
            m_vSpring = Spring(m_vSpring.x, m_vSpring.y, m_Ball.transform.position.y, 1.7f, 2, Time.deltaTime);
        }
    }
    void HardDifficulty()
    {
        //Range Check R = 100
        //Low damping
        Vector2 targetPos = m_Ball.transform.position;
        Vector2 Distance = new Vector2(transform.position.x, transform.position.y) - targetPos;
        float sqLen = Distance.SqrMagnitude();

        if (sqLen < Range)
        {
            m_vSpring = Spring(m_vSpring.x, m_vSpring.y, m_Ball.transform.position.y, 0.9f, sqLen * 0.8f, Time.deltaTime);
        }
        else
        {
            m_vSpring = Spring(m_vSpring.x, m_vSpring.y, m_Ball.transform.position.y, 0.4f, 6, Time.deltaTime);
        }
    }
    void UnfairDifficulty()
    {
        Vector2 targetPos = m_Ball.transform.position;
        Vector2 Distance = new Vector2(transform.position.x, transform.position.y) - targetPos;
        float sqLen = Distance.SqrMagnitude();

        if (sqLen < Range)
        {
            m_vSpring = Spring(m_vSpring.x, m_vSpring.y, m_Ball.transform.position.y, 0.04f, 10, Time.deltaTime);
        }
        else
        {
            m_vSpring = Spring(m_vSpring.x, m_vSpring.y, m_Ball.transform.position.y, 0.4f, sqLen, Time.deltaTime);
        }
    }
    void ImpossibleDifficulty()
    {
        transform.position= new Vector3(transform.position.x,m_Ball.transform.position.y);
    }

    public Vector2 Spring( float x,float v,float Xt,float Zeta,float Omega,float h)
    {
        float f = 1.0f + 2.0f * h * Zeta * Omega;
        float oo = Omega * Omega;
        float hoo = h * oo;
        float hhoo = h * hoo;
        float detInv = 1.0f / (f + hhoo);
        float detX = f * x + h * v + hhoo * Xt;
        float detV = v + hoo * (Xt - x);
        x = detX * detInv;
        v = detV * detInv;
        return new Vector2(x, v);
    }
    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.name == "Ball")
        {
            c.gameObject.SendMessage("BallPaddleCollision", c);
        }
    }
}
