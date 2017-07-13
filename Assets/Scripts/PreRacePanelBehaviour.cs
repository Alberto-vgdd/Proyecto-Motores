using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreRacePanelBehaviour : MonoBehaviour {

	public static PreRacePanelBehaviour currentInstance;

	private bool fadeCalled = false;

	public Text event_title;
	public Text event_subName;
	public Text event_description;
	public Text event_limits;
	public Text event_objectives;
	public CanvasGroup panelCG;
	public CanvasGroup fadeCG;

	private float fadeSpeed = 0.5f;
	private bool fadeFinished = false;

	void Awake ()
	{
		currentInstance = this;
	}
	void Start ()
	{
		SetPanelInfo ();
		StartCoroutine ("FadeInScreen");
	}

	void Update () {
		if (Input.anyKeyDown && !fadeCalled && fadeFinished) {
			fadeCalled = true;
			StartCoroutine ("FadeOutPanel");
		}
	}

	public void SetPanelInfo()
	{
		event_subName.text = GlobalGameData.currentInstance.eventActive.GetEventArea() + " - " + DayNightCycle.currentInstance.getTimeString() 
			+ " [ Road ID: " + GlobalGameData.currentInstance.eventActive.GetSeed().ToString() + " ]";
		event_title.text = GlobalGameData.currentInstance.eventActive.GetEventTypeName ();
		event_description.text = GlobalGameData.currentInstance.eventActive.GetEventTypeShortDesc ();
		event_limits.text = "Checkpoints: " + GlobalGameData.currentInstance.eventActive.GetCheckpointsString () + " | Time limit: " + GlobalGameData.currentInstance.eventActive.GetTimeLimitString ();
		event_objectives.text = GlobalGameData.currentInstance.eventActive.GetObjectiveString ();
	}

	IEnumerator FadeInScreen()
	{
		while (fadeCG.alpha > 0) {
			fadeCG.alpha -= Time.deltaTime * fadeSpeed;
			yield return null;
		}
		fadeFinished = true;
	}
	IEnumerator FadeOutPanel()
	{
		while (panelCG.alpha > 0) {
			panelCG.alpha -= Time.deltaTime * fadeSpeed;
			fadeCG.alpha = 1 - panelCG.alpha;
			yield return null;
		}
		StageData.currentData.StartEvent ();
		panelCG.gameObject.SetActive (false);
	}
}
