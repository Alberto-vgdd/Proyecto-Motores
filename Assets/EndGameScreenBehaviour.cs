﻿using System.Collections;
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

	public Text endGameScoreBreakdownTitle;
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
		endGameScoreBreakdownTitle.text = " Seaside highway - [ ID: " + RoadGenerator.currentInstance.levelSeed + " ]";
		StartCoroutine ("FadeIn");
			// Switch by event end reason
		// ==============================================================================
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
		// ==============================================================================
		switch (StageData.currentData.gamemode) {
		case 1: // Standard Endurance
			{
				endGameScoreBreakdown.text = "Event score: " + ((int)StageData.currentData.GetEventScore ()).ToString ();
				break;
			}
		case 2: // Drift Endurance
			{
				endGameScoreBreakdown.text = "Event score: " + ((int)StageData.currentData.GetEventScore()).ToString() +
					"\nDamage taken penalty: " + ((int)StageData.currentData.damageTaken).ToString() + " [ x" + StageData.currentData.GetEventDamagePenaltyMultiplier() + " ] = " 
					+ ((int)(StageData.currentData.damageTaken * StageData.currentData.GetEventDamagePenaltyMultiplier())).ToString() +
					"\n\nFinal score: " + ((int)StageData.currentData.GetEventScoreMinusPenalty()).ToString();
				break;
			}
		case 3: // Drift Exhibition
			{
				endGameScoreBreakdown.text = "Event score: " + ((int)StageData.currentData.GetEventScore()).ToString() +
					"\nDamage taken penalty: " + ((int)StageData.currentData.damageTaken).ToString() + " [ x" + StageData.currentData.GetEventDamagePenaltyMultiplier() + " ] = " 
					+ ((int)(StageData.currentData.damageTaken * StageData.currentData.GetEventDamagePenaltyMultiplier())).ToString() +
					"\n\nFinal score: " + ((int)StageData.currentData.GetEventScoreMinusPenalty()).ToString();
				break;
			}
		case 4: // High Speed Challenge
			{
				endGameScoreBreakdown.text = "Event score: " + ((int)StageData.currentData.GetEventScore()).ToString() +
					"\nDamage taken penalty: " + ((int)StageData.currentData.damageTaken).ToString() + " [ x" + StageData.currentData.GetEventDamagePenaltyMultiplier() + " ] = " 
					+ ((int)(StageData.currentData.damageTaken * StageData.currentData.GetEventDamagePenaltyMultiplier())).ToString() +
					"\n\nFinal score: " + ((int)StageData.currentData.GetEventScoreMinusPenalty()).ToString();
				break;
			}
		case 5: // Chain Drift Challenge
			{
				endGameScoreBreakdown.text = "Event score: " + ((int)StageData.currentData.GetEventScore()).ToString() +
					"\nDamage taken penalty: " + ((int)StageData.currentData.damageTaken).ToString() + " [ x" + StageData.currentData.GetEventDamagePenaltyMultiplier() + " ] = " 
					+ ((int)(StageData.currentData.damageTaken * StageData.currentData.GetEventDamagePenaltyMultiplier())).ToString() +
					"\n\nFinal score: " + ((int)StageData.currentData.GetEventScoreMinusPenalty()).ToString();
				break;
			}
		case 6: // Time Attack
			{
				endGameScoreBreakdown.text = "Final time: " + StageData.currentData.GetTimePassedString();
				break;
			}
		default: // Free Roam
			{
				endGameScoreBreakdown.text = "- No score awarded on this event -";
				break;
			}
		}
		StageData.currentData.SetEventScoreMinusPenalty();

		// Switch by result.
		// ==============================================================================
		if (failed) {
			endGameResultTitle.text = "Event failed";
			endGameObjectivesAchieved.text = "- No medals awarded -";
			endGameScoreBreakdown.text = " -- ";
		} else {
			// Switch by medal awarded
			// ==============================================================================
			endGameResultTitle.text = "Event completed";
			switch (StageData.currentData.GetPlayerResult ()) {
			case 1:
				{
					endGameObjectivesAchieved.text = "- GOLD medal awarded -";
					break;
				}
			case 2:
				{
					endGameObjectivesAchieved.text = "- SILVER medal awarded -";
					break;
				}
			case 3:
				{
					endGameObjectivesAchieved.text = "- BRONZE medal awarded -";
					break;
				}
			default:
				{
					endGameObjectivesAchieved.text = "- No medals awarded -";
					break;
				}
			}
		}
		endGameObjectives.text = StageData.currentData.GetObjectiveString ();


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