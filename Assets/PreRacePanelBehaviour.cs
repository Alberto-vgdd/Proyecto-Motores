﻿using System.Collections;
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
		event_subName.text = "Seaside highway - " + DayNightCycle.currentInstance.getTimeString() + " [ Road ID: " + RoadGenerator.currentInstance.levelSeed.ToString() + " ]";
		event_objectives.text = "GOLD: ?????" +
			"\nSILVER: ?????" +
			"\nBRONZE: ?????";
		switch (gamemode) {
		case 1: // Standard Endurance
			{
				event_title.text = "ENDURANCE";
				event_limits.text = " Checkpoints: -- | Time limit: " + ((int)StageData.currentData.remainingSec).ToString();
				event_description.text = "Drive as far as you can within the time limit, gain bonus time by drifting and reaching checkpoints.";
				break;
			}
		case 2: // Drift Endurance
			{
				event_title.text = "DRIFT ENDURANCE";
				event_limits.text = " Checkpoints: -- | Time limit: " + ((int)StageData.currentData.remainingSec).ToString();
				event_description.text = "Drive as far as you can within the time limit, gain bonus time ONLY by drifting.";
				break;
			}
		case 3: // Drift Exhibition
			{
				event_title.text = "DRIFT EXHIBITION";
				event_limits.text = " Checkpoints: " + StageData.currentData.GetEventLimitCP() + " | Time limit: " + ((int)StageData.currentData.remainingSec).ToString();
				event_description.text = "Drift to earn points before reaching the last checkpoint, longer drifts have a bonus score multiplier.";
				break;
			}
		case 4: // High Speed Challenge
			{
				event_title.text = "HIGH SPEED CHALLENGE";
				event_limits.text = " Checkpoints: " + StageData.currentData.GetEventLimitCP() + " | Time limit: " + ((int)StageData.currentData.remainingSec).ToString();
				event_description.text = "Reach the last checkpoint within the time limit, time is short, reach high speeds to freeze the timer.";
				break;
			}
		case 5: // Chain Drift Challenge
			{
				event_title.text = "CHAIN DRIFT CHALLENGE";
				event_limits.text = " Checkpoints: " + StageData.currentData.GetEventLimitCP() + " | Time limit: " + ((int)StageData.currentData.remainingSec).ToString();
				event_description.text = "Reach the last checkpoint within the time limit, time is short, drift to freeze the timer.";
				break;
			}
		case 6: // Time Attack
			{
				event_title.text = "TIME ATTACK";
				event_limits.text = " Checkpoints: " + StageData.currentData.GetEventLimitCP() + " | Time limit: " + ((int)StageData.currentData.remainingSec).ToString();
				event_description.text = "Reach the last checkpoint as fast as you can.";
				break;
			}
		default: // Free Roam
			{
				event_title.text = "FREE ROAM";
				event_limits.text = " Checkpoints: -- | Time limit: --";
				event_description.text = "Practice while enjoying the landscape!.";
				event_objectives.text = "- No objectives -";
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
