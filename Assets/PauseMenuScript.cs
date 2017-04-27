using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour 
{
	private bool m_GamePaused;
	private bool m_HUDEnabled;
	public GameObject[] m_UIButtons;
	public GameObject[] m_HUDElements;

	void Start () 
	{
		m_GamePaused = true;
		m_HUDEnabled = true;
		PauseGame ();
	}

	void Update () 
	{
		if (Input.GetKeyDown ("escape"))
		{
			PauseGame ();
		}
	}


	public void PauseGame()
	{
		if (m_GamePaused) 
		{
			Time.timeScale = 1;
			HideButtons();
			m_GamePaused = false;
		}
		else
		{
			Time.timeScale = 0;
			ShowButtons ();
			m_GamePaused = true;
		}
	}

	public void ShowButtons()
	{
		foreach(GameObject button in m_UIButtons)
		{
			button.SetActive (true);
		}
	}

	public void HideButtons()
	{
		foreach(GameObject button in m_UIButtons)
		{
			button.SetActive (false);
		}
	}

	public void ExitGame()
	{
		Application.Quit();
		print ("The game won't close in the editor");
	}


	public void ShowHideHUD()
	{
		if (m_HUDEnabled) 
		{
			foreach(GameObject HUDElement in m_HUDElements)
			{
				HUDElement.SetActive (false);
			}
			m_HUDEnabled = false;
		} 
		else 
		{
			foreach(GameObject HUDElement in m_HUDElements)
			{
				HUDElement.SetActive (true);
			}
			m_HUDEnabled = true;
		}
	}
}
