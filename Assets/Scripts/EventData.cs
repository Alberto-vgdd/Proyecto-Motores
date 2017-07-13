﻿using UnityEngine;

[System.Serializable]
public class EventData {
	
	private int m_eventSeed;

	// Objectives
	private int m_objectiveGold = 0;
	private int m_objectiveSilver = 0;
	private int m_objectiveBronze = 0;
	// Event Rules
	private bool m_objectiveTypeScore = false;
	private bool m_hasObjectives = false;
	private bool m_eventHasTimeLimit = false;
	private bool m_eventHasScore = false;
	private bool m_eventCanBeFailed = false;
	private bool m_seasonalEvent;
	private bool m_canBeRestarted;
	// Event parameters
	private int m_checkPoints;
	private float m_initialTimeRemaining = 0;
	private float m_eventBonusTimeOnCPMultiplier = 0;
	private float m_eventBonusTimeOnDriftMultiplier = 0;
	private float m_eventDamageTakenMultiplier = 1;
	private float m_eventScoreOnDriftMultiplier = 0;
	private float m_eventScoreOnDistance = 0;
	private float m_eventScoreOnCheckpoint = 0;
	private float m_eventScoreOnCleanSection = 0;
	private float m_eventScorePerTimeRemainingOnCpMultiplier = 0;
	private float m_eventScoreDamagePenaltyMultiplier = 0f;
	// Other parameters
	private int m_rewardValue;
	private int m_rewardType;
	private int m_eventLeague;
	private int m_gameMode;
	private float m_roadDifficulty;
	private float m_leagueExtraDifficulty;
	private float m_combinedDifficultyFactor;

