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
	public Text carNameText;
	public Text noCarsAvailableText;
	public Transform carListParent;
	public Transform carPanelParent;
	public CanvasGroup carPanelCG;
	public StatSliderBehaviour maxspeedSlider;
	public StatSliderBehaviour accelerationSlider;
	public StatSliderBehaviour turnrateSlider;
	public StatSliderBehaviour driftstrSlider;
	public StatSliderBehaviour driftspdconsSlider;
	[Header("Loading Panel")]
	public Text loadingInfo;
	public CanvasGroup loadingCG;
	[Header("Other references")]
	public GameObject EventButtonPrefab;
	public GameObject CarButtonPrefab;
	public List<GameObject> eventsOnDisplay;
	public List<GameObject> carsOnDisplay;

	private int carInDisplayIndex = -1;

	private Vector3 eventSliderInitialPos;
	private Vector3 eventDetailsInitialPos;


	private bool CoRoutineActive = false;

	void Awake ()
	{
		currentInstance = this;
	}

	void Start () {
		eventSliderInitialPos = eventSliderParent.transform.localPosition;
		eventDetailsInitialPos = eventDetailsParent.transform.localPosition;

		UpdateCurrencyAndRankValues ();
		StartCoroutine ("RankPromotionPanel");
		if (!GlobalGameData.currentInstance.testing_WelcomeMessagesShown) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Welcome", "Select a car from the garage to begin."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Info (1)", "Some features are in development."));
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Info (2)", "For testing purposes, rank points gain is increased by 1000%"));
			GlobalGameData.currentInstance.testing_WelcomeMessagesShown = true;
		}

	}
	private void SetEventPanels(int eventCategory)
	{
		List<EventData> eventsToRead;
		switch (eventCategory) {
		case 1: // Eventos offline
			{
				eventsToRead = GlobalGameData.currentInstance.eventsAvailable_offline;
				break;
			}
		case 2: // Eventos de temporada
			{
				eventsToRead = GlobalGameData.currentInstance.eventsAvailable_seasonal;
				break;
			}
		default: //...para que no se ralle el compilador
			{
				eventsToRead = GlobalGameData.currentInstance.eventsAvailable_offline;
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
		GlobalGameData.currentInstance.eventActive = data;
		StopCoroutine ("FadeInEventDetailsPanel");
		StartCoroutine ("FadeInEventDetailsPanel");
		SetupEventDetailsPanel ();
	}
	public void SetCarSelected(int index)
	{
		CarData readedCar = GlobalGameData.currentInstance.carsOwned [index];

		carNameText.text = readedCar.GetCarName ();
		maxspeedSlider.SetValues (readedCar.GetBaseMaxSpeed(), readedCar.GetUpgradedMaxSpeed());
		accelerationSlider.SetValues (readedCar.GetBaseAcceleration(), readedCar.GetUpgradedAcceleration());
		turnrateSlider.SetValues (readedCar.GetBaseTurnRate(), readedCar.GetUpgradedTurnRate());
		driftstrSlider.SetValues (readedCar.GetBaseDriftStrenght(), readedCar.GetUpgradedDriftStrenght());
		driftspdconsSlider.SetValues (readedCar.GetBaseDriftSpdCons(), readedCar.GetUpgradedDriftSpdCons());

		carInDisplayIndex = index;
	}
	void CreateSelectableGarageCars()
	{
		//TODO: se puede optimizar un poco.
		if (GlobalGameData.currentInstance.carsOwned.Count == 0) {
			noCarsAvailableText.gameObject.SetActive (true);
		} else {
			noCarsAvailableText.gameObject.SetActive (false);
			if (GlobalGameData.currentInstance.GetCarInUse () == null) {
				SetCarSelected (0);
			} else {
				SetCarSelected (GlobalGameData.currentInstance.GetCarInUseIndex());
			}

		}
		GameObject lastReadedElem;
		while (carsOnDisplay.Count > 0) {
			lastReadedElem = carsOnDisplay [0];
			carsOnDisplay.RemoveAt (0);
			Destroy (lastReadedElem.gameObject);
		}
		for (int i = 0; i < GlobalGameData.currentInstance.carsOwned.Count; i++) {
			lastReadedElem = Instantiate (CarButtonPrefab, carListParent) as GameObject;
			lastReadedElem.GetComponent<GarageSubPanelBehaviour> ().SetPanelForCar (GlobalGameData.currentInstance.carsOwned [i], i);
			carsOnDisplay.Add (lastReadedElem);
		}
		SetSelectedTagForGaragePanel ();
	}
	void SetSelectedTagForGaragePanel()
	{
		if (GlobalGameData.currentInstance.GetCarInUse () == null)
			return;
		for (int i = 0; i < carsOnDisplay.Count; i++) {
			carsOnDisplay [i].GetComponent<GarageSubPanelBehaviour> ().SetSelected (i == GlobalGameData.currentInstance.GetCarInUseIndex());
		}
	}
	public void UpdateCurrencyAndRankValues()
	{
		if (GlobalGameData.currentInstance == null)
			return;
		playerNameText.text = "Player";
		playerRankText.text = GlobalGameData.currentInstance.GetRankName ();
		normalCurrencyText.text = GlobalGameData.currentInstance.GetPlayerCurrency ().ToString();
		alternativeCurrencyText.text = GlobalGameData.currentInstance.GetPlayerAlternativeCurrency ().ToString();
	}
	void SetupEventDetailsPanel()
	{
		if (GlobalGameData.currentInstance.eventActive == null)
			return;
		eventDetailsHeader.text = GlobalGameData.currentInstance.eventActive.GetEventArea () + " - " + GlobalGameData.currentInstance.eventActive.GetEventName();
		eventDetailsDescription.text = GlobalGameData.currentInstance.eventActive.GetEventTypeShortDesc ();
		eventDetailsAditionalDesc.text = "";
		eventDetailsRewards.text = GlobalGameData.currentInstance.eventActive.GetRewardString ();
		eventDetailsSubPanel.text = "Checkpoints: " + GlobalGameData.currentInstance.eventActive.GetCheckpointsString() + "  [ID: " + 
			GlobalGameData.currentInstance.eventActive.GetSeed().ToString() + "]";
	}
	// CO-Routines
	// ======================================================================
	IEnumerator RankPromotionPanel()
	{
		CoRoutineActive = true;

		if (GlobalGameData.currentInstance.GetLastEventPlayedResult () < 0 || GlobalGameData.currentInstance.eventActive == null) {
			StartCoroutine ("FadeInMainSlider");
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
		LEPpanel_reward.text = GlobalGameData.currentInstance.eventActive.GetRewardString(GlobalGameData.currentInstance.GetLastEventPlayedResult());
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
			sliderTargetValue = 0;
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
		mainSlider.gameObject.SetActive (true);
		while (mainSlider.alpha < 1) {
			mainSlider.alpha = Mathf.MoveTowards (mainSlider.alpha, 1, Time.deltaTime*5f);
			yield return null;
		}
	}
	IEnumerator FadeOutMainSlider()
	{
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
		MainMenuCamMovement.currentInstance.SwitchToCarView (true);
		while (carPanelCG.alpha < 1) {
			carPanelCG.alpha = Mathf.MoveTowards (carPanelCG.alpha, 1, Time.deltaTime*5f);
			yield return null;
		}
		CoRoutineActive = false;
	}
	IEnumerator FadeOutGaragePanel()
	{
		mainSlider.gameObject.SetActive (true);
		MainMenuCamMovement.currentInstance.SwitchToCarView (false);
		CoRoutineActive = true;
		while (carPanelCG.alpha > 0) {
			carPanelCG.alpha = Mathf.MoveTowards (carPanelCG.alpha, 0, Time.deltaTime*5f);
			mainSlider.alpha = 1 - carPanelCG.alpha;
			yield return null;
		}
		carPanelCG.gameObject.SetActive (false);
		CoRoutineActive = false;
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
		AsyncOperation AO = SceneManager.LoadSceneAsync ("InGame");
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
		if (GlobalGameData.currentInstance.GetCarInUse () == null) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "No car selected, select a car from your garage to begin."));
			return;
		}
		if (GlobalGameData.currentInstance.eventsAvailable_offline.Count == 0) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "No events available."));
			return;
		}
		SetEventPanels (1);
		SelectEventAsActive (GlobalGameData.currentInstance.eventsAvailable_offline[0]);
		StartCoroutine ("FadeInEventPanel");
		StartCoroutine ("FadeOutMainSlider");
	}
	public void OnWeeklyEventClicked()
	{
		if (CoRoutineActive)
			return;
		if (GlobalGameData.currentInstance.GetCarInUse () == null) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "No car selected, select a car from your garage to begin."));
			return;
		}
		if (GlobalGameData.currentInstance.eventsAvailable_seasonal.Count == 0) {
			MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "No seasonal events available."));
			return;
		}
		SetEventPanels (2);
		SelectEventAsActive (GlobalGameData.currentInstance.eventsAvailable_seasonal[0]);
		StartCoroutine ("FadeInEventPanel");
		StartCoroutine ("FadeOutMainSlider");
	}
	public void OnCustomEventClicked()
	{
		if (CoRoutineActive)
			return;
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feautre in development"));
	}
	public void OnCarShopClicked()
	{
		if (CoRoutineActive)
			return;
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feautre in development"));
	}
	public void OnPartShopClicked()
	{
		if (CoRoutineActive)
			return;
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feautre in development"));
	}
	public void OnGarageClicked()
	{
		if (CoRoutineActive)
			return;
		StartCoroutine ("FadeInGaragePanel");
		StartCoroutine ("FadeOutMainSlider");
		CreateSelectableGarageCars ();
	}
	public void OnSelectCarClicked()
	{
		GlobalGameData.currentInstance.SetCarInUseIndex (carInDisplayIndex);
		SetSelectedTagForGaragePanel ();
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Car changed", "Your selected car is now: " + GlobalGameData.currentInstance.GetCarInUse().GetCarName()));
	}
	public void OnProfileClicked()
	{
		if (CoRoutineActive)
			return;
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feautre in development"));
	}
	public void OnSettingsClicked()
	{
		if (CoRoutineActive)
			return;
		MainMenuNotificationManager.currentInstance.AddNotification (new MainMenuNotificationData ("Error", "Feautre in development"));
	}
	public void OnCloseEventPanelClicked()
	{
		if (CoRoutineActive)
			return;
		StartCoroutine ("FadeOutEventPanel");
		StartCoroutine ("FadeInMainSlider");
	}
	public void OnCloseGaragePanelClicked()
	{
		if (CoRoutineActive)
			return;
		StartCoroutine ("FadeOutGaragePanel");
		StartCoroutine ("FadeInMainSlider");
	}
	public void OnConfirmEventClicked()
	{
		if (CoRoutineActive)
			return;
		StartCoroutine ("LoadScene");
	}
	public void OnCloseRankUpdateDetails()
	{
		if (CoRoutineActive)
			return;
		StartCoroutine ("FadeInMainSlider");
		StartCoroutine ("FadeOutRankDetailsPanel");
	}
}
