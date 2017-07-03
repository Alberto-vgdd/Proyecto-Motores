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

	private float objectiveGold = 0;
	private float objectiveSilver = 0;
	private float objectiveBronze = 0;
	private bool objectiveTypeScore = false;


	private bool eventHasTimeLimit = false;
	private bool eventHasScore = false;
	private bool eventCanBeFailed = false;

	private int eventLimitCP = 0;

	private float eventBonusTimeOnCPMultiplier = 0;
	private float eventBonusTimeOnDriftMultiplier = 0;

	private float eventDamageTakenMultiplier = 1;

	private float eventScoreOnDriftMultiplier = 0;
	private float eventScoreOnDistance = 0;
	private float eventScoreOnCheckpoint = 0;
	private float eventScoreOnCleanSection = 0;
	private float eventScoreDamagePenaltyMultiplier = 1f;


	void Awake () { currentData = this; }
	void Start () {
		EventSetup ();
		SetObjectives ();
		if (eventLimitCP > 0) {
			IngameHudManager.currentInstance.UpdateSectorInfo ();
		}
		PreRacePanelBehaviour.currentInstance.SetPanelInfo (gamemode);
	}

	void Update () {
		if (eventFinished)
		{
			gameOverDelay -= Time.deltaTime;
			if (gameOverDelay <= 0.0f) 
			{
				
			}
		} 
		else 
		{
			UpdateTime ();
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
	void SetObjectives()
	{
		//TODO: Esto no deberia estar aqui.

		switch (gamemode) {
		case 1: // Standard endurance
			{
				objectiveGold = 10000;
				objectiveSilver = 9000;
				objectiveBronze = 8000;
				break;
			}
		case 2: // Drift Endurance
			{
				objectiveGold = 7000;
				objectiveSilver = 6500;
				objectiveBronze = 6000;
				break;
			}
		case 3: // Drift Exhibition
			{
				objectiveGold = 2500 * eventLimitCP;
				objectiveSilver = 2300 * eventLimitCP;
				objectiveBronze = 2100 * eventLimitCP;
				break;
			}
		case 4: // High speed challenge
			{
				objectiveGold = 3000;
				objectiveSilver = 2500;
				objectiveBronze = 2000;
				break;
			}
		case 5: // Drift challenge
			{
				objectiveGold = 5000;
				objectiveSilver = 4500;
				objectiveBronze = 4000;
				break;
			}
		case 6: // Time attack
			{
				objectiveGold = 16f * eventLimitCP;
				objectiveSilver = 17.5f * eventLimitCP;
				objectiveBronze = 18f * eventLimitCP;
				break;
			}
		}
	}

	public void PlayerCrossedNode()
	{
		AddScore (eventScoreOnDistance);
	}

	// Hace 10 de daño al jugador y actualiza el interfaz, este daño NO PUEDE ser letal.

	public void PlayerCrossedCheckPoint(bool clean, float awardedTime)
	{
		if (clean) {
			HealPlayer (10);
			NotificationManager.currentInstance.AddNotification (new GameNotification ("Clean section, health restored!", Color.green, 30));
			eventScore += eventScoreOnCleanSection;
			cleanSections++;
		} else {
			eventScore += eventScoreOnCheckpoint;
		}
		if (eventBonusTimeOnCPMultiplier > 0) {
			time_remainingSec += awardedTime * eventBonusTimeOnCPMultiplier; 
			NotificationManager.currentInstance.AddNotification(new GameNotification("Time extended! +" + awardedTime.ToString("F1") , Color.green, 30));
		}
		if (eventLimitCP > 0) {
			IngameHudManager.currentInstance.UpdateSectorInfo ();
			if (checkPointsCrossed >= eventLimitCP) {
				EndEvent (3);
			}
		}
		checkPointsCrossed++;
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

	public void AddScore(float arg)
	{
		if (!eventHasScore)
			return;
		eventScore += arg;
		IngameHudManager.currentInstance.UpdateScoreInfo ();
	}
	public void SendFinishedDrift(float lenght, float multi = 1)
	{
		if (lenght > 100 && eventBonusTimeOnDriftMultiplier > 0) {
			time_remainingSec += lenght * eventBonusTimeOnDriftMultiplier;
			NotificationManager.currentInstance.AddNotification(new GameNotification((int)lenght + "m. drift! " + (lenght * eventBonusTimeOnDriftMultiplier).ToString("F1") + " bonus time.", Color.blue, 30));
		}
		if (eventScoreOnDriftMultiplier > 0) {
			AddScore (lenght * multi * eventScoreOnDriftMultiplier);
			NotificationManager.currentInstance.AddNotification(new GameNotification((int)lenght + "m. drift! +" + ((int)(lenght * multi * eventScoreOnDriftMultiplier)).ToString() + " score.", Color.yellow, 30));
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
		if (!gameStarted)
			return;
		if (eventHasTimeLimit) {
			if (gamemode == 4 && pm.accumulatedAcceleration / pm.maxFwdSpeed > 0.7f)
				return;
			if (gamemode == 5 && pm.drifting)
				return;
			time_remainingSec = Mathf.MoveTowards (time_remainingSec, 0, Time.deltaTime);
			if (time_remainingSec <= 0 && Mathf.Abs (pm.accumulatedAcceleration) <= 1f) 
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
				time_remainingSec += 0.3f;
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
				time_remainingSec = 25f;
				eventDamageTakenMultiplier = 1.5f;

				eventHasTimeLimit = true;
				eventHasScore = true;
				eventCanBeFailed = false;
				objectiveTypeScore = true;

				eventLimitCP = 0;
				eventBonusTimeOnCPMultiplier = 1f;
				eventBonusTimeOnDriftMultiplier = 0.005f;
				eventScoreOnDriftMultiplier = 0f;
				eventScoreOnDistance = 20f;
				eventScoreOnCheckpoint = 100f;
				eventScoreOnCleanSection = 500f;
				eventScoreDamagePenaltyMultiplier = 20f;
				break;
			}
		case 2: // Drift Endurance
			{
				time_remainingSec = 25f;
				eventDamageTakenMultiplier = 1.5f;

				eventHasTimeLimit = true;
				eventHasScore = true;
				eventCanBeFailed = false;
				objectiveTypeScore = true;

				eventLimitCP = 0;
				eventBonusTimeOnCPMultiplier = 0f;
				eventBonusTimeOnDriftMultiplier = 0.05f;
				eventScoreOnDriftMultiplier = 0f;
				eventScoreOnDistance = 20f;
				eventScoreOnCheckpoint = 100f;
				eventScoreOnCleanSection = 500f;
				eventScoreDamagePenaltyMultiplier = 20f;
				break;
			}
		case 3: // Drift Exhibition
			{
				time_remainingSec = 35f;
				eventDamageTakenMultiplier = 1f;

				eventHasTimeLimit = true;
				eventHasScore = true;
				eventCanBeFailed = false;
				objectiveTypeScore = true;

				eventLimitCP = 6;
				eventBonusTimeOnCPMultiplier = 1.5f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventScoreOnDriftMultiplier = 0.5f;
				eventScoreOnDistance = 0f;
				eventScoreOnCheckpoint = 0f;
				eventScoreOnCleanSection = 200f;
				eventScoreDamagePenaltyMultiplier = 100f;
				break;
			}
		case 4: // High Speed Challenge
			{
				time_remainingSec = 10f;
				eventDamageTakenMultiplier = 3f;

				eventHasTimeLimit = true;
				eventHasScore = false;
				eventCanBeFailed = false;
				objectiveTypeScore = true;

				eventLimitCP = 10;
				eventBonusTimeOnCPMultiplier = 0.1f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventScoreOnDriftMultiplier = 0f;
				eventScoreOnDistance = 0f;
				eventScoreOnCheckpoint = 0f;
				eventScoreOnCleanSection = 0f;
				eventScoreDamagePenaltyMultiplier = 20f;
				break;
			}
		case 5: // Chain Drift Challenge
			{
				time_remainingSec = 6f;
				eventDamageTakenMultiplier = 1f;

				eventHasTimeLimit = true;
				eventHasScore = false;
				eventCanBeFailed = false;
				objectiveTypeScore = true;

				eventLimitCP = 10;
				eventBonusTimeOnCPMultiplier = 0.05f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventScoreOnDriftMultiplier = 0f;
				eventScoreOnDistance = 0f;
				eventScoreOnCheckpoint = 0f;
				eventScoreOnCleanSection = 0f;
				eventScoreDamagePenaltyMultiplier = 20f;
				break;
			}
		case 6: // Time Attack
			{
				time_remainingSec = 0f;
				eventDamageTakenMultiplier = 1f;

				eventHasTimeLimit = false;
				eventHasScore = false;
				eventCanBeFailed = true;
				objectiveTypeScore = false;

				eventLimitCP = 6;
				eventBonusTimeOnCPMultiplier = 0f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventScoreOnDriftMultiplier = 0f;
				eventScoreOnDistance = 0f;
				eventScoreOnCheckpoint = 0f;
				eventScoreOnCleanSection = 0f;
				eventScoreDamagePenaltyMultiplier = 0f;
				break;
			}
		default: // Free Roam
			{
				time_remainingSec = 10f;
				eventDamageTakenMultiplier = 1f;

				eventHasTimeLimit = false;
				eventHasScore = false;
				eventCanBeFailed = false;
				objectiveTypeScore = false;

				eventLimitCP = 0;
				eventBonusTimeOnCPMultiplier = 0f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventScoreOnDriftMultiplier = 0f;
				eventScoreOnDistance = 0f;
				eventScoreOnCheckpoint = 0f;
				eventScoreOnCleanSection = 0f;
				eventScoreDamagePenaltyMultiplier = 0f;
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

	public string GetTimePassedString()
	{
		return time_passedMin.ToString ("D2") + ":" + time_passedSec.ToString ("D2") + ":" + ((int)(time_passedDec * 100)).ToString ("D2");
	}
	public float GetTimePassedValue()
	{
		return time_passedMin * 60 + time_passedSec + time_passedDec;
	}
	public string GetObjectiveString()
	{
		string txt = "";
		if (gamemode == 0) {
			txt = "- No objectives -";
		}
		else if (GetObjectiveIsTypeScore()) {
			txt = 
				"1ST: " + StageData.currentData.GetObjective (1) +
				"\n2ND: " + StageData.currentData.GetObjective (2) +
				"\n3RD: " + StageData.currentData.GetObjective (3);
		} else {
			txt = 
				"1ST: " + ((int)(GetObjective (1) / 60)).ToString () + ":" + ((int)(GetObjective (1) % 60)).ToString("D2") + ":00" +
				"\n2ND: " + ((int)(GetObjective (2) / 60)).ToString () + ":" + ((int)(GetObjective (2) % 60)).ToString("D2") + ":00" +
				"\n3RD: " + ((int)(GetObjective (3) / 60)).ToString () + ":" + ((int)(GetObjective (3) % 60)).ToString("D2") + ":00";
		}
		return txt;
	}

	public bool GetEventHasObjectives()
	{
		return gamemode != 0;
	}
	public bool GetEventHasScore()
	{
		return eventHasScore;
	}
	public float GetEventScore()
	{
		return eventScore;
	}
	public float GetEventDamagePenaltyMultiplier()
	{
		return eventScoreDamagePenaltyMultiplier;
	}
	public float GetEventScoreMinusPenalty()
	{
		return Mathf.Clamp(eventScore - (damageTaken * eventScoreDamagePenaltyMultiplier), 0, 9999999);
	}
	public void SetEventScoreMinusPenalty()
	{
		eventScore = Mathf.Clamp(eventScore - (damageTaken * eventScoreDamagePenaltyMultiplier), 0, 9999999);
	}
	public bool GetEventHasTimelimit()
	{
		return eventHasTimeLimit;
	}
	public int GetEventLimitCP()
	{
		return eventLimitCP;
	}
	public bool GetObjectiveIsTypeScore()
	{
		return objectiveTypeScore;
	}
	public int GetPlayerResult()
	{
		if (eventHasTimeLimit) {
			if (eventScore > objectiveGold) {
				return 1;
			} else if (eventScore > objectiveSilver) {
				return 2;
			} else if (eventScore > objectiveBronze) {
				return 3;
			} else
				return 0;
		} else {
			if (GetTimePassedValue() < objectiveGold) {
				return 1;
			} else if (GetTimePassedValue() < objectiveSilver) {
				return 2;
			} else if (GetTimePassedValue() < objectiveBronze) {
				return 3;
			} else
				return 0;
		}
	}
	public float GetObjective(int id)
	{
		if (id == 3) 
		{
			return objectiveBronze;
		}
		else if (id == 2)
		{
			return objectiveSilver;
		}
		else 
		{
			return objectiveGold;
		}
	}
}