	public EventData (int league, bool seasonal, bool canBeRestarted)
	{
		m_gameMode = Random.Range (1, 7);
		m_eventSeed = Random.Range (1, 99999999);
		m_checkPoints = Random.Range (4, 10) + (int)(league/2);
		m_eventLeague = league;
		m_seasonalEvent = seasonal;
		m_canBeRestarted = canBeRestarted;

		m_rewardType = 1;

		Random.InitState(m_eventSeed);
		int curveChance = Random.Range (10, 71);
		int minStraight = Random.Range (0, 3);
		int maxStraight = Random.Range (minStraight, 8);

		// Difficulty bonus setting.
		m_roadDifficulty = ((curveChance - 10f) / 70f) * 3f;
		m_roadDifficulty += 1 - (minStraight / 3f);
		m_roadDifficulty += 1 - (maxStraight / 8f);
		m_leagueExtraDifficulty = 1 + (league-1) * 0.05f;
		m_combinedDifficultyFactor = m_leagueExtraDifficulty + m_roadDifficulty *0.2f + m_checkPoints *0.1f;
		m_rewardValue = (int)(m_combinedDifficultyFactor*500);

		SetEventRules ();
		SetEventObjectives ();
	}
	void SetEventObjectives()
	{
		switch (m_gameMode) {
		case 1: // Standard endurance
			{
				m_objectiveGold = 10000;
				break;
			}
		case 2: // Drift Endurance
			{
				m_objectiveGold = 7000;
				break;
			}
		case 3: // Drift Exhibition
			{
				m_objectiveGold = 1750 * m_checkPoints;
				break;
			}
		case 4: // High speed challenge
			{
				m_objectiveGold = 450 * m_checkPoints;
				break;
			}
		case 5: // Drift challenge
			{
				m_objectiveGold = 400 * m_checkPoints;
				break;
			}
		case 6: // Time attack
			{
				m_objectiveGold = (int)(16.5f * m_checkPoints);
				break;
			}
		default: // FreeRoam
			{
				m_objectiveGold = m_objectiveSilver = m_objectiveBronze = 0;
				break;
			}
		}
			
		// Scaling
		if (m_objectiveTypeScore) {
			m_objectiveGold = (int)(m_objectiveGold * m_leagueExtraDifficulty);
			m_objectiveSilver = (int)(m_objectiveGold * 0.9f);
			m_objectiveBronze = (int)(m_objectiveGold * 0.8f);
		} else {
			m_objectiveGold = (int)(m_objectiveGold / m_leagueExtraDifficulty);
			m_objectiveSilver = (int)(m_objectiveGold / 0.9f);
			m_objectiveBronze = (int)(m_objectiveGold / 0.8f);
		}
	}
	void SetEventRules()
	{
		switch (m_gameMode) {
		case 1: // Standard Endurance
			{
				m_initialTimeRemaining = 25f;
				m_eventDamageTakenMultiplier = 1.5f;

				m_eventHasTimeLimit = true;
				m_eventHasScore = true;
				m_eventCanBeFailed = false;
				m_hasObjectives = true;
				m_objectiveTypeScore = true;

				m_checkPoints = 0;
				m_eventBonusTimeOnCPMultiplier = 1f;
				m_eventBonusTimeOnDriftMultiplier = 0.005f;
				m_eventScoreOnDriftMultiplier = 0f;
				m_eventScoreOnDistance = 20f;
				m_eventScoreOnCheckpoint = 100f;
				m_eventScoreOnCleanSection = 500f;
				m_eventScorePerTimeRemainingOnCpMultiplier = 0f;
				m_eventScoreDamagePenaltyMultiplier = 0f;
				break;
			}
		case 2: // Drift Endurance
			{
				m_initialTimeRemaining = 25f;
				m_eventDamageTakenMultiplier = 1.5f;

				m_eventHasTimeLimit = true;
				m_eventHasScore = true;
				m_eventCanBeFailed = false;
				m_hasObjectives = true;
				m_objectiveTypeScore = true;

				m_checkPoints = 0;
				m_eventBonusTimeOnCPMultiplier = 0f;
				m_eventBonusTimeOnDriftMultiplier = 0.01f;
				m_eventScoreOnDriftMultiplier = 0f;
				m_eventScoreOnDistance = 20f;
				m_eventScoreOnCheckpoint = 100f;
				m_eventScoreOnCleanSection = 500f;
				m_eventScorePerTimeRemainingOnCpMultiplier = 0f;
				m_eventScoreDamagePenaltyMultiplier = 20f;
				break;
			}
		case 3: // Drift Exhibition
			{
				m_initialTimeRemaining = 45f;
				m_eventDamageTakenMultiplier = 1f;

				m_eventHasTimeLimit = true;
				m_eventHasScore = true;
				m_eventCanBeFailed = false;
				m_hasObjectives = true;
				m_objectiveTypeScore = true;

				m_eventBonusTimeOnCPMultiplier = 1.8f;
				m_eventBonusTimeOnDriftMultiplier = 0f;
				m_eventScoreOnDriftMultiplier = 0.5f;
				m_eventScoreOnDistance = 0f;
				m_eventScoreOnCheckpoint = 0f;
				m_eventScoreOnCleanSection = 500f;
				m_eventScorePerTimeRemainingOnCpMultiplier = 0f;
				m_eventScoreDamagePenaltyMultiplier = 100f;
				break;
			}
		case 4: // High Speed Challenge
			{
				m_initialTimeRemaining = 10f;
				m_eventDamageTakenMultiplier = 3f;

				m_eventHasTimeLimit = true;
				m_eventHasScore = true;
				m_eventCanBeFailed = false;
				m_hasObjectives = true;
				m_objectiveTypeScore = true;

				m_eventBonusTimeOnCPMultiplier = 0.25f;
				m_eventBonusTimeOnDriftMultiplier = 0f;
				m_eventScoreOnDriftMultiplier = 0f;
				m_eventScoreOnDistance = 0f;
				m_eventScoreOnCheckpoint = 0f;
				m_eventScoreOnCleanSection = 0f;
				m_eventScorePerTimeRemainingOnCpMultiplier = 65f;
				m_eventScoreDamagePenaltyMultiplier = 20f;
				break;
			}
		case 5: // Chain Drift Challenge
			{
				m_initialTimeRemaining = 8f;
				m_eventDamageTakenMultiplier = 1f;

				m_eventHasTimeLimit = true;
				m_eventHasScore = true;
				m_eventCanBeFailed = false;
				m_hasObjectives = true;
				m_objectiveTypeScore = true;

				m_eventBonusTimeOnCPMultiplier = 0.2f;
				m_eventBonusTimeOnDriftMultiplier = 0f;
				m_eventScoreOnDriftMultiplier = 0f;
				m_eventScoreOnDistance = 0f;
				m_eventScoreOnCheckpoint = 0f;
				m_eventScoreOnCleanSection = 0f;
				m_eventScorePerTimeRemainingOnCpMultiplier = 65f;
				m_eventScoreDamagePenaltyMultiplier = 20f;
				break;
			}
		case 6: // Time Attack
			{
				m_initialTimeRemaining = 0f;
				m_eventDamageTakenMultiplier = 1f;

				m_eventHasTimeLimit = false;
				m_eventHasScore = false;
				m_eventCanBeFailed = true;
				m_hasObjectives = true;
				m_objectiveTypeScore = false;

				m_eventBonusTimeOnCPMultiplier = 0f;
				m_eventBonusTimeOnDriftMultiplier = 0f;
				m_eventScoreOnDriftMultiplier = 0f;
				m_eventScoreOnDistance = 0f;
				m_eventScoreOnCheckpoint = 0f;
				m_eventScoreOnCleanSection = 0f;
				m_eventScoreDamagePenaltyMultiplier = 0f;
				break;
			}
		default: // Free Roam
			{
				m_initialTimeRemaining = 0f;
				m_eventDamageTakenMultiplier = 1f;

				m_eventHasTimeLimit = false;
				m_eventHasScore = false;
				m_eventCanBeFailed = false;
				m_hasObjectives = false;
				m_objectiveTypeScore = false;

				m_checkPoints = 0;
				m_eventBonusTimeOnCPMultiplier = 0f;
				m_eventBonusTimeOnDriftMultiplier = 0f;
				m_eventScoreOnDriftMultiplier = 0f;
				m_eventScoreOnDistance = 0f;
				m_eventScoreOnCheckpoint = 0f;
				m_eventScoreOnCleanSection = 0f;
				m_eventScoreDamagePenaltyMultiplier = 0f;
				break;
			}
		}
	}


