using UnityEngine;

[System.Serializable]
public class EventData {
	private int m_eventSeed;

	private int m_eventRewardCurrency;
	private int m_eventRewardOtherID;
	private int m_eventLeague;
	private int m_eventType;
	private int m_checkPoints;
	private float m_roadDifficulty;

	private bool m_seasonalEvent;
	private bool m_canBeRestarted;

	public EventData (int league, bool seasonal, bool canBeRestarted)
	{
		m_eventType = Random.Range (1, 7);
		m_eventSeed = Random.Range (1, 99999999);
		m_eventLeague = league;
		m_seasonalEvent = seasonal;
		m_canBeRestarted = canBeRestarted;

		Random.InitState(m_eventSeed);
		int curveChance = Random.Range (10, 71);
		int minStraight = Random.Range (0, 3);
		int maxStraight = Random.Range (minStraight, 8);
		m_roadDifficulty = ((curveChance - 10f) / 70f) * 3f;
		m_roadDifficulty += 1 - (minStraight / 3f);
		m_roadDifficulty += 1 - (maxStraight / 8f);
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
		return m_eventRewardCurrency;
	}
	public float GetRoadDifficulty()
	{
		return m_roadDifficulty;
	}
}
