using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameData : MonoBehaviour {

	public static GlobalGameData currentInstance;

	private int m_lastEventPlayedResult = -1;

	private int m_playerCurrency;
	private int m_playerCurrencyAlternative;
	private int m_playerRank;
	private float m_playerRankStatus;

	public bool testing_WelcomeMessagesShown = false; 

	public List<EventData> eventsAvailable_offline;
	public List<EventData> eventsAvailable_seasonal;
	public List<CarData> carsOwned;
	private int carSelectedIndex = 0;

	private GhostReplayData playerGhostPB;

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
		m_playerRank = 1;
		m_playerRankStatus = 0;
		GenerateEventsAvailable ();
		GetSeasonalEvents ();
		carsOwned = new List<CarData> ();
		carsOwned.Add (new CarData (0));
		carsOwned.Add (new CarData (1));
		carsOwned.Add (new CarData (2));
		carsOwned.Add (new CarData (3));
		carSelectedIndex = -1;

	}
	void GenerateEventsAvailable()
	{
		Random.InitState(System.Environment.TickCount);
		eventsAvailable_offline = new List<EventData> ();
		for (int i = 0; i < 8; i++) {
			eventsAvailable_offline.Add (new EventData (m_playerRank, false, true));
		}
	}
	void GetSeasonalEvents()
	{
		// Demomento ponemos eventos fijados ya que no tiene de donde scarlos.
		eventsAvailable_seasonal = new List<EventData>();
		eventsAvailable_seasonal.Add(new EventData(765, 5, 6, "Custom name LUL"));
		eventsAvailable_seasonal.Add(new EventData(111111, 4, 6, "More testing"));
	}
	public float GetRankChangeOnNextUpdate()
	{
		if (m_lastEventPlayedResult < 0)
			return 0;
		float promotionMultiplier = Mathf.Pow(0.8f, m_playerRank-1);
		if (m_lastEventPlayedResult == 1) {
			return 0.15f * promotionMultiplier;
		} else if (m_lastEventPlayedResult == 2) {
			return 0.05f * promotionMultiplier;
		} else if (m_lastEventPlayedResult == 3) {
			return -0.075f;
		} else {
			return -0.15f;
		} 
	}
	public void UpdatePostEventChanges()
	{
		UpdateRewardStatus ();
		UpdateRankStatus ();
		m_lastEventPlayedResult = -1;
		eventActive = null;
	}
	private void UpdateRewardStatus()
	{
		if (m_lastEventPlayedResult < 0 || eventActive == null)
			return;
		m_playerCurrency += eventActive.GetRewardValueForPosition(m_lastEventPlayedResult);
		//TODO: algo para el caso en el que no sea dinero...
	}
	private void UpdateRankStatus()
	{
		if (m_lastEventPlayedResult < 0 || eventActive == null)
			return;
		//float promotionMultiplier = Mathf.Pow(0.8f, m_playerRank-1);
		float promotionMultiplier = 99f;
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
			GenerateEventsAvailable ();
		}
	}
	public void SetPlayerGhostPB(GhostReplayData ghost)
	{
		if (ghost.GetRecordedAtSeed () != eventActive.GetSeed () || playerGhostPB == null) {
			print ("[REPLAY] No comparable ghost found, setting as PB");
			playerGhostPB = ghost;
		} else {
			if (ghost.GetScoreRecordedIsTime ()) {
				if (ghost.GetScoreRecorded () < playerGhostPB.GetScoreRecorded ()) {
					playerGhostPB = ghost;
					print ("[REPLAY] New ghost PB");
				} else 
					print ("[REPLAY] Discarding ghost.");
			} else {
				if (ghost.GetScoreRecorded () > playerGhostPB.GetScoreRecorded ()) {
					playerGhostPB = ghost;
					print ("[REPLAY] New ghost PB");
				} else
					print ("[REPLAY] Discarding ghost.");
			}
		}
	}
	public GhostReplayData GetPlayerGhostPB()
	{
		return playerGhostPB;
	}
	public void SetLastEventPlayedResult(int result)
	{
		m_lastEventPlayedResult = result;
	}
	public int GetLastEventPlayedResult()
	{
		return m_lastEventPlayedResult;
	}
	public int GetPlayerRank()
	{
		return m_playerRank;
	}
	public float GetPlayerRankStatus()
	{
		return m_playerRankStatus;
	}
	public int GetPlayerCurrency()
	{
		return m_playerCurrency;
	}
	public int GetPlayerAlternativeCurrency()
	{
		return m_playerCurrencyAlternative;
	}
	public CarData GetCarInUse()
	{
		if (carSelectedIndex < 0) {
			return null;
		}
		return carsOwned [carSelectedIndex];
	}
	public int GetCarInUseIndex()
	{
		return carSelectedIndex;
	}
	public void SetCarInUseIndex(int index)
	{
		carSelectedIndex = index;
	}
	public string GetRankName()
	{
		string txt = "Unknown";
		switch (m_playerRank) {
		case 1:
			{
				txt = "Novice 1";	
				break;
			}
		case 2:
			{
				txt = "Novice 2";	
				break;
			}
		case 3:
			{
				txt = "Novice 3";
				break;
			}
		case 4:
			{
				txt = "Semi-professional 1";	
				break;
			}
		case 5:
			{
				txt = "Semi-professional 2";	
				break;
			}
		case 6:
			{
				txt = "Semi-professional 3";
				break;
			}
		case 7:
			{
				txt = "Professional 1";
				break;
			}
		case 8:
			{
				txt = "Professional 2";
				break;
			}
		case 9:
			{
				txt = "Professional 3";
				break;
			}
		case 10:
			{
				txt = "Master 1";
				break;
			}
		case 11:
			{
				txt = "Master 2";
				break;
			}
		case 12:
			{
				txt = "Master 3";
				break;
			}
		case 13:
			{
				txt = "Legend 1";
				break;
			}
		case 14:
			{
				txt = "Legend 2";
				break;
			}
		case 15:
			{
				txt = "Legend 3";
				break;
			}
		case 16:
			{
				txt = "Grand legend";
				break;
			}
		}
		return txt;
	}

}
