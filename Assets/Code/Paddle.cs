using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour 
{
    
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    void OnCollisionEnter2D(Collision2D c)
    {
        if(c.gameObject.name == "Ball")
        {
            c.gameObject.SendMessage("BallPaddleCollision", c);
        }
    }
}
