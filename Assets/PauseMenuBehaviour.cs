using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuBehaviour : MonoBehaviour {

	public GameObject menuParent;
	public CanvasGroup windowCG;

	private Vector3 windowInitialPos;
	private bool gamePaused;
	private bool animationInProgress;
	private bool isOpen;

	// Use this for initialization
	void Start () {
		windowInitialPos = windowCG.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (StageData.currentData.IsEventInProgress () && !ConfirmationPanelBehaviour.currentInstance.IsOpen()) {
				if (isOpen) {
					CloseMenu ();
				} else {
					OpenMenu ();
				}
			}
		}
	}

	void OpenMenu()
	{
		Time.timeScale = 0;
		windowCG.alpha = 0;
		menuParent.SetActive (true);
		isOpen = true;
		StartCoroutine ("FadeInAnimation");
	}
	void CloseMenu()
	{
		if (animationInProgress)
			return;	
		Time.timeScale = 1;
		menuParent.SetActive (false);
		isOpen = false;
	}
	public void OnContinueClicked()
	{
		if (animationInProgress)
			return;	
		CloseMenu ();
	}
	public void OnRestartClicked()
	{
		if (animationInProgress)
			return;	
		ConfirmationPanelBehaviour.currentInstance.OpenMenu (2);
	}
	public void OnAbandonClicked()
	{
		if (animationInProgress)
			return;	
		ConfirmationPanelBehaviour.currentInstance.OpenMenu (3);
	}

	IEnumerator FadeInAnimation()
	{
		animationInProgress = true;
		float t = 0;
		float animSpeed = 7.5f;
		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.unscaledDeltaTime * animSpeed);
			windowCG.alpha = t;
			windowCG.transform.localPosition = windowInitialPos + Vector3.left * 40 * (1 - t);
			yield return null;
		}
		animationInProgress = false;
	}
}
