using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextualHudManager : MonoBehaviour {

	// Esta clase administra el interfaz contextual que aparece alrededor del jugador, para que la clase funcione correctamente se necesita:
	// - Referencias a cada parte del interfaz dinamico.
	// - Referencia al jugador.
	// - Instancia de StageData en la escena.

	public static ContextualHudManager currentInstance;

	private float tempDriftChain = 0;
	private float tempAirTime = 0;

	[Header("CG Ref.")]
	public CanvasGroup HealthCG;
	public CanvasGroup DriftCG;
	public CanvasGroup AirCG;
	public CanvasGroup TimeCG;
	[Header("Text Ref.")]
	public Text DriftText;
	public Text AirText;
	public Text HpText;
	public Text TimeText;
	public Text IncrTimeText;
	public PlayerMovement pm;

	private Vector3 driftTextBasePos;
	private float readedTime;

	[Header("Hud Parameters")]
	public Color hpMaxColor;
	public Color hpMinColor;

	void Awake () { currentInstance = this; }
	void Start () { 
		UpdateDynHealth ();
		driftTextBasePos = DriftText.transform.localPosition;
	}

	void Update () {
		UpdateDynDrift ();
		UpdateDynAir ();
		//UpdateDynTime ();
		if (pm.grounded) {
			if (StageData.currentData.playerHealth == 100)
				HealthCG.alpha = Mathf.MoveTowards (HealthCG.alpha, 0, Time.deltaTime / 2);
			else if (StageData.currentData.playerHealth < 50) {
				HealthCG.alpha = Mathf.MoveTowards (HealthCG.alpha, 0, Time.deltaTime*1.5f);
				if (HealthCG.alpha <= 0)
					HealthCG.alpha = 1;
			}
			else
				HealthCG.alpha = Mathf.MoveTowards (HealthCG.alpha, 0.75f, Time.deltaTime/2);
		} else {
			HealthCG.alpha = Mathf.MoveTowards (HealthCG.alpha, 0, Time.deltaTime);
		}
	}

	// Administra el interfaz dinamico de drift.
	void UpdateDynTime()
	{
		readedTime = StageData.currentData.remainingSec;
		if (readedTime > 5) {
			TimeText.text = ((int)readedTime).ToString();
		} else {
			TimeText.text = readedTime.ToString("N2");
		}
	}
	void UpdateDynDrift()
	{
		if (!pm.drifting) {
			DriftText.transform.localPosition = driftTextBasePos;
			DriftCG.alpha = Mathf.MoveTowards (DriftCG.alpha, 0, Time.deltaTime);
			if (tempDriftChain > 100) {
				NotificationManager.currentInstance.AddNotification (new GameNotification ((int)tempDriftChain + " m. drift! Bonus time + " + (int)tempDriftChain / 100 + " s.", Color.yellow, 30));
				StageData.currentData.remainingSec += (int)tempDriftChain / 100;
			}
			StageData.currentData.totalDrift += tempDriftChain;
			tempDriftChain = 0;
			return;
		}
		//DriftText.transform.localPosition = driftTextBasePos + Vector3.up * (Mathf.Cos (tempDriftChain*0.3f) * 0.01f);
		DriftCG.alpha = Mathf.MoveTowards (DriftCG.alpha, 1, Time.deltaTime);
		tempDriftChain += Time.deltaTime * pm.accumulatedAcceleration * 3.5f;
		float colorT = Mathf.Min (1, tempDriftChain / 3000);
		DriftText.color = Color.Lerp (Color.white, Color.red, colorT);
		DriftText.text = (int)tempDriftChain + " m.";
	}

	// Administra el interfaz dinamico de salud.

	public void UpdateDynHealth()
	{
		HpText.color = Color.Lerp (hpMinColor, hpMaxColor, StageData.currentData.playerHealth / 100);
		HealthCG.alpha = 1;
		HpText.text = ((int) StageData.currentData.playerHealth).ToString();
	}

	// Administra el interfaz dinamico de salto.

	void UpdateDynAir()
	{
		if (pm.ungroundedTime < 0.75f || !pm.cleanAir) {
			AirCG.alpha = Mathf.MoveTowards (AirCG.alpha, 0, Time.deltaTime);
			//AirText.rectTransform.localPosition = Vector2.MoveTowards (AirText.rectTransform.localPosition, Vector2.zero, Time.deltaTime);
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
		//AirText.rectTransform.localPosition = Vector2.MoveTowards (AirText.rectTransform.localPosition, new Vector2(0, 1), Time.deltaTime*4);
		AirText.text = "Air: " + pm.ungroundedTime.ToString("N2");
		tempAirTime = pm.ungroundedTime;

	}
}
