using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour 
{
	private bool m_GamePaused;
	private bool m_HUDEnabled;
	public GameObject[] m_UIButtons;
	public GameObject[] m_HUDElements;
	public Text m_HUDButton;

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
			GameObject.Find ("HUDOnText").GetComponent<RawImage> ().raycastTarget = false;
			GameObject.Find ("HUDOffText").GetComponent<RawImage> ().raycastTarget = false;
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
		SceneManager.LoadScene("Title Screen");
	}


	public void ShowHideHUD()
	{
		if (m_HUDEnabled) 
		{
			foreach(GameObject HUDElement in m_HUDElements)
			{
				HUDElement.SetActive (false);
			}
			GameObject.Find ("HUDOnText").GetComponent<RawImage> ().enabled = false;
			GameObject.Find ("HUDOffText").GetComponent<RawImage> ().enabled = true;
			m_HUDEnabled = false;
		} 
		else 
		{
			foreach(GameObject HUDElement in m_HUDElements)
			{
				HUDElement.SetActive (true);
			}
			GameObject.Find ("HUDOnText").GetComponent<RawImage> ().enabled = true;
			GameObject.Find ("HUDOffText").GetComponent<RawImage> ().enabled = false;
			m_HUDEnabled = true;
		}
	}
}
