using UnityEngine;

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
	private Gamemode m_gameMode;
	private float m_roadDifficulty;
	private bool displayTimeLeftAlwaysFloat = false;
	private string m_customEventName = "";
	private bool m_new;
	// AuxParameters
	private const int BASE_REWARD_VALUE = 500;
	private const float ROAD_DIFFICULTY_MULTIPLIER = 0.075f;
	private const float LEAGUE_DIFFICULTY_MULTIPLIER = 0.0125f;
	private const float SILVER_REWARD_MULTIPLIER = 0.8f;
	private const float BRONZE_REWARD_MULTIPLIER = 0.6f;

	public enum Gamemode
	{
		Endurance = 1,
		DriftEndurance = 2,
		DriftExhibition = 3,
		HighSpeedChallenge = 4,
		ChainDriftChallenge = 5,
		TimeAttack = 6,
		FreeRoam = 7
	}

	public EventData (int seed, int checkpoints, Gamemode gmode, string customName = "")
	{
		m_new = true;

		m_gameMode = gmode;
		m_eventSeed = seed;
		m_checkPoints = checkpoints;
		m_customEventName = customName;
		m_eventLeague = 16;
		m_seasonalEvent = true;
		m_canBeRestarted = true;

		m_rewardType = 0;

		SetEventRules ();
		SetEventObjectives ();

		Random.InitState(m_eventSeed);
		int curveChance = Random.Range (10, 71);
		int minStraight = Random.Range (0, 3);
		int maxStraight = Random.Range (minStraight, 8);

		// Difficulty bonus setting.
		m_roadDifficulty = ((curveChance - 10f) / 70f) * 3f; m_roadDifficulty += 1 - (minStraight / 3f); m_roadDifficulty += 1 - (maxStraight / 8f);




		m_rewardValue = 0;
	}

	public EventData (int league, bool seasonal, bool canBeRestarted)
	{
		m_new = true;

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
		m_roadDifficulty = ((curveChance - 10f) / 70f) * 3f; m_roadDifficulty += 1 - (minStraight / 3f); m_roadDifficulty += 1 - (maxStraight / 8f);

		SetGamemodeBasedOnRoadDifficulty ();
		SetEventRules ();
		SetEventObjectives ();

		if (m_checkPoints <= 0) {
			m_rewardValue = (int)((BASE_REWARD_VALUE * 5) * (1 + (m_roadDifficulty * ROAD_DIFFICULTY_MULTIPLIER) + (league * LEAGUE_DIFFICULTY_MULTIPLIER)));
		} else {
			m_rewardValue = (int)((BASE_REWARD_VALUE * (m_checkPoints*0.35f)) * (1 + (m_roadDifficulty * ROAD_DIFFICULTY_MULTIPLIER) + (league * LEAGUE_DIFFICULTY_MULTIPLIER)));
		}
	}
	void SetGamemodeBasedOnRoadDifficulty()
	{
		int rand;
		if (m_roadDifficulty < 2) {
			// Allows HighSpeedChallenge, TimeAttack
			rand = Random.Range (1, 3);
			switch (rand) {
			case 1:
				{
					m_gameMode = Gamemode.HighSpeedChallenge;
					break;
				}
			case 2:
				{
					m_gameMode = Gamemode.TimeAttack;
					break;
				}
			}
		} else if (m_roadDifficulty < 3) {
			// Allows Endurance, DriftEndurance, ChainDriftChallenge, DriftExhibition, HighSpeedChallenge, TimeAttack
			rand = Random.Range (1, 6);
			switch (rand) {
			case 1:
				{
					m_gameMode = Gamemode.Endurance;
					break;
				}
			case 2:
				{
					m_gameMode = Gamemode.DriftEndurance;
					break;
				}
			case 3:
				{
					m_gameMode = Gamemode.ChainDriftChallenge;
					break;
				}
			case 4:
				{
					m_gameMode = Gamemode.DriftExhibition;
					break;
				}
			case 5:
				{
					m_gameMode = Gamemode.HighSpeedChallenge;
					break;
				}
			case 6:
				{
					m_gameMode = Gamemode.TimeAttack;
					break;
				}
			}
		} else {
			// Allows, DriftEndurance, ChainDriftChallenge, DriftExhibition, TimeAttack
			rand = Random.Range (1, 5);
			switch (rand) {
			case 1:
				{
					m_gameMode = Gamemode.DriftEndurance;
					break;
				}
			case 2:
				{
					m_gameMode = Gamemode.ChainDriftChallenge;
					break;
				}
			case 3:
				{
					m_gameMode = Gamemode.DriftExhibition;
					break;
				}
			case 4:
				{
					m_gameMode = Gamemode.TimeAttack;
					break;
				}
			}
		}
	}
	void SetEventObjectives()
	{
		switch (m_gameMode) {
		case Gamemode.Endurance:
			{
				m_objectiveGold = (int)(10000 * (1 - (m_roadDifficulty * ROAD_DIFFICULTY_MULTIPLIER) + (m_eventLeague * LEAGUE_DIFFICULTY_MULTIPLIER)));
				break;
			}
		case Gamemode.DriftEndurance:
			{
				m_objectiveGold = (int)(6750 * (1 - (m_roadDifficulty * ROAD_DIFFICULTY_MULTIPLIER) + (m_eventLeague * LEAGUE_DIFFICULTY_MULTIPLIER)));
				break;
			}
		case Gamemode.DriftExhibition:
			{
				m_objectiveGold = (int)(m_checkPoints * 1250 * (1 + (m_eventLeague * LEAGUE_DIFFICULTY_MULTIPLIER)));
				break;
			}
		case Gamemode.HighSpeedChallenge:
			{
				m_objectiveGold = (int)(m_checkPoints * 250 * (1 - (m_roadDifficulty * ROAD_DIFFICULTY_MULTIPLIER) + (m_eventLeague * LEAGUE_DIFFICULTY_MULTIPLIER)));
				break;
			}
		case Gamemode.ChainDriftChallenge:
			{
				m_objectiveGold = (int)(m_checkPoints * 215 * (1 - (m_roadDifficulty * ROAD_DIFFICULTY_MULTIPLIER) + (m_eventLeague * LEAGUE_DIFFICULTY_MULTIPLIER)));
				break;
			}
		case Gamemode.TimeAttack:
			{
				m_objectiveGold = (int)(m_checkPoints * 15.5f * (1 + (m_roadDifficulty * ROAD_DIFFICULTY_MULTIPLIER) - (m_eventLeague * LEAGUE_DIFFICULTY_MULTIPLIER)));
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
			m_objectiveSilver = (int)(m_objectiveGold * 0.9f);
			m_objectiveBronze = (int)(m_objectiveGold * 0.8f);
		} else {
			m_objectiveSilver = (int)(m_objectiveGold / 0.9f);
			m_objectiveBronze = (int)(m_objectiveGold / 0.8f);
		}
	}
	void SetEventRules()
	{
		switch (m_gameMode) {
		case Gamemode.Endurance:
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
				m_eventBonusTimeOnDriftMultiplier = 0.001f;
				m_eventScoreOnDriftMultiplier = 0f;
				m_eventScoreOnDistance = 20f;
				m_eventScoreOnCheckpoint = 100f;
				m_eventScoreOnCleanSection = 500f;
				m_eventScorePerTimeRemainingOnCpMultiplier = 0f;
				m_eventScoreDamagePenaltyMultiplier = 0f;
				break;
			}
		case Gamemode.DriftEndurance:
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
				m_eventBonusTimeOnDriftMultiplier = 0.004f;
				m_eventScoreOnDriftMultiplier = 0f;
				m_eventScoreOnDistance = 20f;
				m_eventScoreOnCheckpoint = 100f;
				m_eventScoreOnCleanSection = 500f;
				m_eventScorePerTimeRemainingOnCpMultiplier = 0f;
				m_eventScoreDamagePenaltyMultiplier = 20f;
				break;
			}
		case Gamemode.DriftExhibition:
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
				m_eventScoreOnDriftMultiplier = 0.35f;
				m_eventScoreOnDistance = 0f;
				m_eventScoreOnCheckpoint = 0f;
				m_eventScoreOnCleanSection = 500f;
				m_eventScorePerTimeRemainingOnCpMultiplier = 0f;
				m_eventScoreDamagePenaltyMultiplier = 100f;
				break;
			}
		case Gamemode.HighSpeedChallenge:
			{
				m_initialTimeRemaining = 10f;
				m_eventDamageTakenMultiplier = 3f;

				m_eventHasTimeLimit = true;
				m_eventHasScore = true;
				m_eventCanBeFailed = false;
				m_hasObjectives = true;
				m_objectiveTypeScore = true;
				displayTimeLeftAlwaysFloat = true;

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
		case Gamemode.ChainDriftChallenge:
			{
				m_initialTimeRemaining = 8f;
				m_eventDamageTakenMultiplier = 1f;

				m_eventHasTimeLimit = true;
				m_eventHasScore = true;
				m_eventCanBeFailed = false;
				m_hasObjectives = true;
				m_objectiveTypeScore = true;
				displayTimeLeftAlwaysFloat = true;

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
		case Gamemode.TimeAttack:
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
	public Gamemode GetGamemode() 
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
	public int GetEventLeague()
	{
		return m_eventLeague;
	}
	public bool HasTimeDisplayedAlwaysAsFloat()
	{
		return displayTimeLeftAlwaysFloat;
	}
	public void SetAsViewed()
	{
		m_new = false;
	}
	public bool IsNew()
	{
		return m_new;
	}

	// String getters
	// ==================================================================================================================

	public string GetEventArea() { return "Seaside Highway"; }
	public string GetEventTypeShortDesc()
	{
		string str = "";  // Innecesario, pero evita los warnings del compilador.
		switch (m_gameMode) {
		case Gamemode.Endurance:
			{
				str = "Drive as far as you can within the time limit, gain bonus time by drifting and reaching checkpoints. Points are awarded based on the distance travelled.";
				break;
			}
		case Gamemode.DriftEndurance:
			{
				str = "Drive as far as you can within the time limit, gain bonus time ONLY by drifting. Points are awarded based on the distance travelled.";
				break;
			}
		case Gamemode.DriftExhibition:
			{
				str = "Drift to earn points before reaching the last checkpoint, longer drifts have a bonus score multiplier.";
				break;
			}
		case Gamemode.HighSpeedChallenge:
			{
				str = "Reach the last checkpoint within the time limit, while driving at high speed, timer is slowed down. Score is awarded based on the remaining time when crossing a checkpoint.";
				break;
			}
		case Gamemode.ChainDriftChallenge:
			{
				str = "Reach the last checkpoint within the time limit, while drifting the timer is slowed down. Score is awarded based on the remaining time when crossing a checkpoint.";
				break;
			}
		case Gamemode.TimeAttack:
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
		case Gamemode.Endurance:
			{
				str = "Endurance";
				break;
			}
		case Gamemode.DriftEndurance:
			{
				str = "Drift Endurance";
				break;
			}
		case Gamemode.DriftExhibition:
			{
				str = "Drift Exhibition";
				break;
			}
		case Gamemode.HighSpeedChallenge:
			{
				str = "High Speed Challenge";
				break;
			}
		case Gamemode.ChainDriftChallenge:
			{
				str = "Chain Drift Challenge";
				break;
			}
		case Gamemode.TimeAttack:
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
	public int GetRewardValueForPosition(int pos = 0)
	{
		float _reward;
		switch (pos) {
		case 1:
			{
				_reward = m_rewardValue;
				break;
			}
		case 2:
			{
				_reward = m_rewardValue * SILVER_REWARD_MULTIPLIER;
				break;
			}
		case 3:
			{
				_reward = m_rewardValue * BRONZE_REWARD_MULTIPLIER;
				break;
			}
		default:
			{
				_reward = 0;
				break;
			}
		}
		return (int)_reward;
	}
	public string GetRewardString(int pos = 1)
	{
		string txt;
		switch (pos) {
		case 1:
			{
				if (m_rewardType == 1) {
					txt = m_rewardValue.ToString () + " Cr.";
				} else {
					txt = "Special car part.";
				}
				break;
			}
		case 2:
			{
				txt = ((int)(m_rewardValue*SILVER_REWARD_MULTIPLIER)).ToString () + " Cr.";
				break;
			}
		case 3:
			{
				txt = ((int)(m_rewardValue*BRONZE_REWARD_MULTIPLIER)).ToString () + " Cr.";
				break;
			}
		default:
			{
				txt = " -- ";
				break;
			}
		}
		if (m_seasonalEvent)
			txt = "--";
		return txt;
	}
	public string GetObjectiveString(int pos)
	{
		string txt;
		if (m_gameMode == 0) {
			txt = "- No objectives -";
		} else if (m_objectiveTypeScore) {
			txt = GetObjectiveForPosition (pos).ToString();
		} else {
			txt = ((int)(GetObjectiveForPosition (pos) / 60)).ToString () + ":" + ((int)(GetObjectiveForPosition (pos) % 60)).ToString ("D2") + ":00";
		}
		return txt;
	}
	public string GetEventName()
	{
		if (m_customEventName == "") {
			return GetEventTypeName ();
		} else {
			return m_customEventName;
		}
	}
}
