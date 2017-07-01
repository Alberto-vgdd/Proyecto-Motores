using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageData : MonoBehaviour {

	// Administra y almacena la informacion del nivel. Para que funcione correctamente necesita:
	// - Referencia al jugador.

	public static StageData currentData;									// Referencia estatica

	// GAMEMODES:
	// 0 = Free Roam 
	// 1 = Standard Endurance
	// 2 = Drift Endurance 
	// 3 = Drift Exhibition 
	// 4 = High Speed Challenge
	// 5 = Chain Drift Challenge
	// 6 = Time Attack
	// TODO: Pensar mas?

	public int gamemode;

	public GameObject playerObj;											// GameObject del jugador (?)
	public float playerHealth;												// Salud del jugador
	public float remainingSec;												// Segundos restantes
	public FollowTarget playerCamera;	

	private bool notification_crit50 = false;								// (TEMP) Notificacion de 50% de salud mostrada
	private bool notification_crit25 = false;								// (TEMP) Notificacion de 25% de salud mostrada	
	private bool notification_crit10 = false;								// (TEMP) Notificacion de 10% de salud mostrada
	private bool notification_destroyed = false;							// (TEMP) Notificacion de 0% de salud mostrada

	public CanvasGroup fadeCG;
	public Text countDownText;
	public CanvasGroup endGameStatsCG;
	public Text endGameStatsText;

	public PlayerMovement pm;												// Referencia a PlayerMovement.

    [Header("Ambient Light Parameters")]
    public float lightRange;
    public float lightAngle;


    [Header ("Diferent Player Chasis")]
    public GameObject dayChasis;
    public GameObject nightChasis;
	[Header ("Score data")]
	public int nodesCrossed;
	public int checkPointsCrossed;
	public float damageTaken;
	public int cleanSections;
	public float totalDrift;

	public bool gameStarted;
	private bool eventFinished;
	private float gameOverDelay = 5.0f;
	private int startGameDelay = 6;
	private float eventScore;
	private int finalscore;

	private bool countdownRunning = false;

	private bool eventHasTimeLimit = false;
	private bool eventHasScore = false;
	private bool eventHasBonusTimeOnDrift = false;
	private bool eventHasBonusTimeOnCP = false;
	private bool eventHasLimitCP = false;
	private bool eventCanBeFailed = false;
	private bool eventHasScoreOnNode = false;
	private bool eventHasScoreOnDrift = false;

	private int eventLimitCP = 0;
	private float eventBonusTimeOnCPMultiplier = 1;
	private float eventBonusTimeOnDriftMultiplier = 1;
	private float eventBonusScoreOnDriftMultiplier = 1;
	private float eventDamageTakenMultiplier;

	private float scoringDistance = 10;
	private float scoringCheckPoint = 50;
	private float scoringCleanSection = 100;
	private float scoringDrift = 0.5f;
	private float scoringDamagePenalty = 1f;

	void Awake () { currentData = this; }
	void Start () {
		EventSetup ();
		if (eventHasLimitCP) {
			IngameHudManager.currentInstance.UpdateSectorInfo ();
		}
		PreRacePanelBehaviour.currentInstance.SetPanelInfo (gamemode);
	}

	void Update () {
		
		UpdateTimeInfo ();

		if (eventFinished)
		{
			gameOverDelay -= Time.deltaTime;
			if (gameOverDelay <= 0.0f) 
			{
				
			}
		} 
		else
		{
			if (remainingSec <= 0 && Mathf.Abs (pm.accumulatedAcceleration) <= 1f) 
			{
				EndEvent (2);
			}
		}
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
		pm.SetAsEventFinished ();
		SetEndGameScreen (type);
		eventFinished = true;
		IngameHudManager.currentInstance.SetHudVisibility (false);
	}

	public void PlayerCrossedNode()
	{
		AddScore (scoringDistance);
	}

	// Hace 10 de daño al jugador y actualiza el interfaz, este daño NO PUEDE ser letal.

	public void PlayerCrossedCheckPoint(bool clean)
	{
		if (clean) {
			HealPlayer (10);
			NotificationManager.currentInstance.AddNotification(new GameNotification("Clean section, health restored!", Color.green, 30));
			cleanSections++;
		}
		checkPointsCrossed++;
		if (eventHasLimitCP) {
			IngameHudManager.currentInstance.UpdateSectorInfo ();
			if (checkPointsCrossed >= eventLimitCP) {
				EndEvent (3);
			}
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

	public void DamagePlayer(float dmg)
	{
		if (eventFinished)
			return;
		dmg *= eventDamageTakenMultiplier;
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

	public void ExtendTime(string type, float timeExtension, int aditionalParam = 0)
	{
		switch (type) {
		case "drift":
			{
				if (!eventHasBonusTimeOnDrift)
					return;
				timeExtension *= eventBonusTimeOnDriftMultiplier;
				NotificationManager.currentInstance.AddNotification (new GameNotification (aditionalParam + " m. drift! Bonus time + " + (aditionalParam / 100).ToString("F1") + " s.", Color.yellow, 30));
				StageData.currentData.remainingSec += timeExtension;
				break;
			}
		case "checkpoint":
			{
				if (!eventHasBonusTimeOnCP)
					return;
				timeExtension *= eventBonusTimeOnCPMultiplier;
				NotificationManager.currentInstance.AddNotification (new GameNotification ("Time extended! " + " + " + timeExtension.ToString("F1"), Color.white, 40));
				remainingSec += timeExtension;
				break;
			}
		}
	}
	public void AddScore(float arg)
	{
		if (!eventHasScore)
			return;
		eventScore += arg;
		IngameHudManager.currentInstance.UpdateScoreInfo ();
	}
	public void SendFinishedDrift(float lenght, float multi = 1)
	{
		if (lenght > 100 && eventHasBonusTimeOnDrift) {
			ExtendTime ("drift", (int)(lenght * eventBonusTimeOnDriftMultiplier));
		}
		if (eventHasScoreOnDrift) {
			AddScore (lenght * multi * eventBonusScoreOnDriftMultiplier);
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

	void UpdateTimeInfo()
	{
		if (!eventHasTimeLimit)
			return;

		if (gameStarted) {
			if (gamemode == 4 && pm.accumulatedAcceleration / pm.maxFwdSpeed > 0.7f)
				return;
			if (gamemode == 5 && pm.drifting)
				return;
			remainingSec = Mathf.MoveTowards (remainingSec, 0, Time.deltaTime);
		}
	}

	IEnumerator Countdown()
	{
		print ("Countdown Called");
		IngameHudManager.currentInstance.SetHudVisibility (true);
		while (!(gameStarted && startGameDelay <= -3))
		{
			if (startGameDelay == 5) {
				StartCoroutine ("FadeIn");
			}
			if (startGameDelay > 0 && startGameDelay <= 3) {
				countDownText.text = startGameDelay.ToString();
			} else if (startGameDelay > -2 && startGameDelay <= 0) {
				countDownText.text = "GO!";
				gameStarted = true;
				remainingSec += 0.5f;
			} else {
				countDownText.text = "";
			}

			startGameDelay--;
			yield return new WaitForSeconds (1f);
		}
	}
	IEnumerator FadeIn()
	{
		while (fadeCG.alpha > 0) {
			fadeCG.alpha = Mathf.MoveTowards (fadeCG.alpha, 0, Time.deltaTime);
			yield return null;
		}
	}
	void EventSetup()
	{
		switch (gamemode) {
		case 1: // Standard Endurance
			{
				remainingSec = 25;
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = true;
				eventHasScore = true;
				eventHasTimeLimit = true;
				eventHasLimitCP = false;
				eventCanBeFailed = false;
				eventHasScoreOnDrift = false;
				eventHasScoreOnNode = true;
				eventBonusTimeOnCPMultiplier = 1;
				eventBonusTimeOnDriftMultiplier = 0.05f;
				eventBonusScoreOnDriftMultiplier = 0.5f;
				eventDamageTakenMultiplier = 1.25f;
				break;
			}
		case 2: // Drift Endurance
			{
				remainingSec = 25;
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = true;
				eventHasScore = true;
				eventHasTimeLimit = true;
				eventHasLimitCP = false;
				eventCanBeFailed = false;
				eventHasScoreOnDrift = false;
				eventHasScoreOnNode = true;
				eventBonusTimeOnCPMultiplier = 0.1f;
				eventBonusTimeOnDriftMultiplier = 0.1f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		case 3: // Drift Exhibition
			{
				remainingSec = 30;
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = true;
				eventHasTimeLimit = true;
				eventHasLimitCP = true;
				eventCanBeFailed = true;
				eventHasScoreOnDrift = true;
				eventHasScoreOnNode = false;
				eventLimitCP = 8;
				eventBonusTimeOnCPMultiplier = 1.5f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 1f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		case 4: // High Speed Challenge
			{
				remainingSec = 10;
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = false;
				eventHasTimeLimit = true;
				eventHasLimitCP = true;
				eventCanBeFailed = false;
				eventLimitCP = 10;
				eventBonusTimeOnCPMultiplier = 0.1f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 3f;
				break;
			}
		case 5: // Chain Drift Challenge
			{
				remainingSec = 6;
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = false;
				eventHasTimeLimit = true;
				eventHasLimitCP = true;
				eventCanBeFailed = false;
				eventHasScoreOnDrift = false;
				eventHasScoreOnNode = true;
				eventLimitCP = 10;
				eventBonusTimeOnCPMultiplier = 0.05f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		case 6: // Time Attack
			{
				remainingSec = 99;
				eventHasBonusTimeOnCP = false;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = false;
				eventHasTimeLimit = false;
				eventHasLimitCP = true;
				eventCanBeFailed = true;
				eventHasScoreOnDrift = false;
				eventHasScoreOnNode = false;
				eventLimitCP = 10;
				eventBonusTimeOnCPMultiplier = 0f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		default: // Free Roam
			{
				remainingSec = 99;
				eventHasBonusTimeOnCP = false;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = false;
				eventHasTimeLimit = false;
				eventHasLimitCP = false;
				eventCanBeFailed = false;
				eventHasScoreOnDrift = false;
				eventHasScoreOnNode = false;
				eventBonusTimeOnCPMultiplier = 0f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		}
	}

	// Prepara la pantalla de puntuacion.

	void SetEndGameScreen(int type)
	{
		// 1 = Destroyed, 2 = Time Up, 3 = Last CP reached.
		if (eventCanBeFailed) {
			EndGameScreenBehaviour.currentInstance.SetAndEnable (type, type != 3);
		} else {
			EndGameScreenBehaviour.currentInstance.SetAndEnable (type, false);
		}
	}

	// getters/setters

	public bool GetEventHasLimitCP()
	{
		return eventHasLimitCP;
	}
	public bool GetEventHasScore()
	{
		return eventHasScore;
	}
	public float GetEventScore()
	{
		return eventScore;
	}
	public int GetEventLimitCP()
	{
		return eventLimitCP;
	}
	public float GetScoringDistance()
	{
		return scoringDistance;
	}
	public float GetScoringCheckPoint()
	{
		return scoringCheckPoint;
	}
	public float GetScoringCleanSection()
	{
		return scoringCleanSection;
	}
	public float GetScoringDrift()
	{
		return scoringDrift;
	}
	public float GetScoringDamagePenalty()
	{
		return scoringDamagePenalty;
	}

}
