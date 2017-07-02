﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameHudManager : MonoBehaviour {

	public static IngameHudManager currentInstance;

	public CanvasGroup ingameHudCg;
	public Image timeRemainingBackground;
	public Text timeRemainingText;

	public Text eventScoreTitle;
	public Text eventScoreText;

	public Text eventSectorTitle;
	public Text eventSectorText;

	public Color baseInterfaceColor;

	public Image sidePanelLarge;
	public Image sidePanelSmall;

	public CanvasGroup objectiveCG;
	public Image objectivePanel;
	public Text objectiveText;

	private bool scoreUpdating = false;
	private float tempScore = 0;

	private float timeDec;
	private float timeSec;
	private int timeMin;

	private Color currentTimeColor;

	private bool timeCountHasMinutes;

	void Awake ()
	{
		currentInstance = this;
	}
	// Use this for initialization
	void Start () {
		sidePanelLarge.color = sidePanelSmall.color = baseInterfaceColor = eventScoreText.color = eventSectorText.color = objectiveText.color = objectivePanel.color = currentTimeColor = baseInterfaceColor;
		SetObjectivePanel ();
		eventScoreText.text = tempScore.ToString();
		timeCountHasMinutes = !StageData.currentData.GetObjectiveIsTypeScore ();
		//TODO: Es la mejor forma?
		if (timeCountHasMinutes) {
			timeRemainingText.fontSize = 60;
		}

	}
	
	// Update is called once per frame
	void Update () {
		UpdateRemainingTime ();
	}
	void UpdateRemainingTime()
	{
		timeSec = StageData.currentData.timeSec;
		timeDec = (timeSec - (int)timeSec) * 100; // TODO: Temporal...
		timeMin = StageData.currentData.timeMin;

		if (timeCountHasMinutes) {
			timeRemainingText.text = timeMin.ToString ("D2") + ":" + ((int)timeSec).ToString ("D2") + ":" + ((int)timeDec).ToString("D2");
		} else {
			if (StageData.currentData.timeSec > 5) {
				timeRemainingText.text = ((int)timeSec).ToString();
				currentTimeColor = Color.Lerp (currentTimeColor, baseInterfaceColor, Time.deltaTime * 2f);
			} else {
				timeRemainingText.text = timeSec.ToString("N2");
				currentTimeColor = Color.Lerp (currentTimeColor, Color.red, Time.deltaTime * 2f);
			}
			timeRemainingText.color = timeRemainingBackground.color = currentTimeColor;
		}

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
	public void SetObjectivePanel()
	{
		objectiveCG.gameObject.SetActive(StageData.currentData.GetEventHasObjectives ());
		objectiveText.text = StageData.currentData.GetObjectiveString ();
	}
	public void UpdateSectorInfo()
	{
		eventSectorText.text = StageData.currentData.checkPointsCrossed.ToString() + "/" + StageData.currentData.GetEventLimitCP ().ToString();
	}
	public void UpdateScoreInfo()
	{
		if (!scoreUpdating)
			StartCoroutine ("UpdateScore");
	}
	void SetElementsVisibility()
	{
		if (StageData.currentData.GetEventHasScore () && !(StageData.currentData.GetEventLimitCP () > 0)) {
			eventScoreTitle.transform.position = eventSectorTitle.transform.position;
			eventScoreText.transform.position = eventSectorText.transform.position;
			sidePanelLarge.gameObject.SetActive (false);
			sidePanelSmall.gameObject.SetActive (true);
			eventSectorTitle.gameObject.SetActive (false);
			eventSectorText.gameObject.SetActive (false);
		} else if (StageData.currentData.gamemode == 0) {
			sidePanelLarge.gameObject.SetActive (false);
			sidePanelSmall.gameObject.SetActive (false);
			eventScoreTitle.gameObject.SetActive (false);
			eventScoreText.gameObject.SetActive (false);
			eventSectorText.gameObject.SetActive (false);
			eventSectorTitle.gameObject.SetActive (false);
			timeRemainingBackground.gameObject.SetActive (false);
			timeRemainingText.gameObject.SetActive(false);
		} else {
			sidePanelLarge.gameObject.SetActive (StageData.currentData.GetEventHasScore());
			sidePanelSmall.gameObject.SetActive (!sidePanelLarge.gameObject.activeInHierarchy);
			eventScoreTitle.gameObject.SetActive (sidePanelLarge.gameObject.activeInHierarchy);
			eventScoreText.gameObject.SetActive (sidePanelLarge.gameObject.activeInHierarchy);
		}
	}
	IEnumerator UpdateScore()
	{
		float animSpeed = 1;
		scoreUpdating = true;
		while (tempScore != StageData.currentData.GetEventScore ()) {
			animSpeed = Mathf.Abs (tempScore - StageData.currentData.GetEventScore ()) + 20f;
			tempScore = Mathf.MoveTowards (tempScore, StageData.currentData.GetEventScore (), Time.deltaTime * animSpeed);
			eventScoreText.text = ((int)tempScore).ToString();
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
}
