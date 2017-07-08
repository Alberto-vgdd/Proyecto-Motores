using UnityEngine;

[System.Serializable]
public class EventData {
	private int m_eventSeed;
	private float m_roadDifficulty;

	private int m_eventRewardCurrency;
	private int m_eventRewardOtherID;
	private int m_eventLeague;
	private int m_eventType;

	private bool m_seasonalEvent;
	private bool m_canBeRestarted;

	public EventData (int league, bool seasonal, bool canBeRestarted)
	{
		m_eventSeed = Random.Range (1, 99999999);
		m_eventType = Random.Range (1, 7);
		m_eventLeague = league;
		m_seasonalEvent = seasonal;
		m_canBeRestarted = canBeRestarted;
	}

	public int GetSeed()
	{
		return m_eventSeed;
	}
	public float GetRoadDifficulty()
	{
		return m_roadDifficulty;
	}
	public int GetEventType()
	{
		return m_eventType;
	}
	public int GetRewardCurrency()
	{
		return m_eventRewardCurrency;
	}
}
