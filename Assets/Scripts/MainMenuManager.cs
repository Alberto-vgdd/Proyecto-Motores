using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

	public static MainMenuManager currentInstance;

	[Header("Event List Panel")]
	public CanvasGroup EventPanelCG;
	public Transform EventPanelListParent;

	[Header("Top Panel")]
	public Text normalCurrencyText;
	public Text alternativeCurrencyText;
	public Text playerNameText;
	public Text playerRankText;
	[Header("Event Details Panel")]
	public Text eventDetailsHeader;
	public Text eventDetailsDescription;
	public Text eventDetailsAditionalDesc;
	public Text eventDetailsRewards;
	public Text eventDetailsSubPanel;
	public CanvasGroup eventDetailsCG;
	[Header("RankPromotionPanel")]
	public Slider rankStatusSlider;
	public Text rankName;
	public Text promotionInfo;
	public CanvasGroup rankPromotionCG;
	[Header("Loading Panel")]
	public Text loadingInfo;
	public CanvasGroup loadingCG;
	[Header("Other references")]
	public GameObject EventButtonPrefab;
	public List<GameObject> eventsOnDisplay;

	private bool CoRoutineActive = false;

	void Awake ()
	{
		currentInstance = this;
	}

	void Start () {
		CreateSelectableEvents ();
		GlobalGameData.currentInstance.UpdateRankStatus ();
		StartCoroutine ("RankPromotionPanel");
		UpdateCurrencyAndRankValues ();
	}
	public void SelectEventAsActive(int index)
	{
		if (CoRoutineActive)
			return;
		GlobalGameData.currentInstance.eventActive = GlobalGameData.currentInstance.eventsAvailable [index];
		StartCoroutine ("FadeInEventDetailsPanel");
		SetupEventDetailsPanel ();
	}
	void CreateSelectableEvents()
	{
		GameObject lastCreatedPanel;
		for (int i = 0; i < GlobalGameData.currentInstance.eventsAvailable.Count; i++) {
			lastCreatedPanel = Instantiate (EventButtonPrefab, EventPanelListParent) as GameObject;
			lastCreatedPanel.GetComponent<EventSubPanelBehaviour> ().SetPanelForEvent (GlobalGameData.currentInstance.eventsAvailable [i], i);
		}
	}
	public void UpdateCurrencyAndRankValues()
	{
		if (GlobalGameData.currentInstance == null)
			return;
		playerNameText.text = "Player";
		playerRankText.text = "Driver rank: " + GlobalGameData.currentInstance.GetPlayerRank ().ToString ();
		normalCurrencyText.text = GlobalGameData.currentInstance.GetPlayerCurrency ().ToString();
		alternativeCurrencyText.text = GlobalGameData.currentInstance.GetPlayerAlternativeCurrency ().ToString();
	}
	void SetupEventDetailsPanel()
	{
		if (GlobalGameData.currentInstance.eventActive == null)
			return;
		eventDetailsHeader.text = GlobalGameData.currentInstance.eventActive.GetEventArea () + " - " + GlobalGameData.currentInstance.eventActive.GetEventTypeName ();
		eventDetailsDescription.text = GlobalGameData.currentInstance.eventActive.GetEventTypeShortDesc ();
		eventDetailsAditionalDesc.text = "";
		eventDetailsRewards.text = GlobalGameData.currentInstance.eventActive.GetRewardString ();
		eventDetailsSubPanel.text = "Checkpoints: " + GlobalGameData.currentInstance.eventActive.GetEventCheckpoints().ToString() + "  [ID: " + 
			GlobalGameData.currentInstance.eventActive.GetSeed().ToString() + "]";
	}
	// CO-Routines
	// ======================================================================
	IEnumerator RankPromotionPanel()
	{
		if (GlobalGameData.currentInstance.GetLastEventPlayedResult() < 0)
			yield break;

		int rankOld = GlobalGameData.currentInstance.GetPlayerRank();
		float rankStatusOld = GlobalGameData.currentInstance.GetPlayerRankStatus();
		GlobalGameData.currentInstance.UpdateRankStatus ();
		int rankNew = GlobalGameData.currentInstance.GetPlayerRank ();
		float rankStatusNew = GlobalGameData.currentInstance.GetPlayerRankStatus ();

		CoRoutineActive = true;
		rankPromotionCG.alpha = 1;
		rankPromotionCG.gameObject.SetActive (true);

		float t_current = (rankStatusOld +1f) / 2f;
		float t_target;
		rankStatusSlider.value = t_current;

		if (rankOld < rankNew) {
			t_target = 1;
		} else if (rankOld > rankNew) {
			t_target = 0;
		} else {
			t_target = (rankStatusNew + 1f) / 2f;
		}
		rankName.text = "Rank: " + rankOld.ToString ();

		yield return new WaitForSeconds (1.5f);

		while (t_current != t_target) {
			t_current = Mathf.MoveTowards (t_current, t_target, Time.deltaTime * 0.15f);
			rankStatusSlider.value = t_current;
			yield return null;
		}
		if (rankOld != rankNew) {
			rankStatusSlider.value = 0.5f;
			if (rankOld < rankNew) {
				promotionInfo.text = "Rank increased!";
			} else {
				promotionInfo.text = "Rank decreased...";
			}
		}
		rankName.text = "Rank: " + rankNew.ToString ();
		yield return new WaitForSeconds (2.5f);
		rankPromotionCG.gameObject.SetActive (false);
		CoRoutineActive = false;
	}
	IEnumerator FadeInEventPanel()
	{
		CoRoutineActive = true;
		EventPanelCG.gameObject.SetActive (true);
		while (EventPanelCG.alpha < 1) {
			EventPanelCG.alpha = Mathf.MoveTowards (EventPanelCG.alpha, 1, Time.deltaTime*5f);
			EventPanelCG.transform.localScale =  Vector3.one * (0.5f + EventPanelCG.alpha*0.5f);
			yield return null;
		}
		CoRoutineActive = false;
	}
	IEnumerator FadeOutEventPanel()
	{
		CoRoutineActive = true;
		while (EventPanelCG.alpha > 0) {
			EventPanelCG.alpha = Mathf.MoveTowards (EventPanelCG.alpha, 0, Time.deltaTime*5f);
			EventPanelCG.transform.localScale = Vector3.one * (0.5f + EventPanelCG.alpha*0.5f);
			yield return null;
		}
		EventPanelCG.gameObject.SetActive (false);
		CoRoutineActive = false;
	}
	IEnumerator FadeInEventDetailsPanel()
	{
		eventDetailsCG.gameObject.SetActive (true);
		CoRoutineActive = true;
		while (eventDetailsCG.alpha < 1) {
			eventDetailsCG.alpha = Mathf.MoveTowards (eventDetailsCG.alpha, 1, Time.deltaTime*5f);
			eventDetailsCG.transform.localScale = Vector3.one * (0.5f + eventDetailsCG.alpha*0.5f);
			yield return null;
		}
		CoRoutineActive = false;
	}
	IEnumerator FadeOutEventDetailsPanel()
	{
		CoRoutineActive = true;
		while (eventDetailsCG.alpha > 0) {
			eventDetailsCG.alpha = Mathf.MoveTowards (eventDetailsCG.alpha, 0, Time.deltaTime*5f);
			eventDetailsCG.transform.localScale = Vector3.one * (0.5f + eventDetailsCG.alpha*0.5f);
			yield return null;
		}
		CoRoutineActive = false;
		eventDetailsCG.gameObject.SetActive (false);
	}
	IEnumerator LoadScene()
	{
		CoRoutineActive = true;
		loadingCG.gameObject.SetActive (true);
		loadingInfo.text = "LOADING";
		while (loadingCG.alpha < 1) {
			loadingCG.alpha = Mathf.MoveTowards (loadingCG.alpha, 1, Time.deltaTime * 5);
			yield return null;
		}
		AsyncOperation AO = SceneManager.LoadSceneAsync ("test");
		while (!AO.isDone) {
			loadingInfo.text = "LOADING (" + ((int)(AO.progress * 100)) + "%)";
			yield return null;
		}
		CoRoutineActive = false;
	}
	// Button Click Recievers
	// =======================================================================
	public void OnEventPanelClicked()
	{
		if (CoRoutineActive)
			return;
		print ("EventPanel clicked");
		StartCoroutine ("FadeInEventPanel");
	}
	public void OnWeeklyEventClicked()
	{
		if (CoRoutineActive)
			return;
		print ("WeeklyEvents clicked");
	}
	public void OnCustomEventClicked()
	{
		if (CoRoutineActive)
			return;
		print ("CustomEvent clicked");
	}
	public void OnCarShopClicked()
	{
		if (CoRoutineActive)
			return;
		print ("CarShop clicked");
	}
	public void OnPartShopClicked()
	{
		if (CoRoutineActive)
			return;
		print ("PartShop clicked");
	}
	public void OnGarageClicked()
	{
		if (CoRoutineActive)
			return;
		print ("Garage clicked");
	}
	public void OnProfileClicked()
	{
		if (CoRoutineActive)
			return;
		print ("Profile clicked");
	}
	public void OnSettingsClicked()
	{
		if (CoRoutineActive)
			return;
		print ("Settings clicked");
	}
	public void OnCloseEventPanelClicked()
	{
		if (CoRoutineActive)
			return;
		print ("CloseEventPanel clicked");
		StartCoroutine ("FadeOutEventPanel");
	}
	public void OnConfirmEventClicked()
	{
		if (CoRoutineActive)
			return;
		//TODO: Pantalla de carga quizas?
		StartCoroutine ("LoadScene");

	}
	public void OnCancelEventClicked()
	{
		if (CoRoutineActive)
			return;
		StartCoroutine ("FadeOutEventDetailsPanel");

	}
}
