using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventSubPanelBehaviour : MonoBehaviour {

	public Text text_header;
	public Text text_reward;
	public Image image_background;
	public GameObject newTag;
	public GameObject rewardPanel;
	private int m_index;
	private EventData m_dataReferenced;
	public SelfBlinkAnimation SBA;

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
		m_dataReferenced.SetAsViewed ();
		MainMenuManager.currentInstance.SelectEventAsActive (m_dataReferenced);
		UpdateNewTag ();
		SetAsSelected ();
	}
	public void SetPanelForEvent(EventData data, int index)
	{
		m_index = index;
		m_dataReferenced = data;

		if (data.IsSeasonalEvent ()) {
			rewardPanel.gameObject.SetActive (false);
		} else {
			rewardPanel.gameObject.SetActive (true);
			text_reward.text = m_dataReferenced.GetRewardString();
		}
		text_header.text = m_dataReferenced.GetEventName();
		UpdateNewTag ();
	}
	void UpdateNewTag()
	{
		newTag.SetActive (m_dataReferenced.IsNew());
	}
	public void DeSelect()
	{
		SBA.gameObject.SetActive (false);
	}
	public void SetAsSelected()
	{
		SBA.ResetBlinkingAnimation ();
	}
}
