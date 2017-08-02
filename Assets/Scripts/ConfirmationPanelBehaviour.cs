using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmationPanelBehaviour : MonoBehaviour {

	public static ConfirmationPanelBehaviour currentInstance;

	public GameObject parent;
	public CanvasGroup backgroundCG;
	public CanvasGroup menuCG;
	public CanvasGroup loadingCG;

	public Text loadingInfo;
	public Text HeaderInfo;
	public Text SubHeaderInfo;

	private bool menuOpen = false;
	private bool loading = false;
	private Vector3 menuInitialPos;
	private int menuType;
	// 1 = Return to main menu.
	// 2 = Restart event.
	// 3 = Abandon event.


	void Awake () {
		currentInstance = this;
	}
	void Start () {
		menuInitialPos = menuCG.transform.localPosition;
	}

	IEnumerator OpenMenuAnimation()
	{
		parent.gameObject.SetActive (true);
		menuOpen = true;
		float t = 0;
		while (t < 1) {
			t = Mathf.MoveTowards(t, 1, Time.unscaledDeltaTime * 10f);
			menuCG.transform.localPosition = menuInitialPos + Vector3.left * 40 * (1-t);
			menuCG.alpha = backgroundCG.alpha = t;
			yield return null;
		}
	}
	IEnumerator CloseMenuAnimation()
	{
		float t = 1;
		while (t > 0) {
			t = Mathf.MoveTowards(t, 0, Time.unscaledDeltaTime * 20f);
			backgroundCG.alpha = menuCG.alpha = t;
			yield return null;
		}
		menuOpen = false;
		parent.gameObject.SetActive (false);
	}
	IEnumerator LoadingScreen()
	{
		loading = true;
		loadingCG.gameObject.SetActive (true);
		loadingInfo.text = "LOADING";
		while (loadingCG.alpha < 1) {
			loadingCG.alpha = Mathf.MoveTowards (loadingCG.alpha, 1, Time.deltaTime * 5);
			yield return null;
		}
		AsyncOperation AO;
		if (menuType != 2) { // Not restarting, back to menu
			AO = SceneManager.LoadSceneAsync ("MainMenu");
		} else { // Restarting current scene
			AO = SceneManager.LoadSceneAsync (SceneManager.GetActiveScene().name);
		}
		while (!AO.isDone) {
			loadingInfo.text = "LOADING (" + ((int)(AO.progress * 100)) + "%)";
			yield return null;
		}
		loading = false;
	}
	public void OpenMenu(int mtype)
	{
		if (menuOpen)
			return;
		menuType = mtype;
		switch (mtype) {
		case 1: // Return to main menu
			{
				HeaderInfo.text = "Return to main menu?";
				SubHeaderInfo.text = "Current event result and reward will be saved.";
				break;
			}
		case 2:
			{
				HeaderInfo.text = "Restart event?";
				SubHeaderInfo.text = "Current result will be lost.";
				break;
			}
		case 3:
			{
				HeaderInfo.text = "Abandon event?";
				SubHeaderInfo.text = "This event will be counted as lost.";
				break;
			}
		}
		menuOpen = true;
		StartCoroutine ("OpenMenuAnimation");
	}
	public void OnConfirmClick()
	{
		if (loading)
			return;
		switch (menuType) {
		case 1: // Return to main menu
			{
				StartCoroutine ("LoadingScreen");
				break;
			}
		case 2: // Restart event
			{
				StartCoroutine ("LoadingScreen");
				break;
			}
		case 3: // Abandon event
			{
				StartCoroutine ("LoadingScreen");
				break;
			}
		}
	}
	public void OnCancelClick()
	{
		if (loading)
			return;
		StartCoroutine ("CloseMenuAnimation");
	}
	public bool IsOpen()
	{
		return menuOpen;
	}

}
