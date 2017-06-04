using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenCanvasController : MonoBehaviour
{

	public void StartGame()
	{
		SceneManager.LoadScene("game");
	}

	public void ExitGame()
	{
		Application.Quit();
	}

}
