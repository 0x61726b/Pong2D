using UnityEngine;
using System.Collections;

public class Ai : MonoBehaviour
{
    private Vector3 m_vActorPosition;
    private Vector2 m_vSpring;
    private GameObject m_Ball;
    private enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Unfair
    }
    private float Range;
    private Difficulty m_Difficulty;
	// Use this for initialization
	void Start ()
    {
        m_vActorPosition = new Vector3(0, 0, 0);
        Debug.Log(transform.position.y.ToString());
        m_vSpring = new Vector2(transform.position.y, 0);

        m_Ball = GameObject.Find("Ball") as GameObject;

        Range = 50;

        m_Difficulty = Difficulty.Normal; //Default

	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector2 targetPos = m_Ball.transform.position;
        Vector2 Distance = new Vector2(transform.position.x,transform.position.y) - targetPos;
        float sqLen = Distance.SqrMagnitude();

        if (sqLen < Range)
        {
            m_vSpring = Spring(m_vSpring.x, m_vSpring.y, m_Ball.transform.position.y, 0.9f, sqLen*0.8f, Time.deltaTime);
        }
        else
        {
            m_vSpring = Spring(m_vSpring.x, m_vSpring.y, m_Ball.transform.position.y, 0.9f, 2, Time.deltaTime);
        }
        
        transform.position = new Vector2(transform.position.x,m_vSpring.x);
        
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
