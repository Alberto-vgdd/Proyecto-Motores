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

	[Header("References")]

	public CanvasGroup DynHealthCG;
	public CanvasGroup DriftCG;
	public CanvasGroup AirCG;
	public Text DriftText;
	public Text AirText;
	public Slider hpbar;
	public Image hpbarfill;
	public PlayerMovement pm;

	[Header("Hud Parameters")]
	public Color hpMaxColor;
	public Color hpMinColor;

	void Awake () { currentInstance = this; }
	void Start () { UpdateDynHealth (); }

	void Update () {
		UpdateDynDrift ();
		UpdateDynAir ();
		if (pm.grounded) {
			if (StageData.currentData.playerHealth == 100)
				DynHealthCG.alpha = Mathf.MoveTowards (DynHealthCG.alpha, 0, Time.deltaTime / 2);
			else if (StageData.currentData.playerHealth < 50) {
				DynHealthCG.alpha = Mathf.MoveTowards (DynHealthCG.alpha, 0, Time.deltaTime*1.5f);
				if (DynHealthCG.alpha <= 0)
					DynHealthCG.alpha = 1;
			}
			else
				DynHealthCG.alpha = Mathf.MoveTowards (DynHealthCG.alpha, 0.4f, Time.deltaTime/2);
		} else {
			DynHealthCG.alpha = Mathf.MoveTowards (DynHealthCG.alpha, 0, Time.deltaTime);
		}
	}

	// Administra el interfaz dinamico de drift.

	void UpdateDynDrift()
	{
		if (!pm.drifting) {
			DriftCG.alpha = Mathf.MoveTowards (DriftCG.alpha, 0, Time.deltaTime);
			if (tempDriftChain > 1000) {
				NotificationManager.currentInstance.AddNotification (new GameNotification (  (int)tempDriftChain + " m. drift. Bonus: time + " + (int)tempDriftChain/1000 + " s.", Color.yellow, 30));
				StageData.currentData.remainingSec += (int)tempDriftChain / 1000;
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

	// Administra el interfaz dinamico de salud.

	public void UpdateDynHealth()
	{
		hpbarfill.color = Color.Lerp (hpMinColor, hpMaxColor, StageData.currentData.playerHealth / 100);
		DynHealthCG.alpha = 1;
		hpbar.value = StageData.currentData.playerHealth / 100;
	}

	// Administra el interfaz dinamico de salto.

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
}
