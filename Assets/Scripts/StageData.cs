﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageData : MonoBehaviour {

	// Administra y almacena la informacion del nivel. Para que funcione correctamente necesita:
	// - Referencia al jugador.

	public static StageData currentData;									// Referencia estatica

	public GameObject playerObj;											// GameObject del jugador TODO: (?)
	public float playerHealth;												// Salud del jugador

	public float time_remainingSec;
	public float time_passedDec;
	public int time_passedSec;
	public int time_passedMin;

	public FollowTarget playerCamera;	

	private bool notification_crit50 = false;								// (TEMP) Notificacion de 50% de salud mostrada
	private bool notification_crit25 = false;								// (TEMP) Notificacion de 25% de salud mostrada	
	private bool notification_crit10 = false;								// (TEMP) Notificacion de 10% de salud mostrada
	private bool notification_destroyed = false;							// (TEMP) Notificacion de 0% de salud mostrada

	public CanvasGroup fadeCG;
	public Text countDownText;

	public PlayerMovement pm;												// Referencia a PlayerMovement.
	public CarSkinManager CSManager;

    [Header("Ambient Light Parameters")]									//TODO: Unused?
    public float lightRange;
    public float lightAngle;


    [Header ("Diferent Player Chasis")]
    public GameObject dayChasis;
    public GameObject nightChasis;
	[Header ("Score data")]
	public int nodesCrossed;
	private int checkPointsCrossed = 0;
	public float damageTaken;
	public int cleanSections;
	public float totalDrift;
	[Header("GhostPlayers")]
	public List<GhostPlayer> GhostPlayers;
	[Header("Other parameters")]
	public bool gameStarted;

	private float timeCountMultiplier = 1f;
	private bool eventFinished;
	private float eventScore;
	private int finalscore;
	private bool countdownRunning = false;
	private EventData eventActive;
	private float timeEarnedMultiplier = 1f;
	private float timeEarnedDecay = 0.97f;



	void Awake () 
	{ 
		currentData = this;
	}
	void Start () {
		eventActive = GlobalGameData.currentInstance.m_playerData_eventActive;
		CSManager.ChangeBaseSkin (GlobalGameData.currentInstance.GetCarInUse ().GetSkinId ());
		time_remainingSec = eventActive.GetInitialTimeRemaining ();
		UpdateTime ();
	}

	void Update () {
		UpdateTime ();
	}

	public void StartEvent()
	{
		if (!countdownRunning)
		{
			playerCamera.SetRaceMode ();
			DayNightCycle.currentInstance.StartDayNightCycle ();
			countdownRunning = true;
			StartCoroutine ("Countdown");
		}

	}
	public void EndEvent(int type)
	{
		if (eventFinished)
			return;
	
		if (GhostRecorder.currentInstance != null) {
			if (eventActive.CanBeFailed() && type == 1)
				GhostRecorder.currentInstance.StopRecording (false);
			else
				GhostRecorder.currentInstance.StopRecording (true);
		}

		ContextualHudManager.currentInstance.ForceDriftEnd ();
		pm.AllowPlayerControl (false);
		SetEndGameScreen (type);
		eventFinished = true;
		IngameHudManager.currentInstance.SetHudVisibility (false);
		IngameHudManager.currentInstance.EndEvent ();
		if (eventActive.IsSeasonalEvent ()) {
			GlobalGameData.currentInstance.SetLastEventPlayedResult (-1);
		} else {
			GlobalGameData.currentInstance.SetLastEventPlayedResult (GetPlayerResult ());
		}
		GlobalGameData.currentInstance.SaveData ();

	}

	public void PlayerCrossedNode()
	{
		AddScore (eventActive.GetScoreOnDistance());
	}

	// Hace 10 de daño al jugador y actualiza el interfaz, este daño NO PUEDE ser letal.

	public void PlayerCrossedCheckPoint(bool clean, float awardedTime)
	{
		timeEarnedMultiplier *= timeEarnedDecay;

		checkPointsCrossed++;
		awardedTime *= eventActive.GetBonusTimeOnCheckpointMultiplier () * timeEarnedMultiplier;
		if (clean) {
			HealPlayer (10);
			NotificationManager.currentInstance.AddNotification (new GameNotification ("Clean section, health restored!", Color.green, 30));
			eventScore += eventActive.GetScoreOnCleanSection();
			cleanSections++;
		} else {
			eventScore += eventActive.GetScoreOnCheckpoint();
		}
		if (eventActive.GetScorePerRemainingTimeOnCheckpointMultiplier () > 0) {
			eventScore += (int)(time_remainingSec * eventActive.GetScorePerRemainingTimeOnCheckpointMultiplier ());
		}
		if (eventActive.GetBonusTimeOnCheckpointMultiplier() > 0) {
			if (eventActive.GetGamemode () == EventData.Gamemode.HighSpeedChallenge || eventActive.GetGamemode () == EventData.Gamemode.ChainDriftChallenge) {
				time_remainingSec = 10;
				NotificationManager.currentInstance.AddNotification(new GameNotification("Checkpoint! Remaining time set to 10", Color.green, 30));
			} else {
				time_remainingSec += awardedTime; 
				NotificationManager.currentInstance.AddNotification(new GameNotification("Time extended! +" + awardedTime.ToString("F1") , Color.green, 30));
			}

		}
		if (eventActive.GetEventCheckpoints() > 0) {
			IngameHudManager.currentInstance.UpdateSectorInfo ();
			if (checkPointsCrossed >= eventActive.GetEventCheckpoints()) {
				EndEvent (3);
			}
		}
		if (!eventActive.IsObjectiveTypeScore ()) {
			NotificationManager.currentInstance.AddNotification (new GameNotification ("Sector " + checkPointsCrossed + " - " + GetTimePassedString(), Color.cyan, 30));
		}
	}
	public void RespawnDamage()
	{
		if (playerHealth > 0.1f) {
			DamagePlayer (Mathf.Clamp(0.1f - playerHealth ,-10,0));
		}
		UpdateHealthNotifications ();
	}

	// Daña al jugador y actualiza el interfaz.
	public void SendPlayerCollision(float strenght)
	{
		DamagePlayer (strenght);
	}
	public void DamagePlayer(float dmg)
	{
		if (eventFinished)
			return;
		if (GlobalGameData.currentInstance.m_playerData_eventActive.GetSpecialEventType () == EventData.SpecialEvent.InstaGib)
			dmg = 99999;
		dmg *= eventActive.GetDamageTakenMultiplier();
		dmg = Mathf.Abs (dmg);
		damageTaken += dmg;
		playerHealth = Mathf.Clamp (playerHealth - dmg, 0, 100);
		ContextualHudManager.currentInstance.UpdateDynHealth ();
		UpdateHealthNotifications ();
		if (playerHealth <= 0) {
			EndEvent (1);
		}
	}

	// Extiende el tiempo restante la cantidad indicada.

	public void AddScore(float arg)
	{
		if (!eventActive.HasScore() || eventFinished)
			return;
		eventScore += arg;
		IngameHudManager.currentInstance.UpdateScoreInfo ();
	}
	public void SendFinishedDrift(float lenght, float multi = 1)
	{
		if (eventFinished)
			return;
		if (eventActive.GetBonusTimeOnDriftMultiplier() > 0) {
			time_remainingSec += (lenght * multi) * eventActive.GetBonusTimeOnDriftMultiplier() * timeEarnedMultiplier;
			NotificationManager.currentInstance.AddNotification(new GameNotification((int)lenght + "m. drift!" + " (x" + multi.ToString("F1") + ") " + (lenght * multi * eventActive.GetBonusTimeOnDriftMultiplier() * timeEarnedMultiplier).ToString("F1") + " bonus time.", Color.blue, 30));
		}
		if (eventActive.GetScoreOnDriftMultiplier() > 0) {
			AddScore (lenght * multi * eventActive.GetScoreOnDriftMultiplier());
			NotificationManager.currentInstance.AddNotification(new GameNotification((int)lenght + "m. drift! +" + ((int)(lenght * multi * eventActive.GetScoreOnDriftMultiplier())).ToString() + " score.", Color.yellow, 30));
		}
		totalDrift += lenght;
	}

	// Revisa si es necesario mostrar las alertas de daño.

	void UpdateHealthNotifications()
	{
		if (playerHealth < 50 && !notification_crit50) {
			NotificationManager.currentInstance.AddNotification (new GameNotification ("Below  50%  Health", Color.red, 40));
			notification_crit50 = true;
		}
		if (playerHealth < 25 && !notification_crit25) {
			NotificationManager.currentInstance.AddNotification (new GameNotification ("Below  25%  Health", Color.red, 40));
			notification_crit25 = true;
		}
		if (playerHealth < 10 && !notification_crit10) {
			NotificationManager.currentInstance.AddNotification (new GameNotification ("Critical damage", Color.red, 40));
			notification_crit10 = true;
		}
		if (playerHealth <= 0 && !notification_destroyed) {
			NotificationManager.currentInstance.AddNotification (new GameNotification ("Car destroyed", Color.red, 40));
			notification_destroyed = true;
		}
	}

	// Restaura salud del jugador y actualiza el interfaz.

	public void HealPlayer(float heal)
	{
		playerHealth += heal;
		playerHealth = Mathf.Min (100, playerHealth);
		if (playerHealth > 50) {
			notification_crit50 = false;
		}
		if (playerHealth > 25) {
			notification_crit25 = false;
		}
		if (playerHealth > 10) {
			notification_crit10 = false;
		}
		ContextualHudManager.currentInstance.UpdateDynHealth();
	}

	// Actualiza la iluminacion de todas las piezas activas.

    public void UpdateAllLights()
    {
		// TODO: esto no deberia estar aqui...
		for (int i = 0;i < RoadGenerator.currentInstance.spawnedNodes.Count; i++)
        {
			RoadGenerator.currentInstance.spawnedNodes [i].GetComponent<RoadNode> ().SetLightState (DayNightCycle.currentInstance.getLightsOn());
        }

		nightChasis.SetActive(DayNightCycle.currentInstance.getLightsOn());
		dayChasis.SetActive(!DayNightCycle.currentInstance.getLightsOn());

    }

	// Actualiza la infomracion del interfaz del tiempo.

	void UpdateTime()
	{
		timeCountMultiplier = 1f;
		if (!gameStarted || eventFinished)
			return;
		if (eventActive.HasTimelimit()) {
			if (eventActive.GetGamemode () == EventData.Gamemode.HighSpeedChallenge && pm.FastEnoughForHighSpeedChallenge())
				timeCountMultiplier = 0.05f;
			if (eventActive.GetGamemode() == EventData.Gamemode.ChainDriftChallenge && pm.IsDrifting())
				timeCountMultiplier = 0.05f;
			time_remainingSec = Mathf.MoveTowards (time_remainingSec, 0, Time.deltaTime * timeCountMultiplier);
			if (time_remainingSec <= 0 && pm.IsStopped()) 
			{
				EndEvent (2);
			}
		} else {
			time_passedDec += Time.deltaTime;
			if (time_passedDec >= 1) {
				time_passedDec -= 1;
				time_passedSec++;
				if (time_passedSec >= 60) {
					time_passedMin++;
					time_passedSec -= 60;
				}
			}
		}


	}

	IEnumerator Countdown()
	{
		IngameHudManager.currentInstance.SetHudVisibility (true);
		yield return new WaitForSeconds (1f);
		StartCoroutine ("FadeIn");
		yield return new WaitForSeconds (1.5f);

		float t = 0;
		int i = 3;
		float animSpeed = 5f;
		Vector3 basePos = countDownText.transform.localPosition;
		CanvasGroup CDCG = countDownText.GetComponent<CanvasGroup> ();

		while (i > 0) {
			countDownText.text = i.ToString ();
			while (t < 1) {
				t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
				countDownText.transform.localPosition = basePos + Vector3.left * (1-t) * 40f;
				CDCG.alpha = t;
				yield return null;
			}
			yield return new WaitForSeconds (0.8f);
			i--;
			t = 0;
		}
		countDownText.text = "GO";
		gameStarted = true;
		pm.AllowPlayerControl (true);
		SoundManager.currentInstance.StartPlaying ();
		StartGhostBehaviour ();

		//TODO:
		
		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			countDownText.transform.localPosition = basePos + Vector3.left * (1-t) * 40f;
			CDCG.alpha = t;
			yield return null;
		}
		yield return new WaitForSeconds (0.8f);
		t = 1;
		while (t > 0) {
			CDCG.alpha = t;
			t -= Time.deltaTime * 6f;
			yield return null;
		}
		countDownText.gameObject.SetActive (false);
	}
	IEnumerator FadeIn()
	{
		while (fadeCG.alpha > 0) {
			fadeCG.alpha = Mathf.MoveTowards (fadeCG.alpha, 0, Time.deltaTime);
			yield return null;
		}
	}
	void StartGhostBehaviour()
	{
		if (GhostRecorder.currentInstance != null)
			GhostRecorder.currentInstance.StartRecording ();
		if (GlobalGameData.currentInstance.m_playerData_eventActive.GetSpecialEventType() != EventData.SpecialEvent.AgainstDevs) {
			GhostPlayers [0].StartPlaying (GlobalGameData.currentInstance.GetPlayerGhostPB ());
		} else {
			for (int i = 0; i < 3; i++)
			{
				GhostPlayers [i].StartPlaying (GlobalGameData.currentInstance.GetDevReplay(i));
			}
			GhostPlayers [3].StartPlaying (GlobalGameData.currentInstance.GetPlayerGhostPB());
		}
	}

	// Prepara la pantalla de puntuacion.

	void SetEndGameScreen(int type)
	{
		// 1 = Destroyed, 2 = Time Up, 3 = Last CP reached.
		if (eventActive.CanBeFailed()) {
			EndGameScreenBehaviour.currentInstance.SetAndEnable (type, type != 3);
		} else {
			EndGameScreenBehaviour.currentInstance.SetAndEnable (type, false);
		}
	}

	// getters/setters

	public bool IsEventInProgress()
	{
		return gameStarted && !eventFinished;
	}
	public string GetTimePassedString()
	{
		return time_passedMin.ToString () + ":" + time_passedSec.ToString ("D2") + ":" + ((int)(time_passedDec * 100)).ToString ("D2");
	}
	public float GetTimePassedValue()
	{
		return time_passedMin * 60 + time_passedSec + time_passedDec;
	}
	public float GetEventScore()
	{
		return eventScore;
	}
	public int GetCheckpointsCrossed()
	{
		return checkPointsCrossed;
	}
	public string GetPlayerResultString()
	{
		if (eventActive.IsObjectiveTypeScore ()) {
			return ((int)eventScore).ToString ();
		}
		return GetTimePassedString ();
	}

	public int GetPlayerResult()
	{
		if (eventActive.IsObjectiveTypeScore()) {
			if (eventScore > eventActive.GetObjectiveForPosition(1)) {
				return 1;
			} else if (eventScore > eventActive.GetObjectiveForPosition(2)) {
				return 2;
			} else if (eventScore > eventActive.GetObjectiveForPosition(3)) {
				return 3;
			} else
				return 0;
		} else {
			if (GetTimePassedValue() < eventActive.GetObjectiveForPosition(1)) {
				return 1;
			} else if (GetTimePassedValue() < eventActive.GetObjectiveForPosition(2)) {
				return 2;
			} else if (GetTimePassedValue() < eventActive.GetObjectiveForPosition(3)) {
				return 3;
			} else
				return 0;
		}
	}
}
