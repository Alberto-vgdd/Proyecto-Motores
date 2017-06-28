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
	private bool gameOver;
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

	private int eventLimitCP = 0;
	private float eventBonusTimeOnCPMultiplier = 1;
	private float eventBonusTimeOnDriftMultiplier = 1;
	private float eventBonusScoreOnDriftMultiplier = 1;
	private float eventDamageTakenMultiplier;

	private float DISTANCE_SCORE_MULTIPLIER = 10;
	private float DRIFT_SCORE_MULTIPLIER = 0.5f;
	private float CLEAN_SECTION_SCORE_MULTIPLIER = 50;
	private float DAMAGE_TAKEN_SCORE_MULTIPLIER = 10f;

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

		if (gameOver)
		{
			gameOverDelay -= Time.deltaTime;
			if (gameOverDelay <= 0.0f) 
			{
				if (Input.anyKeyDown)
					SceneManager.LoadScene ("game");
			}
		} 
		else
		{
			if (notification_destroyed || (remainingSec <= 0 && Mathf.Abs (pm.accumulatedAcceleration) <= 1f)) 
			{
				NotificationManager.currentInstance.AddNotification (new GameNotification ("GAME OVER", Color.red, 200));
				EndEvent ();
				SetEndGameScreen ();
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
			StartCoroutine ("FadeIn");
		}

	}
	public void EndEvent()
	{
		gameOver = true;
		IngameHudManager.currentInstance.SetHudVisibility (false);
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
				EndEvent ();
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

	// Daña al jugador y actualiza el interfaz, este daño PUEDE ser letal.

	public void DamagePlayer(float dmg)
	{
		dmg *= eventDamageTakenMultiplier;
		dmg = Mathf.Abs (dmg);
		damageTaken += dmg;
		playerHealth = Mathf.Clamp (playerHealth - dmg, 0, 100);
		ContextualHudManager.currentInstance.UpdateDynHealth ();
		UpdateHealthNotifications ();
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
	}
	public void SendFinishedDrift(float lenght, float multi = 1)
	{
		if (lenght > 100) {
			ExtendTime ("drift", (int)lenght);
		}
		AddScore(lenght * multi * eventBonusScoreOnDriftMultiplier);
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
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = true;
				eventHasScore = false;
				eventHasTimeLimit = true;
				eventHasLimitCP = false;
				eventBonusTimeOnCPMultiplier = 1;
				eventBonusTimeOnDriftMultiplier = 0.2f;
				eventBonusScoreOnDriftMultiplier = 0.5f;
				eventDamageTakenMultiplier = 1.25f;
				break;
			}
		case 2: // Drift Endurance
			{
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = true;
				eventHasScore = false;
				eventHasTimeLimit = true;
				eventHasLimitCP = false;
				eventBonusTimeOnCPMultiplier = 0.1f;
				eventBonusTimeOnDriftMultiplier = 1f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		case 3: // Drift Exhibition
			{
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = true;
				eventHasTimeLimit = true;
				eventHasLimitCP = true;
				eventLimitCP = 8;
				eventBonusTimeOnCPMultiplier = 1.5f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 1f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		case 4: // High Speed Challenge
			{
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = false;
				eventHasTimeLimit = true;
				eventHasLimitCP = true;
				eventLimitCP = 10;
				eventBonusTimeOnCPMultiplier = 0.1f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 3f;
				break;
			}
		case 5: // Chain Drift Challenge
			{
				eventHasBonusTimeOnCP = true;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = false;
				eventHasTimeLimit = true;
				eventHasLimitCP = true;
				eventLimitCP = 10;
				eventBonusTimeOnCPMultiplier = 0.05f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		case 6: // Time Attack
			{
				eventHasBonusTimeOnCP = false;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = false;
				eventHasTimeLimit = false;
				eventHasLimitCP = true;
				eventLimitCP = 10;
				eventBonusTimeOnCPMultiplier = 0f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		default: // Free Roam
			{
				eventHasBonusTimeOnCP = false;
				eventHasBonusTimeOnDrift = false;
				eventHasScore = false;
				eventHasTimeLimit = false;
				eventHasLimitCP = false;
				eventBonusTimeOnCPMultiplier = 0f;
				eventBonusTimeOnDriftMultiplier = 0f;
				eventBonusScoreOnDriftMultiplier = 0f;
				eventDamageTakenMultiplier = 1f;
				break;
			}
		}
	}

	// Prepara la pantalla de puntuacion.

	void SetEndGameScreen()
	{
		switch (gamemode) {
		case 1: // Standard Endurance
			{
				endGameStatsText.text = "[ENDURANCE]" +
				"\nEVENT COMPLETED" +
				"\n";
				break;
			}
		case 2: // Drift Endurance
			{
				endGameStatsText.text = "[DRIFT ENDURANCE] EVENT COMPLETED" +
					"\n";
				break;
			}
		case 3: // Drift Exhibition
			{
				endGameStatsText.text = "[DRIFT EXHIBITION] EVENT COMPLETED" +
					"\n";
				break;
			}
		case 4: // High Speed Challenge
			{
				endGameStatsText.text = "[HIGH SPEED CHALLENGE] EVENT COMPLETED" +
					"\n";
				break;
			}
		case 5: // Chain Drift Challenge
			{
				endGameStatsText.text = "[DRIFT CHAIN CHALLENGE] EVENT COMPLETED" +
					"\n";
				break;
			}
		case 6: // Time Attack
			{
				endGameStatsText.text = "[TIME ATTACK] EVENT COMPLETED" +
					"\n";
				break;
			}
		default: // Free Roam
			{
				endGameStatsText.text = "[FREE ROAM] EVENT COMPLETED" +
					"\n";
				break;
			}
		}
//		finalscore = ( ((int)(totalDrift * DRIFT_SCORE_MULTIPLIER)) + ((int)(nodesCrossed * DISTANCE_SCORE_MULTIPLIER)) 
//			+ ((int)(cleanSections * CLEAN_SECTION_SCORE_MULTIPLIER)) - ((int)(damageTaken*DAMAGE_TAKEN_SCORE_MULTIPLIER)) );
//		endGameStatsText.text = " GAME OVER " + "\n\nTOTAL DISTANCE:    " + nodesCrossed + " [ +" + nodesCrossed*DISTANCE_SCORE_MULTIPLIER + " ] "
//			+ "\nCLEAN SECTIONS:    " + cleanSections + " [ +" + cleanSections * CLEAN_SECTION_SCORE_MULTIPLIER + " ] "
//			+ "\nTOTAL DRIFT:    " + (int)totalDrift + " [ +" + (int)(totalDrift*DRIFT_SCORE_MULTIPLIER) + " ] "
//			+ "\nDAMAGE TAKEN:    " + (int)damageTaken + " [ -" + (int)(damageTaken*DAMAGE_TAKEN_SCORE_MULTIPLIER) + " ] "
//			+ "\n\nFINAL SCORE:    " + finalscore +"\n\nPRESS ANY KEY TO CONTINUE";
	}

	// getters/setters
	public int GetEventLimitCP()
	{
		return eventLimitCP;
	}
}