	// Getters
	// =======================================================================================================

	public float GetObjectiveForPosition(int position)
	{
		if (position == 1)
			return m_objectiveGold;
		else if (position == 2)
			return m_objectiveSilver;
		else
			return m_objectiveBronze;
	}
	public bool IsObjectiveTypeScore()
	{
		return m_objectiveTypeScore;
	}
	public bool HasObjectives()
	{
		return m_hasObjectives;
	}
	public bool HasTimelimit()
	{
		return m_eventHasTimeLimit;
	}
	public bool HasScore()
	{
		return m_eventHasScore;
	}
	public bool CanBeFailed()
	{
		return m_eventCanBeFailed;
	}
	public bool IsSeasonalEvent()
	{
		return m_seasonalEvent;
	}
	public bool CanBeRestarted()
	{
		return m_canBeRestarted;
	}
	public int GetSeed() 
	{
		return m_eventSeed;
	}
	public int GetGamemode() 
	{ 
		return m_gameMode; 
	}
	public int GetEventCheckpoints() 
	{ 
		return m_checkPoints;
	}
	public float GetBonusTimeOnCheckpointMultiplier()
	{
		return m_eventBonusTimeOnCPMultiplier;
	}
	public float GetBonusTimeOnDriftMultiplier()
	{
		return m_eventBonusTimeOnDriftMultiplier;
	}
	public float GetDamageTakenMultiplier()
	{
		return m_eventDamageTakenMultiplier;
	}
	public float GetScoreOnDriftMultiplier()
	{
		return m_eventScoreOnDriftMultiplier;
	}
	public float GetScoreOnDistance()
	{
		return m_eventScoreOnDistance;
	}
	public float GetScoreOnCheckpoint()
	{
		return m_eventScoreOnCheckpoint;
	}
	public float GetScoreOnCleanSection()
	{
		return m_eventScoreOnCleanSection;
	}
	public float GetScorePerRemainingTimeOnCheckpointMultiplier()
	{
		return m_eventScorePerTimeRemainingOnCpMultiplier;
	}
	public float GetScoreDamagePenaltyMultiplier()
	{
		return m_eventScoreDamagePenaltyMultiplier;
	}
	public float GetInitialTimeRemaining()
	{
		return m_initialTimeRemaining;
	}
	public int GetRewardValue() 
	{ 
		return m_rewardValue; 
	}
	public int GetRewardType()
	{
		return m_rewardType;
	}
	public float GetRoadDifficulty() 
	{ 
		return m_roadDifficulty; 
	}
	public float GetLeagueDifficulty()
	{
		return m_leagueExtraDifficulty;
	}
	public float GetCombinedDifficulty()
	{
		return m_combinedDifficultyFactor;
	}
	public int GetEventLeague()
	{
		return m_eventLeague;
	}

