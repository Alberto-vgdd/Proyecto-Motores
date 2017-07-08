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
		MainMenuManager.currentInstance.SelectEventAsActive (m_index);
	}
	public void SetPanelForEvent(EventData data, int index)
	{
		m_index = index;
		text_difficulty.text = data.GetRoadDifficulty ().ToString ("F2");
		text_reward.text = data.GetRewardCurrency () + " cr.";
		switch (data.GetEventType()) {
		case 1: // Endurance
			{
				text_header.text = "Seaside highway - Endurance";
				break;
			}
		case 2: // Drift Endurance
			{
				text_header.text = "Seaside highway - Drift Endurance";
				break;
			}
		case 3: // Drift Exhibition
			{
				text_header.text = "Seaside highway - Drift Exhibition";
				break;
			}
		case 4: // High Speed Challenge
			{
				text_header.text = "Seaside highway - High Speed Challenge";
				break;
			}
		case 5: // Chain Drift Challenge
			{
				text_header.text = "Seaside highway - Chain Drift Challenge";
				break;
			}
		case 6: // Time Attack
			{
				text_header.text = "Seaside highway - Time attack";
				break;
			}
		}
	}
}
