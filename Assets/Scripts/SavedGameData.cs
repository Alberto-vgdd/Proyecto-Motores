using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SavedGameData {

	private int m_lastEventPlayedResult;
	private List<CarData> m_carsOwned;
	private List<EventData> m_offlineEvents;

	private int m_playerRank;
	private float m_playerRankStatus;

	private int m_playerNormalCurrency;
	private int m_playerSpecialCurrency;

	private EventData m_lastEventSelected;
	private int m_carInUseIndex;

	private bool firstTimeOnMainMenu;
	// Setters
	public void SetLastEventSelected(EventData data)
	{
		m_lastEventSelected = data;
	}
	public void SetCarInUseIndex(int arg)
	{
		m_carInUseIndex = arg;
	}
	public void SetLastEventPlayedResult(int arg)
	{
		m_lastEventPlayedResult = arg;
	}
	public void SetCarsOwnedList(List<CarData> cars)
	{
		m_carsOwned = cars;
	}
	public void SetOfflineEventsList(List<EventData> events)
	{
		m_offlineEvents = events;
	}
	public void SetPlayerRank(int arg)
	{
		m_playerRank = arg;
	}
	public void SetPlayerRankStatus(float arg)
	{
		m_playerRankStatus = arg;
	}
	public void SetNormalCurrency(int arg)
	{
		m_playerNormalCurrency = arg;
	}
	public void SetSpecialCurrency(int arg)
	{
		m_playerSpecialCurrency = arg;
	}
	public void SetFirstTimeOnMainMenu(bool arg)
	{
		firstTimeOnMainMenu = arg;
	}
	// Getters
	public EventData GetLastEventSelected()
	{
		return m_lastEventSelected;
	}
	public int GetCarInUseIndex()
	{
		return m_carInUseIndex;
	}
	public int GetLastEventPlayedResult()
	{
		return m_lastEventPlayedResult;
	}
	public List<CarData> GetCarsOwned()
	{
		return m_carsOwned;
	}
	public List<EventData> GetOfflineEvents()
	{
		return m_offlineEvents;
	}
	public int GetPlayerRank()
	{
		return m_playerRank;
	}
	public float GetPlayerRankStatus()
	{
		return m_playerRankStatus;
	}
	public int GetPlayerNormalCurrency()
	{
		return m_playerNormalCurrency;
	}
	public int GetPlayerSpecialCurrency()
	{
		return m_playerSpecialCurrency;
	}
	public bool FirstTimeOnMainMenu()
	{
		return firstTimeOnMainMenu;
	}
}
