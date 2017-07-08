using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameData : MonoBehaviour {

	public static GlobalGameData currentInstance;

	private int m_playerCurrency;
	private int m_playerCurrencyAlternative;
	private int m_playerRank;
	private float m_playerRankStatus;

	public List<EventData> eventsAvailable;

	public EventData selectedEvent;

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
		m_playerRank = 1;
		m_playerRankStatus = 0;
		GenerateEventsAvailable ();
	}
	void GenerateEventsAvailable()
	{
		Random.InitState(System.Environment.TickCount);
		if (eventsAvailable.Count == 0) {
			eventsAvailable = new List<EventData> ();
			for (int i = 0; i < 8; i++) {
				eventsAvailable.Add (new EventData (m_playerRank, false, false));
			}
		} else {
			//TODO: Cambiar solo 3-4 de ellos.
		}
	}

	public int GetPlayerRank()
	{
		return m_playerRank;
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
