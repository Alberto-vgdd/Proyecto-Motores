using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreRacePanelBehaviour : MonoBehaviour {

	public static PreRacePanelBehaviour currentInstance;

	public Text event_title;
	public Text event_subName;
	public Text event_description;
	public Text event_limits;
	public Text event_objective1;
	public Text event_objective2;
	public Text event_objective3;
	public Text event_reward1;
	public Text event_reward2;
	public Text event_reward3;
	public CanvasGroup panelCG;
	public CanvasGroup fadeCG;
	public CanvasGroup PressAnyKeyCG;
	public List<CanvasGroup> panelsWithFadeInAnimation;

	private float fadeSpeed = 0.5f;
	private bool animationsFinished = false;
	private bool fadeCalled = false;

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
		if (Input.anyKeyDown && animationsFinished && !fadeCalled) {
			fadeCalled = true;
			StartCoroutine ("FadeOutPanel");
		}
	}

	public void SetPanelInfo()
	{
		
		event_subName.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetEventArea() + " - " + DayNightCycle.currentInstance.getTimeString() 
			+ " [ Road ID: " + GlobalGameData.currentInstance.m_playerData_eventActive.GetSeed().ToString() + " ]";
		event_title.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetEventName();
		event_description.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetEventTypeShortDesc ();
		event_limits.text = "Checkpoints: " + GlobalGameData.currentInstance.m_playerData_eventActive.GetCheckpointsString () + " | Time limit: " + GlobalGameData.currentInstance.m_playerData_eventActive.GetTimeLimitString ();
		event_objective1.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetObjectiveString (1);
		event_objective2.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetObjectiveString (2);
		event_objective3.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetObjectiveString (3);
		event_reward1.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString (1);
		event_reward2.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString (2);
		event_reward3.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString (3);
		StartCoroutine ("SubPanelAnimation");
	}

	IEnumerator FadeInScreen()
	{
		panelCG.gameObject.SetActive (true);
		while (fadeCG.alpha > 0) {
			fadeCG.alpha -= Time.deltaTime * fadeSpeed;
			yield return null;
		}
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
	IEnumerator SubPanelAnimation()
	{
		yield return new WaitForSeconds (1.5f);
		float t = 0;
		float animSpeed = 5f;

		List<Vector3> basePositions = new List<Vector3>();
		for (int i = 0; i < panelsWithFadeInAnimation.Count; i++)
		{
			basePositions.Add (panelsWithFadeInAnimation [i].transform.localPosition);
		}

		while (t < panelsWithFadeInAnimation.Count+2) {
			for (int i = 0; i < panelsWithFadeInAnimation.Count; i++)
			{
				panelsWithFadeInAnimation [i].transform.localPosition = basePositions[i] + Vector3.left * ((1-Mathf.Clamp01(t-i)) * 40);
				panelsWithFadeInAnimation [i].alpha = Mathf.Clamp01(t-i);
			}
			t += Time.deltaTime * animSpeed;
			yield return null;
		}
		yield return new WaitForSeconds (0.75f);
		while (PressAnyKeyCG.alpha < 1) {
			PressAnyKeyCG.alpha = Mathf.MoveTowards (PressAnyKeyCG.alpha, 1, Time.deltaTime);
			yield return null;
		}
		animationsFinished = true;
		StartCoroutine ("PressAnyKeyBlinkAnimation");
	}
	IEnumerator PressAnyKeyBlinkAnimation()
	{
		float t = 1;
		bool increase = false;
		while (!fadeCalled) {
			if (increase) {
				t += Time.deltaTime;
				if (t > 1)
					increase = false;
			} else {
				t -= Time.deltaTime;
				if (t < 0)
					increase = true;
			}
			PressAnyKeyCG.alpha = t;
			yield return null;
		}
	}
}
