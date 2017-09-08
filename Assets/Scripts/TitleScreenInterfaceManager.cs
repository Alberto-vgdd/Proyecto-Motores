using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenInterfaceManager : MonoBehaviour {

	public CanvasGroup screenFade;
	public CanvasGroup logoParent;
	public CanvasGroup titleParent;
	public CanvasGroup pressAnyKeyParent;
	public CanvasGroup loadingParent;
	public Text loadingText;

	private bool animationFinished = false;
	private bool fadeoutCalled = false;

	// Use this for initialization
	void Start () {
		StartCoroutine ("FadeInScreen");
	}
	void Update()
	{
		if (animationFinished && !fadeoutCalled && Input.anyKeyDown) {
			GlobalGameData.currentInstance.InitializeData ();
			StartCoroutine ("FadeOutScreen");
		}
	}
	void SwitchToMainMenu()
	{
		SceneManager.LoadScene ("MainMenu");
	}

	IEnumerator FadeInScreen()
	{
		titleParent.alpha = 0;
		pressAnyKeyParent.alpha = 0;
		logoParent.alpha = 0;

		yield return new WaitForSeconds (0.5f);

		// Fade In Logo

		float t = 0;
		float fadeSpeed = 0.5f;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * fadeSpeed);
			logoParent.alpha = t;
			yield return null;
		}

		yield return new WaitForSeconds (1f);

		// Fade In Screen

		t = 1;
		fadeSpeed = 0.5f;

		yield return new WaitForSeconds (1f);

		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime * fadeSpeed);
			screenFade.alpha = t;
			logoParent.alpha = t;
			yield return null;
		}

		// Fade In Title
		yield return new WaitForSeconds (0.75f);

		t = 0;
		fadeSpeed = 0.75f;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * fadeSpeed);
			titleParent.alpha = t;
			yield return null;
		}

		// Fade In PressAnyKey Text

		t = 0;
		fadeSpeed = 0.75f;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * fadeSpeed);
			pressAnyKeyParent.alpha = t;
			yield return null;
		}
		animationFinished = true;
	}
	IEnumerator FadeOutScreen()
	{
		fadeoutCalled = true;
		float t = 0;
		float animSpeed = 1f;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			screenFade.alpha = t;
			yield return null;
		}
		yield return new WaitForSeconds (1f);
		if (GlobalGameData.currentInstance.LoadData ()) {
			StartCoroutine ("LoadDataAndMainMenu");
		} else {
			StartCoroutine ("CreateDataAndLoadMainMenu");
		}

	}
	IEnumerator CreateDataAndLoadMainMenu()
	{
		loadingText.text = "No saved data found\nPress any key to create a new profile.";
		float t = 0;
		float animSpeed = 5f;
		Vector3 baseTextPosition = loadingParent.transform.localPosition;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.left * (1-t) * 40f;
			yield return null;
		}

		while(!Input.anyKeyDown)
		{
			yield return null;
		}

		t = 1;
		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.right * (1-t) * 40f;
			yield return null;
		}

		loadingText.text = "Saving new data";
		t = 0;
		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.left * (1-t) * 40f;
			yield return null;
		}
		yield return new WaitForSeconds (1.5f);
		t = 1;
		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.right * (1-t) * 40f;
			yield return null;
		}

		GlobalGameData.currentInstance.SaveData ();
		loadingText.text = "Data successfully saved, starting new game";
		t = 0;
		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.left * (1-t) * 40f;
			yield return null;
		}

		yield return new WaitForSeconds (2f);
		t = 1;
		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.right * (1-t) * 40f;
			yield return null;
		}
		SwitchToMainMenu ();
	}
	IEnumerator LoadDataAndMainMenu()
	{
		loadingText.text = "Loading data";
		float t = 0;
		float animSpeed = 5f;
		Vector3 baseTextPosition = loadingParent.transform.localPosition;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.left * (1-t) * 40f;
			yield return null;
		}

		yield return new WaitForSeconds (1f);

		t = 1;
		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.right * (1-t) * 40f;
			yield return null;
		}

		loadingText.text = "Load completed";
		t = 0;
		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.left * (1-t) * 40f;
			yield return null;
		}
		yield return new WaitForSeconds (1f);
		t = 1;
		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime * animSpeed);
			loadingParent.alpha = t;
			loadingParent.transform.localPosition = baseTextPosition + Vector3.right * (1-t) * 40f;
			yield return null;
		}
		SwitchToMainMenu ();
	}
}
