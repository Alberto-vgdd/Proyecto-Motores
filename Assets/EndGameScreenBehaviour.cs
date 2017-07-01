using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreenBehaviour : MonoBehaviour {

	public static EndGameScreenBehaviour currentInstance;

	public CanvasGroup GlobalCG;
	public CanvasGroup CG1;
	public CanvasGroup CG2;
	public CanvasGroup CG3;
	public CanvasGroup CG4;

	public Text endGameResultTitle;
	public Text endGameResultSubtitle;

	public Text endGameScoreBreakdown;
	public Text endGameObjectives;
	public Text endGameObjectivesAchieved;

	void Awake ()
	{
		currentInstance = this;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void SetAndEnable(int type, bool failed)
	{
		StartCoroutine ("FadeIn");
		// Switch by result.
		if (failed) {
			endGameResultTitle.text = "Event failed";
			endGameObjectivesAchieved.text = "- No medals awarded -";
		} else {
			endGameResultTitle.text = "Event completed";
		}
		switch (type) {
		case 1: // Destroyed
			{
				endGameResultSubtitle.text = "Your car got destroyed.";
				break;
			}
		case 2: // Time Up
			{
				endGameResultSubtitle.text = "Time up";
				break;
			}
		case 3: // Last Checkpoint Reached
			{
				endGameResultSubtitle.text = "All checkpoints reached.";
				break;
			}
		default: // Other?
			{
				endGameResultSubtitle.text = "Event was finished.";
				break;
			}
		}
		// Switch by gamemode.
		switch (StageData.currentData.gamemode) {
		case 1: // Standard Endurance
			{
				endGameScoreBreakdown.text =
				"- Distance traversed: " + StageData.currentData.checkPointsCrossed + " [x" + StageData.currentData.GetScoringDistance() + "] = " + StageData.currentData.checkPointsCrossed * StageData.currentData.GetScoringDistance() +
				"\n- Checkpoints reached: " + StageData.currentData.checkPointsCrossed + " [x" + StageData.currentData.GetScoringCheckPoint() + "] = " + StageData.currentData.checkPointsCrossed * StageData.currentData.GetScoringCheckPoint() +
				"\n- Clean sections: " + StageData.currentData.cleanSections + " [x " + StageData.currentData.GetScoringCleanSection() + "] = " + StageData.currentData.cleanSections * StageData.currentData.GetScoringCleanSection() +
				"\n- Damage penalty: " + StageData.currentData.damageTaken + " [x " + StageData.currentData.GetScoringDamagePenalty() + "] = -" + StageData.currentData.damageTaken * StageData.currentData.GetScoringDamagePenalty();
				break;
			}
		case 2: // Drift Endurance
			{
				
				break;
			}
		case 3: // Drift Exhibition
			{
				
				break;
			}
		case 4: // High Speed Challenge
			{
				
				break;
			}
		case 5: // Chain Drift Challenge
			{
				
				break;
			}
		case 6: // Time Attack
			{
				
				break;
			}
		default: // Free Roam
			{
				
				break;
			}
		}
	}
	IEnumerator FadeIn()
	{
		while (GlobalCG.alpha < 1)
		{
			GlobalCG.alpha = Mathf.MoveTowards (GlobalCG.alpha, 1, Time.deltaTime);
			yield return null;
		}
	}
}
