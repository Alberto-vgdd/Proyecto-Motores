using UnityEngine;

[System.Serializable]
public class EventData {
	private int m_eventSeed;

	private int m_rewardValue;
	private int m_rewardType;
	private int m_eventLeague;
	private int m_eventType;
	private int m_checkPoints;
	private float m_roadDifficulty;
	private float m_leagueExtraDifficulty;
	private float m_combinedDifficultyFactor;

	private bool m_seasonalEvent;
	private bool m_canBeRestarted;

	public EventData (int league, bool seasonal, bool canBeRestarted)
	{
		m_eventType = Random.Range (1, 7);
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
		m_leagueExtraDifficulty = 1 + (league-1) * 0.1f;
		m_combinedDifficultyFactor = m_leagueExtraDifficulty + m_roadDifficulty *0.2f + m_checkPoints *0.1f;
		m_rewardValue = (int)(m_combinedDifficultyFactor*500);
	}
	public string GetEventArea()
	{
		return "Seaside Highway";
	}
	public string GetEventTypeShortDesc()
	{
		switch (m_eventType) {
		case 1: // Standard Endurance
			{
				return "Drive as far as you can within the time limit, gain bonus time by drifting and reaching checkpoints.";
				break;
			}
		case 2: // Drift Endurance
			{
				return "Drive as far as you can within the time limit, gain bonus time ONLY by drifting.";
				break;
			}
		case 3: // Drift Exhibition
			{
				return "Drift to earn points before reaching the last checkpoint, longer drifts have a bonus score multiplier.";
				break;
			}
		case 4: // High Speed Challenge
			{
				return "Reach the last checkpoint within the time limit, time is short, reach high speeds to freeze the timer.";
				break;
			}
		case 5: // Chain Drift Challenge
			{
				return "Reach the last checkpoint within the time limit, time is short, drift to freeze the timer.";
				break;
			}
		case 6: // Time Attack
			{
				return "Reach the last checkpoint as fast as you can.";
				break;
			}
		default: // Free Roam
			{
				return "Practice while enjoying the landscape!.";
				break;
			}
		}
	}
	public string GetEventTypeName()
	{
		switch (m_eventType) {
		case 1: // Endurance
			{
				return "Endurance";
				break;
			}
		case 2: // Drift Endurance
			{
				return "Drift Endurance";
				break;
			}
		case 3: // Drift Exhibition
			{
				return "Drift Exhibition";
				break;
			}
		case 4: // High Speed Challenge
			{
				return "High Speed Challenge";
				break;
			}
		case 5: // Chain Drift Challenge
			{
				return "Chain Drift Challenge";
				break;
			}
		case 6: // Time Attack
			{
				return "Time attack";
				break;
			}
		default:
			{
				return "Free Roam";
				break;
			}
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
	public int GetSeed()
	{
		return m_eventSeed;
	}
	public int GetEventType()
	{
		return m_eventType;
	}
	public int GetEventCheckpoints()
	{
		return m_checkPoints;
	}
	public int GetRewardCurrency()
	{
		return m_rewardValue;
	}
	public float GetRoadDifficulty()
	{
		return m_roadDifficulty;
	}
}
