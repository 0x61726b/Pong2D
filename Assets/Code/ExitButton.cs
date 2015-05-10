using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    private Button[] m_Buttons;
    private Button m_ExitButton;
    private Button m_StartButton;
    
	// Use this for initialization
	void Start ()
    {
        m_Buttons = GameObject.Find("GlobalCanvas").GetComponentsInChildren<Button>();
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
