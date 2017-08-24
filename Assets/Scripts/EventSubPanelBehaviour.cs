using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventSubPanelBehaviour : MonoBehaviour {

	public Text text_header;
	public Text text_reward;
	public Text text_difficulty;
	public Image image_background;
	private int m_index;
	private EventData m_dataReferenced;

	// GAMEMODES:
	// 0 = Free Roam 
	// 1 = Standard Endurance
	// 2 = Drift Endurance 
	// 3 = Drift Exhibition 
	// 4 = High Speed Challenge
	// 5 = Chain Drift Challenge
	// 6 = Time Attack

	// Use this for initialization
	public void OnClick()
	{
		MainMenuManager.currentInstance.SelectEventAsActive (m_dataReferenced);
	}
	public void SetPanelForEvent(EventData data, int index)
	{
		m_index = index;
		m_dataReferenced = data;
		text_reward.text = m_dataReferenced.GetRewardString();
		text_difficulty.text = m_dataReferenced.GetRoadDifficulty().ToString ("F1");
		text_header.text = m_dataReferenced.GetEventName();
	}
}
