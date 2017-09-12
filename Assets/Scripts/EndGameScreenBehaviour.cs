using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameScreenBehaviour : MonoBehaviour {

	public static EndGameScreenBehaviour currentInstance;

	[Header("EndGame Notice")]
	public CanvasGroup endGameNoticePanel;
	public Text endGameNoticeInfo;

	[Header("EndGame Panel")]
	public CanvasGroup endGameBreakdownCG;
	public List<CanvasGroup> panelsWithFadeInAnimation;
	public CanvasGroup highlightFlash;
	public Text headerInfo;
	public Text subHeaderInfo;
	public Text objective1;
	public Text objective2;
	public Text objective3;
	public Text reward1;
	public Text reward2;
	public Text reward3;
	public Text playerResult;
	public Text playerReward;
	public Text awardInfo;
	public CanvasGroup continueButton;
	public CanvasGroup restartButton;

	private bool panelEnabled = false;
	private bool animationsFinished = false;
	private bool leavingScene = false;

	void Awake ()
	{
		currentInstance = this;
	}
	public void SetAndEnable(int type, bool failed)
	{
		if (panelEnabled)
			return;
		
		panelEnabled = true;
		endGameNoticePanel.gameObject.SetActive (true);
		endGameNoticePanel.alpha = 0;
			// Switch by event end reason
		// ==============================================================================
		switch (type) {
		case 1: // Destroyed
			{
				endGameNoticeInfo.text = "Car destroyed";
				break;
			}
		case 2: // Time Up
			{
				endGameNoticeInfo.text = "Time up";
				break;
			}
		case 3: // Last Checkpoint Reached
			{
				endGameNoticeInfo.text = "Event completed";
				break;
			}
		default: // Other?
			{
				endGameNoticeInfo.text = "Event completed";
				break;
			}
		}

		if (failed) {
			headerInfo.text = "Event failed";
		} else {
			headerInfo.text = "Event completed";
		}
		subHeaderInfo.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetEventName() + " | " + GlobalGameData.currentInstance.m_playerData_eventActive.GetEventArea () +
		" | " + "[ Road ID: " + GlobalGameData.currentInstance.m_playerData_eventActive.GetSeed ().ToString () + " ]";
		objective1.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetObjectiveString (1);
		objective2.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetObjectiveString (2);
		objective3.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetObjectiveString (3);
		reward1.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString(1);
		reward2.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString(2);
		reward3.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString(3);


		if (failed) {
			playerReward.text = "No reward";
			awardInfo.text = "No medal awarded";
			highlightFlash.gameObject.SetActive (false);
			playerResult.text = " -- ";
		} else {
			playerResult.text = StageData.currentData.GetPlayerResultString ();
			switch (StageData.currentData.GetPlayerResult())
			{
			case 1:
				{
					playerReward.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString (1);
					awardInfo.text = "Gold medal awarded";
					highlightFlash.transform.localPosition = panelsWithFadeInAnimation [3].transform.localPosition;
					break;
				}
			case 2:
				{
					playerReward.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString (2);
					awardInfo.text = "Silver medal awarded";
					highlightFlash.transform.localPosition = panelsWithFadeInAnimation [4].transform.localPosition;
					break;
				}
			case 3:
				{
					playerReward.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString (3);
					highlightFlash.transform.localPosition = panelsWithFadeInAnimation [5].transform.localPosition;
					awardInfo.text = "Bronze medal awarded";
					break;
				}
			default:
				{
					playerReward.text = "No reward";
					awardInfo.text = "No medal awarded";
					highlightFlash.gameObject.SetActive (false);
					break;
				}
			}
		}


		StartCoroutine ("EndGameNotice");
	}
	IEnumerator EndGameNotice()
	{
		Vector3 initialPos = endGameNoticePanel.gameObject.transform.localPosition;
		float t = 0;
		float animSpeed = 3.5f;
		yield return new WaitForSeconds (0.5f);
		while (t < 1) {
			endGameNoticePanel.transform.localPosition = initialPos + (Vector3.left * (1 - t) * 50);
			endGameNoticePanel.alpha = t;
			t = Mathf.MoveTowards(t, 1, Time.deltaTime * animSpeed);
			yield return null;
		}
		yield return new WaitForSeconds (2f);
		while (endGameNoticePanel.alpha > 0) {
			endGameNoticePanel.alpha -= Time.deltaTime * animSpeed;
			yield return null;
		}
		StartCoroutine ("EndGameDetails");
	}
	IEnumerator EndGameDetails()
	{
		float t = 0;
		float animSpeed = 5f;

		endGameBreakdownCG.gameObject.SetActive (true);
		endGameBreakdownCG.alpha = 0;
		while (endGameBreakdownCG.alpha < 1) {
			endGameBreakdownCG.alpha = Mathf.MoveTowards (endGameBreakdownCG.alpha, 1, Time.deltaTime*2f);
		}
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
		yield return new WaitForSeconds (0.5f);
		continueButton.GetComponent<Button> ().interactable = true;
		restartButton.GetComponent<Button> ().interactable = GlobalGameData.currentInstance.m_playerData_eventActive.CanBeRestarted();
		t = 0;
		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime*2f);
			continueButton.alpha = restartButton.alpha = t;
			yield return null;
		}
		animationsFinished = true;
		if (StageData.currentData.GetPlayerResult () != 0) {
			StartCoroutine ("HighlightFlashAnimation");
		}
	}
	IEnumerator HighlightFlashAnimation()
	{
		float t = 0;
		bool increase = true;

		while (!leavingScene) {
			if (increase) {
				t += Time.deltaTime;
				if (t > 1)
					increase = false;
			} else {
				t -= Time.deltaTime;
				if (t < 0)
					increase = true;
			}
			highlightFlash.alpha = t*0.5f;
			yield return null;
		}
	}
	public void ContinueButtonPressed()
	{
		if (!animationsFinished || ConfirmationPanelBehaviour.currentInstance.IsOpen())
			return;
		ConfirmationPanelBehaviour.currentInstance.OpenMenu (1);

		MainMenuSoundManager.instance.playAcceptSound ();

	}
	public void RestartButtonPressed()
	{
		if (!animationsFinished || ConfirmationPanelBehaviour.currentInstance.IsOpen())
			return;
		ConfirmationPanelBehaviour.currentInstance.OpenMenu (2);

		MainMenuSoundManager.instance.playAcceptSound ();

	}
}
