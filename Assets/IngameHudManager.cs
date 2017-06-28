using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameHudManager : MonoBehaviour {

	public static IngameHudManager currentInstance;

	public CanvasGroup timeRemainingCG;
	public Image timeRemainingBackground;
	public Text timeRemainingText;
	public Text eventScoreText;
	public Text eventSectorText;

	public Color baseInterfaceColor;

	private float timeRemaining;
	private Color currentTimeColor;

	void Awake ()
	{
		currentInstance = this;
	}
	// Use this for initialization
	void Start () {
		currentTimeColor = baseInterfaceColor;
		eventScoreText.color = baseInterfaceColor;
		eventSectorText.color = baseInterfaceColor;

	}
	
	// Update is called once per frame
	void Update () {
		UpdateRemainingTime ();
	}
	void UpdateRemainingTime()
	{
		timeRemaining = StageData.currentData.remainingSec;
		//timeRemainingText.color = timeRemainingBackground.color = Color.Lerp(Color.red, Color.blue, Mathf.Clamp(timeRemaining/6f, 0, 1));
		if (StageData.currentData.remainingSec > 5) {
			timeRemainingText.text = ((int)timeRemaining).ToString();
			currentTimeColor = Color.Lerp (currentTimeColor, baseInterfaceColor, Time.deltaTime * 2f);
		} else {
			timeRemainingText.text = timeRemaining.ToString("N2");
			currentTimeColor = Color.Lerp (currentTimeColor, Color.red, Time.deltaTime * 2f);
		}
		timeRemainingText.color = timeRemainingBackground.color = currentTimeColor;
	}
	public void SetHudVisibility(bool arg)
	{
		if (arg) {
			StartCoroutine ("FadeInHud");
		} else {
			StartCoroutine ("FadeOutHud");
		}
	}
	public void UpdateSectorInfo()
	{
		eventSectorText.text = StageData.currentData.checkPointsCrossed.ToString() + "/" + StageData.currentData.GetEventLimitCP ().ToString();
	}
	IEnumerator FadeOutHud()
	{
		while (timeRemainingCG.alpha > 0) {
			timeRemainingCG.alpha = Mathf.MoveTowards (timeRemainingCG.alpha, 0, Time.deltaTime*2f);
			yield return null;
		}
	}
	IEnumerator FadeInHud()
	{
		while (timeRemainingCG.alpha < 1) {
			timeRemainingCG.alpha = Mathf.MoveTowards (timeRemainingCG.alpha, 1, Time.deltaTime*0.25f);
			yield return null;
		}
	}
}
