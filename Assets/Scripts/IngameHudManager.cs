﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameHudManager : MonoBehaviour {

	public static IngameHudManager currentInstance;

	public PlayerMovement pm;

	public CanvasGroup ingameHudCg;

	[Header("Time info")]
	public GameObject timeRemainingParent;
	public GameObject timeElapsedParent;
	public Image timeRemainingBorder;
	public Image timeElapsedBorder;
	public Text timeRemainingText;
	public Text timeElapsedText;

	[Header("Score/Sector info")]
	public GameObject ScoreParent;
	public GameObject CpointParent;
	public Text ScoreInfo;
	public Text CpointInfo;

	[Header("Objective info")]
	public GameObject objectivePanelParent;
	public Text objective1;
	public Text objective2;
	public Text objective3;

	[Header("Speed meter")]
	public Image speedMeterFill;
	public Text speedMeterText;


	private bool scoreUpdating = false;
	private float tempScore = 0;
	private Color currentTimeColor = Color.white;
	private float playerSpeedConversion = 8.5f;
	private bool eventFinished = false;

	void Awake ()
	{
		currentInstance = this;
	}
	// Use this for initialization
	void Start () {
		SetObjectivePanel ();
		if (GlobalGameData.currentInstance.eventActive.HasTimelimit ()) {
			StartCoroutine ("UpdateRemainingTime");
		} else {
			StartCoroutine ("UpdateElapsedTime");
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateSpeedMeter ();
	}
	public void SetHudVisibility(bool arg)
	{
		if (arg) {
			StartCoroutine ("FadeInHud");
			SetElementsVisibility ();
		} else {
			StartCoroutine ("FadeOutHud");
		}
	}
	public void EndEvent()
	{
		eventFinished = true;
	}
	public void SetObjectivePanel()
	{
		objectivePanelParent.SetActive(GlobalGameData.currentInstance.eventActive.HasObjectives());
		objective1.text = GlobalGameData.currentInstance.eventActive.GetObjectiveString(1);
		objective2.text = GlobalGameData.currentInstance.eventActive.GetObjectiveString(2);
		objective3.text = GlobalGameData.currentInstance.eventActive.GetObjectiveString(3);
	}
	public void UpdateSpeedMeter()
	{
		speedMeterText.text = ((int)(pm.GetCurrentSpeed () * playerSpeedConversion)).ToString();
		speedMeterFill.fillAmount = Mathf.Clamp(pm.GetCurrentSpeedPercentage () * 0.75f, 0, 0.75f);
	}
	public void UpdateSectorInfo()
	{
		CpointInfo.text = StageData.currentData.GetCheckpointsCrossed().ToString() + "/" + GlobalGameData.currentInstance.eventActive.GetEventCheckpoints().ToString();
	}
	public void UpdateScoreInfo()
	{
		if (!scoreUpdating)
			StartCoroutine ("UpdateScore");
	}
	void SetElementsVisibility()
	{
		// Free roam gamemode.
		if (GlobalGameData.currentInstance.eventActive.GetGamemode () == 0) {
			timeRemainingParent.SetActive (false);
			timeElapsedParent.SetActive (false);
			objectivePanelParent.SetActive (false);
			CpointParent.SetActive (false);
			ScoreParent.SetActive (false);
		} else {
			timeRemainingParent.SetActive (GlobalGameData.currentInstance.eventActive.HasTimelimit ());
			timeElapsedParent.SetActive (!GlobalGameData.currentInstance.eventActive.HasTimelimit ());
		}
		objectivePanelParent.SetActive (GlobalGameData.currentInstance.eventActive.HasObjectives ());
		CpointParent.SetActive (GlobalGameData.currentInstance.eventActive.GetEventCheckpoints () > 0);
		ScoreParent.SetActive (GlobalGameData.currentInstance.eventActive.HasScore ());
		if (GlobalGameData.currentInstance.eventActive.GetEventCheckpoints () <= 0) {
			ScoreParent.transform.localPosition = CpointParent.transform.localPosition;
		}
		UpdateSectorInfo ();
		ScoreInfo.text = tempScore.ToString();
	}
	IEnumerator UpdateScore()
	{
		float animSpeed = 1;
		scoreUpdating = true;
		while (tempScore != StageData.currentData.GetEventScore ()) {
			animSpeed = Mathf.Abs (tempScore - StageData.currentData.GetEventScore ()) + 20f;
			tempScore = Mathf.MoveTowards (tempScore, StageData.currentData.GetEventScore (), Time.deltaTime * animSpeed);
			ScoreInfo.text = ((int)tempScore).ToString();
			yield return null;
		}
		scoreUpdating = false;
	}
	IEnumerator FadeOutHud()
	{
		while (ingameHudCg.alpha > 0) {
			ingameHudCg.alpha = Mathf.MoveTowards (ingameHudCg.alpha, 0, Time.deltaTime*2f);
			yield return null;
		}
	}
	IEnumerator FadeInHud()
	{
		while (ingameHudCg.alpha < 1) {
			ingameHudCg.alpha = Mathf.MoveTowards (ingameHudCg.alpha, 1, Time.deltaTime*0.25f);
			yield return null;
		}
	}
	IEnumerator UpdateRemainingTime()
	{
		float readedTime;
		while (!eventFinished) {
			readedTime = StageData.currentData.time_remainingSec;
			if (readedTime < 5) {
				timeRemainingText.text = readedTime.ToString ("F2");
				currentTimeColor = Color.Lerp (currentTimeColor, Color.red, Time.deltaTime*4);
			} else {
				timeRemainingText.text = ((int)readedTime).ToString ();
				currentTimeColor = Color.Lerp (currentTimeColor, Color.white, Time.deltaTime*4);
			}
			timeRemainingText.color = timeRemainingBorder.color = currentTimeColor;
			yield return null;
		}
	}
	IEnumerator UpdateElapsedTime()
	{
		while (!eventFinished) {
			timeElapsedText.text = StageData.currentData.GetTimePassedString ();
			yield return null;
		}
	}
}
