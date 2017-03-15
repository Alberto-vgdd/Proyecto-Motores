using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageData : MonoBehaviour {

	public static StageData currentData;

	public GameObject playerObj;
	public float playerHealth;
	public float remainingSec;
	public int nodesCrossed;
	public CanvasGroup DynHealthCG;
	public CanvasGroup DriftCG;
	public CanvasGroup AirCG;
	public Text DriftText;
	public Text AirText;

	public Slider hpbar;
	public Image hpbarfill;

	public Text timeRemainingInfo;

	private bool notification_crit50 = false;
	private bool notification_crit25 = false;
	private bool notification_crit10 = false;
	private bool notification_destroyed = false;

	public PlayerMovement pm;

	private bool cleanSection = true;
	private float tempDriftChain;
	private float tempAirTime;

	void Awake () { currentData = this; }

	void Start () {
		tempDriftChain = 0;
	}

	// Update is called once per frame
	void Update () {
		UpdateDynDrift ();
		UpdateDynAir ();
		if (pm.grounded) {
			if (playerHealth == 100)
				DynHealthCG.alpha = Mathf.MoveTowards (DynHealthCG.alpha, 0, Time.deltaTime / 2);
			else if (playerHealth < 50) {
				DynHealthCG.alpha = Mathf.MoveTowards (DynHealthCG.alpha, 0, Time.deltaTime*1.5f);
				if (DynHealthCG.alpha <= 0)
					DynHealthCG.alpha = 1;
			}
			else
				DynHealthCG.alpha = Mathf.MoveTowards (DynHealthCG.alpha, 0.4f, Time.deltaTime/2);
		} else {
			DynHealthCG.alpha = Mathf.MoveTowards (DynHealthCG.alpha, 0, Time.deltaTime);
		}
		hpbar.value = playerHealth / 100;
		if (remainingSec > 5) {
			timeRemainingInfo.text = ((int)remainingSec).ToString();
		} else {
			timeRemainingInfo.text = remainingSec.ToString("N2");
		}

		remainingSec = Mathf.MoveTowards (remainingSec, 0, Time.deltaTime);

		
	}
	void UpdateDynDrift()
	{
		if (!pm.drifting) {
			DriftCG.alpha = Mathf.MoveTowards (DriftCG.alpha, 0, Time.deltaTime);
			if (tempDriftChain > 1000) {
				NotificationManager.currentInstance.AddNotification (new GameNotification (  (int)tempDriftChain + "m. drift!", Color.yellow, 30));
			}
			tempDriftChain = 0;
			return;
		}
		DriftCG.alpha = Mathf.MoveTowards (DriftCG.alpha, 1, Time.deltaTime);
		tempDriftChain += Time.deltaTime * pm.accumulatedAcceleration * 2.5f;
		float colorT = Mathf.Min (1, tempDriftChain / 3000);
		DriftText.color = Color.Lerp (Color.white, Color.red, colorT);
		DriftText.text = (int)tempDriftChain + " m.";
	}
	void UpdateDynHealth()
	{
		hpbarfill.color = Color.Lerp (Color.red, Color.green, playerHealth / 100);
		DynHealthCG.alpha = 1;
	}
	void UpdateDynAir()
	{
		if (pm.ungroundedTime < 0.75f || !pm.cleanAir) {
			AirCG.alpha = Mathf.MoveTowards (AirCG.alpha, 0, Time.deltaTime);
			AirText.rectTransform.localPosition = Vector2.MoveTowards (AirText.rectTransform.localPosition, Vector2.zero, Time.deltaTime);
			if (tempAirTime > 1.5f && pm.cleanAir) {
				NotificationManager.currentInstance.AddNotification (new GameNotification (tempAirTime.ToString ("N2") + " s. air!", Color.yellow, 30));
				tempAirTime = 0;
			} else if (!pm.cleanAir) {
				AirText.color = Color.red;
				AirText.text = " - Crashed - ";
				tempAirTime = 0;
			}
			return;
		}
			
		AirText.color = Color.white;
		AirCG.alpha = Mathf.MoveTowards (AirCG.alpha, 1, Time.deltaTime);
		AirText.rectTransform.localPosition = Vector2.MoveTowards (AirText.rectTransform.localPosition, new Vector2(0, 1), Time.deltaTime*4);
		AirText.text = "Air: " + pm.ungroundedTime.ToString("N2");
		tempAirTime = pm.ungroundedTime;

	}
	public void DamagePlayer(float dmg)
	{
		playerHealth -= dmg;
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
		UpdateDynHealth();
		cleanSection = false;
	}
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
		UpdateDynHealth();
	}
	public void CrossCheckPoint()
	{
		remainingSec += 10;
		if (cleanSection) {
			HealPlayer (10);
			NotificationManager.currentInstance.AddNotification(new GameNotification("Clean section, health restored", Color.green, 30));
		}
		cleanSection = true;
	}
}
