using System.Collections;
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
	public Text movingInfoText;
	[Header("Event Details Panel")]
	public Text eventDetailsHeader;
	public Text eventDetailsDescription;
	public Text eventDetailsCheckpoints;
	public Text eventDetailsRoadid;
	public Text eventDetailsHour;
	public Text eventDetailsRoadDifficulty;
	public Text eventDetailsRewards;
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
	[Header("SettingsPanel")]
	public Transform settingsPanelParent;
	public CanvasGroup settingsPanelBackground;
	public CanvasGroup settingsPanelWindow;
	public Text settingsPanelSliderValue;
	public Slider settingsPanelSlider;
	public Text settingsPanelProfileName;
	public Toggle settingsPanelToggle;
	public InputField ProfileNameInputField;
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
	public CarSkinManager CSManager;

	private int carInDisplayIndex = 0;

	private Vector3 eventSliderInitialPos;
	private Vector3 eventDetailsInitialPos;
	private Vector3 garageSliderInitialPos;
	private Vector3 garageStatsInitialPos;
	private Vector3 garageOptionsInitialPos;
	private Vector3 topPanelInitialPos;
	private Vector3 bottomPanelInitialPos;
	private Vector3 movingInfoInitialPos;
	private Vector3 settingsWindowInitialPos;

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
		movingInfoInitialPos = movingInfoText.transform.localPosition;
		settingsWindowInitialPos = settingsPanelWindow.transform.localPosition;

		if (GlobalGameData.currentInstance.m_playerData_firstTimeOnMainMenu) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("New profile (1/5)", "Welcome to Project Racing D."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("New profile (2/5)", "This is a demo version. Some features are in development."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("New profile (3/5)", "Select a car from the garage to begin."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("New profile (4/5)", "Go to the event panel to start playing."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("New profile (5/5)", "You can also practice with the season challenges."));

			GlobalGameData.currentInstance.m_playerData_firstTimeOnMainMenu = false;
			GlobalGameData.currentInstance.SaveData ();
		} else {
			if (GlobalGameData.currentInstance.GetLastEventPlayedResult () == 0) {
				MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Info", "Last event played was not finished or got no medals and will decrease your driver rank heavily."));
			}
		}

		UpdateCurrencyAndRankValues ();
		StartCoroutine ("RankPromotionPanel");
		GlobalGameData.currentInstance.SaveData ();
	}
	private void ChangeCarDisplayed(bool active, int skinID)
	{
		CSManager.gameObject.SetActive (active);
		CSManager.ChangeBaseSkin (skinID);
	}
	private void SetMovingInfo (string arg)
	{
		movingInfoText.text = arg;
		StopCoroutine ("MovingInfoAnimation");
		StartCoroutine ("MovingInfoAnimation");
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
		for (int i = 0; i < eventsInList.Count; i++) {
			eventsInList [i].DeSelect ();
		}

		MainMenuSoundManager.instance.playAcceptSound ();

	}
	public void SetCarSelected(int index)
	{
		for (int i = 0; i < carsInList.Count; i++) {
			if (i == index)
				carsInList [i].SBA.ResetBlinkingAnimation ();
			else
				carsInList [i].SBA.gameObject.SetActive (false);
		}

		CarData readedCar = GlobalGameData.currentInstance.m_playerData_carsOwned [index];
		
		carNameText.text = readedCar.GetCarName ();
		statSliders[0].SetValues (readedCar.GetBaseMaxSpeed(), readedCar.GetUpgradedMaxSpeed());
		statSliders[1].SetValues (readedCar.GetBaseAcceleration(), readedCar.GetUpgradedAcceleration());
		statSliders[2].SetValues (readedCar.GetBaseTurnRate(), readedCar.GetUpgradedTurnRate());
		statSliders[3].SetValues (readedCar.GetBaseDriftStrenght(), readedCar.GetUpgradedDriftStrenght());
		statSliders[4].SetValues (readedCar.GetBaseWeight (), readedCar.GetUpgradedWeight ());

		ChangeCarDisplayed (true, readedCar.GetSkinId ());

		carInDisplayIndex = index;

		MainMenuSoundManager.instance.playAcceptSound ();

	}
	void SetGarageCarButtons()
	{
		for (int i = 0; i < carsInList.Count; i++) {
			carsInList [i].SBA.gameObject.SetActive (i == GlobalGameData.currentInstance.GetCarInUseIndex());
			if (i >= GlobalGameData.currentInstance.m_playerData_carsOwned.Count) {
				carsInList [i].SetPanelForCar(null, i);
			} else {
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
		eventDetailsDescription.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetEventDescription();
		eventDetailsRewards.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetRewardString ();
		eventDetailsCheckpoints.text = "Checkpoints: " + GlobalGameData.currentInstance.m_playerData_eventActive.GetCheckpointsString ();
		eventDetailsRoadDifficulty.text = "Road difficulty: " + GlobalGameData.currentInstance.m_playerData_eventActive.GetRoadDifficulty ().ToString ("F1");
		eventDetailsHour.text = GlobalGameData.currentInstance.m_playerData_eventActive.GetHourString ();
		eventDetailsRoadid.text = "ID: " + GlobalGameData.currentInstance.m_playerData_eventActive.GetSeed ().ToString ();
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

		if (GlobalGameData.currentInstance.GetCarInUse () == null) {
			ChangeCarDisplayed(false, 0);
		} else {
			ChangeCarDisplayed(true, GlobalGameData.currentInstance.GetCarInUse ().GetSkinId ());
		}


		StopCoroutine ("FadeOutMainSlider");
		SetMovingInfo ("Welcome to Project racing D!, check the season challenges panel to find unique events, can you beat them?");
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

		SetMovingInfo ("Select a car to use from the list, check the car stats to find which one fits your playstyle. Drag to rotate the view arround the car");

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
	IEnumerator FadeInSettingsPanel()
	{
		settingsPanelParent.gameObject.SetActive (true);
		CoRoutineActive = true;
		float t = 0;
		float animSpeed = 4f;

		settingsPanelSlider.value = GlobalGameData.currentInstance.m_gameSettings_nodesLoaded;
		settingsPanelToggle.isOn = GlobalGameData.currentInstance.m_gameSettings_postProcessing;
		settingsPanelProfileName.text = GlobalGameData.currentInstance.GetPlayerName ();

		while (t < 1) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime * animSpeed);
			settingsPanelBackground.alpha = t;
			settingsPanelWindow.alpha = t;
			settingsPanelWindow.transform.localPosition = settingsWindowInitialPos + Vector3.left * (1 - t);
			yield return null;
		}

		CoRoutineActive = false;

	}
	IEnumerator FadeOutSettingsPanel()
	{
		CoRoutineActive = true;
		float t = 1;
		float animSpeed = 4f;

		while (t > 0) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			settingsPanelBackground.alpha = t;
			settingsPanelWindow.alpha = t;
			settingsPanelWindow.transform.localPosition = settingsWindowInitialPos + Vector3.right * (1 - t);
			yield return null;
		}

		settingsPanelParent.gameObject.SetActive (false);
		CoRoutineActive = false;
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

	IEnumerator MovingInfoAnimation()
	{
		float info_OffsetX = movingInfoText.text.Length * 20f;
		float info_OffsetY = -100f;
		float animSpeed;
		float t;


		while (true) {
			movingInfoText.transform.localPosition = movingInfoInitialPos + Vector3.up * info_OffsetY;
			yield return new WaitForSeconds (0.5f);
			animSpeed = 2f;
			t = 0;
			while (t < 1) {
				t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
				movingInfoText.transform.localPosition = movingInfoInitialPos + Vector3.up * (1 - t) * info_OffsetY;
				yield return null;
			}

			yield return new WaitForSeconds (0.75f);

			t = 0;
			animSpeed = (1/info_OffsetX) * 125f;
			while (t < 1) {
				t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
				movingInfoText.transform.localPosition = movingInfoInitialPos + Vector3.left * t * info_OffsetX;
				yield return null;
			}
			yield return null;
		}
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

		if (GlobalGameData.currentInstance.m_playerData_firstTimeOnEventPanel) {
			GlobalGameData.currentInstance.m_playerData_firstTimeOnEventPanel = false;
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Events (1/5)", "Here you can select events to participate in."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Events (2/5)", "These events are randonly generated and new ones are added after every played event."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Events (3/3)", "Your result on these events will change your driver rank."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Events (4/5)", "The higher the driver rank, the higher the rewards and the difficulty."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Events (5/5)", "Keep in mind that losing will impact your rank negatively."));
		}

		SetEventPanels (Category.Offline);
		SelectEventAsActive (GlobalGameData.currentInstance.m_playerData_eventsOffline[0]);
		eventsInList [0].SetAsSelected ();
		SetMovingInfo ("Select a event to participate in, the list will be updated after every race with new events.");
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
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "No season challenges available."));
			return;
		}

		if (GlobalGameData.currentInstance.m_playerData_firstTimeOnSeasonalPanel) {
			GlobalGameData.currentInstance.m_playerData_firstTimeOnSeasonalPanel = false;
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Season Challenges (1/4)", "You can play season challenges here."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Season Challenges (2/4)", "Season challenges will offer you events with unusual rules."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Season Challenges (3/4)", "Those events will reward you a special currency used to purchase visual customization parts."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Season Challenges (4/4)", "This events WONT have any impact on your driver rank."));
		}

		SetEventPanels (Category.Seasonal);
		SelectEventAsActive (GlobalGameData.currentInstance.eventsAvailable_seasonal[0]);
		eventsInList [0].SetAsSelected ();
		SetMovingInfo ("Season challenges wont have any impact on your driver rank, but they are much harder than normal events.");
		StartCoroutine ("FadeInEventPanel");
		StartCoroutine ("FadeOutMainSlider");

		MainMenuSoundManager.instance.playAcceptSound ();

	}
	public void OnCustomEventClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature not available in the demo."));
	}
	public void OnCarShopClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature not available in the demo."));
	}
	public void OnPartShopClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature not available in the demo."));
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
		if (GlobalGameData.currentInstance.m_playerData_firstTimeOnGaragePanel) {
			GlobalGameData.currentInstance.m_playerData_firstTimeOnGaragePanel = false;
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Garage (1/4)", "You can check and manage your owned cars here."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Garage (2/4)", "Select a car from the list on the top to see its properties."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Garage (3/4)", "To drive the car click on the button at the middle bottom."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Garage (4/4)", "Car management features are disabled for this demo."));
		}
		SetGarageCarButtons ();
		slotsInUse.text = GlobalGameData.currentInstance.m_playerData_carsOwned.Count + "/" + GlobalGameData.currentInstance.GetGarageSlots() + " slots in use";
		if (GlobalGameData.currentInstance.GetCarInUse () == null) {
			SetCarSelected (0);
			carsInList [0].SBA.ResetBlinkingAnimation();
		} else {
			SetCarSelected (GlobalGameData.currentInstance.GetCarInUseIndex());
			carsInList [GlobalGameData.currentInstance.GetCarInUseIndex()].SBA.ResetBlinkingAnimation();
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
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature not available in the demo."));
	}
	public void OnSettingsClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuSoundManager.instance.playAcceptSound ();
		StartCoroutine ("FadeInSettingsPanel");
		StartCoroutine ("FadeOutMainSlider");
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
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature not available in the demo."));
	}
	public void OnCustomizeCarClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature not available in the demo."));
	}
	public void OnChangeCarPartsClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feature not available in the demo."));
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

	// Settings panel buttons
	// =====================================================================================
	public void OnConfirmSettingsClicked()
	{
		if (!IsButtonAcctionAvailable ()) {
			return;
		}
		MainMenuSoundManager.instance.playAcceptSound ();
		StartCoroutine ("FadeOutSettingsPanel");
		StartCoroutine ("FadeInMainSlider");
	}
	public void OnSettingsRenderDistanceSliderValueChanged()
	{
		GlobalGameData.currentInstance.m_gameSettings_nodesLoaded = (int)settingsPanelSlider.value;
		settingsPanelSliderValue.text = ((int)(settingsPanelSlider.value)).ToString();
	}
	public void OnSettingsPostProcessingToggleValueChanged()
	{
		GlobalGameData.currentInstance.m_gameSettings_postProcessing = settingsPanelToggle.isOn;
	}
	public void OnSettingsProfileNameValueChanged()
	{
		GlobalGameData.currentInstance.SetPlayerName (ProfileNameInputField.text);
		playerNameText.text = ProfileNameInputField.text;
		settingsPanelProfileName.text = ProfileNameInputField.text;
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
