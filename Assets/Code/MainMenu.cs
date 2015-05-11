using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Button[] m_Buttons;
    
    private Button m_ExitButton;
    private Button m_StartButton;
    private Slider[] m_Slider;
    
	// Use this for initialization
	void Start ()
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
        }
        m_ExitButton.onClick.AddListener(new UnityEngine.Events.UnityAction(ExitButtonClick));
        m_StartButton.onClick.AddListener(new UnityEngine.Events.UnityAction(LoadLevel));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void LoadLevel()
    {
        Application.LoadLevel("Level1");
        PlayerPrefs.SetFloat("Difficulty", m_Slider[0].value);
    }
    void ExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
