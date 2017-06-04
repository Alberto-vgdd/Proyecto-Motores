using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageData : MonoBehaviour {

	// Administra y almacena la informacion del nivel. Para que funcione correctamente necesita:
	// - Referencia al jugador.

	public static StageData currentData;									// Referencia estatica

	public GameObject playerObj;											// GameObject del jugador (?)
	public float playerHealth;												// Salud del jugador
	public float remainingSec;												// Segundos restantes

	public Text timeRemainingInfo;											// Referencia al texto (UI) de tiempo restante

	private bool notification_crit50 = false;								// (TEMP) Notificacion de 50% de salud mostrada
	private bool notification_crit25 = false;								// (TEMP) Notificacion de 25% de salud mostrada	
	private bool notification_crit10 = false;								// (TEMP) Notificacion de 10% de salud mostrada
	private bool notification_destroyed = false;							// (TEMP) Notificacion de 0% de salud mostrada

	public Text countDownText;
	public CanvasGroup fadeoutCG;
	public CanvasGroup endGameStatsCG;
	public Text endGameStatsText;

	public PlayerMovement pm;												// Referencia a PlayerMovement.

    [Header("Ambient Light Parameters")]
    public bool lightsOn;
    public float lightRange;
    public float lightAngle;


    [Header ("Diferent Player Chasis")]
    public GameObject dayChasis;
    public GameObject nightChasis;
	[Header ("Score data")]
	public int nodesCrossed;
	public float damageTaken;
	public int cleanSections;
	public float totalDrift;

	public bool gameStarted;
	private bool gameOver;
	private float gameOverDelay = 5.0f;
	private float startGameDelay = 6f;
	private float countdownOverDelay = 1.5f;
	private int finalscore;

	private float DISTANCE_SCORE_MULTIPLIER = 10;
	private float DRIFT_SCORE_MULTIPLIER = 0.5f;
	private float CLEAN_SECTION_SCORE_MULTIPLIER = 50;
	private float DAMAGE_TAKEN_SCORE_MULTIPLIER = 0.8f;

	void Awake () { currentData = this; }
	void Start () {
	}

	void Update () {
		if (remainingSec > 5) {
			timeRemainingInfo.text = ((int)remainingSec).ToString();
		} else {
			timeRemainingInfo.text = remainingSec.ToString("N2");
		}

		if (startGameDelay > 0) {
			if (startGameDelay < 3) {
				countDownText.text = ((int)(startGameDelay+1)).ToString();
			}
			startGameDelay -= Time.deltaTime;
			if (startGameDelay <= 0)
				gameStarted = true;
		} else {
			if (countdownOverDelay > 0) {
				countdownOverDelay -= Time.deltaTime;
				countDownText.text = " GO! ";
				if (countdownOverDelay <= 0)
					countDownText.text = "";
			}
			remainingSec = Mathf.MoveTowards (remainingSec, 0, Time.deltaTime);
		}


		if (gameOver)
		{
			fadeoutCG.alpha = Mathf.MoveTowards (fadeoutCG.alpha, 1, Time.deltaTime * 0.2f);
			endGameStatsCG.alpha = fadeoutCG.alpha;
			gameOverDelay -= Time.deltaTime;
			endGameStatsText.text = " GAME OVER " + "\n\nTOTAL DISTANCE:    " + nodesCrossed + " [ +" + nodesCrossed*DISTANCE_SCORE_MULTIPLIER + " ] "
				+ "\nCLEAN SECTIONS:    " + cleanSections + " [ +" + cleanSections * CLEAN_SECTION_SCORE_MULTIPLIER + " ] "
				+ "\nTOTAL DRIFT:    " + (int)totalDrift + " [ +" + (int)(totalDrift*DRIFT_SCORE_MULTIPLIER) + " ] "
				+ "\nDAMAGE TAKEN:    " + (int)damageTaken + " [ -" + (int)(damageTaken*DAMAGE_TAKEN_SCORE_MULTIPLIER) + " ] "
				+ "\n\nFINAL SCORE:    " + finalscore +"\n\nPRESS ANY KEY TO CONTINUE";
			if (gameOverDelay <= 0.0f) 
			{
				if (Input.anyKeyDown)
					SceneManager.LoadScene ("game");
			}
		} 
		else
		{
			fadeoutCG.alpha = Mathf.MoveTowards (fadeoutCG.alpha, 0, Time.deltaTime * 0.3f);
			if (notification_destroyed || (remainingSec <= 0 && Mathf.Abs (pm.accumulatedAcceleration) <= 1f)) 
			{
				NotificationManager.currentInstance.AddNotification (new GameNotification ("GAME OVER", Color.red, 200));
				gameOver = true;
				finalscore = ( ((int)(totalDrift * DRIFT_SCORE_MULTIPLIER)) + ((int)(nodesCrossed * DISTANCE_SCORE_MULTIPLIER)) 
					+ ((int)(cleanSections * CLEAN_SECTION_SCORE_MULTIPLIER)) - ((int)(damageTaken*DAMAGE_TAKEN_SCORE_MULTIPLIER)) );
			}
		}



		
	}

	// Hace 10 de daño al jugador y actualiza el interfaz, este daño NO PUEDE ser letal.

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
		dmg = Mathf.Abs (dmg);
		damageTaken += dmg;
		playerHealth -= dmg;
		ContextualHudManager.currentInstance.UpdateDynHealth ();
		UpdateHealthNotifications ();
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

    public void UpdateAllLights(bool lightsEnabled)
    {
        lightsOn = lightsEnabled;
		if (RoadGenerator.currentInstance == null) {return;	}
        lightsOn = lightsEnabled;
		for (int i = 12;i < RoadGenerator.currentInstance.spawnedNodes.Count; i++)
        {
			
			RoadGenerator.currentInstance.spawnedNodes [i].GetComponent<RoadNode> ().SetLightState (lightsEnabled);
        }

        nightChasis.SetActive( lightsEnabled);
        dayChasis.SetActive(!lightsEnabled);

    }
		
}
