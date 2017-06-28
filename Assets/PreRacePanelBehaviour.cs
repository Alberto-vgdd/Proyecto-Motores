using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreRacePanelBehaviour : MonoBehaviour {

	public static PreRacePanelBehaviour currentInstance;

	private bool fadeCalled = false;

	public Text event_title;
	public Text event_description;
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
		StartCoroutine ("FadeInScreen");
	}
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown && !fadeCalled && fadeFinished) {
			fadeCalled = true;
			StartCoroutine ("FadeOutPanel");
		}
	}

	public void SetPanelInfo(int gamemode)
	{
		switch (gamemode) {
		case 1: // Standard Endurance
			{
				event_title.text = "ENDURANCE";
				event_description.text = "Drive as far as you can within the time limit." +
				"\n- Score is awarded based on the distance traversed." +
				"\n- Bonus time awarded with long chain drifts." +
				"\n- Bonus time awarded on checkpoints." +
				"\n- Health restored on clean sections." +
				"\n- All the time awarded will be reduced over time.";
				event_objectives.text = "GOLD: ?????" +
					"\nSILVER: ?????" +
					"\nBRONZE: ?????";
				break;
			}
		case 2: // Drift Endurance
			{
				event_title.text = "DRIFT ENDURANCE";
				event_description.text = "Drive as far as you can within the time limit." +
					"\n- Score is awarded based on the distance traversed." +
					"\n- Bonus time awarded ONLY on chain drifts." +
					"\n- All the time awarded will be reduced over time.";
				event_objectives.text = "GOLD: ?????" +
					"\nSILVER: ?????" +
					"\nBRONZE: ?????";
				break;
			}
		case 3: // Drift Exhibition
			{
				event_title.text = "DRIFT EXHIBITION";
				event_description.text = "Score as much drift points as you can within the time limit." +
					"\n- Score is awarded based on the drift lenght." +
					"\n- Long drifts have a score bonus multiplier." +
					"\n- Bonus time awarded on checkpoints." +
					"\n- Health restored on clean sections." +
					"\n- After 10 checkpoints the event will finish.";
				event_objectives.text = "GOLD: ?????" +
					"\nSILVER: ?????" +
					"\nBRONZE: ?????";
				break;
			}
		case 4: // High Speed Challenge
			{
				event_title.text = "HIGH SPEED CHALLENGE";
				event_description.text = "Drive as far as you can within the time limit." +
					"\n- Score is awarded based on the distance traversed." +
					"\n- A small bonus time is awarded on checkpoints." +
					"\n- While close to the top speed, time wont countdown." +
					"\n- All the time awarded will be reduced over time." +
					"\n- Collision damage increased by 300%.";
				event_objectives.text = "GOLD: ?????" +
					"\nSILVER: ?????" +
					"\nBRONZE: ?????";
				break;
			}
		case 5: // Chain Drift Challenge
			{
				event_title.text = "CHAIN DRIFT CHALLENGE";
				event_description.text = "Drive as far as you can within the time limit." +
					"\n- Score is awarded based on the distance traversed." +
					"\n- A tiny amount of bonus time is awarded on checkpoints." +
					"\n- While drifting, time wont countdown." +
					"\n- All the time awarded will be reduced over time.";
				event_objectives.text = "GOLD: ?????" +
					"\nSILVER: ?????" +
					"\nBRONZE: ?????";
				break;
			}
		case 6: // Time Attack
			{
				event_title.text = "TIME ATTACK";
				event_description.text = "Reach the last checkpoint as fast as you can." +
					"\n- Health restored on clean sections.";
				event_objectives.text = "GOLD: ?????" +
					"\nSILVER: ?????" +
					"\nBRONZE: ?????";
				break;
			}
		default: // Free Roam
			{
				event_title.text = "FREE ROAM";
				event_description.text = "Practice while enjoying the landscape!.";
				event_objectives.text = "GOLD: ?????" +
					"\nSILVER: ?????" +
					"\nBRONZE: ?????";
				break;
			}
		}
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