	// String getters
	// ==================================================================================================================

	public string GetEventArea() { return "Seaside Highway"; }
	public string GetEventTypeShortDesc()
	{
		string str = "";  // Innecesario, pero evita los warnings del compilador.
		switch (m_gameMode) {
		case 1: // Standard Endurance
			{
				str = "Drive as far as you can within the time limit, gain bonus time by drifting and reaching checkpoints.";
				break;
			}
		case 2: // Drift Endurance
			{
				str = "Drive as far as you can within the time limit, gain bonus time ONLY by drifting.";
				break;
			}
		case 3: // Drift Exhibition
			{
				str = "Drift to earn points before reaching the last checkpoint, longer drifts have a bonus score multiplier.";
				break;
			}
		case 4: // High Speed Challenge
			{
				str = "Reach the last checkpoint within the time limit, while driving at high speed, timer is slowed down. Score is awarded based on the remaining time when crossing a checkpoint.";
				break;
			}
		case 5: // Chain Drift Challenge
			{
				str = "Reach the last checkpoint within the time limit, while drifting the timer is slowed down. Score is awarded based on the remaining time when crossing a checkpoint.";
				break;
			}
		case 6: // Time Attack
			{
				str = "Reach the last checkpoint as fast as you can.";
				break;
			}
		default: // Free Roam
			{
				str = "Practice while enjoying the landscape!";
				break;
			}
		}
		return str;
	}
	public string GetEventTypeName()
	{
		string str = ""; // Innecesario pero evita warnings del compilador
		switch (m_gameMode) {
		case 1: // Endurance
			{
				str = "Endurance";
				break;
			}
		case 2: // Drift Endurance
			{
				str = "Drift Endurance";
				break;
			}
		case 3: // Drift Exhibition
			{
				str = "Drift Exhibition";
				break;
			}
		case 4: // High Speed Challenge
			{
				str = "High Speed Challenge";
				break;
			}
		case 5: // Chain Drift Challenge
			{
				str = "Chain Drift Challenge";
				break;
			}
		case 6: // Time Attack
			{
				str = "Time attack";
				break;
			}
		default:
			{
				str = "Free Roam";
				break;
			}
		}
		return str;
	}
	public string GetCheckpointsString()
	{
		if (m_checkPoints > 0) {
			return m_checkPoints.ToString ();
		} else {
			return "--";
		}
	}
	public string GetTimeLimitString()
	{
		if (m_eventHasTimeLimit) {
			return ((int)m_initialTimeRemaining).ToString ();
		} else {
			return "--";
		}
	}
	public string GetRewardString()
	{
		if (m_rewardType == 1) {
			return m_rewardValue.ToString () + " Cr.";
		} else {
			return "Special car part.";
		}
	}
	public string GetObjectiveString()
	{
		string txt = "";
		if (m_gameMode == 0) {
			txt = "- No objectives -";
		}
		else if (m_objectiveTypeScore) {
			txt = 
				"1ST: " + GetObjectiveForPosition(1) +
				"\n2ND: " + GetObjectiveForPosition(2) +
				"\n3RD: " + GetObjectiveForPosition(3);
		} else {
			txt = 
				"1ST: " + ((int)(GetObjectiveForPosition(1) / 60)).ToString () 
				+ ":" + ((int)(GetObjectiveForPosition(1) % 60)).ToString("D2") + ":00" +
				"\n2ND: " + ((int)(GetObjectiveForPosition(2) / 60)).ToString () 
				+ ":" + ((int)(GetObjectiveForPosition(2) % 60)).ToString("D2") + ":00" +
				"\n3RD: " + ((int)(GetObjectiveForPosition(3) / 60)).ToString () 
				+ ":" + ((int)(GetObjectiveForPosition(3) % 60)).ToString("D2") + ":00";
		}
		return txt;
	}
}
