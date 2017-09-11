using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SavedGameData {

	public int m_lastEventPlayedResult;
	public List<CarData> m_carsOwned;
	public List<EventData> m_offlineEvents;

	public string m_playerName;
	public int m_playerRank;
	public float m_playerRankStatus;

	public int m_playerNormalCurrency;
	public int m_playerSpecialCurrency;

	public EventData m_lastEventSelected;
	public int m_carInUseIndex;

	public bool m_firstTimeOnMainMenu;
	public bool m_firstTimeOfflineEvents;
	public bool m_firstTimeSeasonalEvents;
	public bool m_firstTimeGarage;
}
