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
	public CanvasGroup profileCreationParent;
	public Text profileCreationInfo;
	public InputField InFld;

	public Text loadingText;

	private bool animationFinished = false;
	private bool fadeoutCalled = false;
	private bool pressAnyKeyBlinking = false;
	private bool profileCreationOpen = false;
	private bool profileCreationInputReady = false;
	private int profileCreationPhase = 1;

	private string profileCreationNameSelected = "";

	void Start () {
		StartCoroutine ("FadeInScreen");
	}

	void Update()
	{
		if (animationFinished && !fadeoutCalled && Input.anyKeyDown && !profileCreationOpen) {
			GlobalGameData.currentInstance.InitializeData ();
			if (GlobalGameData.currentInstance.HasAnySavedData ()) {
				StartCoroutine ("FadeOutScreen");
			} else if (!profileCreationOpen) {
				profileCreationOpen = true;
				StopCoroutine ("PressAnyKeyBlinkAnimation");
				StartCoroutine ("ProfileCreationWindowAnimation");
			}

		}
	}
	void SwitchToMainMenu()
	{
		SceneManager.LoadScene ("MainMenu");
	}

	IEnumerator PressAnyKeyBlinkAnimation()
	{
		pressAnyKeyBlinking = true;
		float t = 1;
		float blinkSpeed = 1f;
		float targetAlpha = 1;

		while (pressAnyKeyBlinking) {
			t = Mathf.MoveTowards (t, targetAlpha, Time.deltaTime * blinkSpeed);
			pressAnyKeyParent.alpha = t;
			if (t == targetAlpha) {
				if (targetAlpha == 1)
					targetAlpha = 0;
				else
					targetAlpha = 1;
			}
			yield return null;
		}
	}

	IEnumerator ProfileCreationWindowAnimation()
	{
		profileCreationInputReady = false;
		pressAnyKeyParent.gameObject.SetActive (false);
		profileCreationParent.gameObject.SetActive (true);
		screenFade.gameObject.SetActive (true);
		Vector3 profileWindowInitPos = profileCreationParent.transform.localPosition;
		float animSpeed = 5;
		float t = 0;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			profileCreationParent.alpha = t;
			profileCreationParent.transform.localPosition = profileWindowInitPos + Vector3.left * 40 * (1 - t);
			yield return null;
		}
		profileCreationInputReady = true;
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
		fadeSpeed = 1f;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * fadeSpeed);
			pressAnyKeyParent.alpha = t;
			yield return null;
		}
		StartCoroutine ("PressAnyKeyBlinkAnimation");
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
	IEnumerator NewGameToMainMenu()
	{
		loadingText.text = "Loading";
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
		SwitchToMainMenu ();
	}
	public void SetProfileCreationName()
	{
		profileCreationNameSelected = InFld.text;
	}
	public void OnProfileCreationNextButtonClicked()
	{
		if (profileCreationNameSelected.Length == 0 || !profileCreationInputReady)
			return;

		switch (profileCreationPhase) {
		case 1: // Autosave warning
			{
				StartCoroutine ("ProfileCreationWindowAnimation");
				profileCreationPhase++;
				GlobalGameData.currentInstance.SetPlayerName (profileCreationNameSelected);
				profileCreationInfo.text = "The game will automatically save any progress, closing the game while playing a event will count as a loss.";
				InFld.gameObject.SetActive (false);
				break;
			}
		case 2: // Last message before starting
			{
				StartCoroutine ("ProfileCreationWindowAnimation");
				profileCreationPhase++;
				profileCreationInfo.text = "This is a demo version, some features are disabled.\nSettings and preferences can be edited from the Main Menu, welcome, and have fun.";
				GlobalGameData.currentInstance.SaveData ();
				break;
			}
		case 3: // Switch to loading
			{
				profileCreationPhase++;
				StartCoroutine ("FadeOutScreen");
				break;
			}
		}
	}
}
