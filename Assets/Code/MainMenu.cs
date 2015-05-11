//--------------------------------------------------------------------------------
//This is a file from Pong2D.A hobby project for Gram Games evaluation
//
//Copyright (c) Alperen Gezer.All rights reserved.
//
//MainMenu.cs
//
//This is the class that handles the buttons to launch the level
//and setting difficulty
//--------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//--------------------------------------------------------------------------------
public class MainMenu : MonoBehaviour
{
    //--------------------------------------------------------------------------------
	public void Start ()
    {
        m_Buttons = GameObject.Find("GlobalCanvas").GetComponentsInChildren<Button>();
        m_Slider = GameObject.Find("GlobalCanvas").GetComponentsInChildren<Slider>();
        for( int i=0; i < m_Buttons.Length; i++ )
        {
            
            if( m_Buttons[i].name == "ExitButton")
            {
                
                m_ExitButton = m_Buttons[i];
            }
            if (m_Buttons[i].name == "StartButton")
            {
                m_StartButton = m_Buttons[i];
            }
            if (m_Buttons[i].name == "TwoPlayerButton")
            {
                m_2PlayerStart = m_Buttons[i];
            }
        }
        m_ExitButton.onClick.AddListener(new UnityEngine.Events.UnityAction(ExitButtonClick));
        m_StartButton.onClick.AddListener(new UnityEngine.Events.UnityAction(LoadLevel));
        m_2PlayerStart.onClick.AddListener(new UnityEngine.Events.UnityAction(Load2Player));
	}
    //--------------------------------------------------------------------------------
	public void Update ()
    {
	
	}
    //--------------------------------------------------------------------------------
    public void Load2Player()
    {
        PlayerPrefs.SetInt("Is2Player", 1);
        Application.LoadLevel("Level1");
        
    }
    //--------------------------------------------------------------------------------
    public void LoadLevel()
    {
        Application.LoadLevel("Level1");
        PlayerPrefs.SetFloat("Difficulty", m_Slider[0].value);
        PlayerPrefs.SetInt("Is2Player", 0);
    }
    //--------------------------------------------------------------------------------
    public void ExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    //-------------------------------Variables----------------------------------------
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    private Button[] m_Buttons;
    private Button m_ExitButton;
    private Button m_StartButton;
    private Button m_2PlayerStart;
    private Slider[] m_Slider;
}
//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------
// ~End of MainMenu.cs
//--------------------------------------------------------------------------------
