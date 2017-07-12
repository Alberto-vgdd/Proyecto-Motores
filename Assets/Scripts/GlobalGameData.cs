using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameData : MonoBehaviour {

	public static GlobalGameData currentInstance;

	public int m_lastEventPlayedResult = -1;

	private int m_playerCurrency;
	private int m_playerCurrencyAlternative;
	private int m_playerRank;
	private int m_playerRank_old;
	private float m_playerRankStatus;
	private float m_playerRankStatus_old;

	public List<EventData> eventsAvailable;

	public EventData eventActive;

	void Awake ()
	{
		if (currentInstance == null) {
			DontDestroyOnLoad (transform.gameObject);
			currentInstance = this;
			InitializeData ();
		}
		else {
			Destroy (this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	void InitializeData()
	{
		m_playerCurrency = 0;
		m_playerCurrencyAlternative = 0;
		m_playerRank = m_playerRank_old = 1;
		m_playerRankStatus = m_playerRankStatus_old = 0;
		GenerateEventsAvailable ();
	}
	void GenerateEventsAvailable()
	{
		Random.InitState(System.Environment.TickCount);
		eventsAvailable = new List<EventData> ();
		for (int i = 0; i < 8; i++) {
			eventsAvailable.Add (new EventData (m_playerRank, false, false));
		}
	}
	public void UpdateRankStatus()
	{
		if (m_lastEventPlayedResult < 0)
			return;
		float promotionMultiplier = Mathf.Pow(0.8f, m_playerRank-1);
		if (m_lastEventPlayedResult == 0) {
			m_playerRankStatus -= 0.15f;
		} else if (m_lastEventPlayedResult == 1) {
			m_playerRankStatus += 0.15f * promotionMultiplier;
		} else if (m_lastEventPlayedResult == 2) {
			m_playerRankStatus += 0.05f * promotionMultiplier;
		} else if (m_lastEventPlayedResult == 3) {
			m_playerRankStatus -= 0.05f;
		}

		if (m_playerRankStatus < -1) {
			if (m_playerRank == 1) {
				m_playerRankStatus = -1;
			} else {
				m_playerRank--;
				GenerateEventsAvailable ();
			}
		} else if (m_playerRankStatus > 1) {
			m_playerRankStatus = 0;
			m_playerRank++;
			GenerateEventsAvailable();
		}
	}
	public void UpdateOldRankData()
	{
		m_playerRank_old = m_playerRank;
		m_playerRankStatus_old = m_playerRankStatus;
	}
	public int GetPlayerRank()
	{
		return m_playerRank;
	}
	public int GetPlayerRankOld()
	{
		return m_playerRank_old;
	}
	public float GetPlayerRankStatus()
	{
		return m_playerRankStatus;
	}
	public float GetPlayerRankStatusOld()
	{
		return m_playerRankStatus_old;
	}
	public int GetPlayerCurrency()
	{
		return m_playerCurrency;
	}
	public int GetPlayerAlternativeCurrency()
	{
		return m_playerCurrencyAlternative;
	}

}
