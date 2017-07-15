using System.Collections;
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
	public CanvasGroup endGameStatsCG;
	public Text endGameStatsText;

	public PlayerMovement pm;												// Referencia a PlayerMovement.

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

	private float timeCountMultiplier = 1f;
	public bool gameStarted;
	private bool eventFinished;
	private float gameOverDelay = 5.0f;
	private int startGameDelay = 6;
	private float eventScore;
	private int finalscore;

	private bool countdownRunning = false;

	private EventData eventActive;



	void Awake () { currentData = this; }
	void Start () {
		eventActive = GlobalGameData.currentInstance.eventActive;
		time_remainingSec = eventActive.GetInitialTimeRemaining ();
		time_remainingSec += 0.3f; // Pequeño margen.
		UpdateTime ();
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

	public void PlayerCrossedNode()
	{
		AddScore (eventActive.GetScoreOnDistance());
	}

	// Hace 10 de daño al jugador y actualiza el interfaz, este daño NO PUEDE ser letal.

	public void PlayerCrossedCheckPoint(bool clean, float awardedTime)
	{
		checkPointsCrossed++;
		awardedTime *= eventActive.GetBonusTimeOnCheckpointMultiplier ();
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
			time_remainingSec += awardedTime; 
			NotificationManager.currentInstance.AddNotification(new GameNotification("Time extended! +" + awardedTime.ToString("F1") , Color.green, 30));
		}
		if (eventActive.GetEventCheckpoints() > 0) {
			IngameHudManager.currentInstance.UpdateSectorInfo ();
			if (checkPointsCrossed >= eventActive.GetEventCheckpoints()) {
				ContextualHudManager.currentInstance.ForceDriftEnd ();
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

	public void DamagePlayer(float dmg)
	{
		if (eventFinished)
			return;
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
		if (!eventActive.HasScore())
			return;
		eventScore += arg;
		IngameHudManager.currentInstance.UpdateScoreInfo ();
	}
	public void SendFinishedDrift(float lenght, float multi = 1)
	{
		if (lenght > 100 && eventActive.GetBonusTimeOnDriftMultiplier() > 0) {
			time_remainingSec += lenght * eventActive.GetBonusTimeOnDriftMultiplier();
			NotificationManager.currentInstance.AddNotification(new GameNotification((int)lenght + "m. drift! " + (lenght * eventActive.GetBonusTimeOnDriftMultiplier()).ToString("F1") + " bonus time.", Color.blue, 30));
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
		if (!gameStarted)
			return;
		if (eventActive.HasTimelimit()) {
			if (eventActive.GetGamemode () == 4 && pm.GetCurrentSpeedPercentage() > 0.65f)
				timeCountMultiplier = 0.05f;
			if (eventActive.GetGamemode() == 5 && pm.IsDrifting())
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

	public string GetTimePassedString()
	{
		return time_passedMin.ToString ("D2") + ":" + time_passedSec.ToString ("D2") + ":" + ((int)(time_passedDec * 100)).ToString ("D2");
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

	public int GetPlayerResult()
	{
		if (eventActive.HasTimelimit()) {
			if (eventScore > eventActive.GetObjectiveForPosition(1)) {
				return 1;
			} else if (eventScore > eventActive.GetObjectiveForPosition(2)) {
				return 2;
			} else if (eventScore >eventActive.GetObjectiveForPosition(3)) {
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
