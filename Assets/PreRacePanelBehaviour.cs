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
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown && !fadeCalled && fadeFinished) {
			fadeCalled = true;
			StartCoroutine ("FadeOutPanel");
		}
	}

	public void SetPanelInfo()
	{
		event_subName.text = GlobalGameData.currentInstance.selectedEvent.GetEventArea() + " - " + DayNightCycle.currentInstance.getTimeString() 
			+ " [ Road ID: " + GlobalGameData.currentInstance.selectedEvent.GetSeed().ToString() + " ]";
		SetObjectivePanel ();
		event_title.text = GlobalGameData.currentInstance.selectedEvent.GetEventTypeName ();
		event_description.text = GlobalGameData.currentInstance.selectedEvent.GetEventTypeShortDesc ();
		switch (GlobalGameData.currentInstance.selectedEvent.GetGamemode()) {
		case 1: // Standard Endurance
			{
				event_title.text = "ENDURANCE";
				event_limits.text = " Checkpoints: -- | Time limit: " + ((int)StageData.currentData.time_remainingSec).ToString();
				break;
			}
		case 2: // Drift Endurance
			{
				event_title.text = "DRIFT ENDURANCE";
				event_limits.text = " Checkpoints: -- | Time limit: " + ((int)StageData.currentData.time_remainingSec).ToString();
				break;
			}
		case 3: // Drift Exhibition
			{
				event_title.text = "DRIFT EXHIBITION";
				event_limits.text = " Checkpoints: " + GlobalGameData.currentInstance.selectedEvent.GetEventCheckpoints() + " | Time limit: " + ((int)GlobalGameData.currentInstance.selectedEvent.GetInitialTimeRemaining()).ToString();
				break;
			}
		case 4: // High Speed Challenge
			{
				event_title.text = "HIGH SPEED CHALLENGE";
				event_limits.text = " Checkpoints: " + GlobalGameData.currentInstance.selectedEvent.GetEventCheckpoints() + " | Time limit: " + ((int)GlobalGameData.currentInstance.selectedEvent.GetInitialTimeRemaining()).ToString();
				break;
			}
		case 5: // Chain Drift Challenge
			{
				event_title.text = "CHAIN DRIFT CHALLENGE";
				event_limits.text = " Checkpoints: " + GlobalGameData.currentInstance.selectedEvent.GetEventCheckpoints() + " | Time limit: " + ((int)GlobalGameData.currentInstance.selectedEvent.GetInitialTimeRemaining()).ToString();
				break;
			}
		case 6: // Time Attack
			{
				event_title.text = "TIME ATTACK";
				event_limits.text = " Checkpoints: " + GlobalGameData.currentInstance.selectedEvent.GetEventCheckpoints() + " | Time limit: --";
				break;
			}
		default: // Free Roam
			{
				event_title.text = "FREE ROAM";
				event_limits.text = " Checkpoints: -- | Time limit: --";
				event_objectives.text = "- No objectives -";
				break;
			}
		}
	}
	public void SetObjectivePanel()
	{
		event_objectives.text = StageData.currentData.GetObjectiveString ();
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
