﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

	public static MainMenuManager currentInstance;

	[Header("Event List Panel")]
	public CanvasGroup EventPanelCG;
	public GameObject eventSliderParent;
	public GameObject eventDetailsParent;
	public List<EventSubPanelBehaviour> eventsInList;

	[Header("MainSlider")]
	public CanvasGroup mainSlider;
	public GameObject newTag_offlineEvents;
	public GameObject newTag_seasonalEvents;
	public GameObject newTag_garage;
	[Header("Top&Bottom Panels")]
	public CanvasGroup topParent;
	public CanvasGroup bottomParent;
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
	public Transform eventDetailsWindow;
	[Header("Last Event Played Result Panel")]
	public List<CanvasGroup> LEPpanel_animatedSubPanels;
	public Text LEPpanel_eventResult;
	public Text LEPpanel_playerRank;
	public Text LEPpanel_rankPoints;
	public Text LEPpanel_reward;
	public Slider LEPpanel_slider;
	public Button LEPpanel_continueButton;
	public CanvasGroup LEPpanel_globalCG;
	[Header("Garage Panel")]
	public CanvasGroup carPanelCG;
	public GameObject carSliderParent;
	public GameObject carStatsParent;
	public GameObject carOptionsParent;
	public Text carNameText;
	public Text slotsInUse;
	public List<StatSliderBehaviour> statSliders;
	public List<GarageSubPanelBehaviour> carsInList;
	[Header("Loading Panel")]
	public Text loadingInfo;
	public CanvasGroup loadingCG;
	[Header("Other references")]
	public List<GameObject> eventsOnDisplay;
	public List<GameObject> carsOnDisplay;

	private int carInDisplayIndex = -1;

	private Vector3 eventSliderInitialPos;
	private Vector3 eventDetailsInitialPos;
	private Vector3 garageSliderInitialPos;
	private Vector3 garageStatsInitialPos;
	private Vector3 garageOptionsInitialPos;
	private Vector3 topPanelInitialPos;
	private Vector3 bottomPanelInitialPos;

	// Categoria de eventos
	private Category categoryOfEventsInDisplay;
	enum Category
	{
		Offline = 0,
		Seasonal = 1,
		Other = 2
	}


	private bool CoRoutineActive = false;

	void Awake ()
	{
		currentInstance = this;
	}

	void Start () {
		playerNameText.text = GlobalGameData.currentInstance.GetPlayerName();

		eventSliderInitialPos = eventSliderParent.transform.localPosition;
		eventDetailsInitialPos = eventDetailsParent.transform.localPosition;
		garageSliderInitialPos = carSliderParent.transform.localPosition;
		garageStatsInitialPos = carStatsParent.transform.localPosition;
		garageOptionsInitialPos = carOptionsParent.transform.localPosition;
		topPanelInitialPos = topParent.transform.localPosition;
		bottomPanelInitialPos = bottomParent.transform.localPosition;

		if (GlobalGameData.currentInstance.FirstTimeOnMainMenu()) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("New profile", "Welcome to Project Racing D."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Info (1)", "This is a beta version. Some features are in development."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Info (2)", "Select a car from the garage to begin."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Info (3)", "Go to the event panel to start playing."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Info (4)", "You can also practice with the seasonal events."));

			GlobalGameData.currentInstance.SetFirstTimeOnMainMenu (false);
			GlobalGameData.currentInstance.SaveData ();
		} else {
			if (GlobalGameData.currentInstance.GetLastEventPlayedResult () == 0) {
				MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Info", "Last event played was not finished and will be counted as a loss."));
			}
		}

		UpdateCurrencyAndRankValues ();
		StartCoroutine ("RankPromotionPanel");
		GlobalGameData.currentInstance.SaveData ();
	}
	private void SetEventPanels(Category eventCategory)
	{
		List<EventData> eventsToRead;
		categoryOfEventsInDisplay = eventCategory;

		switch (eventCategory) {
		case Category.Other:
			{
				eventsToRead = GlobalGameData.currentInstance.m_playerData_eventsOffline;
				break;
			}
		case Category.Seasonal:
			{
				eventsToRead = GlobalGameData.currentInstance.eventsAvailable_seasonal;
				break;
			}
		default: //...para que no se ralle el compilador
			{
				eventsToRead = GlobalGameData.currentInstance.m_playerData_eventsOffline;
				break;
			}
		}

		for (int i = 0; i < eventsInList.Count; i++) {
			if (i >= eventsToRead.Count) {
				eventsInList [i].gameObject.SetActive (false);
			} else {
				eventsInList [i].gameObject.SetActive (true);
				eventsInList [i].SetPanelForEvent (eventsToRead [i], i);
			}
		}
	}
	public void SelectEventAsActive(EventData data)
	{
		if (CoRoutineActive)
			return;
		GlobalGameData.currentInstance.m_playerData_eventActive = data;
		StopCoroutine ("FadeInEventDetailsPanel");
		StartCoroutine ("FadeInEventDetailsPanel");
		SetupEventDetailsPanel ();

		MainMenuSoundManager.instance.playAcceptSound ();

	}
	public void SetCarSelected(int index)
	{
		CarData readedCar = GlobalGameData.currentInstance.m_playerData_carsOwned [index];

		carNameText.text = readedCar.GetCarName ();
		statSliders[0].SetValues (readedCar.GetBaseMaxSpeed(), readedCar.GetUpgradedMaxSpeed());
		statSliders[1].SetValues (readedCar.GetBaseAcceleration(), readedCar.GetUpgradedAcceleration());
		statSliders[2].SetValues (readedCar.GetBaseTurnRate(), readedCar.GetUpgradedTurnRate());
		statSliders[3].SetValues (readedCar.GetBaseDriftStrenght(), readedCar.GetUpgradedDriftStrenght());
		statSliders[4].SetValues (readedCar.GetBaseWeight (), readedCar.GetUpgradedWeight ());

		carInDisplayIndex = index;

		MainMenuSoundManager.instance.playAcceptSound ();

	}
	void SetGarageCarButtons()
	{
		for (int i = 0; i < carsInList.Count; i++) {
			if (i >= GlobalGameData.currentInstance.m_playerData_carsOwned.Count) {
				carsInList [i].gameObject.SetActive (false);
			} else {
				carsInList [i].gameObject.SetActive (true);
				carsInList [i].SetPanelForCar(GlobalGameData.currentInstance.m_playerData_carsOwned[i], i);
				if (GlobalGameData.currentInstance.GetCarInUseIndex () == i) {
					carsInList [i].SetSelected (true);
				} else {
					carsInList [i].SetSelected (false);
				}
			}
		}
	}
	private bool IsButtonAcctionAvailable()
	{
		return !CoRoutineActive && !MainMenuNotificationManager.currentInstance.IsOpen ();
	}
	public void UpdateCurrencyAndRankValues()
	{
		if (GlobalGameData.currentInstance == null)
			return;
		playerRankText.text = GlobalGameData.currentInstance.GetRankName ();
		normalCurrencyText.text = GlobalGameData.currentInstance.GetPlayerCurrency ().ToString();
		alternativeCurrencyText.text = GlobalGameData.currentInstance.GetPlayerAlternativeCurrency ().ToString();
	}
	void SetupEventDetailsPanel()
	{
		if (GlobalGameData.currentInstance.m_playerData_eventActive == null)
			return;
		eventDetailsHeader.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetEventArea () + " - " + GlobalGameData.currentInstance.m_playerData_eventActive.GetEventName();
		eventDetailsDescription.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetEventTypeShortDesc ();
		eventDetailsAditionalDesc.text = "";
		eventDetailsRewards.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString ();
		eventDetailsSubPanel.text = "Checkpoints: " + GlobalGameData.currentInstance.m_playerData_eventActive.GetCheckpointsString() + "  [ID: " + 
			GlobalGameData.currentInstance.m_playerData_eventActive.GetSeed().ToString() + "]";
	}

	// =====================================================================================
	// Co-Routines
	// =====================================================================================
	// Todas las co-rutinas utilizadas por la clase, mayormente para animaciones de transicion
	// de interfaz, cuando haya alguna animacion en curso, la variable CoRoutineActive tendra
	// true, mientras este activa, no se recibira ningun click de ningun boton, para evitar
	// que las animaciones se solapen y otros errores.
	// =====================================================================================

	IEnumerator FadeInTopAndBottomPanels()
	{
		// No hay fadeout, innecesario demomento.
		topParent.alpha = bottomParent.alpha = 0;
		topParent.gameObject.SetActive(true);
		bottomParent.gameObject.SetActive (true);
		yield return new WaitForSeconds (0.25f);

		while (MainMenuNotificationManager.currentInstance.IsOpen ())
			yield return null;
		
		float t = 0;
		float animSpeed = 5f;
		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			topParent.transform.localPosition = topPanelInitialPos + Vector3.up * 40 * (1 - t);
			bottomParent.transform.localPosition = bottomPanelInitialPos + Vector3.down * 40 * (1 - t);
			topParent.alpha = bottomParent.alpha = t;
			yield return null;
		}
	}
	IEnumerator RankPromotionPanel()
	{
		CoRoutineActive = true;

		if (GlobalGameData.currentInstance.GetLastEventPlayedResult () < 0 || GlobalGameData.currentInstance.m_playerData_eventActive == null) {
			StartCoroutine ("FadeInMainSlider");
			StartCoroutine ("FadeInTopAndBottomPanels");
			CoRoutineActive = false;
			UpdateCurrencyAndRankValues ();
			yield break;
		}


		List<Vector3> panelsInitialPositions;
		panelsInitialPositions = new List<Vector3>();
		for (int i = 0; i < LEPpanel_animatedSubPanels.Count; i++) {
			panelsInitialPositions.Add(LEPpanel_animatedSubPanels [i].transform.localPosition);
		}

		LEPpanel_globalCG.gameObject.SetActive (true);
		LEPpanel_playerRank.text = GlobalGameData.currentInstance.GetRankName ();
		if (GlobalGameData.currentInstance.GetLastEventPlayedResult () == 0) {
			LEPpanel_eventResult.text = "No medal awarded";
		} else if (GlobalGameData.currentInstance.GetLastEventPlayedResult () == 1) {
			LEPpanel_eventResult.text = "Gold medal awarded";
		} else if (GlobalGameData.currentInstance.GetLastEventPlayedResult () == 2) {
			LEPpanel_eventResult.text = "Silver medal awarded";
		} else {
			LEPpanel_eventResult.text = "Bronze medal awarded";
		}
		LEPpanel_reward.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString(GlobalGameData.currentInstance.GetLastEventPlayedResult());
		LEPpanel_rankPoints.text = "Rank points earned: " + GlobalGameData.currentInstance.GetRankChangeOnNextUpdate ().ToString ("F2");

		int rankOld = GlobalGameData.currentInstance.GetPlayerRank();
		float rankStatusOld = GlobalGameData.currentInstance.GetPlayerRankStatus();
		GlobalGameData.currentInstance.UpdatePostEventChanges ();
		int rankNew = GlobalGameData.currentInstance.GetPlayerRank ();
		float rankStatusNew = GlobalGameData.currentInstance.GetPlayerRankStatus ();

		LEPpanel_slider.value = rankStatusOld;

		while (LEPpanel_globalCG.alpha < 1) {
			LEPpanel_globalCG.alpha = Mathf.MoveTowards (LEPpanel_globalCG.alpha, 1, Time.deltaTime * 2f);
			yield return null;
		}
		float t = 0;
		float animSpeed = 4f;
		while (t < 4) {
			for (int i = 0; i < 4; i++)
			{
				LEPpanel_animatedSubPanels [i].transform.localPosition = panelsInitialPositions[i] + Vector3.left * ((1-Mathf.Clamp01(t-i)) * 40);
				LEPpanel_animatedSubPanels [i].alpha = Mathf.Clamp01(t-i);
			}
			t += Time.deltaTime * animSpeed;
			yield return null;
		}
		yield return new WaitForSeconds (0.5f);

		float sliderTargetValue;
		if (rankOld > rankNew) {
			sliderTargetValue = -1;
		} else if (rankOld < rankNew) {
			sliderTargetValue = 1;
		} else {
			sliderTargetValue = rankStatusNew;
		}
		float sliderSpeed = Mathf.Abs (sliderTargetValue - LEPpanel_slider.value) *2f;
		while (LEPpanel_slider.value != sliderTargetValue) {
			LEPpanel_slider.value = Mathf.MoveTowards(LEPpanel_slider.value, sliderTargetValue, Time.deltaTime * sliderSpeed);
			yield return null;
		}

		LEPpanel_slider.value = rankStatusNew;
		t = 0;
		int reverseAnim = 1;
		if (rankOld > rankNew) {
			reverseAnim = -1;
		}

		if (rankOld != rankNew) {
			Vector3 rankInitialPos = LEPpanel_playerRank.transform.localPosition;
			Vector3 pointsInitialPos = LEPpanel_rankPoints.transform.localPosition;
			CanvasGroup ranknameCG = LEPpanel_playerRank.GetComponent<CanvasGroup> ();
			CanvasGroup rankpointsCG = LEPpanel_rankPoints.GetComponent<CanvasGroup> ();
			while (t < 1) {
				ranknameCG.alpha = rankpointsCG.alpha = 1 - t;
				ranknameCG.transform.localPosition = rankInitialPos + Vector3.left * (t) * -40 * reverseAnim;
				rankpointsCG.transform.localPosition = pointsInitialPos + Vector3.left * (t) * -40 * reverseAnim;
				t = Mathf.MoveTowards(t, 1, Time.deltaTime*4f);
				yield return null;
			}
			t = 0;

			LEPpanel_playerRank.text = GlobalGameData.currentInstance.GetRankName ();
			if (rankOld > rankNew) {
				LEPpanel_rankPoints.text = "Driver rank decreased!";
				MainMenuNotificationManager.currentInstance.AddNotification(new MainMenuNotificationData("Driver rank updated", "Your driver rank has decreased to " + GlobalGameData.currentInstance.GetRankName()));
				MainMenuNotificationManager.currentInstance.AddNotification(new MainMenuNotificationData("Driver rank updated", "Event panel has been updated with easier events with less rewards."));
			} else {
				LEPpanel_rankPoints.text = "Driver rank increased!";
				MainMenuNotificationManager.currentInstance.AddNotification(new MainMenuNotificationData("Driver rank updated", "Your driver rank has increased to " + GlobalGameData.currentInstance.GetRankName()));
				MainMenuNotificationManager.currentInstance.AddNotification(new MainMenuNotificationData("Driver rank updated", "Event panel has been updated with harder events with better rewards."));
			}

			while (t < 1) {
				ranknameCG.alpha = rankpointsCG.alpha = t;
				ranknameCG.transform.localPosition = rankInitialPos + Vector3.left * (1-t) * 40 * reverseAnim;
				rankpointsCG.transform.localPosition = pointsInitialPos + Vector3.left * (1 - t) * 40 * reverseAnim;
				t = Mathf.MoveTowards(t, 1, Time.deltaTime*4f);
				yield return null;
			}
		}

		yield return new WaitForSeconds (0.5f);

		t = 4;
		while (t < 6) {
			for (int i = 4; i < LEPpanel_animatedSubPanels.Count; i++)
			{
				LEPpanel_animatedSubPanels [i].transform.localPosition = panelsInitialPositions[i] + Vector3.left * ((1-Mathf.Clamp01(t-i)) * 40);
				LEPpanel_animatedSubPanels [i].alpha = Mathf.Clamp01(t-i);
			}
			t += Time.deltaTime * animSpeed;
			yield return null;
		}

		UpdateCurrencyAndRankValues ();
		CoRoutineActive = false;

	}
	IEnumerator FadeOutRankDetailsPanel()
	{
		CoRoutineActive = true;
		while (LEPpanel_globalCG.alpha > 0)
		{
			LEPpanel_globalCG.alpha = Mathf.MoveTowards (LEPpanel_globalCG.alpha, 0, Time.deltaTime*5f);
			yield return null;
		}
		LEPpanel_globalCG.gameObject.SetActive (false);
		CoRoutineActive = false;
	}
	IEnumerator FadeInEventPanel()
	{
		CoRoutineActive = true;
		EventPanelCG.gameObject.SetActive (true);
		EventPanelCG.alpha = 0;

		float t = 0;
		float animSpeed = 5f;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime*animSpeed);
			EventPanelCG.alpha = t;
			eventSliderParent.transform.localPosition = eventSliderInitialPos + Vector3.left * 40 * (1 - t);
			yield return null;
		}
		CoRoutineActive = false;
	}
	IEnumerator FadeOutEventPanel()
	{
		CoRoutineActive = true;
		EventPanelCG.alpha = 1;

		float t = 1;
		float animSpeed = 5f;

		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime*animSpeed);
			EventPanelCG.alpha = t;
			eventSliderParent.transform.localPosition = eventSliderInitialPos + Vector3.left * 40 * (1 - t);
			yield return null;
		}
		CoRoutineActive = false;
		EventPanelCG.gameObject.SetActive (false);
	}
	IEnumerator FadeInEventDetailsPanel()
	{
		float t = 0;
		float animSpeed = 5f;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			eventDetailsCG.alpha = t;
			eventDetailsParent.transform.localPosition = eventDetailsInitialPos + Vector3.right * 40f * (1 - t);
			yield return null;
		}
	}
	IEnumerator FadeInMainSlider()
	{
		newTag_offlineEvents.SetActive (GlobalGameData.currentInstance.HasNewOfflineEvents ());
		newTag_seasonalEvents.SetActive (GlobalGameData.currentInstance.HasNewSeasonalEvents ());
		newTag_garage.SetActive (false); // TODO: Temporal, añadir condicion real.

		StopCoroutine ("FadeOutMainSlider");
		mainSlider.gameObject.SetActive (true);

		yield return new WaitForSeconds (0.25f);

		while (MainMenuNotificationManager.currentInstance.IsOpen ())
			yield return null;
		while (mainSlider.alpha < 1) {
			mainSlider.alpha = Mathf.MoveTowards (mainSlider.alpha, 1, Time.deltaTime*5f);
			yield return null;
		}
	}
	IEnumerator FadeOutMainSlider()
	{
		StopCoroutine ("FadeInMainSlider");
		while (mainSlider.alpha > 0) {
			mainSlider.alpha = Mathf.MoveTowards (mainSlider.alpha, 0, Time.deltaTime*5f);
			yield return null;
		}
		mainSlider.gameObject.SetActive (false);
	}
	IEnumerator FadeInGaragePanel()
	{
		CoRoutineActive = true;
		carPanelCG.gameObject.SetActive (true);
		carPanelCG.alpha = 0;
		MainMenuCamMovement.currentInstance.SwitchToCarView (true);

		float t = 0;
		float animSpeed = 5f;

		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime*animSpeed);
			carPanelCG.alpha = t;
			carOptionsParent.transform.localPosition = garageOptionsInitialPos + Vector3.left * 40 * (1 - t);
			carStatsParent.transform.localPosition = garageStatsInitialPos + Vector3.right * 40 * (1 - t);
			carSliderParent.transform.localPosition = garageSliderInitialPos + Vector3.up * 40 * (1 - t);
			yield return null;
		}
		CoRoutineActive = false;
	}
	IEnumerator FadeOutGaragePanel()
	{
		CoRoutineActive = true;
		MainMenuCamMovement.currentInstance.SwitchToCarView (false);

		float t = 1;
		float animSpeed = 5f;

		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime*animSpeed);
			carPanelCG.alpha = t;
			carOptionsParent.transform.localPosition = garageOptionsInitialPos + Vector3.left * 40 * (1 - t);
			carStatsParent.transform.localPosition = garageStatsInitialPos + Vector3.right * 40 * (1 - t);
			carSliderParent.transform.localPosition = garageSliderInitialPos + Vector3.up * 40 * (1 - t);
			yield return null;
		}
		carPanelCG.gameObject.SetActive (false);
		CoRoutineActive = false;
		StartCoroutine ("FadeInMainSlider");
	}
	IEnumerator LoadScene()
	{
		CoRoutineActive = true;
		loadingCG.gameObject.SetActive (true);
		if (GlobalGameData.currentInstance.m_playerData_eventActive.IsSeasonalEvent ()) {
			GlobalGameData.currentInstance.SetLastEventPlayedResult (-1);
		} else {
			GlobalGameData.currentInstance.SetLastEventPlayedResult (0);
			GlobalGameData.currentInstance.ReplaceLastEventPlayed ();
		}
		GlobalGameData.currentInstance.SaveData ();

		float animSpeed = 5;
		float t = 0;
		loadingInfo.text = "Loading track";
		CanvasGroup textCG = loadingInfo.gameObject.GetComponent<CanvasGroup> ();
		textCG.alpha = t;
		Vector3 textInitPos = loadingInfo.transform.localPosition;

		while (loadingCG.alpha < 1) {
			loadingCG.alpha = Mathf.MoveTowards (loadingCG.alpha, 1, Time.deltaTime * animSpeed * 2);
			yield return null;
		}
		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			loadingInfo.transform.localPosition = Vector3.left * 40 * (1 - t) + textInitPos;
			textCG.alpha = t;
			yield return null;
		}
		AsyncOperation AO = SceneManager.LoadSceneAsync ("InGame");
		AO.allowSceneActivation = false;
		while (AO.progress < 0.9f) {
			yield return null;
		}

		yield return new WaitForSeconds(0.25f);

		t = 1;
		animSpeed = 5;
		while (t > 0) {
			t = Mathf.MoveTowards(t, 0, Time.deltaTime * animSpeed);
			loadingInfo.transform.localPosition = Vector3.right * 40 * (1 - t) + textInitPos;
			textCG.alpha = t;
			yield return null;
		}
		yield return new WaitForSeconds (0.5f);

		AO.allowSceneActivation = true;

		CoRoutineActive = false;
	}

	// =====================================================================================
	// Button Click Recievers
	// =====================================================================================
	// Todos los listeners de los botones del menu principal, los clicks seran ignorados si
	// la funcion IsButtonActionAvailable() devuelve falso, esto ocurrira si hay alguna
	// animacion de transicion en curso, o queda alguna notificacion por mostrarse.
	// =====================================================================================


	// Main slider buttons
	// =====================================================================================

	public void OnEventPanelClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		if (GlobalGameData.currentInstance.GetCarInUse () == null) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "No car selected, select a car from your garage to begin."));
			return;
		}
		if (GlobalGameData.currentInstance.m_playerData_eventsOffline.Count == 0) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "No events available."));
			return;
		}
		SetEventPanels (Category.Offline);
		SelectEventAsActive (GlobalGameData.currentInstance.m_playerData_eventsOffline[0]);
		StartCoroutine ("FadeInEventPanel");
		StartCoroutine ("FadeOutMainSlider");

		MainMenuSoundManager.instance.playAcceptSound ();

	}
	public void OnWeeklyEventClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		if (GlobalGameData.currentInstance.GetCarInUse () == null) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "No car selected, select a car from your garage to begin."));
			return;
		}
		if (GlobalGameData.currentInstance.eventsAvailable_seasonal.Count == 0) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "No seasonal events available."));
			return;
		}
		SetEventPanels (Category.Seasonal);
		SelectEventAsActive (GlobalGameData.currentInstance.eventsAvailable_seasonal[0]);
		StartCoroutine ("FadeInEventPanel");
		StartCoroutine ("FadeOutMainSlider");

		MainMenuSoundManager.instance.playAcceptSound ();

	}
	public void OnCustomEventClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature in development"));
	}
	public void OnCarShopClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature in development"));
	}
	public void OnPartShopClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature in development"));
	}
	public void OnGarageClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		if (GlobalGameData.currentInstance.m_playerData_carsOwned.Count == 0) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "You don't own any car"));
			return;
		}
		SetGarageCarButtons();
		slotsInUse.text = GlobalGameData.currentInstance.m_playerData_carsOwned.Count + "/" + GlobalGameData.currentInstance.GetGarageSlots() + " slots in use";
		if (GlobalGameData.currentInstance.GetCarInUse () == null) {
			SetCarSelected (0);
		} else {
			SetCarSelected (GlobalGameData.currentInstance.GetCarInUseIndex());
		}
		StartCoroutine ("FadeInGaragePanel");
		StartCoroutine ("FadeOutMainSlider");

		MainMenuSoundManager.instance.playAcceptSound ();

	}
	public void OnProfileClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature in development"));
	}
	public void OnSettingsClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature in development"));
	}

	// Event buttons
	// =====================================================================================

	public void OnCloseEventPanelClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		StartCoroutine ("FadeOutEventPanel");
		StartCoroutine ("FadeInMainSlider");
		MainMenuSoundManager.instance.playCancelSound ();
	}
	public void OnConfirmEventClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuSoundManager.instance.playAcceptSound ();
		StartCoroutine ("LoadScene");
	}

	// Garage buttons
	// =====================================================================================

	public void OnSellCarClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature in development"));
	}
	public void OnCustomizeCarClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature in development"));
	}
	public void OnChangeCarPartsClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature in development"));
	}
	public void OnCloseGaragePanelClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		StartCoroutine ("FadeOutGaragePanel");
		StartCoroutine ("FadeInMainSlider");

		MainMenuSoundManager.instance.playCancelSound();

	}
	public void OnSelectCarClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		GlobalGameData.currentInstance.SetCarInUseIndex (carInDisplayIndex);
		SetGarageCarButtons();
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Car changed", "Your selected car is now: \n" + GlobalGameData.currentInstance.GetCarInUse().GetCarName()));

		MainMenuSoundManager.instance.playAcceptSound ();

	}

	// Last event played result & rank update panel
	// =====================================================================================

	public void OnCloseRankUpdateDetails()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		StartCoroutine ("FadeInMainSlider");
		StartCoroutine ("FadeInTopAndBottomPanels");
		StartCoroutine ("FadeOutRankDetailsPanel");

		MainMenuSoundManager.instance.playCancelSound();

	}
}
