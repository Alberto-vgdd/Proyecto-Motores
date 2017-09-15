using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GlobalGameData : MonoBehaviour {

	public static GlobalGameData currentInstance;

	private int m_playerData_lastEventPlayedResult = -1;
	private int m_playerData_currencyNormal;
	private int m_playerData_currencySpecial;
	private int m_playerData_playerRank;
	private int m_playerData_carSelectedIndex = 0;
	private float m_playerData_rankStatus;
	private string m_playerData_playerName = "Player";

	public bool m_playerData_firstTimeOnMainMenu = true;
	public bool m_playerData_firstTimeOnEventPanel = true;
	public bool m_playerData_firstTimeOnSeasonalPanel = true;
	public bool m_playerData_firstTimeOnGaragePanel = true;

	public int m_gameSettings_nodesLoaded = 20;
	public bool m_gameSettings_postProcessing = true;

	public EventData m_playerData_eventActive;
	public List<EventData> m_playerData_eventsOffline;
	public List<CarData> m_playerData_carsOwned;


	private bool hasAnySavedData = false;
	public List<EventData> eventsAvailable_seasonal;

	private int maxGarageSize = 10;

	private GhostReplayData playerGhostPB;
	private List<GhostReplayData> devSavedReplays;
	private bool devReplaysLoadedSuccesfully = true;

	public Material[] m_carSkins;

	void Awake ()
	{
		if (currentInstance == null) {
			DontDestroyOnLoad (this.gameObject);
			currentInstance = this;
			//InitializeData ();
		}
		else {
			Destroy (this.gameObject);
		}
	}

	public void InitializeData()
	{
		LoadDevReplays ();
		GetSeasonalEvents ();

		if (LoadData()) {
			hasAnySavedData = true;
			return;
		}
		m_playerData_currencyNormal = 0;
		m_playerData_currencySpecial = 0;
		m_playerData_playerRank = 1;
		m_playerData_rankStatus = 0;
		GenerateEventsAvailable ();
		m_playerData_carsOwned = new List<CarData> ();
		m_playerData_carsOwned.Add (new CarData (0));
		m_playerData_carsOwned.Add (new CarData (1));
		m_playerData_carsOwned.Add (new CarData (2));
		m_playerData_carsOwned.Add (new CarData (3));
		m_playerData_carSelectedIndex = -1;

	}
	public void SaveData()
	{
		BinaryFormatter binForm = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/SavedData.dat");
		SavedGameData data = new SavedGameData ();

		data.m_playerRank = m_playerData_playerRank;
		data.m_playerRankStatus = m_playerData_rankStatus;
		data.m_playerNormalCurrency = m_playerData_currencyNormal;
		data.m_playerSpecialCurrency = m_playerData_currencySpecial;
		data.m_lastEventPlayedResult = m_playerData_lastEventPlayedResult;
		data.m_offlineEvents = m_playerData_eventsOffline;
		data.m_carsOwned = m_playerData_carsOwned;
		data.m_carInUseIndex = m_playerData_carSelectedIndex;
		data.m_lastEventSelected = m_playerData_eventActive;
		data.m_firstTimeOnMainMenu = m_playerData_firstTimeOnMainMenu;
		data.m_playerName = m_playerData_playerName;
		data.m_firstTimeGarage = m_playerData_firstTimeOnGaragePanel;
		data.m_firstTimeOfflineEvents = m_playerData_firstTimeOnGaragePanel;
		data.m_firstTimeSeasonalEvents = m_playerData_firstTimeOnSeasonalPanel;
		data.m_settingsPostProcessing = m_gameSettings_postProcessing;
		data.m_settingsRoadLoadDistance = m_gameSettings_nodesLoaded;

		binForm.Serialize (file, data);
		file.Close ();

		print ("[SYSTEM]: Saving on " + Application.persistentDataPath);
		hasAnySavedData = true;
	}
	public bool LoadData()
	{
		print ("[SYSTEM]: Loading from " + Application.persistentDataPath);

		if (File.Exists (Application.persistentDataPath + "/SavedData.dat")) {
			BinaryFormatter binForm = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/SavedData.dat", FileMode.Open);
			SavedGameData data = (SavedGameData)binForm.Deserialize (file);

			m_playerData_playerRank = data.m_playerRank;
			m_playerData_rankStatus = data.m_playerRankStatus;
			m_playerData_currencyNormal = data.m_playerNormalCurrency;
			m_playerData_currencySpecial = data.m_playerSpecialCurrency;
			m_playerData_lastEventPlayedResult = data.m_lastEventPlayedResult;
			m_playerData_eventsOffline = data.m_offlineEvents;
			m_playerData_carsOwned = data.m_carsOwned;
			m_playerData_carSelectedIndex = data.m_carInUseIndex;
			m_playerData_eventActive = data.m_lastEventSelected;
			m_playerData_firstTimeOnMainMenu = data.m_firstTimeOnMainMenu;
			m_playerData_playerName = data.m_playerName;
			m_playerData_firstTimeOnGaragePanel = data.m_firstTimeGarage;
			m_playerData_firstTimeOnSeasonalPanel = data.m_firstTimeSeasonalEvents;
			m_playerData_firstTimeOnEventPanel = data.m_firstTimeOfflineEvents;
			m_gameSettings_nodesLoaded = data.m_settingsRoadLoadDistance;
			m_gameSettings_postProcessing = data.m_settingsPostProcessing;

			file.Close (); // <- NO OLVIDAR NUNCA
			return true;
		}

		print ("[SYSTEM]: No available data found, Load failed.");
		return false;
	}
	void LoadDevReplays()
	{
		if (devSavedReplays != null)
			return;
		if (!Directory.Exists (Application.dataPath + "/DevReplays")) {
			devReplaysLoadedSuccesfully = false;
			return;
		}
		DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/DevReplays");
		FileInfo[] filesFound = di.GetFiles ();
		BinaryFormatter binForm;
		FileStream file;
		int loaded = 0;

		devSavedReplays = new List<GhostReplayData> ();
		for (int i = 0; i < filesFound.Length; i++) {
			if (filesFound [i].FullName.EndsWith (".Ghost") && loaded < 3) {
				binForm = new BinaryFormatter ();
				file = File.Open (filesFound[i].FullName, FileMode.Open);
				devSavedReplays.Add((GhostReplayData)binForm.Deserialize (file));
				loaded++;

				file.Close ();
			}
		}
		if (devSavedReplays.Count < 3) {
			devReplaysLoadedSuccesfully = false;
		}

	}
	void OutputLastGhostToFile()
	{
		BinaryFormatter binForm = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/Ghost_" + m_playerData_playerName + "_" + playerGhostPB.GetRecordedAtSeed().ToString() 
			+ "_" + System.Environment.TickCount + ".Ghost");
		binForm.Serialize (file, playerGhostPB);
		file.Close ();
	}
	void GenerateEventsAvailable()
	{
		UnityEngine.Random.InitState(System.Environment.TickCount);
		m_playerData_eventsOffline = new List<EventData> ();
		for (int i = 0; i < 8; i++) {
			m_playerData_eventsOffline.Add (new EventData (m_playerData_playerRank, false, true));
		}
	}
	public void ReplaceLastEventPlayed()
	{
		UnityEngine.Random.InitState (System.Environment.TickCount);
		m_playerData_eventsOffline.Remove (m_playerData_eventActive);
		m_playerData_eventsOffline.Add (new EventData (m_playerData_playerRank, false, true));
	}
	void GetSeasonalEvents()
	{
		// Demomento ponemos eventos fijados ya que no tiene de donde scarlos.
		eventsAvailable_seasonal = new List<EventData>();
		//eventsAvailable_seasonal.Add(new EventData(111111, 4, EventData.Gamemode.TimeAttack, EventData.SpecialEvent.None, "GRABAD AQUI", 
		//	"Pues eso, grabad vuestros fantasmas aqui, es basicamente el evento donde testeabamos antes"));

		eventsAvailable_seasonal.Add(new EventData(765, 4, EventData.Gamemode.Endurance, EventData.SpecialEvent.InstaGib, "Clean drive",
			"A real endurance race, how far can you get with a car that will get instantly destroyed with the first collision? We hope you're used to perform clean sections."));
		eventsAvailable_seasonal.Add(new EventData(322322, 4, EventData.Gamemode.Endurance, EventData.SpecialEvent.SoundSpeed, "Like the wind",
			"A simple endurance race on a easy road, and your car will get all its stats boosted to the maximum! Sounds easy, right? Don't worry, just make sure you're not driving too fast..." +
			" Did I forgot to mention this car has no brakes?"));
		if (devReplaysLoadedSuccesfully) {
			eventsAvailable_seasonal.Add(new EventData(111111, 4, EventData.Gamemode.TimeAttack, EventData.SpecialEvent.AgainstDevs, "Better than the best", 
				"A short Time Attack where you will race against the ghost replays of the developers, on the track seed used for testing. Can you beat us?"));
		}
	}
	public float GetRankChangeOnNextUpdate()
	{
		if (m_playerData_lastEventPlayedResult < 0)
			return 0;
		float promotionMultiplier = Mathf.Pow(0.925f, m_playerData_playerRank-1);
		if (m_playerData_lastEventPlayedResult == 1) {
			return 0.45f * promotionMultiplier;
		} else if (m_playerData_lastEventPlayedResult == 2) {
			return 0.2f * promotionMultiplier;
		} else if (m_playerData_lastEventPlayedResult == 3) {
			return 0f;
		} else {
			return -0.25f;
		} 
	}
	public void UpdatePostEventChanges()
	{
		UpdateRewardStatus ();
		UpdateRankStatus ();
		m_playerData_lastEventPlayedResult = -1;
		m_playerData_eventActive = null;
	}
	private void UpdateRewardStatus()
	{
		if (m_playerData_lastEventPlayedResult < 0 || m_playerData_eventActive == null)
			return;
		m_playerData_currencyNormal += m_playerData_eventActive.GetRewardValueForPosition(m_playerData_lastEventPlayedResult);
		//TODO: algo para el caso en el que no sea dinero...
	}
	private void UpdateRankStatus()
	{
		if (m_playerData_lastEventPlayedResult < 0 || m_playerData_eventActive == null)
			return;
		float promotionMultiplier = Mathf.Pow(0.9f, m_playerData_playerRank-1);

		m_playerData_rankStatus += GetRankChangeOnNextUpdate ();

		if (m_playerData_rankStatus < -1) {
			if (m_playerData_playerRank == 1) {
				m_playerData_rankStatus = -1;
			} else {
				m_playerData_playerRank--;
				GenerateEventsAvailable ();
				m_playerData_rankStatus = 0;
			}
		} else if (m_playerData_rankStatus > 1) {
			m_playerData_rankStatus = 0;
			m_playerData_playerRank++;
			GenerateEventsAvailable ();
		}
	}
	public void SetPlayerGhostPB(GhostReplayData ghost)
	{
		
		if (ghost.GetRecordedAtSeed () != m_playerData_eventActive.GetSeed () || playerGhostPB == null) {
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
		//OutputLastGhostToFile ();
	}
	public void SetPlayerName(string arg)
	{
		m_playerData_playerName = arg;
	}
	public GhostReplayData GetPlayerGhostPB()
	{
		return playerGhostPB;
	}
	public void SetLastEventPlayedResult(int result)
	{
		m_playerData_lastEventPlayedResult = result;
	}
	public int GetLastEventPlayedResult()
	{
		return m_playerData_lastEventPlayedResult;
	}
	public int GetPlayerRank()
	{
		return m_playerData_playerRank;
	}
	public float GetPlayerRankStatus()
	{
		return m_playerData_rankStatus;
	}
	public int GetPlayerCurrency()
	{
		return m_playerData_currencyNormal;
	}
	public int GetPlayerAlternativeCurrency()
	{
		return m_playerData_currencySpecial;
	}
	public CarData GetCarInUse()
	{
		if (m_playerData_carSelectedIndex < 0) {
			return null;
		}
		return m_playerData_carsOwned [m_playerData_carSelectedIndex];
	}
	public int GetCarInUseIndex()
	{
		return m_playerData_carSelectedIndex;
	}
	public void SetCarInUseIndex(int index)
	{
		m_playerData_carSelectedIndex = index;
	}
	public int GetGarageSlots()
	{
		return maxGarageSize;
	}
	public bool HasNewOfflineEvents()
	{
		if (m_playerData_eventsOffline.Count == 0)
			return false;
		
		for (int i = 0; i < m_playerData_eventsOffline.Count; i++) {
			if (m_playerData_eventsOffline [i].IsNew ())
				return true;
		}

		return false;
	}
	public bool HasNewSeasonalEvents()
	{
		if (eventsAvailable_seasonal.Count == 0)
			return false;

		for (int i = 0; i < eventsAvailable_seasonal.Count; i++) {
			if (eventsAvailable_seasonal [i].IsNew ())
				return true;
		}

		return false;
	}
	public bool HasAnySavedData()
	{
		return hasAnySavedData;
	}
	public string GetPlayerName()
	{
		return m_playerData_playerName;
	}
	public GhostReplayData GetDevReplay(int index)
	{
		if (index < devSavedReplays.Count) {
			return devSavedReplays [index];
		}
		return null;
	}
	public string GetRankName()
	{
		string txt = "Unknown";
		switch (m_playerData_playerRank) {
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
