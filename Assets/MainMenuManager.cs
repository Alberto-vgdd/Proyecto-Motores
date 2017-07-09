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
	[Header("Other references")]
	public GameObject EventButtonPrefab;
	public List<GameObject> eventsOnDisplay;

	private bool CoRoutineActive = false;

	void Awake ()
	{
		currentInstance = this;
	}
	// Use this for initialization
	void Start () {
		CreateSelectableEvents ();
		UpdateCurrencyValues ();
	}
	
	// Update is called once per frame
	void Update () {
		// Question mark?
	}
	public void SelectEventAsActive(int index)
	{
		if (CoRoutineActive)
			return;
		GlobalGameData.currentInstance.selectedEvent = GlobalGameData.currentInstance.eventsAvailable [index];
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
	public void UpdateCurrencyValues()
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
		if (GlobalGameData.currentInstance.selectedEvent == null)
			return;
		eventDetailsHeader.text = GlobalGameData.currentInstance.selectedEvent.GetEventArea () + " - " + GlobalGameData.currentInstance.selectedEvent.GetEventTypeName ();
		eventDetailsDescription.text = GlobalGameData.currentInstance.selectedEvent.GetEventTypeShortDesc ();
		eventDetailsAditionalDesc.text = "";
		eventDetailsRewards.text = GlobalGameData.currentInstance.selectedEvent.GetRewardString ();
		eventDetailsSubPanel.text = "Checkpoints: " + GlobalGameData.currentInstance.selectedEvent.GetEventCheckpoints().ToString() + "  [ID: " + 
			GlobalGameData.currentInstance.selectedEvent.GetSeed().ToString() + "]";
	}
	// CO-Routines
	// ======================================================================
	IEnumerator FadeInEventPanel()
	{
		CoRoutineActive = true;
		EventPanelCG.gameObject.SetActive (true);
		while (EventPanelCG.alpha < 1) {
			EventPanelCG.alpha = Mathf.MoveTowards (EventPanelCG.alpha, 1, Time.deltaTime*5f);
			EventPanelCG.transform.localScale = Vector3.one * EventPanelCG.alpha;
			yield return null;
		}
		CoRoutineActive = false;
	}
	IEnumerator FadeOutEventPanel()
	{
		CoRoutineActive = true;
		while (EventPanelCG.alpha > 0) {
			EventPanelCG.alpha = Mathf.MoveTowards (EventPanelCG.alpha, 0, Time.deltaTime*5f);
			EventPanelCG.transform.localScale = Vector3.one * EventPanelCG.alpha;
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
			eventDetailsCG.transform.localScale = Vector3.one * eventDetailsCG.alpha;
			yield return null;
		}
		CoRoutineActive = false;
	}
	IEnumerator FadeOutEventDetailsPanel()
	{
		CoRoutineActive = true;
		while (eventDetailsCG.alpha > 0) {
			eventDetailsCG.alpha = Mathf.MoveTowards (eventDetailsCG.alpha, 0, Time.deltaTime*5f);
			eventDetailsCG.transform.localScale = Vector3.one * eventDetailsCG.alpha;
			yield return null;
		}
		CoRoutineActive = false;
		eventDetailsCG.gameObject.SetActive (false);
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
		SceneManager.LoadScene ("test");

	}
	public void OnCancelEventClicked()
	{
		if (CoRoutineActive)
			return;
		StartCoroutine ("FadeOutEventDetailsPanel");

	}
}
