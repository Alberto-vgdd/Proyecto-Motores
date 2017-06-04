using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageData : MonoBehaviour {

	// Administra y almacena la informacion del nivel. Para que funcione correctamente necesita:
	// - Referencia al jugador.

	public static StageData currentData;									// Referencia estatica

	public GameObject playerObj;											// GameObject del jugador (?)
	public float playerHealth;												// Salud del jugador
	public float remainingSec;												// Segundos restantes
	public int nodesCrossed;												// Nodos cruzados

	public Text timeRemainingInfo;											// Referencia al texto (UI) de tiempo restante

	private bool notification_crit50 = false;								// (TEMP) Notificacion de 50% de salud mostrada
	private bool notification_crit25 = false;								// (TEMP) Notificacion de 25% de salud mostrada	
	private bool notification_crit10 = false;								// (TEMP) Notificacion de 10% de salud mostrada
	private bool notification_destroyed = false;							// (TEMP) Notificacion de 0% de salud mostrada

	public PlayerMovement pm;												// Referencia a PlayerMovement.

    [Header("Ambient Light Parameters")]
    public bool lightsOn;
    public float lightRange;
    public float lightAngle;


    [Header ("Diferent Player Chasis")]
    public GameObject dayChasis;
    public GameObject nightChasis;


	void Awake () { currentData = this; }

	void Update () {
		if (remainingSec > 5) {
			timeRemainingInfo.text = ((int)remainingSec).ToString();
		} else {
			timeRemainingInfo.text = remainingSec.ToString("N2");
		}

		remainingSec = Mathf.MoveTowards (remainingSec, 0, Time.deltaTime);

		
	}

	// Hace 10 de daño al jugador y actualiza el interfaz, este daño NO PUEDE ser letal.

	public void RespawnDamage()
	{
		if (playerHealth > 0.1f)
			DamagePlayer (Mathf.Clamp(0.1f - playerHealth ,-10,0));
		UpdateHealthNotifications ();
	}

	// Daña al jugador y actualiza el interfaz, este daño PUEDE ser letal.

	public void DamagePlayer(float dmg)
	{
		dmg = Mathf.Abs (dmg);
		playerHealth -= dmg;
		ContextualHudManager.currentInstance.UpdateDynHealth ();
		UpdateHealthNotifications ();
	}

	// Revisa si es necesario mostrar las alertas de daño.

	void UpdateHealthNotifications()
	{
		if (playerHealth < 50 && !notification_crit50) {
			NotificationManager.currentInstance.AddNotification (new GameNotification ("Below 50% Health", Color.red, 40));
			notification_crit50 = true;
		}
		if (playerHealth < 25 && !notification_crit25) {
			NotificationManager.currentInstance.AddNotification (new GameNotification ("Below 25% Health", Color.red, 40));
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
		for (int i = 7;i < RoadGenerator.currentInstance.spawnedNodes.Count; i++)
        {
			
			RoadGenerator.currentInstance.spawnedNodes [i].GetComponent<RoadNode> ().SetLightState (lightsEnabled);
        }

        nightChasis.SetActive( lightsEnabled);
        dayChasis.SetActive(!lightsEnabled);

    }
}
